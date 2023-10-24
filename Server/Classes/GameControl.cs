using Fanior.Server.Hubs;
using Fanior.Shared;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Fanior.Server.Classes
{
    /// <summary>
    /// Class handling whole server
    /// </summary>
    public class GameControl
    {
        public int clientMessageId = -1;
        public readonly object actionLock = new object();
        public readonly object tempListsLock = new object();
        public readonly object creatingObjectsLock = new object();
        //number of milliseconds from the launch of server
        public Stopwatch sw = new Stopwatch();
        //all games by string (url)
        public Dictionary<string, Gvars> games = new Dictionary<string, Gvars>();
        public ManualResetEvent mre = new ManualResetEvent(false);
        //gvarsId, ItemId, (angle, x, y)
        public Dictionary<string, Dictionary<int, double>> tempPlayerInfo = new();
        //gvarsId, ItemId, (action, pressed/released)   
        public Dictionary<string, Dictionary<int, List<(PlayerActions.PlayerActionsEnum, bool)>>> tempPlayerActions = new();
        //gvars id, action (when to execute by random (actual time, lower and upper bound), action itself)
        public Dictionary<string, List<(double, int, int, Action<IHubContext<MyHub>>)>> gvarsActions = new();
        //control
        public int controlCount;
        public GameControl()
        {
            sw.Start();
            PlayerActions.SetupActions();
            LambdaActions.SetupLambdaActions();
        }
        //adds new player to concrete arena if possible
        public Gvars AddPlayer(string gameId)
        {
            
            if (!games.ContainsKey(gameId) || !(games[gameId].ItemsPlayers.Count < Constants.PLAYERS_LIMIT))
            {
                return AddPlayer();
            }
            else
            {
                return games[gameId];
            }
        }
        //adds new player to any arena
        public Gvars AddPlayer()
        {
            foreach (Gvars gvars in games.Values)
            {
                if (gvars.ItemsPlayers.Count < Constants.PLAYERS_LIMIT)
                    return gvars;
            }
            games["someId"] = new Gvars("someId");
            games["someId"].StartMeasuringTime(sw.Elapsed.TotalMilliseconds);
            games["someId"].server = true;
            tempPlayerInfo.Add("someId", new Dictionary<int, double>());
            tempPlayerActions.Add("someId", new Dictionary<int, List<(PlayerActions.PlayerActionsEnum, bool)>>());
            gvarsActions.Add("someId", new List<(double, int, int, Action<IHubContext<MyHub>>)>());
            ServerGameLogic.SetupGvarsActions(games["someId"], this);
            return games["someId"];
        }
    }
}