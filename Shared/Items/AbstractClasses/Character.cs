
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public abstract class Character : Movable
    {
        private float lives { get; set; }
        public float Lives
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
        public Character(Gvars gvars, float x, float y, Shape shape, Mask mask, float movementSpeed, float lives, Type defaultMovement = null, bool isVisible = true) :
            base(gvars, x, y, shape, mask, movementSpeed, defaultMovement, isVisible)
        {
            this.Lives = lives;
        }
        public abstract void Death();
    }
}
