
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class ConstantMovement : IMovement
    {
        private double baseSpeed = 0;
        private int stopMovement = 0;
        public ConstantMovement(double movementSpeed, double angle) : base(movementSpeed, angle)
        {
            baseSpeed = movementSpeed;
        }

        public override void Frame(double friction)
        {
            stopMovement++;
            if (stopMovement > 1)
            {
                MovementSpeed = 0;
            }
            else
            {
                MovementSpeed = baseSpeed;
            }
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
            stopMovement = 0;
        }
    }
}
