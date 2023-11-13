using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    /// <summary>
    /// Class for player stats upgrade (such as movement speed etc.)
    /// </summary>
    public class Upgrades
    {
        public Upgrades(string name, string color, Action<Player> onIncrease)
        {
            this.OnIncrease = onIncrease;
            Name = name;
            Points = 0;
            Color = color;
        }
        //value, id, Gvars
        public Action<Player> OnIncrease { get; set; }
        public string Name { get; set; }
        public int Points { get; private set; }
        public void IncreasePoint(Player player)
        {
            this.Points++;
            OnIncrease(player);
        }
        public string Color { get; set; }


    }
}
