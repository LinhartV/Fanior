using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Fanior.Server.Classes;
using Fanior.Shared;
using System.Numerics;
using System.Collections.Generic;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Fanior.Server.Hubs
{
    public class MyHub : Hub
    {
        private GameControl game;

        public MyHub(GameControl game)
        {
            this.game = game;
        }

        public async Task OnLogin(string gameId)
        {
            if (gameId == "@@@")
                await NewPlayer(game.AddPlayer());
            else
                await NewPlayer(game.AddPlayer(gameId));
        }
        public async Task NewPlayer(Gvars gvars)
        {
            await Clients.Caller.SendAsync("JoinGame", ToolsGame.CreateNewPlayer(gvars).Id, game.sw.ElapsedMilliseconds);
        }
        public async Task KeyDown(string gameId, int playerId, string key)
        {
            //Commands.KeyDown(key, Game.games[gameId], Game.games[gameId].ItemsPlayers[playerId]);

        }

        //je to dobrej nápad? Nebude server přehlcenej?
        /*public async Task FetchData(string gameId, int playerId)
        {
            //nejvíc moc nejhlavnější
            await Clients.Caller.SendAsync("ReceiveData", Game.games[gameId].DataToSend);
        }*/
    }
}