using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class RandomGoingAI : IEnemyAI
    {
        int randomX;
        int randomY;
        public void Control(Gvars gvars, Enemy enemy)
        {
            enemy.RotateControlledMovement("default", ToolsMath.GetAngleFromLengts(enemy.X - randomX, enemy.Y - randomY), false);
            if ((Math.Abs(enemy.X - randomX) < enemy.BaseSpeed && Math.Abs(enemy.Y - randomY) < enemy.BaseSpeed) || (randomX == 0 && randomY == 0))
            {
                randomX = ToolsGame.random.Next(0, (int)gvars.ArenaWidth);
                randomY = ToolsGame.random.Next(0, (int)gvars.ArenaWidth);
            }
            enemy.UpdateControlledMovement("default");
        }
    }
}
