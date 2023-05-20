
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class AcceleratedMovement : IMovement
    {
        private double maxSpeed;
        private double acceleration;
        private double deceleration;
        public AcceleratedMovement(string movementName, double initialSpeed, double angle, double acceleration, double deceleration, double maxSpeed) : base(movementName, initialSpeed, angle)
        {
            this.acceleration = acceleration;
            this.deceleration = deceleration;
            this.maxSpeed = maxSpeed;
        }

        public override (double, double) Move()
        {
            MovementSpeed *= acc
            return ((double)(MovementSpeed * Math.Sin(Angle)), (double)(MovementSpeed * Math.Cos(Angle)));

        }

        public override void RenewMovement(double angle, double speed)
        {
            this.Angle = angle;
            MovementSpeed = speed;
        }

        public override void SmoothStop(double friction)
        {
            this.SuddenStop();
        }
    }
}
