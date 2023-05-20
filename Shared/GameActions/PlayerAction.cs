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


        public static void InvokeAction(PlayerActionsEnum actionName, bool keyDown, int itemId, Gvars gvars)
        {
            try
            {
                if (actions.ContainsKey(actionName))
                {
                    if (keyDown)
                    {
                        actions[actionName]?.KeyDown(itemId, gvars);
                    }
                    else
                    {
                        actions[actionName]?.KeyUp(itemId, gvars);
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
            actions.Add(PlayerActionsEnum.moveUp, new KeyCommand((id, gvars) =>
            {
                gvars.ItemsPlayers[id].AddAction(new ItemAction((Item item, ItemAction itemAction) =>
                {
                    (item as Player).UpdateControlledMovement("up");

                }, 1, true), "up");

            },
            (id, gvars) =>
            {
                gvars.ItemsPlayers[id].DeleteAction("up");
            }));

            actions.Add(PlayerActionsEnum.moveDown, new KeyCommand((id, gvars) =>
            {
                gvars.ItemsPlayers[id].AddAction(new ItemAction((Item item, ItemAction itemAction) =>
                {
                    (item as Player).UpdateControlledMovement("down");

                }, 1, true), "down");

            },
            (id, gvars) =>
            {
                gvars.ItemsPlayers[id].DeleteAction("down");
            }));

            actions.Add(PlayerActionsEnum.moveRight, new KeyCommand((id, gvars) =>
            {
                gvars.ItemsPlayers[id].AddAction(new ItemAction((Item item, ItemAction itemAction) =>
                {
                    (item as Player).UpdateControlledMovement("right");

                }, 1, true), "right");

            },
            (id, gvars) =>
            {
                gvars.ItemsPlayers[id].DeleteAction("right");
            }));

            actions.Add(PlayerActionsEnum.moveLeft, new KeyCommand((id, gvars) =>
            {
                gvars.ItemsPlayers[id].AddAction(new ItemAction((Item item, ItemAction itemAction) =>
                {
                    (item as Player).UpdateControlledMovement("left");

                }, 1, true), "left");

            },
            (id, gvars) =>
            {
                gvars.ItemsPlayers[id].DeleteAction("left");
            }));


        }

    }

}
