using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class Nothing :Movable
    {
        public Nothing():base(){}
        public Nothing(Gvars gvars, float x, float y, Shape shape, Mask mask)
            : base(gvars, x, y, shape, mask, 5) { }
    }
}
