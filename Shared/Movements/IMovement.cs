
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public abstract class IMovement
    {
        public IMovement(double movementSpeed, double angle)
        {
            this.Angle = angle;
            this.MovementSpeed = movementSpeed;
        }
        public double MovementSpeed { get; set; }
        [JsonProperty]
        private double angle;
        public double Angle
        {
            get => angle;
            set
            {
                angle = value % (Math.PI * 2);
            }
        }
        public abstract (double, double) Move(double percentage);
        //what will happen every frame
        public abstract void Frame(double friction, double percentage);
        public abstract void AntiFrame(double friction);
        //change properties of this movement
        public abstract void ResetMovementAngle(double angle);
        public abstract void ResetMovementSpeed(double speed);
        /// <summary>
        /// proceed action of this movement on call (eg. player keeps on pressing arrow up)
        /// </summary>
        public abstract void UpdateMovement(double percentage);
        public abstract void AntiUpdateMovement();
        public virtual void SuddenStop()
        {
            this.MovementSpeed = 0;
        }
    }
}
