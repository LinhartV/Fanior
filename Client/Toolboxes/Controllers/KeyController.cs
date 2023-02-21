using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanior.Shared;

namespace Fanior.Client
{

    public static class KeyController
    {
        private static Dictionary<string, Stack<RegisteredKey>> registeredKeys = new();

        public static RegisteredKey GetRegisteredKey(string keyName)
        {
            if (registeredKeys.ContainsKey(keyName))
                return registeredKeys[keyName].Peek();
            else
                return null;
        }
        public static void PopKey(string keyName)
        {
            if (registeredKeys.ContainsKey(keyName) && registeredKeys[keyName].Count > 0)
                registeredKeys[keyName].Pop();
        }
        public static void AddKey(string keyName, RegisteredKey key)
        {
            if (!registeredKeys.ContainsKey(keyName))
                registeredKeys.Add(keyName, new Stack<RegisteredKey>());
            registeredKeys[keyName].Push(key);
        }
        //Zajímavý nápad, ale ne.
        /* private Dictionary<string, RegisteredKey> registeredKeys = new();
         private static Stack<KeyController> constrollers = new();

         public KeyController(Dictionary<string, RegisteredKey> registeredKeys)
         {
             this.registeredKeys = registeredKeys;
             KeyController.constrollers.Push(this);
         }

         public static RegisteredKey GetRegisteredKey(string keyName)
         {
             Stack<KeyController> temp = new Stack<KeyController>(constrollers);
             while (temp.Count > 0)
             {
                 if(temp.Peek().registeredKeys.ContainsKey(keyName))
                 {
                     return temp.Peek().registeredKeys[keyName];
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
         }*/

    }
}
