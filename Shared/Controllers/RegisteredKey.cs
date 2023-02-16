using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanior.Shared.Items.HelperClasses;

namespace Fanior.Shared
{
    public class RegisteredKey
    {
        public ItemAction KeyDown { get; set; }
        public ItemAction KeyUp { get; set; }
        public ItemAction KeyPressed { get; set; }


        public RegisteredKey(ItemAction keyDown, ItemAction keyUp, ItemAction keyPressed)
        {
            this.KeyDown = keyDown;
            this.KeyUp = keyUp;
            this.KeyPressed = keyPressed;
        }
    }
}
