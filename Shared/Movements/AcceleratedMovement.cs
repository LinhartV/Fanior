
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
        private double acceleration;
        public AcceleratedMovement(double initialSpeed, double angle, double acceleration, double maxSpeed) : base(initialSpeed, angle)
        {
            this.acceleration = acceleration;
            this.maxSpeed = maxSpeed;
        }

        public override void AntiFrame(double friction)
        {
            MovementSpeed += friction;
        }

        public override void AntiUpdateMovement()
        {
            if (MovementSpeed > 0)
            {
                MovementSpeed -= acceleration;
            }
            else
            {
                MovementSpeed = 0;
            }
        }

        public override void Frame(double friction, double percentage)
        {
            if (this.MovementSpeed > 0.05)
                MovementSpeed -= friction * percentage;
            else
                MovementSpeed = 0;
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
                MovementSpeed += acceleration * percentage;
            }
            else
            {
                MovementSpeed = maxSpeed;
            }
        }
    }
}
