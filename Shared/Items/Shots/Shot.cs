using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public abstract class Shot : Movable
    {
        public int CharacterId { get; set; }
        public double Damage { get; set; }
        public Shot(Gvars gvars, double x, double y, Shape shape, Mask mask, double movementSpeed, IMovement movement, double acceleration, double friction, double damage, int characterId, double angle, bool isVisible = true)
            : base(gvars, x, y, shape, mask, movementSpeed, movement, acceleration, friction, isVisible)
        {
            this.Damage = damage;
            ThroughSolid = true;
            this.CharacterId = characterId;
            Solid = false;
            this.AddAutomatedMovement(new AcceleratedMovement(7, angle, 0, 7));
            this.AddAction(new ItemAction((Item item, ItemAction itemAction) => { this.Dispose(); },30, false), "bulletDeletion");
        }
        /*public override void Collide(Item collider, double angle, params Globals.ActionsAtCollision[] actionsNotToPerform)
        {
            base.Collide(collider, angle, actionsNotToPerform);
        }*/
    }
}
