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
            //gameControl.games[gvars.GameId].AddAction(gameControl.games[gvars.GameId], new ItemAction("createCoin", 2, ItemAction.ExecutionType.EveryTime, false));
            //Create boss
            //gameControl.games[gvars.GameId].AddAction(gameControl.games[gvars.GameId], new ItemAction("createBoss",100,ItemAction.ExecutionType.OnlyFirstTime,false));              
        }
        
    }
}
