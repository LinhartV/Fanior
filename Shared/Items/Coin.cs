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
        public Coin(int value, Gvars gvars, double x, double y, Shape shape, Mask mask = null) : base(gvars, x, y, shape, mask, true, false)
        {
            gvars.CountOfItems[ToolsGame.Counts.coins]++;
            this.Value = value;
        }

        public int Value { get; set; }

        public override void CollideServer(Item collider, double angle)
        {
            if (collider is Player)
            {
                this.Dispose();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            gvars.CountOfItems[ToolsGame.Counts.coins]--;
        }
    }
}
