using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanior.Shared;

namespace Fanior.Shared
{

    public class KeyController
    {
        private Dictionary<string, RegisteredKey> registeredKeys = new();
        private static Stack<KeyController> constrollers = new();

        public KeyController(Dictionary<string, RegisteredKey> registeredKeys)
        {
            this.registeredKeys = registeredKeys;
            KeyController.constrollers.Push(this);
        }

        public static RegisteredKey GetRegisteredKey(string name)
        {
            Stack<KeyController> temp = new Stack<KeyController>(constrollers);
            while (temp.Count > 0)
            {
                if(temp.Peek().registeredKeys.ContainsKey(name))
                {
                    return temp.Peek().registeredKeys[name];
                }
                temp.Pop();
            }
            return null;
        }

        public static void ClearPeekController()
        {
            constrollers.Pop();
        }

        public static void ClearControllers()
        {
            constrollers.Clear();
        }
        
    }
}
