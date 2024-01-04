using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Fanior.Shared.PlayerActions;

namespace Fanior.Shared
{
    /// <summary>
    /// Class for every Action delegate used in the game (for the case of JSON)
    /// </summary>
    public static class LambdaActions
    {
        delegate void LambdaAction(Gvars gvars, int id, params object[] parameters);
        /// <summary>
        /// Dictionary of actions. Key - name, Value - Action(gvars, id of item)
        /// </summary>
        private static Dictionary<string, LambdaAction> lambdaActions = new();


        public static void SetupLambdaActions()
        {

            lambdaActions.Add("fire1", (gvars, id, parameters) =>
            {
                Character character = gvars.Items[id] as Character;
                if (!character.WeaponNode.Weapon.Reloaded)
                {
                    character.DeleteAction("fire");
                    character.WeaponNode.Weapon.Reloaded = true;
                }
                else
                {
                    character.WeaponNode.Weapon.Fire(gvars);
                }

            }
            );
            lambdaActions.Add("fire2", ((gvars, id, parameters) => { (gvars.Items[id] as Character).WeaponNode.Weapon.Reloaded = true; }));
            lambdaActions.Add("abilityReload", ((gvars, id, parameters) =>
            {
                (parameters[0] as Ability).Reloaded = true;
            }));
            lambdaActions.Add("dispose", ((gvars, id, parameters) => { gvars.Items[id].Dispose(); }));

            lambdaActions.Add("move", (gvars, id, parameters) =>
            {
                (gvars.Items[id] as Movable).Move(gvars.PercentageOfFrame, parameters.Length > 0 ? (bool)parameters[0] : false);
            }
            );
            lambdaActions.Add("up", ((gvars, id, parameters) => { (gvars.Items[id] as Player).UpdateControlledMovement("up", gvars.PercentageOfFrame); }));
            lambdaActions.Add("down", ((gvars, id, parameters) => { (gvars.Items[id] as Player).UpdateControlledMovement("down", gvars.PercentageOfFrame); }));
            lambdaActions.Add("right", ((gvars, id, parameters) => { (gvars.Items[id] as Player).UpdateControlledMovement("right", gvars.PercentageOfFrame); }));
            lambdaActions.Add("left", ((gvars, id, parameters) => { (gvars.Items[id] as Player).UpdateControlledMovement("left", gvars.PercentageOfFrame); }));

            lambdaActions.Add("regenerate", ((gvars, id, parameters) => { ILived l = gvars.Items[id] as ILived; if (l.CurLives > 0 && l.CurLives < l.MaxLives) { l.ChangeCurLives(l.Regeneration * gvars.PercentageOfFrame, null); } }));
            lambdaActions.Add("outsideArena", ((gvars, id, parameters) =>
            {
                Character player = gvars.Items[id] as Character;
                if (player.X < 0 || player.X > gvars.ArenaWidth || player.Y > gvars.ArenaHeight || player.Y < 0)
                {
                    player.ReceiveDamage(1 * gvars.PercentageOfFrame, null);
                }
            }));

            lambdaActions.Add("enemyAI", ((gvars, id, parameters) => { (gvars.ItemsStep[id] as Enemy).AI.Control(gvars, gvars.ItemsStep[id] as Enemy); }));
            lambdaActions.Add("createBoss", ((gvars, id, parameters) =>
            {
                if (gvars.Cheating)
                    return;
                int bounty = 2000;
                
                IHitReaction hr = null;
                WeaponNode wn = ToolsGame.GetWeaponTreeRoot();
                double damage = 4 + ToolsGame.random.NextDouble() * 3;
                double weaponSpeed = 1 + ToolsGame.random.NextDouble() * 2;
                switch (ToolsGame.random.Next(0, 2))
                {
                    case 0:
                        hr = new Chase();
                        bounty += 500;
                        break;
                    case 1:
                        hr = new ShootBack();
                        bounty += (int)(damage * 100);
                        bounty += (int)(weaponSpeed * 100);
                        while (wn.Children.Length > 0 && ToolsGame.random.NextDouble() > 0.5)
                        {
                            wn = wn.Children[ToolsGame.random.Next(0, wn.Children.Length)];
                            bounty += 200;
                        }
                        break;
                    default:
                        bounty -= 500;
                        hr = null;
                        break;
                }
                double bodyDamage = 0.5 + ToolsGame.random.NextDouble() * 2;
                bounty += (int)(bodyDamage * 300);
                double lives = 400 + ToolsGame.random.NextDouble() * 400;
                bounty += (int)(lives);
                double movementSpeed = 2 + ToolsGame.random.NextDouble() * 6;
                bounty += (int)(movementSpeed * 100);
                double angle = ToolsGame.random.NextDouble() * Math.PI * 2;
                IEnemyMovementAI emai = null;
                switch (ToolsGame.random.Next(0, 2))
                {
                    case 0:
                        emai = new RandomGoingAI();
                        break;
                    default:
                        emai = new FollowClosestAI();
                        movementSpeed *= 0.5;
                        break;
                }
                int r = ToolsGame.random.Next(0, 256);
                int g = ToolsGame.random.Next(0, 256);
                int b = ToolsGame.random.Next(0, 256);
                int rs = ToolsGame.random.Next(0, 256);
                int gs = ToolsGame.random.Next(0, 256);
                int bs = ToolsGame.random.Next(0, 256);
                Enemy e = new Enemy(gvars, Math.Sin(angle) * gvars.ArenaWidth * 2 + gvars.ArenaWidth / 2, Math.Cos(angle) * gvars.ArenaHeight * 2 + gvars.ArenaHeight / 2, new Shape("rgb(" + r.ToString() + "," + g.ToString() + "," + b.ToString() + ")", "rgb(" + rs.ToString() + "," + gs.ToString() + "," + bs.ToString() + ")", 5, 300, 300, Shape.GeometryEnum.circle), new ConstantMovement(movementSpeed, 0), movementSpeed, 0.05, 1, lives, 0.2, wn, bounty, emai, hr, true, damage, bodyDamage, weaponSpeed, 500);


            }));
            lambdaActions.Add("createCoin", ((gvars, id, parameters) =>
            {
                if (gvars.CountOfItems[ToolsGame.Counts.coins] < 22)
                {
                    Coin c;
                    var rand = ToolsGame.random.NextDouble();
                    if (rand < 0.25)
                        c = new Coin(10, gvars, (double)(ToolsGame.random.NextDouble() * gvars.ArenaWidth), (double)(ToolsGame.random.NextDouble() * gvars.ArenaHeight), new Shape("orange", "black", 2, 15, 15, Shape.GeometryEnum.circle));
                    else if (rand < 0.6)
                        c = new Coin(20, gvars, (double)(ToolsGame.random.NextDouble() * gvars.ArenaWidth), (double)(ToolsGame.random.NextDouble() * gvars.ArenaHeight), new Shape("yellow", "black", 2, 17, 17, Shape.GeometryEnum.circle));
                    else if (rand < 0.85)
                        c = new Coin(40, gvars, (double)(ToolsGame.random.NextDouble() * gvars.ArenaWidth), (double)(ToolsGame.random.NextDouble() * gvars.ArenaHeight), new Shape("green", "black", 2, 18, 18, Shape.GeometryEnum.circle));
                    else
                        c = new Coin(70, gvars, (double)(ToolsGame.random.NextDouble() * gvars.ArenaWidth), (double)(ToolsGame.random.NextDouble() * gvars.ArenaHeight), new Shape("#D9049E", "black", 2, 19, 19, Shape.GeometryEnum.circle));


                    gvars.ChangeRepeatTime(ToolsGame.random.Next(300, 600), "createCoin");
                }
            }));
            lambdaActions.Add("abilityRunOut", (gvars, id, parameters) =>
            {
                (parameters[0] as Ability).BeingUsed = false;
            });
            lambdaActions.Add("disposeOnStop", (gvars, id, parameters) =>
            {
                Movable m = gvars.Items[id] as Movable;
                if (m.MovementsAutomated.Count == 0)
                {
                    m.Dispose();
                }
            });
            lambdaActions.Add("Immortality", ((gvars, id, parameters) =>
            {
                var character = (gvars.Items[id] as Character);
                character.Immortal = true;
                character.AddAction(gvars, new ItemAction("loseImmortality", (double)parameters[0], ItemAction.ExecutionType.OnlyFirstTime));
            }));
            lambdaActions.Add("loseImmortality", ((gvars, id, parameters) =>
            {
                var character = (gvars.Items[id] as Character);
                character.Immortal = false;
            }));
            lambdaActions.Add("Bomb", ((gvars, id, parameters) =>
            {
                var c = (gvars.Items[id] as Character);
                new Bomb(gvars, c.X, c.Y, new Shape("#0D1E93", "black", 2, 30, 30, Shape.GeometryEnum.circle, "#760A0A", "black"), new Mask(30, 30, Shape.GeometryEnum.circle), 0, 0, c.Id, 0, 0, 0, ToolsMath.TimeToFrames(2));
            }));

            lambdaActions.Add("loseEmpowerment", ((gvars, id, parameters) =>
            {
                var character = (gvars.Items[id] as Character);
                character.Damage /= 3;
                character.Empowered = false;
            }));
            lambdaActions.Add("Empowerment", ((gvars, id, parameters) =>
            {
                var character = (gvars.Items[id] as Character);
                character.Empowered = true;
                character.Damage *= 3;
                character.AddAction(gvars, new ItemAction("loseEmpowerment", (double)parameters[0], ItemAction.ExecutionType.OnlyFirstTime));
            }));
            lambdaActions.Add("Rapid Fire", ((gvars, id, parameters) =>
            {
                var character = (gvars.Items[id] as Character);
                character.ReloadTime /= 4;
                character.AddAction(gvars, new ItemAction("loseRapid Fire", (double)parameters[0], ItemAction.ExecutionType.OnlyFirstTime), 0, ActionHandler.RewriteEnum.AddNew);
            }));
            lambdaActions.Add("loseRapid Fire", ((gvars, id, parameters) =>
            {
                var character = (gvars.Items[id] as Character);
                character.ReloadTime *= 4;
            }));
            lambdaActions.Add("Insta Heal", ((gvars, id, parameters) =>
            {
                var character = (gvars.Items[id] as Character);
                character.ChangeCurLives((character.MaxLives - character.CurLives) * 3 / 4, null);
            }));
            lambdaActions.Add("Repulsion", ((gvars, id, parameters) =>
            {
                var character = (gvars.Items[id] as Character);
                new PressureWave(gvars, character.X, character.Y, new Shape("black", "black", 0, 1, 1, Shape.GeometryEnum.circle), new Mask(1, 1, Shape.GeometryEnum.circle), id);
            }));

            lambdaActions.Add("pressureWaveSpreading", ((gvars, id, parameters) =>
            {
                var wave = (gvars.Items[id] as PressureWave);
                wave.Mask.Width += 20;
                wave.Mask.Height += 20;
            }));
            lambdaActions.Add("burn", ((gvars, id, parameters) =>
            {
                if (gvars.Items.ContainsKey(id))
                {
                    var bb = (gvars.Items[id] as BurningBoulder);
                    new BasicShot(gvars, bb.X, bb.Y, new Shape(2, 8, 8, Shape.GeometryEnum.circle), new Mask(8, 8, Shape.GeometryEnum.circle), ToolsGame.random.NextDouble() * 4 + 6, 7, bb.CharacterId, ToolsGame.random.NextDouble() * Math.PI * 2, 0, 0.5, 90);
                }
            }));
            lambdaActions.Add("slingshot", ((gvars, id, parameters) =>
            {
                var c = (gvars.Items[id] as Character);
                new BasicShot(gvars, c.X, c.Y, new Shape("lightblue", "darkblue", 2, 15, 15, Shape.GeometryEnum.circle, "rgb(255, 20, 50)", "darkred"), new Mask(15, 15, Shape.GeometryEnum.circle), c.WeaponNode.Weapon.WeaponSpeedCoef * c.BulletSpeed, c.WeaponNode.Weapon.DamageCoef * c.Damage, id, (gvars.Items[id] as Movable).Angle + ToolsGame.random.NextDouble() * Math.PI / 3 - Math.PI / 6, 0, 0.4, ToolsGame.random.Next(40, 60));

            }));
            lambdaActions.Add("chase", ((gvars, id, parameters) =>
            {
                var c = (gvars.Items[id] as Character);
                c.UpdateControlledMovement("chase", gvars.PercentageOfFrame);
            }));
            lambdaActions.Add("stopChase", ((gvars, id, parameters) =>
            {
                var c = (gvars.Items[id] as Character);
                c.DeleteAction("chase");

            }));
        }
        public static void ExecuteActions(string actionName, Gvars gvars, int id, params object[] parameters)
        {
            lambdaActions[actionName].Invoke(gvars, id, parameters);
        }
    }
}
