
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class BasicShot : Shot
    {
        public BasicShot(Gvars gvars, double x, double y, Shape shape, Mask mask, double movementSpeed, double damage, int characterId, double angle, Type defalutMovement = null, bool isVisible = true)
            : base(gvars, x, y, shape, mask, movementSpeed, defalutMovement, damage, characterId, angle, isVisible)
        {
        }
        /*public override void Collide(Item collider, double angle, params Globals.ActionsAtCollision[] actionsNotToPerform)
        {
            if (collider.solid && collider.id != character.id)
            {
                Dispose();
            }
        }*/
    }
}
