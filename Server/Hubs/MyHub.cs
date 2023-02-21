using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Fanior.Server.Classes;
using Fanior.Shared;
using System.Numerics;
using System.Collections.Generic;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Reflection;
using System;

namespace Fanior.Server.Hubs
{
    public class MyHub : Hub
    {
        private GameControl gameControl;

        public MyHub(GameControl game)
        {
            this.gameControl = game;
        }

        public async Task OnLogin(string gameId)
        {
            if (gameId == "@@@")
                await NewPlayer(gameControl.AddPlayer());
            else
                await NewPlayer(gameControl.AddPlayer(gameId));
        }
        public async Task NewPlayer(Gvars gvars)
        {
            await Clients.Caller.SendAsync("JoinGame", ToolsGame.CreateNewPlayer(gvars).Id, gameControl.sw.ElapsedMilliseconds);
        }
        public async Task Execute(string actionMethodName, string gameId, int playerId)
        {
            Type thisType = typeof(PlayerAction);
            MethodInfo theMethod = thisType.GetMethod(actionMethodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            theMethod.Invoke(null, new object[]{ playerId, gameControl.games[gameId]});
        }

        //je to dobrej nápad? Nebude server přehlcenej?
        /*public async Task FetchData(string gameId, int playerId)
        {
            //nejvíc moc nejhlavnější
            await Clients.Caller.SendAsync("ReceiveData", Game.games[gameId].DataToSend);
        }*/
    }
}