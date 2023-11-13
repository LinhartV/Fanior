
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class BasicWeapon : Weapon
    {
        public BasicWeapon(bool autoFire, double reloadTime, int shotSpeed, double damage) : base(autoFire, reloadTime, shotSpeed, damage)
        {

        }

        protected override void CreateShot(Gvars gvars)
        {
            try
            {
                new BasicShot(gvars, gvars.Items[CharacterId].X, gvars.Items[CharacterId].Y, new Shape("lightblue", "darkblue", 2, 20, 20, Shape.GeometryEnum.circle, "rgb(255, 20, 50)", "darkred"), new Mask(20, 20, Shape.GeometryEnum.circle), this.WeaponSpeed, Damage, CharacterId, (gvars.Items[CharacterId] as Movable).Angle, 0, 0.2, 60);

            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
