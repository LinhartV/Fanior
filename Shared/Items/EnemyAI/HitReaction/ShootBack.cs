﻿using System;
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
            if (gvars.Items.ContainsKey(shooterId) && gvars.Items[shooterId] is Character shooter)
            {
                var enemy = gvars.Items[characterId] as Enemy;
                enemy.Angle = ToolsMath.GetAngleFromLengts(enemy, shooter);
                enemy.RotateControlledMovement("default", enemy.Angle, false);
                if (enemy.WeaponNode != null)
                {
                    enemy.WeaponNode.Weapon.Fire(gvars);
                }
            }
        }
    }
}
