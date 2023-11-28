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
    public class Upgrade
    {
        public Upgrade(string name, string color, Action<Player> onIncrease)
        {
            this.OnIncrease = onIncrease;
            Name = name;
            Color = color;
        }

        public Action<Player> OnIncrease { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }


    }
}
