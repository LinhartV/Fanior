using Fanior.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fanior.Server.Classes
{
    /// <summary>
    /// Class handling whole server
    /// </summary>
    public class GameControl
    {
        public readonly object actionLock = new object();
        //number of milliseconds from the launch of server
        public Stopwatch sw = new Stopwatch();
        //all games by string (url)
        public Dictionary<string, Gvars> games = new Dictionary<string, Gvars>();
       
        public GameControl()
        {
            sw.Start();
            PlayerAction.SetupActions();
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
            games["someId"] = new Gvars("someId", sw.ElapsedMilliseconds);
            return games["someId"];
        }
    }
}