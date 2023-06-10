using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Fanior.Shared.PlayerAction;

namespace Fanior.Shared
{
    /// <summary>
    /// Class for every Action delegate used in the game (for the case of JSON)
    /// </summary>
    public class LambdaActions
    {
        /// <summary>
        /// Dictionary of all actions. Key - name, Value - Action(gvars, id of item), AntiAction for reversing frames
        /// </summary>
        private static Dictionary<string, (Action<Gvars, int>, Action<Gvars, int>)> lambdaActions = new();

        public static void setupLambdaActions()
        {
            lambdaActions.Add("move", ((gvars, id) =>
            {
                (gvars.Items[id] as Movable).Move();
            }, (gvars, id) =>
            {
                (gvars.Items[id] as Movable).AntiMove();
            }
            ));

            lambdaActions.Add("fire1", ((gvars, id) =>
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

            }, (gvars, id) =>
            {

            }
            ));
            lambdaActions.Add("fire2", ((gvars, id) =>
            {
                (gvars.Items[id] as Character).Weapon.reloaded = true;
            }, (gvars, id) =>
            {
                (gvars.Items[id] as Character).Weapon.reloaded = false;
            }
            ));
            lambdaActions.Add("up", ((gvars, id) =>
            {
                (gvars.Items[id] as Player).UpdateControlledMovement("up");
            }, (gvars, id) =>
            {
                (gvars.Items[id] as Player).AntiUpdateControlledMovement("up");
            }
            ));
            lambdaActions.Add("down", ((gvars, id) =>
            {
                (gvars.Items[id] as Player).UpdateControlledMovement("down");
            }, (gvars, id) =>
            {
                (gvars.Items[id] as Player).AntiUpdateControlledMovement("down");
            }
            ));
            lambdaActions.Add("right", ((gvars, id) =>
            {
                (gvars.Items[id] as Player).UpdateControlledMovement("right");
            }, (gvars, id) =>
            {
                (gvars.Items[id] as Player).AntiUpdateControlledMovement("right");
            }
            ));
            lambdaActions.Add("left", ((gvars, id) =>
            {
                (gvars.Items[id] as Player).UpdateControlledMovement("left");
            }, (gvars, id) =>
            {
                (gvars.Items[id] as Player).AntiUpdateControlledMovement("left");
            }
            ));
            lambdaActions.Add("dispose", ((gvars, id) =>
            {
                gvars.Items[id].Dispose(gvars);
            }, (gvars, id) =>
            {
                //Antidispose?
            }
            ));
        }
        public static void executeAction(string actionName, Gvars gvars, int id)
        {
            lambdaActions[actionName].Item1(gvars, id);
        }
        public static void executeAntiAction(string actionName, Gvars gvars, int id)
        {
            lambdaActions[actionName].Item2(gvars, id);
        }
    }
}
