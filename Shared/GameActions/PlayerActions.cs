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
    public static class PlayerActions
    {
        private class KeyCommand
        {
            public Action<int, Gvars> KeyDown { get; set; }
            public Action<int, Gvars> KeyUp { get; set; }
            //int = id, Gvars = gvars
            public KeyCommand(Action<int, Gvars> keyDown, Action<int, Gvars> keyUp)
            {
                this.KeyDown = new Action<int, Gvars>(async (int a, Gvars g) =>
                {
                    if (keyDown != null)
                    {
                        keyDown(a, g);
                    }
                });
                this.KeyUp = new Action<int, Gvars>(async (int a, Gvars g) =>
                {
                    if (keyUp != null)
                    {
                        keyUp(a, g);
                    }
                });
            }
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum PlayerActionsEnum { none = 0, moveUp = 1, moveDown = 2, moveLeft = 3, moveRight = 4, fire = 5, ability1 = 6, ability2 = 7, other = 8 }

        static Dictionary<PlayerActionsEnum, KeyCommand> actions = new();
        //when; what; keydown?; id
        static List<(double, PlayerActionsEnum, bool, int)> pendingActions = new();



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
                    pendingActions.Add((gvars.GetNow(), actionName, keyDown, itemId));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void CheckForActions(Gvars gvars)
        {
            var tempList = new List<(double, PlayerActionsEnum, bool, int)>(pendingActions);
            foreach (var item in tempList)
            {
                if (item.Item1 < gvars.GetNow())
                {
                    if (item.Item3)
                    {
                        actions[item.Item2]?.KeyDown(item.Item4, gvars);
                    }
                    else
                    {
                        actions[item.Item2]?.KeyUp(item.Item4, gvars);
                    }
                    pendingActions.Remove(item);
                }
            }
        }

        public static void SetupActions()
        {
            actions.Add(PlayerActionsEnum.fire, new KeyCommand((id, gvars) =>
              {
                  if (gvars.ItemsPlayers[id].Weapon.reloaded)
                  {
                      if (gvars.ItemsPlayers[id].Weapon.autoFire)
                      {
                          gvars.ItemsPlayers[id].AddAction(gvars, new ItemAction("fire1", gvars.ItemsPlayers[id].Weapon.reloadTime, ItemAction.ExecutionType.EveryTime), "fire");
                      }
                      else
                      {
                          gvars.ItemsPlayers[id].Weapon.Fire(gvars);
                          gvars.ItemsPlayers[id].Weapon.reloaded = false;
                          gvars.ItemsPlayers[id].AddAction(gvars, new ItemAction("fire2", gvars.ItemsPlayers[id].Weapon.reloadTime, ItemAction.ExecutionType.OnlyFirstTime), "fire");
                      }
                  }
                  else
                  {
                      gvars.ItemsPlayers[id].Weapon.reloaded = true;
                  }
              },
            (id, gvars) =>
            {
                gvars.ItemsPlayers[id].Weapon.reloaded = false;
            }));
            //Movements
            actions.Add(PlayerActionsEnum.moveUp, new KeyCommand((id, gvars) =>
            {
                gvars.ItemsPlayers[id].AddAction(gvars, new ItemAction("up", 1, ItemAction.ExecutionType.EveryTime, true));
                //AddDelayedAction(gvars, id, "up", delay);
            },
            (id, gvars) =>
            {
                gvars.ItemsPlayers[id].DeleteAction("up");
            }));

            actions.Add(PlayerActionsEnum.moveDown, new KeyCommand((id, gvars) =>
            {
                gvars.ItemsPlayers[id].AddAction(gvars, new ItemAction("down", 1, ItemAction.ExecutionType.EveryTime, true));
                //AddDelayedAction(gvars, id, "down", delay);
            },
            (id, gvars) =>
            {
                gvars.ItemsPlayers[id].DeleteAction("down");
            }));

            actions.Add(PlayerActionsEnum.moveRight, new KeyCommand((id, gvars) =>
            {
                gvars.ItemsPlayers[id].AddAction(gvars, new ItemAction("right", 1, ItemAction.ExecutionType.EveryTime, true));
                //AddDelayedAction(gvars, id, "right", delay);
            },
            (id, gvars) =>
            {
                gvars.ItemsPlayers[id].DeleteAction("right");
            }));

            actions.Add(PlayerActionsEnum.moveLeft, new KeyCommand((id, gvars) =>
            {
                gvars.ItemsPlayers[id].AddAction(gvars, new ItemAction("left", 1, ItemAction.ExecutionType.EveryTime, true));
                //AddDelayedAction(gvars, id, "left", delay);
            },
            (id, gvars) =>
            {
                gvars.ItemsPlayers[id].DeleteAction("left");
            }));
        }
    }
}
