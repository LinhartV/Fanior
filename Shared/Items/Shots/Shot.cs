using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public abstract class Shot : Movable
    {
        public int CharacterId { get; set; }
        public double Damage { get; set; }
        public Shot(Gvars gvars, double x, double y, Shape shape, Mask mask, double movementSpeed, double acceleration, double friction, double damage, int characterId, double angle, int lifeSpan, bool isVisible = true)
            : base(gvars, x, y, shape, mask, movementSpeed, null, acceleration, friction, isVisible)
        {
            this.Damage = damage;
            ThroughSolid = true;
            this.CharacterId = characterId;
            Solid = false;
            AddMovement(movementSpeed, angle, acceleration);
            this.AddAction(new ItemAction(() =>
            {
                this.Dispose(gvars);
            }, lifeSpan, ItemAction.ExecutionType.OnlyFirstTime), "bulletDeletion");
        }
        /*public override void Collide(Item collider, double angle, params Globals.ActionsAtCollision[] actionsNotToPerform)
        {
            base.Collide(collider, angle, actionsNotToPerform);
        }*/
        protected virtual void AddMovement(double movementSpeed, double angle, double acceleration)
        {
            this.AddAutomatedMovement(new AcceleratedMovement(movementSpeed, angle, acceleration, movementSpeed));
        }
    }
}
