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
using System.Threading;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata;

namespace Fanior.Server.Hubs
{
    public class MyHub : Hub
    {
        private GameControl gameControl;

        public MyHub(GameControl game)
        {
            gameControl = game;
        }
        /// <summary>
        /// Creation of new player, when he logs in.
        /// </summary>
        public async Task OnLogin(string gameId, string name)
        {
            for (int i = 0; i < 1; i++)
            {
                if (gameId == "@@@")
                    await NewPlayer(gameControl.AddPlayer(), name);
                else
                    await NewPlayer(gameControl.AddPlayer(gameId), name);
            }
        }

        public async Task NewPlayer(Gvars gvars, string name)
        {
            try
            {
                Player player = ToolsGame.CreateNewPlayer(gvars, Context.ConnectionId, name);
                string json = JsonConvert.SerializeObject(gvars, ToolsSystem.jsonSerializerSettings);

                await Groups.AddToGroupAsync(Context.ConnectionId, gvars.GameId);
                await Clients.Caller.SendAsync("JoinGame", player.Id, json, gameControl.sw.ElapsedMilliseconds, gvars.messageId);
                await Clients.All.SendAsync("PlayerJoinGame", JsonConvert.SerializeObject(player, ToolsSystem.jsonSerializerSettings), player.Id);
            }
            catch (Exception e)
            {

            }

        }

        /// <summary>
        /// Listens to incoming messages from clients, which are then proceeded in the Frame. It also joins actions of this player in the list of his actions, which are subsequently sent to every usery (done every frame).
        /// </summary>
        public void ExecuteList(string actionMethodNamesJson, string gameId, int itemId, double angle, int messageId)
        {

            gameControl.clientMessageId = messageId;

            Task.Run(async () => { AddActionsToList(gameId, itemId, angle, actionMethodNamesJson); });
        }

        private async Task AddActionsToList(string gameId, int itemId, double angle, string actionMethodNamesJson)
        {
            lock (gameControl.tempListsLock)
            {
                if (gameControl.tempPlayerInfo[gameId].ContainsKey(itemId))
                {
                    gameControl.tempPlayerInfo[gameId][itemId] = (angle, gameControl.games[gameId].ItemsPlayers[itemId].X, gameControl.games[gameId].ItemsPlayers[itemId].Y);
                }
                else
                {
                    gameControl.tempPlayerInfo[gameId].Add(itemId, (angle, gameControl.games[gameId].ItemsPlayers[itemId].X, gameControl.games[gameId].ItemsPlayers[itemId].Y));
                }
                gameControl.games[gameId].ItemsPlayers[itemId].Angle = angle;
                List<(PlayerAction.PlayerActionsEnum, bool)> actionMethodNames = JsonConvert.DeserializeObject<List<(PlayerAction.PlayerActionsEnum, bool)>>(actionMethodNamesJson, ToolsSystem.jsonSerializerSettings);
                if (actionMethodNames.Count > 0)
                {
                    if (gameControl.tempPlayerActions[gameId].ContainsKey(itemId))
                    {
                        gameControl.tempPlayerActions[gameId][itemId].AddRange(actionMethodNames);
                    }
                    else
                    {
                        gameControl.tempPlayerActions[gameId].Add(itemId, actionMethodNames);
                    }
                }
                
            }
        }
        /// <summary>
        /// Sends all GVars to caller.
        /// </summary>
        public async void SendGvars(string gameId)
        {
            string json = JsonConvert.SerializeObject(gameControl.games[gameId], ToolsSystem.jsonSerializerSettings);
            await Clients.Caller.SendAsync("ReceiveGvars", json);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            bool doublebreak = false;
            foreach (var game in gameControl.games.Values)
            {
                foreach (var player in game.ItemsPlayers.Values)
                {
                    if (player.ConnectionId == Context.ConnectionId)
                    {
                        Clients.All.SendAsync("PlayerDisconnected", player.ConnectionId);
                        player.Dispose(game);
                        doublebreak = true;
                        break;
                    }
                }
                if (doublebreak)
                    break;
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}