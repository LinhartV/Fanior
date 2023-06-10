﻿
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
            return new Player(connectionId, gvars, (double)(random.NextDouble() * (gvars.ArenaWidth - 50 - 10) + 10), (double)(random.NextDouble() * (gvars.ArenaWidth - 50 - 10) + 10), new Shape("blue", "darkblue", "red", "darkred", 1, 40, 40, Shape.GeometryEnum.circle), null, 4, 0.5, 0.1, 100, new BasicWeapon(true, 30, 20, 10));
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
            ProcedeGameAlgorithms(gvars);
            try
            {
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
            //player.Lives = 3;
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
