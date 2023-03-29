
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanior.Shared
{
    public static class ToolsGame
    {
        //public static List<ItemAction> actionsToDelete = new();
        public static List<Item> itemsToDelete = new();
        public static List<(Action<Item>, Item)> startActionsToPerform = new();
        public static int counter = 0;
        public static Random random = new();
        public static Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
        public static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Newtonsoft.Json.Formatting.Indented,
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects

        };
        public static Player CreateNewPlayer(Gvars gvars)
        {
            return new Player(gvars, (float)(random.NextDouble() * (gvars.ArenaWidth - 50 - 10) + 10), (float)(random.NextDouble() * (gvars.ArenaWidth - 50 - 10) + 10), new Shape("blue", "darkblue", "red", "darkred", 1, 40, 40, Shape.GeometryEnum.circle), typeof(ConstantMovement), 4, 3);
        }
        public class Coords
        {
            public double x;
            public double y;

            public Coords(double x, double y)
            {
                this.x = x;
                this.y = y;
            }
        }
        public static void ProcedeActions()
        {
           /* for (int i = 0; i < itemsStep.Count; i++)
            {
                for (int j = 0; j < itemsStep[i].itemActions.Count; j++)
                {
                    itemsStep[i].itemActions[j].RunAction();
                }
                foreach (var action in actionsToDelete)
                {
                    itemsStep[i].itemActions.Remove(action);
                }
                foreach (var action in startActionsToPerform)
                {
                    action.Item1.Invoke(action.Item2);
                }
                startActionsToPerform.Clear();
                actionsToDelete.Clear();
            }

            foreach (var item in itemsToDelete)
            {
                itemsStep.Remove(item);
            }
            itemsToDelete.Clear();*/
        }


        static public void EndGame()
        {
            Console.WriteLine("GameEnded");
            //player.Lives = 3;
        }

        /*static public bool ResetActionByName(Item item, string actionName, bool invokeStartAction)
        {
            for (int i = 0; i < item.itemActions.Count; i++)
            {
                if (item.itemActions[i].actionName == actionName)
                {
                    item.itemActions[i].ResetAction(invokeStartAction);
                    return true;
                }
            }
            return false;
        }*/






    }
}
