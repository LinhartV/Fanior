
using System.Collections.Generic;
using Fanior.Shared;
using Newtonsoft.Json;

namespace Fanior.Shared
{
    /// <summary>
    /// Class containing game variables of particular arena
    /// </summary>
    public class Gvars
    {
        //id of sent messages
        public long messageId = 0;
        //milliseconds elapsed from the launch of server
        public long now;

        //all items
        public Dictionary<int, Item> Items { get; set; } = new Dictionary<int, Item>();
        //derived dictionaries
        public Dictionary<int, Item> ItemsStep { get; set; } = new Dictionary<int, Item>();
        public Dictionary<int, Player> ItemsPlayers { get; set; } = new Dictionary<int, Player>();

        //actions of every item in the game with information when to be executed
        public List<(long, ItemAction)> ItemActions { get; set; } = new();
        //actions that players just did
        public Dictionary<int, List<(PlayerAction.PlayerActionsEnum, bool)>> PlayerActions { get; set; } = new();
        
        //size of arena
        public double ArenaWidth { get; set; }
        public double ArenaHeight { get; set; }

        //Game id
        public string GameId { get; set; }

        //Id for item creation
        public int Id { get; set; } = 1;
        public Gvars(string gameId, long now)
        {
            this.now = now;
            GameId = gameId;
            ArenaWidth = 500;
            ArenaHeight = 500;
        }
        public Gvars()
        { }
    }
}