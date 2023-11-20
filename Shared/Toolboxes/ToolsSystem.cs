using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize
        };

        public static Player DeserializePlayer(string jsonText, Gvars gvars)
        {
            Player item = JsonConvert.DeserializeObject<Player>(jsonText, jsonSerializerSettings);
            item.SetItemFromClient(gvars);
            return item;
        }
        public static double GetPercentageOfFrame(double previousTick, double now)
        {
            double percentage = (now - previousTick) / (double)Constants.GAMEPLAY_FRAME_TIME;
            /*if (percentage > 5)
            {
                return -1;
            }*/
            return percentage;
        }


        
        /*public static void Execute(string actionMethodName, Gvars gvars, int playerId)
        {
            Type thisType = typeof(PlayerAction);
            MethodInfo theMethod = thisType.GetMethod(actionMethodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            theMethod.Invoke(null, new object[] { playerId, gvars});
        }*/
    }
}
