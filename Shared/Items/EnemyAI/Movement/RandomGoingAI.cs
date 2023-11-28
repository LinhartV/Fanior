using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    /// <summary>
    /// Chooses a point and goes towards it
    /// </summary>
    public class RandomGoingAI : IEnemyMovementAI
    {
        int randomX;
        int randomY;
        public void Control(Gvars gvars, Enemy enemy)
        {
            enemy.RotateControlledMovement("default", ToolsMath.GetAngleFromLengts(randomX - enemy.X, enemy.Y - randomY), false);

            if (gvars.server && (Math.Abs(enemy.X - randomX) < enemy.BaseSpeed * gvars.PercentageOfFrame && Math.Abs(enemy.Y - randomY) < enemy.BaseSpeed * gvars.PercentageOfFrame || randomX == 0 && randomY == 0))
            {
                randomX = ToolsGame.random.Next(0, (int)gvars.ArenaWidth);
                randomY = ToolsGame.random.Next(0, (int)gvars.ArenaWidth);
                gvars.Msg.randomNumbersList.Add(new Gvars.Message.RandomNumbers(new List<double>() { randomX, randomY }, "randomAI", enemy.Id));
            }
            enemy.UpdateControlledMovement("default", gvars.PercentageOfFrame);

        }
        public void ReceiveRandomNumbers(List<double> numbers)
        {
            randomX = (int)numbers[0];
            randomY = (int)numbers[1];
        }
    }
}
