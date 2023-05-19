
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public abstract class Movable : Item
    {
        /// <summary>
        /// Overall speed of the item
        /// </summary>
        public float BaseSpeed { get; set; }
        private Type defaultMovement { get; set; }
        public Type DefaultMovement
        {
            get => defaultMovement;
            set
            {
                if (typeof(IMovement).IsAssignableFrom(value))
                {
                    defaultMovement = value;
                }
                else
                {
                    defaultMovement = typeof(ConstantMovement);
                }
            }
        }
        private float friction;
        public float Friction { get => friction; set => value = Math.Abs(friction); }
        public IMovement PartialMovement { get; set; } //movement it's moving right now by;
        public Dictionary<string, IMovement> Movements { get; set; } = new Dictionary<string, IMovement>();
        public bool ThroughSolid { get; set; } = false;
        public Movable() { }
        public Movable(Gvars gvars, float x, float y, Shape shape, Mask mask, float baseSpeed, Type defaultMovement = null, bool isVisible = true) : base(gvars, x, y, shape, mask, isVisible)
        {
            this.DefaultMovement = defaultMovement;
            this.BaseSpeed = baseSpeed;
            //this.AddAction((Item item) => { (item as Movable).movement.Move(); }, "SetMovable");
        }


        public virtual void AddMovement(string movementName, float movementSpeed, double angle, Type type = null)
        {
            if (Movements.ContainsKey(movementName))
            {
                Movements[movementName].MovementSpeed = movementSpeed;
                Movements[movementName].Angle = angle;
            }
            else
                Movements.Add(movementName, (IMovement)Activator.CreateInstance(type == null ? defaultMovement : type, movementName, movementSpeed, angle));
        }

        public void SmoothStop(string movementName)
        {
            if (Movements.ContainsKey(movementName))
            {
                Movements[movementName].MovementSpeed -= Friction * Movements[movementName].MovementSpeed;
                if (Movements[movementName].MovementSpeed <= 0)
                {
                    Movements.Remove(movementName);
                }
            }
        }
        public void SuddentStop(string movementName)
        {
            if (Movements.ContainsKey(movementName))
            {
                Movements.Remove(movementName);
            }
        }
        /* public override void Move()
         {
             PartialMovement pm = GetCurrentMovement();
             if (pm.MovementSpeed == 0)
                 return;
             float xspeed = (float)(pm.MovementSpeed * Math.Sin(pm.Angle));
             float zspeed = (float)(pm.MovementSpeed * Math.Cos(pm.Angle));
             //(List<double>, bool) collisionResultX = ToolsItem.CheckCollision(Item, xspeed, 0);
             //if (!collisionResultX.Item2 || Item.ThroughSolid)
             {
                 Item.X += xspeed;
             }
             //(List<double>, bool) collisionResultZ = ToolsItem.CheckCollision(Item, 0, zspeed);
             //if (!collisionResultZ.Item2 || Item.ThroughSolid)
             {
                 Item.Y += zspeed;
             }
             Item.PartialMovement = new PartialMovement(Item.Id, "finalMovement", (float)Math.Sqrt(xspeed * xspeed + zspeed * zspeed), ToolsMath.GetAngleFromLengts(xspeed, zspeed));
         }*/

        public void StopAllMovement()
        {
            Movements.Clear();
        }
    }
}
