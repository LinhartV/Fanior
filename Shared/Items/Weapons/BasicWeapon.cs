
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
        public BasicWeapon(bool autoFire, double reloadTime, int shotSpeed, double damage) : base(autoFire, reloadTime, shotSpeed, damage)
        {

        }

        protected override void CreateShot(Gvars gvars)
        {
            try
            {
                new BasicShot(gvars, gvars.Items[characterId].X, gvars.Items[characterId].Y, new Shape("lightblue", "darkblue", "lightred", "darkred", 2, 20, 20, Shape.GeometryEnum.circle), new Mask(20, 20, Shape.GeometryEnum.circle), this.shotSpeed, damage, characterId, (gvars.Items[characterId] as Character).Angle, 0, 0.2, 60);

            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
