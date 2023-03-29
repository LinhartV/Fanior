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
            for (int i = 0; i < 1; i++)
            {
                if (gameId == "@@@")
                    await NewPlayer(gameControl.AddPlayer());
                else
                    await NewPlayer(gameControl.AddPlayer(gameId));
            }
        }
        public async Task NewPlayer(Gvars gvars)
        {
            await Clients.Caller.SendAsync("JoinGame", ToolsGame.CreateNewPlayer(gvars).Id, gameControl.sw.ElapsedMilliseconds);
            string json = JsonConvert.SerializeObject(gvars, ToolsGame.jsonSerializerSettings);
            await Clients.Caller.SendAsync("ReceiveGvars", json);
        }
        public void Execute(string actionMethodName, string gameId, int playerId)
        {
            Type thisType = typeof(PlayerAction);
            MethodInfo theMethod = thisType.GetMethod(actionMethodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            theMethod.Invoke(null, new object[]{ playerId, gameControl.games[gameId]});
        }
        public void ExecuteList(List<string> actionMethodNames, string gameId, int playerId)
        {
            foreach (var item in actionMethodNames)
            {
                Execute(item, gameId, playerId);
            }
        }
        //je to dobrej nápad? Nebude server přehlcenej?
        /*public async Task FetchData(string gameId, int playerId)
        {
            //nejvíc moc nejhlavnější
            await Clients.Caller.SendAsync("ReceiveData", Game.games[gameId].DataToSend);
        }*/
    }
}