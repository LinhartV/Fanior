
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
            public Action<Gvars> action;

            public GvarsAction(double repeat, Action<Gvars> action)
            {
                this.repeat = repeat;
                this.action = action;
            }
        }
        public Message Msg { get;} = new Message();
        /// <summary>
        /// class for collection messages sent to client
        /// </summary>
        public class Message
        {
            public Message()
            {
            }

            public List<Item> itemsToCreate = new List<Item>();
            /// <summary>
            /// list of items to destroy by their id
            /// </summary>
            public List<int> itemsToDestroy = new List<int>();
            public List<RandomNumbers> randomNumbersList = new();

            public class RandomNumbers
            {
                public List<double> numbers;
                public string purpose;
                public int id;

                public RandomNumbers()
                {
                }

                public RandomNumbers(List<double> numbers, string purpose, int id)
                {
                    this.numbers = numbers;
                    this.purpose = purpose;
                    this.id = id;
                }
            }
            public void ClearThis()
            {
                itemsToCreate.Clear();
                itemsToDestroy.Clear();
                randomNumbersList.Clear();
            }
        }
        //id of sent messages
        public long messageId = 0;
        //milliseconds elapsed from the launch of server
        private double now;
        private Stopwatch sw = new Stopwatch();
        //if this particular Gvars are on server or client
        public bool server;
        //all items
        public Dictionary<int, Item> Items { get; set; } = new Dictionary<int, Item>();
        //derived dictionaries
        public Dictionary<int, Item> ItemsStep { get; set; } = new Dictionary<int, Item>();
        public Dictionary<int, Player> ItemsPlayers { get; set; } = new Dictionary<int, Player>();

        //actions controlled by game (reference to gvars and repeat delay) with information when to be executed (such as playing music...)
        private List<(double, GvarsAction)> gameActions = new();

        //actions that players just did
        public Dictionary<int, List<(PlayerActions.PlayerActionsEnum, bool)>> PlayerActions { get; set; } = new();
        //angles of all players (id, angle)
        public Dictionary<int, double> PlayerInfo { get; set; } = new();
        
        //count of items in arena.
        public Dictionary<ToolsGame.Counts, int> CountOfItems { get; set; } = new() { { ToolsGame.Counts.coins, 0 }, { ToolsGame.Counts.enemies, 0} };
        //size of arena
        public double ArenaWidth { get; set; }
        public double ArenaHeight { get; set; }
        //indication whether arena is fully ready
        public bool ready = false;
        //Game id of this arena
        public string GameId { get; set; }

        public double PercentageOfFrame { get; set; }

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
        public void StartMeasuringTime(double now)
        {
            this.now = now;
            sw.Start();
        }
        public double GetNow()
        {
            return now + sw.Elapsed.TotalMilliseconds;
        }
        public void AddGvarsAction(Action<Gvars> action, double repeat)
        {
            gameActions.Add((0, new GvarsAction(repeat, action)));
        }
        public void ExecuteActions(double now)
        {
            List<(double, GvarsAction)> tempActions = new List<(double, GvarsAction)>(gameActions);
            foreach (var action in tempActions)
            {
                if (action.Item1 < now)
                {
                    action.Item2.action(this);
                    gameActions.Remove(action);
                    if (action.Item2.repeat > 0)
                    {
                        gameActions.Add((now + (double)(action.Item2.repeat * Constants.GAMEPLAY_FRAME_TIME), action.Item2));
                    }
                }
            }
        }
    }
}