
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public abstract class IMovement
    {
        public IMovement(string movementName, double movementSpeed, double angle)
        {
            this.Angle = angle;
            this.MovementSpeed = movementSpeed;
            MovementName = movementName;
        }
        public string MovementName { get; }
        public double MovementSpeed { get; set; }
        private double angle;
        public double Angle
        {
            get => angle;
            set
            {
                angle = value % (Math.PI * 2);
            }
        }
        public abstract (double, double) Move();
        public abstract void SmoothStop(double friction);
        public abstract void RenewMovement(double angle, double speed);
        public virtual void SuddenStop()
        {
            this.MovementSpeed = 0;
        }
    }
}
