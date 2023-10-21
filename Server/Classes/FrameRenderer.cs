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
using System.Reflection.Metadata;
using Fanior.Server.Hubs;

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
                    await Task.Delay(Constants.CONTROL_FRAME_TIME, stoppingToken);
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
                    var hub = provider.GetService<IHubContext<MyHub>>();
                    var game = provider.GetService<GameControl>();

                    lock (game.tempListsLock)
                    {
                        foreach (var infoDict in game.tempPlayerInfo)
                        {
                            game.games[infoDict.Key].PlayerInfo = new Dictionary<int, double>(infoDict.Value);
                            infoDict.Value.Clear();
                        }
                        foreach (var playerDict in game.tempPlayerActions)
                        {
                            game.games[playerDict.Key].PlayerActions = new Dictionary<int, List<(PlayerActions.PlayerActionsEnum, bool)>>(playerDict.Value);
                            playerDict.Value.Clear();
                        }
                    }

                    lock (game.actionLock)
                    {
                        Frame(game, hub);
                    }

                }
            }
            catch (Exception e)
            {

            }
        }

        long now = 0;
        /// <summary>
        /// Algorithms to be done each frame
        /// </summary>
        private async Task Frame(GameControl game, IHubContext<MyHub> hub)
        {
            try
            {
                double percantage = (game.sw.ElapsedMilliseconds - now) / (double)Constants.GAMEPLAY_FRAME_TIME;
                now = game.sw.ElapsedMilliseconds;
                foreach (Gvars gvars in game.games.Values)
                {
                    gvars.PercentageOfFrame = percantage;
                    ToolsGame.ProceedFrame(gvars, now, Constants.DELAY, true);
                    ServerGameLogic.ExecuteActions(game.sw.ElapsedMilliseconds, game, gvars, hub);
                    gvars.messageId++;
                    foreach (var player in gvars.ItemsPlayers.Values)
                    {
                        if (player.GetCurLives() <= 0)
                        {
                            hub?.Clients.All.SendAsync("PlayerDied", player.Id);
                            player.Dispose(gvars);
                            break;
                        }
                    }
                }
                await SendData(game, hub);
            }
            catch (Exception e)
            {
            }
        }


        /// <summary>
        /// Send all received actions to all clients for them to know, who did what.
        /// </summary>
        private async Task SendData(GameControl game, IHubContext<MyHub> hub)
        {
            foreach (Gvars gvars in game.games.Values)
            {
                try
                {//Group(gvars.GameId)
                 //actual time for delay, message id for check if messages are in order, playerActions, angle of every player, itemsToCreate, itemsToDestroy

                    hub?.Clients.All.SendAsync("ExecuteList", game.sw.ElapsedMilliseconds, gvars.messageId, JsonConvert.SerializeObject(gvars.PlayerActions, ToolsSystem.jsonSerializerSettings), gvars.PlayerInfo,
                        JsonConvert.SerializeObject(gvars.Msg.itemsToCreate, ToolsSystem.jsonSerializerSettings), gvars.Msg.itemsToDestroy
                        /*, JsonConvert.SerializeObject(GetItemCoordinates(gvars), ToolsSystem.jsonSerializerSettings)*/);
                    if (gvars.Msg.randomNumbersList.Count > 0)
                    {
                        hub?.Clients.All.SendAsync("ReceiveRandomNumbers", JsonConvert.SerializeObject(gvars.Msg.randomNumbersList, ToolsSystem.jsonSerializerSettings));
                    }
                    gvars.PlayerActions.Clear();
                    gvars.PlayerInfo.Clear();
                    gvars.Msg.ClearThis();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private Dictionary<int, (double, double)> GetItemCoordinates(Gvars gvars)
        {
            var coordinates = new Dictionary<int, (double, double)>();
            foreach (var item in gvars.Items.Values)
            {
                coordinates.Add(item.Id, (item.X, item.Y));
            }
            return coordinates;
        }

    }
}