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
        //actions assigned to each event - long is time, when the action happened
        public Action<long> KeyDown { get; set; }
        public Action<long> KeyUp { get; set; }
        public Action KeyPressed { get; set; }

        //Constroctur for actions proceeded on client side (settings etc.)
        public RegisteredKey(Action<long> keyDown, Action<long> keyUp, Action keyPressed)
        {
            this.KeyDown = keyDown;
            this.KeyUp = keyUp;
            this.KeyPressed = keyPressed;
        }

        //Constroctur for actions proceeded on both server and client side. So far only name of action enum sent to server. Better idea (PlayerAction.PlayerActionsEnum, bool, long) providing time when it happed
        public RegisteredKey(PlayerAction.PlayerActionsEnum action, List<(PlayerAction.PlayerActionsEnum, bool)> myActions)
        {
            this.KeyDown = new Action<long>(async(long now) =>
            {
                if (Pressed == false)
                {
                    if (action != PlayerAction.PlayerActionsEnum.none)
                    {
                        myActions.Add((action, true));
                        //PlayerAction.InvokeAction(keyDown, true, a, g);
                    }
                }
                Pressed = true;
            });
            this.KeyUp = new Action<long>(async (long now) =>
            {
                if (action != PlayerAction.PlayerActionsEnum.none)
                {
                    myActions.Add((action, false));
                    //PlayerAction.InvokeAction(keyUp, false, a, g);
                }
                Pressed = false;
            });
        }

    }
}
