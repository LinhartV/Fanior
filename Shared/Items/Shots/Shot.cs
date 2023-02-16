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
        public float Damage { get; set; }
        public Shot(Gvars gvars, float x, float y, Shape shape, Mask mask, float movementSpeed, Type movement, float damage, int characterId, double angle, bool isVisible = true)
            : base(gvars, x, y, shape, mask, movementSpeed, movement, isVisible)
        {
            this.Damage = damage;
            ThroughSolid = true;
            this.CharacterId = characterId;
            Solid = false;
            //movement.AddMovement("bulletMovement", movementSpeed, angle);
        }
        /*public override void Collide(Item collider, double angle, params Globals.ActionsAtCollision[] actionsNotToPerform)
        {
            base.Collide(collider, angle, actionsNotToPerform);
        }*/
    }
}
