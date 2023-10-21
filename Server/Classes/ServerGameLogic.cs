using Fanior.Server.Hubs;
using Fanior.Shared;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Fanior.Server.Classes
{
    public static class ServerGameLogic
    {
        
        public static void SetupGvarsActions(Gvars gvars, GameControl gameControl)
        {

            //Create coin
           /* gameControl.gvarsActions[gvars.GameId].Add((gameControl.sw.ElapsedMilliseconds+1000, 5000, 10000, (hub) =>
            {
                if (gvars.counts[0] < 12)
                {
                    Coin c;
                    if (ToolsGame.random.NextDouble() < 0.5)
                        c = new Coin(10, gvars, (double)(ToolsGame.random.NextDouble() * gvars.ArenaWidth), (double)(ToolsGame.random.NextDouble() * gvars.ArenaHeight), new Shape("yellow", "black", 2, 15, 15, Shape.GeometryEnum.circle));
                    else
                        c = new Coin(20, gvars, (double)(ToolsGame.random.NextDouble() * gvars.ArenaWidth), (double)(ToolsGame.random.NextDouble() * gvars.ArenaHeight), new Shape("orange", "black", 2, 17, 17, Shape.GeometryEnum.circle));

                    //hub?.Clients.All.SendAsync("CreateNewItem", JsonConvert.SerializeObject(c, ToolsSystem.jsonSerializerSettings));
                }
            }
            ));*/
            //Create boss
            gameControl.gvarsActions[gvars.GameId].Add((gameControl.sw.ElapsedMilliseconds + 1000, 2000, 5000, (hub) =>
            {
                lock (gameControl.creatingObjectsLock)
                {
                    if (gvars.CountOfItems[ToolsGame.Counts.enemies] < 1)
                    {
                        Enemy e = new Enemy(gvars, (double)gvars.ArenaWidth / 2, (double)gvars.ArenaHeight / 2, new Shape("black", "black", 5, 300, 300, Shape.GeometryEnum.circle), new AcceleratedMovement(2, 0, 0.05, 5), 5, 0.05, 0, 200, 1, null, 2000, new RandomGoingAI(), 100);

                    }
                }
            }
            ));
        }
        public static void ExecuteActions(long now, GameControl gameControl, Gvars gvars, IHubContext<MyHub> hub)
        {
            if (!gvars.ready)
            {
                return;
            }
            var tempActions = new List<(long, int, int, Action<IHubContext<MyHub>>)>(gameControl.gvarsActions[gvars.GameId]);
            int newTime = 0;
            foreach (var action in tempActions)
            {
                if (action.Item1 < now)
                {
                    action.Item4(hub);
                    newTime = ToolsGame.random.Next(action.Item2, action.Item3);
                    gameControl.gvarsActions[gvars.GameId].Remove(action);
                    gameControl.gvarsActions[gvars.GameId].Add((now + (long)newTime, action.Item2, action.Item3, action.Item4));
                }
            }
        }
    }
}
