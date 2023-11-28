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
        /// <summary>
        /// Id of play that created this shot
        /// </summary>
        public int CharacterId { get; set; }
        public double Damage { get; set; }

        public Shot() { }
        public Shot(Gvars gvars, double x, double y, Shape shape, Mask mask, double movementSpeed, double acceleration, double friction, double damage, int characterId, double angle, double lifeSpan, bool setAngle,bool isVisible = true)
            : base(gvars, x, y, shape, mask, movementSpeed, null, acceleration, friction, setAngle,isVisible)
        {
            this.Damage = damage;
            ThroughSolid = true;
            this.CharacterId = characterId;
            Solid = false;
            this.AddAutomatedMovement(new AcceleratedMovement(movementSpeed, angle, acceleration, movementSpeed));
            this.AddAction(gvars, new ItemAction("dispose", lifeSpan, ItemAction.ExecutionType.OnlyFirstTime), "dispose");
        }
        public Shot(Gvars gvars, double x, double y, Shape shape, Mask mask, double baseSpeed, IMovement movement, double acceleration, double friction, double damage, int characterId, double angle, double lifeSpan, bool setAngle, bool isVisible = true)
            : base(gvars, x, y, shape, mask, baseSpeed, movement, acceleration, friction, setAngle, isVisible)
        {
            this.Damage = damage;
            ThroughSolid = true;
            this.CharacterId = characterId;
            Solid = false;
            this.AddAction(gvars, new ItemAction("dispose", lifeSpan, ItemAction.ExecutionType.OnlyFirstTime), "dispose");
        }
        public override void CollideServer(Item collider, double angle)
        {
            if ((collider.Solid || collider is Character) && collider.Id != this.CharacterId)
            {
                this.Dispose();
            }
        }
    }
}
