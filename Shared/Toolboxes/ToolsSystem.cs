using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public static class ToolsSystem
    {
        public static Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
        public static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Newtonsoft.Json.Formatting.Indented,
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects

        };

        public static Player DeserializePlayer(string jsonText, Gvars gvars)
        {
            Player item = JsonConvert.DeserializeObject<Player>(jsonText, jsonSerializerSettings);
            item.SetItem(gvars);
            return item;
        }

        /*public static void Execute(string actionMethodName, Gvars gvars, int playerId)
        {
            Type thisType = typeof(PlayerAction);
            MethodInfo theMethod = thisType.GetMethod(actionMethodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            theMethod.Invoke(null, new object[] { playerId, gvars});
        }*/
    }
}
