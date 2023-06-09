using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    /// <summary>
    /// Actions that player can invoke - shared on server and client and executed by name.
    /// </summary>
    public static class PlayerAction
    {
        private class KeyCommand
        {
            public Action<int, Gvars, int> KeyDown { get; set; }
            public Action<int, Gvars, int> KeyUp { get; set; }
            //int = id, Gvars = gvars, int = delay
            public KeyCommand(Action<int, Gvars, int> keyDown, Action<int, Gvars, int> keyUp)
            {
                this.KeyDown = new Action<int, Gvars, int>(async (int a, Gvars g, int delay) =>
                {
                    if (keyDown != null)
                    {
                        keyDown(a, g, delay);
                    }
                });
                this.KeyUp = new Action<int, Gvars, int>(async (int a, Gvars g, int delay) =>
                {
                    if (keyUp != null)
                    {
                        keyUp(a, g, delay);
                    }
                });
            }
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum PlayerActionsEnum { none = 0, moveUp = 1, moveDown = 2, moveLeft = 3, moveRight = 4, fire = 5, ability1 = 6, ability2 = 7, other = 8 }

        static Dictionary<PlayerActionsEnum, KeyCommand> actions = new();

        /// <summary>
        /// Invokes a predefined action from PlayerAction.actions list.
        /// </summary>
        /// <param name="actionName">Name of action to be invoked</param>
        /// <param name="keyDown">Whether player pressed this particular key or released it</param>
        /// <param name="itemId">Id of the player</param>
        /// <param name="gvars">Gvars reference</param>
        /// <param name="delay">Time delay caused by lag between server and client</param>
        public static void InvokeAction(PlayerActionsEnum actionName, bool keyDown, int itemId, Gvars gvars, int delay)
        {
            try
            {
                if (actions.ContainsKey(actionName))
                {
                    if (keyDown)
                    {
                        actions[actionName]?.KeyDown(itemId, gvars, delay);
                    }
                    else
                    {
                        actions[actionName]?.KeyUp(itemId, gvars, delay);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void SetupActions()
        {
            actions.Add(PlayerActionsEnum.fire, new KeyCommand((id, gvars, delay) =>
              {
                if (gvars.ItemsPlayers[id].Weapon.reloaded)
                {
                    if (gvars.ItemsPlayers[id].Weapon.autoFire)
                    {
                        gvars.ItemsPlayers[id].AddAction(new ItemAction("fire1", gvars.ItemsPlayers[id].Weapon.reloadTime, ItemAction.ExecutionType.EveryTime), "fire");
                    }
                    else
                    {
                        gvars.ItemsPlayers[id].Weapon.Fire(gvars);
                        gvars.ItemsPlayers[id].Weapon.reloaded = false;
                        gvars.ItemsPlayers[id].AddAction(new ItemAction("fire2", gvars.ItemsPlayers[id].Weapon.reloadTime, ItemAction.ExecutionType.OnlyFirstTime), "fire");
                    }
                }
                else
                {
                    gvars.ItemsPlayers[id].Weapon.reloaded = true;
                }
            },
            (id, gvars, delay) =>
            {
                gvars.ItemsPlayers[id].Weapon.reloaded = false;
            }));
            //Movements
            actions.Add(PlayerActionsEnum.moveUp, new KeyCommand((id, gvars, delay) =>
            {
                gvars.ItemsPlayers[id].AddAction(new ItemAction("up", 1));
                //AddDelayedAction(gvars, id, "up", delay);
            },
            (id, gvars, delay) =>
            {
                gvars.ItemsPlayers[id].DeleteAction("up");
            }));

            actions.Add(PlayerActionsEnum.moveDown, new KeyCommand((id, gvars, delay) =>
            {
                gvars.ItemsPlayers[id].AddAction(new ItemAction("down", 1));
                //AddDelayedAction(gvars, id, "down", delay);
            },
            (id, gvars, delay) =>
            {
                gvars.ItemsPlayers[id].DeleteAction("down");
            }));

            actions.Add(PlayerActionsEnum.moveRight, new KeyCommand((id, gvars, delay) =>
            {
                gvars.ItemsPlayers[id].AddAction(new ItemAction("right", 1));
                //AddDelayedAction(gvars, id, "right", delay);
            },
            (id, gvars, delay) =>
            {
                gvars.ItemsPlayers[id].DeleteAction("right");
            }));

            actions.Add(PlayerActionsEnum.moveLeft, new KeyCommand((id, gvars, delay) =>
            {
                gvars.ItemsPlayers[id].AddAction(new ItemAction("left", 1));
                //AddDelayedAction(gvars, id, "left", delay);
            },
            (id, gvars, delay) =>
            {
                gvars.ItemsPlayers[id].DeleteAction("left");
            }));
        }
    }
}
