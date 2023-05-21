
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
        // Angle where the character is "looking" (for picture, shooting and stuff)
        private double angle;
        public double Angle
        {
            get => angle;
            set
            {
                angle = value % (Math.PI * 2);
            }
        }
        public Weapon Weapon { get; set; }
        public Character() { }
        public Character(Gvars gvars, double x, double y, Shape shape, Mask mask, double movementSpeed, double acceleration, double friction, double lives, Weapon weapon, IMovement defaultMovement = null, bool isVisible = true) :
            base(gvars, x, y, shape, mask, movementSpeed, defaultMovement, acceleration, friction, isVisible)
        {
            this.Lives = lives;
            this.Weapon = weapon;
            weapon.characterId = this.Id;
        }
        public abstract void Death();
    }
}
