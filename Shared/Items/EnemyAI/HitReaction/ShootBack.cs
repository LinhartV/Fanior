using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    /// <summary>
    /// Rotates to face the shooter and shoots back with it's weapon
    /// </summary>
    public class ShootBack : IHitReaction
    {
        public void React(Gvars gvars, int characterId, int shooterId)
        {
            var enemy = gvars.Items[characterId] as Enemy;
            var shooter = gvars.Items[shooterId] as Character;
            enemy.RotateControlledMovement("default", ToolsMath.GetAngleFromLengts(enemy, shooter), false);
            if (enemy.WeaponNode != null)
            {
                //enemy.WeaponNode.Weapon.Fire(gvars);
            }
        }
    }
}
