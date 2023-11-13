
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class Fist : Weapon
    {
        public Fist(double reloadTime, int shotSpeed, double damage) : base(true, reloadTime, shotSpeed, damage)
        {

        }

        public override void Fire(Gvars gvars)
        {
            
        }
    }
}
