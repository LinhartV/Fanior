
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Fanior.Shared;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static Fanior.Shared.Item;

namespace Fanior.Shared
{
    /// <summary>
    /// Class containing game variables of particular arena
    /// </summary>
    public class Gvars : ActionHandler
    {
        public Message Msg { get; } = new Message();
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

        public bool Cheating { get; set; } = false;
        public readonly object frameLock = new object();
        public readonly object addPropertyLock = new object();
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


        //actions that players just did
        public Dictionary<int, List<(PlayerActions.PlayerActionsEnum, bool)>> PlayerActions { get; set; } = new();
        //angles of all players (id, angle)
        public Dictionary<int, double> PlayersAngle { get; set; } = new();
        //properties of items that changed from previous frame
        public Dictionary<int, string> ItemInfo { get; set; } = new();

        //List of items that changed their property during this frame.
        public List<Item> justChanged = new List<Item>();
        //count of items in arena.
        public Dictionary<ToolsGame.Counts, int> CountOfItems { get; set; } = new() { { ToolsGame.Counts.coins, 0 }, { ToolsGame.Counts.enemies, 0 } };
        //size of arena
        public double ArenaWidth { get; set; }
        public double ArenaHeight { get; set; }
        //indication whether arena is fully ready
        [JsonIgnore]
        public bool ready = false;
        //Game id of this arena
        public string GameId { get; set; }

        public double PercentageOfFrame { get; set; }

        //Id for item creation
        public int Id { get; set; } = 1;

        //idea is that every property has it's "id". I'll just add new thing to dictionary with key "Y" and value for Y and I know it's Y.
        //int = id; Dictionary of ItemProperties and it's value
        private Dictionary<int, Dictionary<ItemProperties, double>> changedProperties = new();
        //for communication
        public Dictionary<int, Dictionary<ItemProperties, double>> GetProperties()
        {
            return changedProperties;
        }
        public void ClearProperties()
        {
            changedProperties.Clear();
        }
        public void AddProperty(int id, ItemProperties prop, double val)
        {
            lock (addPropertyLock)
            {
                if (server)
                {
                    if (changedProperties.ContainsKey(id))
                    {
                        if (changedProperties[id].ContainsKey(prop))
                            changedProperties[id][prop] = val;
                        else
                            changedProperties[id].Add(prop, val);
                    }
                    else
                    {
                        changedProperties.Add(id, new Dictionary<ItemProperties, double>() { { prop, val } });
                    }
                }
            }
        }
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

    }
}