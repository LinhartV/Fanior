using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanior.Shared;

namespace Fanior.Client
{
    /// <summary>
    /// Class for controlling actions assigned to particular keys by their keycode
    /// </summary>
    public static class KeyController
    {
        //Dictionary of actions assigned to particular key (string is acutally the keycode of the key)
        private static Dictionary<string, Stack<RegisteredKey>> registeredKeys = new();

        //gets the registeredKey on top of the stack for particulare keycode.
        public static RegisteredKey GetRegisteredKey(string keyName)
        {
            if (registeredKeys.ContainsKey(keyName))
                return registeredKeys[keyName].Peek();
            else
                return null;
        }
        //Pops the topmost functionality of particular key (if W is for move up, but in settings W is for new soundtrack (idk), than after return to the game, the setting functionality can be popped)
        public static void PopKey(string keyName)
        {
            if (registeredKeys.ContainsKey(keyName) && registeredKeys[keyName].Count > 0)
                registeredKeys[keyName].Pop();
        }
        //Pushes or creates new functionality for particular key
        public static void AddKey(string keyName, RegisteredKey registeredKey)
        {
            if (!registeredKeys.ContainsKey(keyName))
                registeredKeys.Add(keyName, new Stack<RegisteredKey>());
            registeredKeys[keyName].Push(registeredKey);
        }
    }
}
