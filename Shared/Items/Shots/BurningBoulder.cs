
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class BurningBoulder : Shot
    {
        public BurningBoulder(Gvars gvars, double x, double y, Shape shape, Mask mask, double movementSpeed, double damage, int characterId, double angle, double acceleration, double friction, double lifeSpan, bool isVisible = true)
            : base(gvars, x, y, shape, mask, movementSpeed, acceleration, friction, damage, characterId, angle, lifeSpan, true, isVisible)
        {
            AddAction(gvars, new ItemAction("burn", 6, ItemAction.ExecutionType.EveryTime));
        }
        public BurningBoulder() { }
        /*public override void Collide(Item collider, double angle, params Globals.ActionsAtCollision[] actionsNotToPerform)
        {
            if (collider.solid && collider.id != character.id)
            {
                Dispose();
            }
        }*/
    }
}
