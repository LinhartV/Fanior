
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    class BasicWeapon : Weapon
    {
        public BasicWeapon(Gvars gvars, bool autoFire, int reloadTime, int shotSpeed, double damage) : base(gvars, autoFire, reloadTime, shotSpeed, damage)
        {

        }

        protected override void CreateShot()
        {
            try
            {
                new BasicShot(this.gvars, gvars.Items[characterId].X, gvars.Items[characterId].Y, new Shape("lightblue", "darkblue", "lightred", "darkred", 2, 20, 20, Shape.GeometryEnum.circle), new Mask(20, 20, Shape.GeometryEnum.circle), this.shotSpeed, damage, characterId, (gvars.Items[characterId] as Character).Angle, 0, 0.2, 1000);

            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
