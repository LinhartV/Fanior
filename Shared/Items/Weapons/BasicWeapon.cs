
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    class BasicWeapon : Weapon
    {
        public BasicWeapon(bool autoFire, int reloadTime, int shotSpeed, double damage, int characterId) : base(autoFire, reloadTime, shotSpeed, damage, characterId)
        {

        }

        protected override void CreateShot()
        {
            //new BasicShot(damage, character, character.RotationX, new ConstantMovement(), 0, shotSpeed, character.x, character.y, character.z, ShapeCreation.CreateSphere(2f, 10, 10, Globals.TextureType.Fire, null, character.RotationX));
        }
    }
}
