
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class Bomb : Shot
    {
        private double damageCoef;
        public Bomb(Gvars gvars, double x, double y, Shape shape, Mask mask, double movementSpeed, double damage, int characterId, double angle, double acceleration, double friction, double lifeSpan, bool isVisible = true)
            : base(gvars, x, y, shape, mask, movementSpeed, acceleration, friction, damage, characterId, angle, lifeSpan, true, isVisible)
        {
            this.damageCoef = (gvars.Items[CharacterId] as Character).Damage;
        }
        public Bomb() {}

        public override void Dispose()
        {
            if (gvars.server)
            {
                for (int i = 0; i < 12; i++)
                {
                    new BasicShot(gvars, this.X, this.Y, new Shape("lightblue", "darkblue", 2, 10, 10, Shape.GeometryEnum.circle, "rgb(255, 20, 50)", "darkred"), new Mask(10, 10, Shape.GeometryEnum.circle), 20, damageCoef * 20, -1, 2 * Math.PI * i / 12, 0, 0.5, 60);
                }
            }
            base.Dispose();
        }
    }
}
