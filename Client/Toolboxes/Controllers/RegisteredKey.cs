using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanior.Shared;
using Microsoft.AspNetCore.SignalR.Client;

namespace Fanior.Client
{
    /// <summary>
    /// Functionality of a key
    /// </summary>
    public class RegisteredKey
    {
        //whether the key is currently pressed or not
        public bool Pressed { get; set; } = false;
        //whether the functionality of this key is active of not
        public bool Active { get; set; } = true;
        //actions assigned to each event
        //int is ID of player and Gvars are Gvars...
        public Action<int, Gvars> KeyDown { get; set; }
        public Action<int, Gvars> KeyUp { get; set; }
        public Action<int, Gvars> KeyPressed { get; set; }

        //Constroctur for actions proceeded on client side (settings etc.)
        public RegisteredKey(Action keyDown, Action keyUp, Action keyPressed)
        {
            this.KeyDown = new Action<int, Gvars>((int a, Gvars g) => { keyDown(); });
            this.KeyUp = new Action<int, Gvars>((int a, Gvars g) => { keyUp(); });
            this.KeyPressed = new Action<int, Gvars>((int a, Gvars g) => { keyPressed(); });
        }
        //Constroctur for actions proceeded on both server and client side. So far only name of action enum sent to server.
        //Last argument SendToServer is function for sending info to server.
        public RegisteredKey(PlayerAction.PlayerActionsEnum keyDown, PlayerAction.PlayerActionsEnum keyUp, PlayerAction.PlayerActionsEnum keyPressed, Func<PlayerAction.PlayerActionsEnum, bool, Task> sendToServer)
        {
            this.KeyDown = new Action<int, Gvars>(async (int a, Gvars g) =>
            {
                if (Pressed == false)
                {
                    if (keyDown != PlayerAction.PlayerActionsEnum.none)
                    {
                        await sendToServer(keyDown, true);
                        //PlayerAction.InvokeAction(keyDown, true, a, g);
                    }
                }
                Pressed = true;
            });
            this.KeyUp = new Action<int, Gvars>(async (int a, Gvars g) =>
            {
                if (keyUp != PlayerAction.PlayerActionsEnum.none)
                {
                    await sendToServer(keyUp, true);
                    //PlayerAction.InvokeAction(keyUp, false, a, g);
                }
                Pressed = false;
            });

            this.KeyPressed = new Action<int, Gvars>(async (int a, Gvars g) =>
            {
                if (keyPressed != PlayerAction.PlayerActionsEnum.none)
                {
                    /*await sendToServer(keyPressed.Method.Name);
                    keyPressed(a, g);*/
                }
            });

        }

    }
}
