
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    /// <summary>
    /// class for every potencially moving object 
    /// </summary>
    public abstract class Movable : Item
    {
        /// <summary>
        /// Overall speed of the item
        /// </summary>
        public double BaseSpeed { get; set; }
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
        private double friction;
        public double Friction { get => friction; set => value = Math.Abs(friction); }
        public IMovement PartialMovement { get; set; } //movement it's moving right now by;
        public Dictionary<string, IMovement> Movements { get; set; } = new Dictionary<string, IMovement>();
        public bool ThroughSolid { get; set; } = false;
        public Movable() { }
        public Movable(Gvars gvars, double x, double y, Shape shape, Mask mask, double baseSpeed, Type defaultMovement = null, bool isVisible = true) : base(gvars, x, y, shape, mask, isVisible)
        {
            this.DefaultMovement = defaultMovement;
            this.BaseSpeed = baseSpeed;
            //this.AddAction((Item item) => { (item as Movable).movement.Move(); }, "SetMovable");
        }

        /// <summary>
        /// Creates new movement or updates existing one
        /// </summary>
        /// <param name="movementName"></param>
        /// <param name="movementSpeed"></param>
        /// <param name="angle"></param>
        /// <param name="type"></param>
        public virtual void AddMovement(string movementName, double movementSpeed, double angle, Type type = null)
        {
            if (Movements.ContainsKey(movementName))
            {
                Movements[movementName].RenewMovement(angle, movementSpeed);
            }
            else
                Movements.Add(movementName, (IMovement)Activator.CreateInstance(type == null ? defaultMovement : type, movementName, movementSpeed, angle));
        }

        /// <summary>
        /// Stops all movements in one direction
        /// </summary>
        /// <param name="angle">angle which the movement will be stoped under</param>
        public void StopInDirection(double angle)
        {
            foreach (var movement in Movements.Values)
            {
                if (Math.Abs(movement.Angle - angle) < Math.PI / 2 || Math.Abs(movement.Angle - angle) > 3 * Math.PI / 2)
                {
                    movement.Angle = angle;
                    movement.MovementSpeed = movement.MovementSpeed * Math.Sin(Math.PI/2 - angle - movement.Angle);
                }
            }
        }
        
        /* public override void Move()
         {
             PartialMovement pm = GetCurrentMovement();
             if (pm.MovementSpeed == 0)
                 return;
             double xspeed = (double)(pm.MovementSpeed * Math.Sin(pm.Angle));
             double zspeed = (double)(pm.MovementSpeed * Math.Cos(pm.Angle));
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
             Item.PartialMovement = new PartialMovement(Item.Id, "finalMovement", (double)Math.Sqrt(xspeed * xspeed + zspeed * zspeed), ToolsMath.GetAngleFromLengts(xspeed, zspeed));
         }*/

        public void StopAllMovement()
        {
            Movements.Clear();
        }
    }
}
