using Fanior.Server.Hubs;
using Fanior.Shared;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Fanior.Server.Classes
{
    public static class ServerGameLogic
    {
        public static void SetupGvarsActions(Gvars gvars, GameControl gameControl)
        {
            Random rnd = new Random();
            List<(double, Action<IHubContext<MyHub>> hub)> actionList = new();
            //Create coin
            actionList.Add(((double)rnd.Next(500, 5000), (hub) =>
            {

            }
            ));
            gameControl.gvarsAction.Add(gvars.GameId, actionList);
        }
        public static void CreateCoin(Gvars gvars, IHubContext<MyHub> hub)
        {
            new Coin();
            //hub?.Clients.All.SendAsync("ExecuteList", );
        }
    }
}
