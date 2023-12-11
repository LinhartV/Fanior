
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class AcceleratedMovement : IMovement
    {
        [JsonProperty]
        private double maxSpeed;
        [JsonProperty]
        public double Acceleration { get; set; }
        /// <summary>
        /// Movement constantly slowing by friction but accelerates when updated
        /// </summary>
        public AcceleratedMovement(double initialSpeed, double angle, double acceleration, double maxSpeed) : base(initialSpeed, angle)
        {
            this.Acceleration = acceleration;
            this.maxSpeed = maxSpeed;
        }
        /// <summary>
        /// Only slowing movement
        /// </summary>
        public AcceleratedMovement(double initialSpeed, double angle) : base(initialSpeed, angle)
        {
            this.Acceleration = 0;
            this.maxSpeed = initialSpeed;
        }
        public AcceleratedMovement():base(){}

        public override void AntiFrame(double friction)
        {
            MovementSpeed += friction;
        }

        public override void AntiUpdateMovement()
        {
            if (MovementSpeed > 0)
            {
                MovementSpeed -= Acceleration;
            }
            else
            {
                MovementSpeed = 0;
            }
        }

        public override bool Frame(double friction, double percentage)
        {
            if (this.MovementSpeed > 0.05)
            {
                MovementSpeed -= friction * percentage;
                return false;
            }
            else
            {
                MovementSpeed = 0;
                return true;
            }
        }

        public override (double, double) Move(double percentage)
        {
            return ToolsMath.PolarToCartesian(Angle, MovementSpeed * percentage);
        }

        public override void ResetMovementAngle(double angle)
        {
            this.Angle = angle;
        }
        public override void ResetMovementSpeed(double speed)
        {
            maxSpeed = speed;
        }

        public override void UpdateMovement(double percentage)
        {
            if (MovementSpeed < maxSpeed)
            {
                MovementSpeed += Acceleration * percentage;
            }
            else
            {
                MovementSpeed = maxSpeed;
            }
        }
    }
}
