
using System.Collections.Generic;
using Fanior.Shared;

namespace Fanior.Shared
{
    public class Gvars
    {
        public Dictionary<int, Item> Items { get; set; } = new Dictionary<int, Item>();
        //derived dictionaries
        public Dictionary<int, Item> ItemsStep { get; set; } = new Dictionary<int, Item>();
        public Dictionary<int, Player> ItemsPlayers { get; set; } = new Dictionary<int, Player>();
        public List<(long, ItemAction)> Actions { get; set; } = new();
        //public string DataToSend { get; set; }

        public float ArenaWidth { get; set; }
        public float ArenaHeight { get; set; }

        //Game id
        public string GameId { get; set; }

        //Id for item creation
        public int Id { get; set; } = 1;
        public Gvars(string gameId)
        {
            GameId = gameId;
            ArenaWidth = 1500;
            ArenaHeight = 1500;
            //broadcast1;broadcast2...;@;playerId1;x;y;playerId2;x;y;... 
        }
        public Gvars()
        { }
    }
}