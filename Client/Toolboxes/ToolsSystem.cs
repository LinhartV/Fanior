
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using static Fanior.Shared.Gvars;
using Fanior.Shared;

namespace Fanior.Client
{
    static class ToolsSystem
    {
        /*static public void PressedKeys()
        {
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Pressed && keys[i].KeyPressed != null)
                    keys[i].KeyPressed();
            }
            if(mouse.Pressed && mouse.KeyPressed != null)
            {
                mouse.KeyPressed();
            }
        }
        static public void KeyDown(Gvars gvars, Dictionary<string,ClientKey> keys,string keyCode)
        {
            if(keys.ContainsKey(keyCode))
            {
                if (!keys[keyCode].Pressed)
                {
                    Fanior.Shared.KeyController.GetRegisteredKey(keyCode).KeyDown.Action();
                    keys[keyCode].Pressed = true;
                }
            }
            for (int i = 0; i < keys.Count; i++)
            {
                if (!keys[i].Pressed && keyCode == keys[i].Key)
                {
                    keys[i]!.KeyDown();
                    keys[i].Pressed = true;
                }
            }

        }
        static public void KeyUp(string keyCode)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Pressed && keyCode == keys[i].Key)
                {
                    keys[i]!.KeyUp();
                    keys[i].Pressed = false;
                }
            }
        }*/


        /*
        static public void Window_MouseMove(MouseMoveEventArgs e)
        {
            //if ((e.Position.X == window.Size.X / 2 && e.Position.Y == window.Size.Y / 2))
            player.RotationX -= e.DeltaX/2000;
            //SetCursorPos(window.Size.X/2, window.Size.Y / 2);
        }

        internal static void Window_MouseDown(MouseButtonEventArgs obj)
        {
            if (!mouse.pressed)
            {
                if (mouse.keyDown != null)
                    mouse.keyDown();
                mouse.pressed = true;
            }
        }
        internal static void Window_MouseUp(MouseButtonEventArgs obj)
        {
            if (mouse.pressed)
            {
                if (mouse.keyDown != null)
                    mouse.keyDown();
                mouse.pressed = false;
            }
        }*/
    }
}
