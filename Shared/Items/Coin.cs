using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class Coin : Item
    {
        public Coin() : base() { }
        public Coin(int value, Gvars gvars, double x, double y, Shape shape, Mask mask = null) : base(gvars, x, y, shape, mask)
        {
            gvars.CountOfItems[ToolsGame.Counts.coins]++;
            this.Value = value;
        }

        public int Value { get; set; }

        public override void CollideServer(Item collider, double angle, Gvars gvars)
        {
            if (collider is Player)
            {
                this.Dispose(gvars);
            }
        }

        public override void Dispose(Gvars gvars)
        {
            base.Dispose(gvars);
            gvars.CountOfItems[ToolsGame.Counts.coins]--;
        }
    }
}
