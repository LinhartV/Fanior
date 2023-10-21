using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Fanior.Shared.PlayerActions;

namespace Fanior.Shared
{
    /// <summary>
    /// Class for every Action delegate used in the game (for the case of JSON)
    /// </summary>
    public class LambdaActions
    {
        /// <summary>
        /// Dictionary of actions. Key - name, Value - Action(gvars, id of item)
        /// </summary>
        private static Dictionary<string, Action<Gvars, int>> lambdaActions = new();
        

        public static void SetupLambdaActions()
        {

            lambdaActions.Add("fire1", (gvars, id) =>
            {
                Character character = gvars.Items[id] as Character;
                if (!character.Weapon.reloaded)
                {
                    character.DeleteAction("fire");
                    character.Weapon.reloaded = true;
                }
                else
                {
                    character.Weapon.Fire(gvars);
                }

            }
            );
            lambdaActions.Add("fire2", ((gvars, id) => { (gvars.Items[id] as Character).Weapon.reloaded = true; }));
            lambdaActions.Add("dispose", ((gvars, id) => { gvars.Items[id].Dispose(gvars); }));

            lambdaActions.Add("move", (gvars, id) =>
            {
                (gvars.Items[id] as Movable).Move(gvars.PercentageOfFrame);
            }
            );
            lambdaActions.Add("up", ((gvars, id) => { (gvars.Items[id] as Player).UpdateControlledMovement("up", gvars.PercentageOfFrame); }));
            lambdaActions.Add("down", ((gvars, id) => { (gvars.Items[id] as Player).UpdateControlledMovement("down", gvars.PercentageOfFrame); }));
            lambdaActions.Add("right", ((gvars, id) => { (gvars.Items[id] as Player).UpdateControlledMovement("right", gvars.PercentageOfFrame); }));
            lambdaActions.Add("left", ((gvars, id) => { (gvars.Items[id] as Player).UpdateControlledMovement("left", gvars.PercentageOfFrame); }));

            lambdaActions.Add("regenerate", ((gvars, id) => { ILived l = gvars.Items[id] as ILived; if (l.GetCurLives() > 0) { l.ChangeCurLives(l.Regeneration*gvars.PercentageOfFrame, null, gvars); } }));
            lambdaActions.Add("outsideArena", ((gvars, id) =>
            {
                Player player = gvars.Items[id] as Player;
                if (player.X < 0 || player.X > gvars.ArenaWidth || player.Y > gvars.ArenaHeight || player.Y < 0)
                {
                    player.ChangeCurLives(-1*gvars.PercentageOfFrame, null, gvars);
                }
            }));

            lambdaActions.Add("enemyAI", ((gvars, id) => { (gvars.ItemsStep[id] as Enemy).ai.Control(gvars, gvars.ItemsStep[id] as Enemy); }));

        }
        public static void executeAction(string actionName, Gvars gvars, int id)
        {
            lambdaActions[actionName](gvars, id);
        }
    }
}
