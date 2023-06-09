
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

        public override void Frame(double friction)
        {
            if (this.MovementSpeed > 0.05)
                MovementSpeed -= friction;
            else
                MovementSpeed = 0;
        }

        public override (double, double) Move()
        {
            return ToolsMath.PolarToCartesian(Angle, MovementSpeed);
        }

        public override void ResetMovement(double angle, double speed)
        {
            this.Angle = angle;
            MovementSpeed = speed;
        }

        public override void UpdateMovement()
        {
            if (MovementSpeed < maxSpeed)
            {
                MovementSpeed += acceleration;
            }
            else
            {
                MovementSpeed = maxSpeed;
            }
        }
    }
}
