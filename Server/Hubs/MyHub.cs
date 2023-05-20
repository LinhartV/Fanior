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
        /// <summary>
        /// Creation of new player, when he logs in.
        /// </summary>
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
            try
            {
                Player player = ToolsGame.CreateNewPlayer(gvars, Context.ConnectionId);
                string json = JsonConvert.SerializeObject(gvars, ToolsSystem.jsonSerializerSettings);

                await Groups.AddToGroupAsync(Context.ConnectionId, gvars.GameId);
                await Clients.Caller.SendAsync("JoinGame", player.Id, json, gameControl.sw.ElapsedMilliseconds);
                await Clients.All.SendAsync("PlayerJoinGame", JsonConvert.SerializeObject(player, ToolsSystem.jsonSerializerSettings), player.Id);
            }
            catch (Exception e)
            {

            }
           
        }
        
        /// <summary>
        /// Listens to incoming messages from clients, which are then proceeded in the Frame. It also joins actions of this player in the list of his actions, which are subsequently sent to every usery (done every frame).
        /// </summary>
        public void ExecuteList(string actionMethodNamesJson, string gameId, int itemId)
        {
            List<(PlayerAction.PlayerActionsEnum, bool)> actionMethodNames = JsonConvert.DeserializeObject<List<(PlayerAction.PlayerActionsEnum, bool)>>(actionMethodNamesJson, ToolsSystem.jsonSerializerSettings);
            gameControl.games[gameId].PlayerActions.Add(itemId, actionMethodNames);
        }
        /// <summary>
        /// Sends all GVars to caller.
        /// </summary>
        public async void SendGvars(string gameId)
        {
            string json = JsonConvert.SerializeObject(gameControl.games[gameId], ToolsSystem.jsonSerializerSettings);
            await Clients.Caller.SendAsync("ReceiveGvars", json);
        }
    }
}