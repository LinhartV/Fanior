using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Fanior.Server.Classes;
using System.Text.Json;
using MessagePack;
using Fanior.Shared;
using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.Extensions.Options;


namespace Fanior.Server
{
    /// <summary>
    /// Class taking care of actions to be done each frame.
    /// </summary>
    public class FrameRenderer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        
        public FrameRenderer(IServiceProvider serviceProvider)
        {
            ToolsSystem.serializer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());

            _serviceProvider = serviceProvider;

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                while (stoppingToken.IsCancellationRequested == false)
                {
                    await DoWork();
                    await Task.Delay(1000 / 60, stoppingToken);
                }
            });
        }

        /// <summary>
        /// Get current hub and use it in Frame
        /// </summary>
        private async Task DoWork()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var provider = scope.ServiceProvider;
                    var hub = provider.GetService<IHubContext<Fanior.Server.Hubs.MyHub>>();
                    var game = provider.GetService<GameControl>();
                    await Frame(game, hub);
                }
            }
            catch (Exception e)
            {

            }
        }


        /// <summary>
        /// Algorithms to be done each frame
        /// </summary>
        private async Task Frame(GameControl game, IHubContext<Hubs.MyHub> hub)
        {
            long now = game.sw.ElapsedMilliseconds;
            foreach (Gvars gvars in game.games.Values)
            {

                ProcedeGameAlgorithms(gvars);
                ProcedePlayerActions(gvars);
                ProcedeItemActions(now, gvars);

                gvars.messageId++;
            }
            await SendData(game, hub);
        }

        /// <summary>
        /// Procede algorithm of game logic (collision detection etc.)
        /// </summary>
        private void ProcedeGameAlgorithms(Gvars gvars)
        {

        }

        /// <summary>
        /// Procede actions that players just did
        /// </summary>
        private void ProcedePlayerActions(Gvars gvars)
        {
            foreach (int playerId in gvars.PlayerActions.Keys)
            {
                foreach (var action in gvars.PlayerActions[playerId])
                {
                    PlayerAction.InvokeAction(action.Item1, action.Item2, playerId, gvars);
                }
            }
        }

        /// <summary>
        /// Handles all actions of every item (excluding player actions)
        /// </summary>
        private void ProcedeItemActions(long now, Gvars gvars)
        {
            List<(long, ItemAction)> temp = new List<(long, ItemAction)>(gvars.ItemActions);
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].Item1 <= now)
                {
                    temp[i].Item2.Action();
                    gvars.Actions.Remove(temp[i]);
                    if (temp[i].Item2.Repeat > 0)
                    {
                        gvars.Actions.Add((now + temp[i].Item2.Repeat, temp[i].Item2));
                    }
                }
                else
                    break;
            }
            gvars.Actions.Sort();
        }

        /// <summary>
        /// Send all received actions to all clients for them to know, who did what.
        /// </summary>
        private async Task SendData(GameControl game, IHubContext<Hubs.MyHub> hub)
        {
            foreach (Gvars gvars in game.games.Values)
            {
                try
                {
                    if (gvars.PlayerActions.Count>0)
                    {
                        await hub?.Clients.Group(gvars.GameId).SendAsync("ExecuteList", gvars.PlayerActions, JsonConvert.SerializeObject(gvars.PlayerActions[gvars], ToolsSystem.jsonSerializerSettings));
                        gvars.PlayerActions.Clear();
                    }
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }


    }
}