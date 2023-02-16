
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class ConstantMovement : IMovement
    {
        public ConstantMovement(string movementName, float movementSpeed, double angle) : base(movementName, movementSpeed, angle)
        {
        }

        public override (float, float) Move()
        {
            return ((float)(MovementSpeed * Math.Sin(Angle)), (float)(MovementSpeed * Math.Cos(Angle)));

        }
    }
}
