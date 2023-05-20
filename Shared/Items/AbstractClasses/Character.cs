
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public abstract class Character : Movable
    {
        private double lives { get; set; }
        public double Lives
        {
            get => lives; set
            {
                lives = value; if (lives <= 0)
                {
                    Death();
                }
            }
        }
        public Weapon Weapon { get; set; }
        public Character() { }
        public Character(Gvars gvars, double x, double y, Shape shape, Mask mask, double movementSpeed, double acceleration, double friction, double lives, IMovement defaultMovement = null, bool isVisible = true) :
            base(gvars, x, y, shape, mask, movementSpeed, defaultMovement, acceleration, friction, isVisible)
        {
            this.Lives = lives;
        }
        public abstract void Death();
    }
}
