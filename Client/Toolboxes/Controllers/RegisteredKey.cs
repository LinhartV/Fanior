using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanior.Shared;
using Microsoft.AspNetCore.SignalR.Client;

namespace Fanior.Client
{
    public class RegisteredKey
    {
        //Action se zde myslí jen ovládání u clienta (neposílá se serveru) nebo ovládání hráče (posílá se serveru)
        public Action<int, Gvars> KeyDown { get; set; }
        public Action<int, Gvars> KeyUp { get; set; }
        public Action<int, Gvars> KeyPressed { get; set; }

        public RegisteredKey(Action keyDown, Action keyUp, Action keyPressed)
        {
            this.KeyDown = new Action<int, Gvars>((int a, Gvars g)=>{ keyDown(); });
            this.KeyUp = new Action<int, Gvars>((int a, Gvars g) => { keyUp(); });
            this.KeyPressed = new Action<int, Gvars>((int a, Gvars g) => { keyPressed(); });
        }
        public RegisteredKey(Action<int,Gvars> keyDown, Action<int, Gvars> keyUp, Action<int, Gvars> keyPressed, Func<string, Task> sendToServer)
        {
            if(keyDown != null)
                this.KeyDown = new Action<int, Gvars>(async (int a, Gvars g) => {
                await sendToServer(keyDown.Method.Name);
                keyDown(a, g); 
                });
            if (keyUp != null)
                this.KeyUp = new Action<int, Gvars>(async (int a, Gvars g) => {
                    await sendToServer(keyUp.Method.Name);
                    keyUp(a, g);
                });
            if (keyPressed != null)
                this.KeyPressed = new Action<int, Gvars>(async (int a, Gvars g) => {
                    await sendToServer(keyPressed.Method.Name);
                    keyPressed(a, g);
                });
        }
        
    }
}
