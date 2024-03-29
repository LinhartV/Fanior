﻿
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
        public BasicWeapon(bool autoFire, double reloadTime, int shotSpeed, double damage, double lifeSpan, string name, string imageName) : base(autoFire, reloadTime, shotSpeed, damage, lifeSpan, name, imageName)
        {

        }

        protected override void CreateShot(Gvars gvars)
        {
            try
            {
                var c = gvars.Items[CharacterId] as Character;
                new BasicShot(gvars, c.X, c.Y, new Shape("lightblue", "darkblue", 2, 20, 20, Shape.GeometryEnum.circle, "rgb(255, 20, 50)", "darkred"), new Mask(20, 20, Shape.GeometryEnum.circle), this.WeaponSpeedCoef * c.BulletSpeed, DamageCoef * c.Damage, CharacterId, (gvars.Items[CharacterId] as Movable).Angle, 0, 0.4, LifeSpan);

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message + " " + e.Source + " " + e.StackTrace);
            }
        }
    }
}
