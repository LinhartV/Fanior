
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
        
        public static Player CreateNewPlayer(Gvars gvars, string connectionId)
        {
            return new Player(connectionId, gvars, (double)(random.NextDouble() * (gvars.ArenaWidth - 50 - 10) + 10), (double)(random.NextDouble() * (gvars.ArenaWidth - 50 - 10) + 10), new Shape("blue", "darkblue", "red", "darkred", 1, 40, 40, Shape.GeometryEnum.circle), typeof(AcceleratedMovement), 4, 3);
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
        public static void ProceedFrame(Gvars gvars, long now)
        {
            ProcedeGameAlgorithms(gvars);
            ProcedePlayerActions(gvars);
            ProcedeItemActions(now, gvars);
        }
        /// <summary>
        /// Procede algorithm of game logic (collision detection etc.)
        /// </summary>
        private static void ProcedeGameAlgorithms(Gvars gvars)
        {

        }

        /// <summary>
        /// Procede actions that players just did
        /// </summary>
        private static void ProcedePlayerActions(Gvars gvars)
        {
            foreach (int playerId in gvars.PlayerActions.Keys)
            {
                foreach (var action in gvars.PlayerActions[playerId])
                {
                    PlayerAction.InvokeAction(action.Item1, action.Item2, playerId, gvars);
                }
            }
        }

        /// <summary>
        /// Handles all actions of every item (excluding player actions)
        /// </summary>
        private static void ProcedeItemActions(long now, Gvars gvars)
        {
            List<(long, ItemAction)> temp = new List<(long, ItemAction)>(gvars.ItemActions);
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].Item1 <= now)
                {
                    temp[i].Item2.Action();
                    gvars.ItemActions.Remove(temp[i]);
                    if (temp[i].Item2.Repeat > 0)
                    {
                        gvars.ItemActions.Add((now + temp[i].Item2.Repeat, temp[i].Item2));
                    }
                }
                else
                    break;
            }
            gvars.ItemActions.Sort();
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
