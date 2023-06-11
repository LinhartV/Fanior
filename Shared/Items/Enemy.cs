using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class Enemy : Character
    {
        [JsonProperty]
        private int scoreToReturn;
        public IEnemyAI ai;
        public Enemy(Gvars gvars, double x, double y, Shape shape, IMovement defaultMovement, double movementSpeed, double acceleration, double friction, double lives, double regeneration, Weapon weapon, int bounty, IEnemyAI ai,double shield = 0, bool isVisible = true)
            : base(gvars, x, y, shape, new Mask(shape.ImageWidth, shape.ImageHeight, shape.Geometry), movementSpeed, acceleration, friction, lives, regeneration, weapon, shield, defaultMovement, isVisible)
        {
            gvars.counts[1]++;
            this.ai = ai;
            this.AddAction(new ItemAction("enemyAI", 1));
        }

        public override int Bounty()
        {
            return scoreToReturn;
        }
        public override void Dispose(Gvars gvars)
        {
            base.Dispose(gvars);
            gvars.counts[1]--;
        }
    }
}
