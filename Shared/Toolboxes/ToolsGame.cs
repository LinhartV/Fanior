
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static List<Upgrade> upgrades = new List<Upgrade>
            {
                { new Upgrade("MAX HEALTH", "pink", (Player player)=>{player.MaxLives += 15; }) },
                { new Upgrade("REGENERATION", "violet", (Player player)=>{player.Regeneration *= 1.4; }) },
                { new Upgrade("WEAPON DAMAGE", "red", (Player player) => { player.Weapon.Damage *= 1.1; }) },
                { new Upgrade("WEAPON SPEED", "orange", (Player player) => { player.Weapon.WeaponSpeed *= 1.1; }) },
                { new Upgrade("RELOAD", "green", (Player player) => { player.Weapon.ReloadTime *= 0.8; }) },
                { new Upgrade("BODY DAMAGE", "yellow", (Player player) => { }) },
                { new Upgrade("MOVEMENT SPEED", "blue", (Player player) => { player.BaseSpeed += 1+1/player.BaseSpeed; player.Friction*=1.1; player.Acceleration+=1.1; }) }
            };

        /*public static Ability GetAbility(int num)
        {
            switch (num)
            {
                case 0:
                    return new Ability(40, 3, "Immortality", 3, "shield.svg", "Three seconds of immortality");
                case 1:
                    return new Ability(30, 0, "Bomb", 3, "bomb.svg", "Sets a bomb that savagely damages everything around");
                case 2:
                    return new Ability(60, 10, "Empowerment", 3, "damageUpgrade.svg", "Ten seconds of highly boosted damage");
                case 3:
                    return new Ability(60, 10, "Rapid Fire", 3, "reloadUpgrade.svg", "Ten seconds of highly boosted reload");
                case 4:
                    return new Ability(90, 0, "Insta Heal", 3, "heal.svg", "Instantly recovers 3/4 health");
                case 5:
                    return new Ability(20, 10, "Repulsion", 3, "repulsion.svg", "Creates a pressure wave around yourself");
                default:
                    return new Ability(20, 10, "Repulsion", 3, "repulsion.svg", "Creates a pressure wave around yourself");
            }
        }*/
        public static List<Ability> abilities = new List<Ability>
            {
                { new Ability(40, 3, "Immortality",3, "shield.svg", "Three seconds of immortality") },
                { new Ability(30, 0, "Bomb", 3, "bomb.svg", "Sets a bomb that savagely damages everything around") },
                { new Ability(60, 10, "Empowerment", 3, "damageUpgrade.svg", "Ten seconds of highly boosted damage") },
                { new Ability(60, 10, "Rapid Fire", 3, "reloadUpgrade.svg", "Ten seconds of highly boosted reload") },
                { new Ability(90, 0, "Insta Heal", 3, "heal.svg", "Instantly recovers 3/4 health") },
                { new Ability(20, 10, "Repulsion", 3, "repulsion.svg", "Creates a pressure wave around yourself") }
            };


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
