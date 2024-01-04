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
        //actions assigned to each event - double is time, when the action happened
        public Func<PlayerActions.PlayerActionsEnum> KeyDown { get; set; }
        public Func<PlayerActions.PlayerActionsEnum> KeyUp { get; set; }
        public Action KeyPressed { get; set; }

        //Constroctur for actions proceeded on client side (settings etc.)
        public RegisteredKey(Func<PlayerActions.PlayerActionsEnum> keyDown, Func<PlayerActions.PlayerActionsEnum> keyUp, Action keyPressed)
        {
            this.KeyDown = keyDown;
            this.KeyUp = keyUp;
            this.KeyPressed = keyPressed;
        }

        /// <summary>
        /// Bind up new key to action
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <param name="myActions">"List where this new action will be added"</param>
        /// <param name="clientActionDown">Optional action to happen only on client side (for key press)</param>
        /// <param name="clientActionUp">Optional action to happen only on client side (for key release)</param>
        public RegisteredKey(PlayerActions.PlayerActionsEnum action, List<(PlayerActions.PlayerActionsEnum, bool)> myActions, Action clientActionDown = null, Action clientActionUp = null)
        {
            this.KeyDown = () =>
            {
                if (Pressed == false)
                {
                    Pressed = true;
                    if (clientActionDown != null)
                    {
                        clientActionDown();
                    }
                    if (action != PlayerActions.PlayerActionsEnum.none)
                    {
                        myActions.Add((action, true));
                        return action;
                        //PlayerAction.InvokeAction(keyDown, true, a, g);
                    }
                    
                }

                return PlayerActions.PlayerActionsEnum.none;
            };
            this.KeyUp = () =>
            {
                Pressed = false;
                if (clientActionUp != null)
                {
                    clientActionUp();
                }
                if (action != PlayerActions.PlayerActionsEnum.none)
                {
                    myActions.Add((action, false));
                    return action;
                    //PlayerAction.InvokeAction(keyUp, false, a, g);
                }
                

                return PlayerActions.PlayerActionsEnum.none;
            };
        }

    }
}
