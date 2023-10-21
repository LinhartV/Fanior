using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class Nothing : Item
    {
        public Nothing():base(){}
        public Nothing(Gvars gvars, double x, double y, Shape shape, Mask mask)
            : base(gvars, x, y, shape, mask) { }

        public override void CollideClient(Item collider, double angle, Gvars gvars)
        {
            throw new NotImplementedException();
        }

        public override void CollideServer(Item collider, double angle, Gvars gvars)
        {
            throw new NotImplementedException();
        }
    }
}
