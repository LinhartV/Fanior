
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Fanior.Shared
{
    public static class ToolsGame
    {

        public enum Counts { coins, enemies };
        public static Random random = new();
        public static Player CreateNewPlayer(Gvars gvars, string connectionId, string name)
        {
            return new Player(name, connectionId, gvars, (double)(random.NextDouble() * (gvars.ArenaWidth - 50 - 10) + 10), (double)(random.NextDouble() * (gvars.ArenaWidth - 50 - 10) + 10), new Shape("blue", "darkblue", 1, 40, 40, Shape.GeometryEnum.circle, "red", "darkred"), null, 8, 1, 0.2, 100, 0.02, new BasicWeapon(true, 30, 30, 10), 50);
        }
        public static void ProceedFrame(Gvars gvars, double now, int delay, bool server)
        {
            try
            {
                lock (gvars.frameLock)
                {
                    ProcedeGameAlgorithms(gvars, now, server);
                    ProcedePlayerActions(gvars, delay, server);
                    ProcedeItemActions(now, gvars, server);
                }
            }
            catch (Exception e)
            {


            }
            gvars.ExecuteActions(now, gvars, server, -1);
        }

        /// <summary>
        /// Proceeds algorithm of game logic (collision detection etc.)
        /// </summary>
        private static void ProcedeGameAlgorithms(Gvars gvars, double now, bool server)
        {
            //Gvars Actions
            gvars.ExecuteActions(now, gvars, server, -1);

            var tempList = new List<Item>(gvars.Items.Values);
            //Everyone with everyone - later quadtree
            //Supposing everything is sphere - later more geometries
            for (int i = 0; i < tempList.Count; i++)
            {
                for (int j = i + 1; j < tempList.Count; j++)
                {
                    if (Math.Sqrt(Math.Pow(tempList[i].X - tempList[j].X, 2) + Math.Pow(tempList[i].Y - tempList[j].Y, 2)) < tempList[i].Mask.Height / 2 + tempList[j].Mask.Height / 2)
                    {
                        if (server)
                        {
                            tempList[i].CollideServer(tempList[j], 0);
                            tempList[j].CollideServer(tempList[i], 0);
                        }
                        tempList[i].CollideClient(tempList[j], 0);
                        tempList[j].CollideClient(tempList[i], 0);

                    }
                }
            }
        }

        /// <summary>
        /// Proceeds actions that players just did
        /// </summary>
        private static void ProcedePlayerActions(Gvars gvars, double delay, bool server)
        {
            try
            {
                PlayerActions.CheckForActions(gvars, delay);
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Handles all actions of every item (excluding player actions)
        /// </summary>
        private static void ProcedeItemActions(double now, Gvars gvars, bool server)
        {
            var temp = new List<Item>(gvars.Items.Values);
            foreach (var item in temp)
            {
                item.ExecuteActions(now, gvars, server, item.Id);
            }
        }


        static public void EndGame()
        {
            Console.WriteLine("GameEnded");
        }

        public static List<Upgrades> InicializeUpgradeDictionary()
        {
            return new List<Upgrades>
            {
                { new Upgrades("MAX HEALTH", "pink", (Player player)=>{player.MaxLives += 15; }) },
                { new Upgrades("REGENERATION", "violet", (Player player)=>{player.Regeneration *= 1.4; }) },
                { new Upgrades("WEAPON DAMAGE", "red", (Player player) => { player.Weapon.Damage *= 1.1; }) },
                { new Upgrades("WEAPON SPEED", "orange", (Player player) => { player.Weapon.WeaponSpeed *= 1.1; }) },
                { new Upgrades("RELOAD", "green", (Player player) => { player.Weapon.ReloadTime *= 0.8; }) },
                { new Upgrades("BODY DAMAGE", "yellow", (Player player) => { }) },
                { new Upgrades("MOVEMENT SPEED", "blue", (Player player) => { player.BaseSpeed += 1+1/player.BaseSpeed; player.Friction*=1.1; player.Acceleration+=1.1; }) }
            };
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
