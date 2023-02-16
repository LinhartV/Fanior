
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public abstract class IMovement
    {
        public IMovement(string movementName, float movementSpeed, double angle)
        {
            this.Angle = angle;
            this.MovementSpeed = movementSpeed;
            MovementName = movementName;
        }
        public string MovementName { get; }
        public float MovementSpeed { get; set; }
        private double angle;
        public double Angle
        {
            get => angle;
            set
            {
                angle = value % (Math.PI * 2);
            }
        }
        public abstract (float, float) Move();

    }
}
