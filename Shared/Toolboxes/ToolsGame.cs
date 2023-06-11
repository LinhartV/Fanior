
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

        public static Player CreateNewPlayer(Gvars gvars, string connectionId, string name)
        {
            return new Player(name, connectionId, gvars, (double)(random.NextDouble() * (gvars.ArenaWidth - 50 - 10) + 10), (double)(random.NextDouble() * (gvars.ArenaWidth - 50 - 10) + 10), new Shape("blue", "darkblue", "red", "darkred", 1, 40, 40, Shape.GeometryEnum.circle), null, 4, 0.5, 0.1, 100, 0.02, new BasicWeapon(true, 30, 20, 10), 50);
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
        public static void ProceedFrame(Gvars gvars, long now, int delay, bool server)
        {
            try
            {
                ProcedeGameAlgorithms(gvars);
                ProcedePlayerActions(gvars, delay, server);
                ProcedeItemActions(now, gvars, server);
            }
            catch (Exception e)
            {


            }
            gvars.ExecuteActions(now);
        }

        /// <summary>
        /// Proceeds algorithm of game logic (collision detection etc.)
        /// </summary>
        private static void ProcedeGameAlgorithms(Gvars gvars)
        {
            //Check if player outside of arena
            foreach (Player player in gvars.ItemsPlayers.Values)
            {
                if (player.X < 0 || player.X > gvars.ArenaWidth || player.Y > gvars.ArenaHeight || player.Y < 0)
                {
                    player.ChangeCurLives(-1, null, gvars);
                }
            }
            var tempList = new List<Item>(gvars.Items.Values);
            //Everyone with everyone - later quadtree
            //Supposing everything is sphere - later more geometries
            for (int i = 0; i < tempList.Count; i++)
            {
                for (int j = i + 1; j < tempList.Count; j++)
                {
                    if (Math.Sqrt(Math.Pow(tempList[i].X - tempList[j].X, 2) + Math.Pow(tempList[i].Y - tempList[j].Y, 2)) < tempList[i].Mask.Height / 2 + tempList[j].Mask.Height / 2)
                    {
                        tempList[i].Collide(tempList[j], 0, gvars);
                        tempList[j].Collide(tempList[i], 0, gvars);
                    }
                }
            }
        }

        /// <summary>
        /// Proceeds actions that players just did
        /// </summary>
        private static void ProcedePlayerActions(Gvars gvars, int delay, bool server)
        {
            if (server)
            {
                foreach (int playerId in gvars.PlayerActions.Keys)
                {
                    foreach (var action in gvars.PlayerActions[playerId])
                    {
                        PlayerAction.InvokeAction(action.Item1, action.Item2, playerId, gvars, delay);
                    }
                }
            }
            PlayerAction.CheckForActions(gvars);

        }

        /// <summary>
        /// Handles all actions of every item (excluding player actions)
        /// </summary>
        private static void ProcedeItemActions(long now, Gvars gvars, bool server)
        {
            var temp = new List<Item>(gvars.Items.Values);
            foreach (var item in temp)
            {
                item.ExecuteActions(now, gvars, server);
            }
        }


        static public void EndGame()
        {
            Console.WriteLine("GameEnded");
        }

        /*static public bool ResetActionByName(Item item, string actionName, bool invokeStartAction)
        {
            for (int i = 0; i < item.gameActions.Count; i++)
            {
                if (item.gameActions[i].actionName == actionName)
                {
                    item.gameActions[i].ResetAction(invokeStartAction);
                    return true;
                }
            }
            return false;
        }*/






    }
}
