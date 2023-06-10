
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Fanior.Shared;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Fanior.Shared
{
    /// <summary>
    /// Class containing game variables of particular arena
    /// </summary>
    public class Gvars
    {
        private class GvarsAction
        {
            public double repeat;
            public Action<Gvars, double> action;

            public GvarsAction(double repeat, Action<Gvars, double> action)
            {
                this.repeat = repeat;
                this.action = action;
            }
        }
        //id of sent messages
        public long messageId = 0;
        //milliseconds elapsed from the launch of server
        private long now;
        private Stopwatch sw = new Stopwatch();

        //all items
        public Dictionary<int, Item> Items { get; set; } = new Dictionary<int, Item>();
        //derived dictionaries
        public Dictionary<int, Item> ItemsStep { get; set; } = new Dictionary<int, Item>();
        public Dictionary<int, Player> ItemsPlayers { get; set; } = new Dictionary<int, Player>();

        //actions controlled by game (reference to gvars and repeat delay) with information when to be executed (such as playing music...)
        private List<(long, GvarsAction)> gameActions = new();

        //actions that players just did
        public Dictionary<int, List<(PlayerAction.PlayerActionsEnum, bool)>> PlayerActions { get; set; } = new();
        //angles of all players (id, angle)
        public Dictionary<int, (double, double, double)> PlayerInfo { get; set; } = new();

        //size of arena
        public double ArenaWidth { get; set; }
        public double ArenaHeight { get; set; }

        //Game id of this arena
        public string GameId { get; set; }

        //Id for item creation
        public int Id { get; set; } = 1;
        public Gvars(string gameId)
        {
            GameId = gameId;
            ArenaWidth = 1500;
            ArenaHeight = 1500;
        }
        public Gvars()
        { }
        public void StartMeasuringTime(long now)
        {
            this.now = now;
            sw.Start();
        }
        public long GetNow()
        {
            return now + sw.ElapsedMilliseconds;
        }
        public void AddGvarsAction(Action<Gvars, double> action, double repeat)
        {
            gameActions.Add((0, new GvarsAction(repeat, action)));
        }
        public void ExecuteActions(long now)
        {
            List<(long, GvarsAction)> tempActions = new List<(long, GvarsAction)>(gameActions);
            foreach (var action in tempActions)
            {
                if (action.Item1 < now)
                {
                    action.Item2.action(this, action.Item2.repeat);
                    gameActions.Remove(action);
                    if (action.Item2.repeat > 0)
                    {
                        gameActions.Add((now + (long)(action.Item2.repeat * Constants.FRAME_TIME), action.Item2));
                    }
                }
            }
        }
    }
}