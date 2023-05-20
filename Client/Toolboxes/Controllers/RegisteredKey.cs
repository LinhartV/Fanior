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
        public Action KeyDown { get; set; }
        public Action KeyUp { get; set; }
        public Action KeyPressed { get; set; }

        //Constroctur for actions proceeded on client side (settings etc.)
        public RegisteredKey(Action keyDown, Action keyUp, Action keyPressed)
        {
            this.KeyDown = keyDown;
            this.KeyUp = KeyUp;
            this.KeyPressed = keyPressed;
        }

        //Constroctur for actions proceeded on both server and client side. So far only name of action enum sent to server.
        public RegisteredKey(PlayerAction.PlayerActionsEnum keyDown, PlayerAction.PlayerActionsEnum keyUp, List<(PlayerAction.PlayerActionsEnum, bool)> myActions)
        {
            this.KeyDown = new Action(async() =>
            {
                if (Pressed == false)
                {
                    if (keyDown != PlayerAction.PlayerActionsEnum.none)
                    {
                        myActions.Add((keyDown, true));
                        //PlayerAction.InvokeAction(keyDown, true, a, g);
                    }
                }
                Pressed = true;
            });
            this.KeyUp = new Action(async () =>
            {
                if (keyUp != PlayerAction.PlayerActionsEnum.none)
                {
                    myActions.Add((keyDown, true));
                    //PlayerAction.InvokeAction(keyUp, false, a, g);
                }
                Pressed = false;
            });
        }

    }
}
