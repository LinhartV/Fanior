
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class BurningArrow : Shot
    {
        public BurningArrow(Gvars gvars, double x, double y, Shape shape, Mask mask, double movementSpeed, double damage, int characterId, double angle, double acceleration, double friction, double lifeSpan, bool isVisible = true)
            : base(gvars, x, y, shape, mask, movementSpeed, acceleration, friction, damage, characterId, angle, lifeSpan, true, isVisible)
        {
            AddAction(gvars, new ItemAction("disposeOnStop", 1, ItemAction.ExecutionType.EveryTime));
        }
        public BurningArrow() { }
        /*public override void Collide(Item collider, double angle, params Globals.ActionsAtCollision[] actionsNotToPerform)
        {
            if (collider.solid && collider.id != character.id)
            {
                Dispose();
            }
        }*/
        public override void Dispose()
        {
            if(gvars.server)
                new BurningBoulder(gvars, this.X, this.Y, new Shape("yellow","pink",2, this.Shape.ImageWidth, this.Shape.ImageWidth, Shape.GeometryEnum.circle), new Mask(this.Mask.Width, this.Mask.Height, Shape.GeometryEnum.circle), 0, this.Damage, CharacterId, 0, 0, 0.4, 100);
            base.Dispose();
        }
    }
}
