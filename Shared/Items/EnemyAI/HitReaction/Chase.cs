using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    /// <summary>
    /// Sprint in attackers direction
    /// </summary>
    public class Chase : IHitReaction
    {
        public void React(Gvars gvars, int characterId, int shooterId)
        {
            if (gvars.Items.ContainsKey(shooterId) && gvars.Items[shooterId] is Character shooter)
            {
                var enemy = gvars.Items[characterId] as Enemy;
                enemy.Angle = ToolsMath.GetAngleFromLengts(enemy, shooter);
                enemy.AddControlledMovement(new AcceleratedMovement(0, enemy.Angle, 3, 40), "chase");
                enemy.RotateControlledMovement("chase", enemy.Angle, false);
                enemy.AddAction(gvars, new ItemAction("chase", 1, ItemAction.ExecutionType.EveryTime));
                enemy.AddAction(gvars, new ItemAction("stopChase", 20, ItemAction.ExecutionType.OnlyFirstTime), 0, ActionHandler.RewriteEnum.Ignore);
            }
        }
    }
}
