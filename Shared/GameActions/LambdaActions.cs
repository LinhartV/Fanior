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
        /// Dictionary of all actions. Key - name, Value - Action(gvars, id of item)
        /// </summary>
        private static Dictionary<string, Action<Gvars, int>> lambdaActions = new();
        public static void setupLambdaActions()
        {
            lambdaActions.Add("move", (gvars, id) => { (gvars.Items[id] as Movable).Move();});
            lambdaActions.Add("fire", (gvars, id) => {
                Console.WriteLine("just");
            });
            lambdaActions.Add("fire1", (gvars, id) =>
            {
                Character character= gvars.Items[id] as Character;
                if (!character.Weapon.reloaded)
                {
                    character.DeleteAction("fire");
                    character.Weapon.reloaded = true;
                }
                else
                {
                    character.Weapon.Fire();
                }

            });
            lambdaActions.Add("fire2", (gvars, id) =>
            {
                (gvars.Items[id] as Character).Weapon.reloaded = true;
            });
            lambdaActions.Add("up", (gvars, id) =>
            {
                (gvars.Items[id] as Player).UpdateControlledMovement("up");
            });
            lambdaActions.Add("down", (gvars, id) =>
            {
                (gvars.Items[id] as Player).UpdateControlledMovement("down");
            });
            lambdaActions.Add("right", (gvars, id) =>
            {
                (gvars.Items[id] as Player).UpdateControlledMovement("right");
            });
            lambdaActions.Add("left", (gvars, id) =>
            {
                (gvars.Items[id] as Player).UpdateControlledMovement("left");
            });
            lambdaActions.Add("dispose", (gvars, id) =>
            {
                gvars.Items[id].Dispose(gvars);
            });
        }
        public static void executeAction(string actionName, Gvars gvars, int id)
        {
            lambdaActions[actionName](gvars, id);
        }
    }
}
