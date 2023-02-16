using Fanior.Shared;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fanior.Server.Classes
{
    public class GameControl
    {

        public Stopwatch sw = new Stopwatch();
        public Dictionary<string, Gvars> games = new Dictionary<string, Gvars>();
        public GameControl()
        {
            sw.Start();
        }
        public Gvars AddPlayer(string gameId)
        {
            
            if (!games.ContainsKey(gameId))
            {
                return AddPlayer();
            }
            else
            {
                if (games[gameId].ItemsPlayers.Count < Constants.PLAYERS_LIMIT)
                    return games[gameId];
                else
                {
                    games["someId"] = new Gvars("someId");
                    return games["someId"];
                }
            }
        }
        public Gvars AddPlayer()
        {
            foreach (Gvars gvars in games.Values)
            {
                if (gvars.ItemsPlayers.Count < Constants.PLAYERS_LIMIT)
                    return gvars;
            }
            games["someId"] = new Gvars("someId");
            return games["someId"];
        }
    }
}