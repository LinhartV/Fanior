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
        delegate void LambdaAction(Gvars gvars, int id, params object[] parameters);
        /// <summary>
        /// Dictionary of actions. Key - name, Value - Action(gvars, id of item)
        /// </summary>
        private static Dictionary<string, LambdaAction> lambdaActions = new();


        public static void SetupLambdaActions()
        {

            lambdaActions.Add("fire1", (gvars, id, parameters) =>
            {
                Character character = gvars.Items[id] as Character;
                if (!character.Weapon.Reloaded)
                {
                    character.DeleteAction("fire");
                    character.Weapon.Reloaded = true;
                }
                else
                {
                    character.Weapon.Fire(gvars);
                }

            }
            );
            lambdaActions.Add("fire2", ((gvars, id, parameters) => { (gvars.Items[id] as Character).Weapon.Reloaded = true; }));
            lambdaActions.Add("dispose", ((gvars, id, parameters) => { gvars.Items[id].Dispose(); }));

            lambdaActions.Add("move", (gvars, id, parameters) =>
            {
                (gvars.Items[id] as Movable).Move(gvars.PercentageOfFrame, parameters.Length > 0 ? (bool)parameters[0] : false);
            }
            );
            lambdaActions.Add("up", ((gvars, id, parameters) => { (gvars.Items[id] as Player).UpdateControlledMovement("up", gvars.PercentageOfFrame); }));
            lambdaActions.Add("down", ((gvars, id, parameters) => { (gvars.Items[id] as Player).UpdateControlledMovement("down", gvars.PercentageOfFrame); }));
            lambdaActions.Add("right", ((gvars, id, parameters) => { (gvars.Items[id] as Player).UpdateControlledMovement("right", gvars.PercentageOfFrame); }));
            lambdaActions.Add("left", ((gvars, id, parameters) => { (gvars.Items[id] as Player).UpdateControlledMovement("left", gvars.PercentageOfFrame); }));

            lambdaActions.Add("regenerate", ((gvars, id, parameters) => { ILived l = gvars.Items[id] as ILived; if (l.CurLives > 0 && l.CurLives < l.MaxLives) { l.ChangeCurLives(l.Regeneration * gvars.PercentageOfFrame, null); } }));
            lambdaActions.Add("outsideArena", ((gvars, id, parameters) =>
            {
                Character player = gvars.Items[id] as Character;
                if (player.X < 0 || player.X > gvars.ArenaWidth || player.Y > gvars.ArenaHeight || player.Y < 0)
                {
                    player.ChangeCurLives(-1 * gvars.PercentageOfFrame, null);
                }
            }));

            lambdaActions.Add("enemyAI", ((gvars, id, parameters) => { (gvars.ItemsStep[id] as Enemy).ai.Control(gvars, gvars.ItemsStep[id] as Enemy); }));
            lambdaActions.Add("createBoss", ((gvars, id, parameters) =>
            {

                if (gvars.CountOfItems[ToolsGame.Counts.enemies] < 1)
                {
                    Enemy e = new Enemy(gvars, 0, 0, new Shape("black", "black", 5, 300, 300, Shape.GeometryEnum.circle), new AcceleratedMovement(2, 0, 0.05, 5), 5, 0.05, 0, 200, 0.2, null, 2000, new RandomGoingAI(), true, 100);
                }

            }));
            lambdaActions.Add("createCoin", ((gvars, id, parameters) =>
            {
                if (gvars.CountOfItems[ToolsGame.Counts.coins] < 12)
                {
                    Coin c;
                    if (ToolsGame.random.NextDouble() < 0.5)
                        c = new Coin(10, gvars, (double)(ToolsGame.random.NextDouble() * gvars.ArenaWidth), (double)(ToolsGame.random.NextDouble() * gvars.ArenaHeight), new Shape("yellow", "black", 2, 15, 15, Shape.GeometryEnum.circle));
                    else
                        c = new Coin(20, gvars, (double)(ToolsGame.random.NextDouble() * gvars.ArenaWidth), (double)(ToolsGame.random.NextDouble() * gvars.ArenaHeight), new Shape("orange", "black", 2, 17, 17, Shape.GeometryEnum.circle));
                    gvars.ChangeRepeatTime(ToolsGame.random.Next(300, 1000), "createCoin");
                }
            }));




        }
        public static void ExectureActions(string actionName, Gvars gvars, int id, params object[] parameters)
        {
            lambdaActions[actionName].Invoke(gvars, id, parameters);
        }
    }
}
