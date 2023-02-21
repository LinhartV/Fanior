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


namespace Fanior.Server
{
    public class FrameRenderer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
        JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Newtonsoft.Json.Formatting.Indented,
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore

        };
        public FrameRenderer(IServiceProvider serviceProvider)
        {
            serializer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());

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



        private async Task Frame(GameControl game, IHubContext<Hubs.MyHub> hub)
        {
            long now = game.sw.ElapsedMilliseconds;
            List<Task> tasks = new List<Task>();
            foreach (Gvars gvars in game.games.Values)
            {
                ProcedeActions(now, gvars);
                tasks.Add(Task.Run(()=>SendData(game, hub, now)));
            }
            Task.WaitAll(tasks.ToArray());
        }


        private void ProcedeActions(long now, Gvars gvars)
        {
            List<(long, ItemAction)> temp = new List<(long, ItemAction)>(gvars.Actions);
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

        private async Task SendData(GameControl game, IHubContext<Hubs.MyHub> hub, long now)
        {
            foreach (Gvars gvars in game.games.Values)
            {
                try
                {
                    string json = JsonConvert.SerializeObject(gvars, jsonSerializerSettings);
                    await hub?.Clients.All.SendAsync("ReceiveGvars", json, now);
                }
                catch (Exception e)
                {
                    string str = e.Message;
                }

            }
        }


    }
}