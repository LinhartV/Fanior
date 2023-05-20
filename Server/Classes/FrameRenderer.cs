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

                ToolsGame.ProceedFrame(gvars, now);

                gvars.messageId++;
            }
            await SendData(game, hub);
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
                    await hub?.Clients.Group(gvars.GameId).SendAsync("ExecuteList", game.sw.ElapsedMilliseconds ,gvars.messageId, JsonConvert.SerializeObject(gvars.PlayerActions, ToolsSystem.jsonSerializerSettings));
                    gvars.PlayerActions.Clear();
                    
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }


    }
}