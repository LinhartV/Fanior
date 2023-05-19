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
            public bool Pressed { get; set; } = false;
            public Action<int, Gvars> KeyDown { get; set; }
            public Action<int, Gvars> KeyUp { get; set; }
            public Action<int, Gvars> KeyPressed { get; set; }

            public KeyCommand(Action<int, Gvars> keyDown, Action<int, Gvars> keyUp, Action<int, Gvars> keyPressed)
            {
                this.KeyDown = new Action<int, Gvars>(async (int a, Gvars g) =>
                {
                    if (Pressed == false || true)
                    {
                        if (keyDown != null)
                        {
                            keyDown(a, g);
                        }
                        if (keyPressed!=null)
                        {
                            //g.Actions.Add(((long)5, new ItemAction());

                        }
                    }
                    Pressed = true;
                });
                this.KeyUp = new Action<int, Gvars>(async (int a, Gvars g) =>
                {
                    if (keyUp != null)
                    {
                        keyUp(a, g);
                    }
                    Pressed = false;
                });

                this.KeyPressed = new Action<int, Gvars>(async (int a, Gvars g) =>
                {
                    if (keyPressed != null)
                    {
                        keyPressed(a, g);
                    }
                });
            }
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public enum PlayerActionsEnum { none = 0, moveUp = 1, moveDown = 2, moveLeft = 3, moveRight = 4, fire = 5, ability1 = 6, ability2 = 7, other = 8}

        static Dictionary<PlayerActionsEnum, KeyCommand> actions = new ();

        
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
            actions.Add(PlayerActionsEnum.moveUp, new KeyCommand((id, gvars) => { gvars.Items[id].Y -= 5; }, (id, gvars) => { }, (id, gvars) => { }));
        }

    }

}
