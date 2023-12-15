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
                string json;
                Player player;
                lock (gameControl.actionLock)
                {
                    player = ToolsGame.CreateNewPlayer(gvars, Context.ConnectionId, name);
                    json = JsonConvert.SerializeObject(gvars, ToolsSystem.jsonSerializerSettings);
                }
                await Groups.AddToGroupAsync(Context.ConnectionId, gvars.GameId);
                await Clients.Caller.SendAsync("JoinGame", player.Id, json, gameControl.sw.Elapsed.TotalMilliseconds, gvars.messageId);
                await Clients.All.SendAsync("PlayerJoinGame", JsonConvert.SerializeObject(player, ToolsSystem.jsonSerializerSettings), player.Id);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " " + e.Source + " " + e.StackTrace);
            }

        }

        /// <summary>
        /// Executes player action on server and sends it to all other clients
        /// </summary>
        public void ExecuteAction(PlayerActions.PlayerActionsEnum action, bool down, string gameId, int itemId, double angle, int actionId)
        {
            if (gameControl.games[gameId].ItemsPlayers.ContainsKey(itemId))
                AddActionsToList(gameId, itemId, angle, action, down, actionId);
        }
        /// <summary>
        /// Listens to incoming messages from clients.
        /// </summary>
        public void ExecuteList(string gameId, int itemId, double angle, int messageId)
        {
            gameControl.games[gameId].ready = true;
            gameControl.clientMessageId = messageId;

            Task.Run(() => { AddActionsToList(gameId, itemId, angle); });
        }

        /// <summary>
        /// Listens to incoming messages from clients, which are then proceeded in the Frame. It also joins actions of this player in the list of his actions, which are subsequently sent to every usery (done every frame).
        /// </summary>
        private void AddActionsToList(string gameId, int itemId, double angle)
        {
            lock (gameControl.tempListsLock)
            {
                if (gameControl.tempPlayerInfo[gameId].ContainsKey(itemId))
                {
                    gameControl.tempPlayerInfo[gameId][itemId] = angle;
                }
                else
                {
                    gameControl.tempPlayerInfo[gameId].Add(itemId, angle);
                }
                if (gameControl.games[gameId].ItemsPlayers.ContainsKey(itemId))
                {
                    gameControl.games[gameId].ItemsPlayers[itemId].Angle = angle;
                }
                /*List<(PlayerActions.PlayerActionsEnum, bool)> actionMethodNames = JsonConvert.DeserializeObject<List<(PlayerActions.PlayerActionsEnum, bool)>>(actionMethodNamesJson, ToolsSystem.jsonSerializerSettings);
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
                }*/

            }
        }
        private async void AddActionsToList(string gameId, int itemId, double angle, PlayerActions.PlayerActionsEnum action, bool down, int actionId)
        {
            
                lock (gameControl.tempListsLock)
                {
                    if (gameControl.tempPlayerInfo[gameId].ContainsKey(itemId))
                    {
                        gameControl.tempPlayerInfo[gameId][itemId] = angle;
                    }
                    else
                    {
                        gameControl.tempPlayerInfo[gameId].Add(itemId, angle);
                    }
                    if (gameControl.games[gameId].ItemsPlayers.ContainsKey(itemId))
                    {
                        gameControl.games[gameId].ItemsPlayers[itemId].Angle = angle;
                    }
                    if (gameControl.tempPlayerActions[gameId].ContainsKey(itemId))
                    {
                        gameControl.tempPlayerActions[gameId][itemId].Add((action, down));
                    }
                    else
                    {
                        gameControl.tempPlayerActions[gameId].Add(itemId, new List<(PlayerActions.PlayerActionsEnum, bool)>() { (action, down) });

                    }
                }
                //Clients.All.SendAsync("ExecuteAction", action, down, itemId, angle, gameControl.games[gameId].Items[itemId].X, gameControl.games[gameId].Items[itemId].Y, actionId);
            
        }
        /// <summary>
        /// Receive chosen stat upgrade.
        /// </summary>
        public void UpgradeWeapon(string gameId, int itemId, int childNum)
        {
            Player p = gameControl.games[gameId].ItemsPlayers[itemId];
            if (p.PointsGained >= Constants.POINTS_NEEDED_TO_UPGRADE_WEAPON)
            {
                p.PointsGained -= Constants.POINTS_NEEDED_TO_UPGRADE_WEAPON;
                p.WeaponNode = p.WeaponNode.Children[childNum];
            }
        }
        /// <summary>
        /// Receive chosen stat upgrade.
        /// </summary>
        public void UpgradeStat(string gameId, int itemId, int statNum)
        {
            Player p = gameControl.games[gameId].ItemsPlayers[itemId];
            if (p.UpgradePoints > 0)
            {
                p.IncreaseStatPoint(statNum);
            }
        }
        /// <summary>
        /// Receive chosen stat upgrade.
        /// </summary>
        public void ObtainAbility(string gameId, int itemId, int abilityNum)
        {
            Player p = gameControl.games[gameId].ItemsPlayers[itemId];
            //Deep copy
            var ability = JsonConvert.DeserializeObject<Ability>(JsonConvert.SerializeObject(ToolsGame.abilities[abilityNum]));
            if (p.UpgradePoints >= ability.Cost)
            {
                if (p.AbilityE == null)
                {
                    p.AbilityE = ability;
                }
                else if (p.AbilityQ == null)
                {
                    p.AbilityQ = ability;
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
                        player.Dispose();
                        doublebreak = true;
                        break;
                    }
                }
                if (doublebreak)
                    break;
            }
            return base.OnDisconnectedAsync(exception);
        }

        /*public async Task Ping(double time)
        {
            // await Clients.All.SendAsync("Ping", time+gameControl.sw.Elapsed.TotalMilliseconds);
            gameControl.ping = true;
        }*/
    }
}