
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class ConstantMovement : IMovement
    {
        public ConstantMovement(double movementSpeed, double angle) : base(movementSpeed, angle)
        {
        }

        public override void Frame(double friction)
        {
            
        }

        public override (double, double) Move()
        {
            return ((double)(MovementSpeed * Math.Sin(Angle)), (double)(MovementSpeed * Math.Cos(Angle)));

        }

        public override void ResetMovement(double angle, double speed)
        {
            this.Angle = angle;
            MovementSpeed = speed;
        }

        public override void UpdateMovement()
        {
            
        }
    }
}
