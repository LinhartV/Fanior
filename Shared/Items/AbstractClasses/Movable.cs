
using Newtonsoft.Json;
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
        // Angle where the character is "looking" (for picture, shooting and stuff)
        private double angle;
        public double Angle
        {
            get => angle;
            set
            {
                angle = value % (Math.PI * 2);
                gvars?.AddProperty(Id, ItemProperties.Angle, angle);
            }
        }
        [JsonProperty]
        private double baseSpeed;
        /// <summary>
        /// Overall speed of the item
        /// </summary>
        public double BaseSpeed
        {
            get => baseSpeed; set
            {
                baseSpeed = value;
                foreach (var movement in MovementsControlled.Values)
                {
                    movement.ResetMovementSpeed(value);
                }
            }
        }
        [JsonProperty]
        private double friction;
        public double Friction { get => friction; set => friction = Math.Abs(value); }
        private double acceleration;
        public double Acceleration
        {
            get => acceleration;
            set {
                acceleration = value;
                foreach (var movement in MovementsControlled.Values)
                {
                    if(movement is AcceleratedMovement am)
                        am.Acceleration = value;
                }
            }
        }
        //Movements once set and controlled just by the movement itself. It deletes itself when speed is 0. (e.g. explosion)
        [JsonProperty]
        protected List<IMovement> MovementsAutomated { get; set; } = new List<IMovement>();
        //Movements controlled by other actions, such as player control. Accesed by id and not deleted even when speed is 0.
        [JsonProperty]
        protected Dictionary<string, IMovement> MovementsControlled { get; set; } = new Dictionary<string, IMovement>();
        public bool ThroughSolid { get; set; } = false;
        public Movable() { }
        public Movable(Gvars gvars, double x, double y, Shape shape, Mask mask, double baseSpeed, IMovement movement, double acceleration, double friction, bool setAngle, bool isVisible = true) : base(gvars, x, y, shape, mask, isVisible)
        {
            this.Friction = friction;
            this.Acceleration = acceleration;
            this.BaseSpeed = baseSpeed;
            if (movement != null)
            {
                AddControlledMovement(movement, "default");
            }
            this.AddAction(gvars, new ItemAction("move", 1, ItemAction.ExecutionType.EveryTime, true, setAngle));
            gvars.ItemsStep.Add(Id, this);
        }
        public override void SetItemFromClient(Gvars gvars)
        {
            if (!gvars.ItemsStep.ContainsKey(Id))
            {
                gvars.ItemsStep.Add(Id, this);
            }
            base.SetItemFromClient(gvars);
            this.AddAction(gvars, new ItemAction("move", 1, ItemAction.ExecutionType.EveryTime, true, false));
        }
        /// <summary>
        /// Creates new movement which is controlled only by movement itself (deletes itself when 0)
        /// </summary>
        public void AddAutomatedMovement(IMovement movement)
        {
            MovementsAutomated.Add(movement);
        }
        /// <summary>
        /// Cretes new movement which can be controlled by other events (key press etc.)
        /// </summary>
        public void AddControlledMovement(IMovement movement, string movementName)
        {
            if (!MovementsControlled.ContainsKey(movementName))
            {
                MovementsControlled.Add(movementName, movement);
            }
        }
        /// <summary>
        /// Calls UpdateMovement of particular movement
        /// </summary>
        public void UpdateControlledMovement(string movementName, double percentage)
        {
            MovementsControlled[movementName].UpdateMovement(percentage);
        }
        public void AntiUpdateControlledMovement(string movementName)
        {
            MovementsControlled[movementName].AntiUpdateMovement();
        }
        /// <summary>
        /// Rotates particular movement or sets its angle
        /// </summary>
        /// <param name="movementName">Name of the movement to rotate</param>
        /// <param name="angleRotation">Angle to set or rotate by</param>
        /// <param name="rotate">Set true for relative rotation, set false to set the angle</param>
        public void RotateControlledMovement(string movementName, double angleRotation, bool rotate = true)
        {
            if (MovementsControlled.ContainsKey(movementName))
            {

                if (rotate)
                    MovementsControlled[movementName].Angle -= angleRotation;
                else
                    MovementsControlled[movementName].ResetMovementAngle(angleRotation);
            }
        }

        /// <summary>
        /// Stops all movements in one direction
        /// </summary>
        /// <param name="angle">angle which the movement will be stoped under</param>
        public void StopInDirection(double angle)
        {
            foreach (var movement in MovementsAutomated)
            {
                StopMovementInDirection(movement, angle);
            }
            foreach (var movement in MovementsControlled.Values)
            {
                StopMovementInDirection(movement, angle);
            }
        }
        private void StopMovementInDirection(IMovement movement, double angle)
        {
            if (Math.Abs(movement.Angle - angle) < Math.PI / 2 || Math.Abs(movement.Angle - angle) > 3 * Math.PI / 2)
            {
                movement.Angle = angle;
                movement.MovementSpeed = movement.MovementSpeed * Math.Sin(Math.PI / 2 - angle - movement.Angle);
            }
        }
        /*public void AntiMove()
        {
            List<IMovement> allMovements = new List<IMovement>(MovementsAutomated);
            allMovements.AddRange(MovementsControlled.Values);
            (double, double) xy;
            double x = 0;
            double y = 0;
            foreach (var movement in allMovements)
            {
                movement.AntiFrame(friction);
                xy = movement.Move(percentage);
                x -= xy.Item1;
                y += xy.Item2;
            }
            this.X += x;
            this.Y += y;
        }*/
        /// <summary>
        /// Move the current object based on it's movements
        /// </summary>
        /// <param name="percentage">Percentage of current frame</param>
        /// <param name="setAngle">Whether to set angle of object to match the movement angle</param>
        public void Move(double percentage, bool setAngle)
        {
            List<IMovement> allMovements = new List<IMovement>(MovementsAutomated);
            allMovements.AddRange(MovementsControlled.Values);
            (double, double) xy;
            double x = 0;
            double y = 0;
            foreach (var movement in allMovements)
            {
                movement.Frame(friction, percentage);
                xy = movement.Move(percentage); //ToolsMath.PolarToCartesian(movement.Angle, movement.MovementSpeed);
                x += xy.Item1;
                y -= xy.Item2;
            }
            this.X += x;
            this.Y += y;
            if (setAngle)
            {
                Angle = ToolsMath.GetAngleFromLengts(x, -y);
            }
            /*
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
       */
        }

        public void StopAllMovement()
        {
            MovementsAutomated.Clear();
            MovementsControlled.Clear();
        }
    }
}
