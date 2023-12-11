
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Fanior.Shared
{
    public class Player : Character
    {

        [JsonProperty]
        private int score;
        public int GetScore()
        {
            return score;
        }
        /// <summary>
        /// Increase player score
        /// </summary>
        /// <param name="value">Amount of score to increase by</param>
        /// <param name="increase">Whether to increase value (true) or absolutely set (false)</param>
        /// <returns>Whether player gained Upgrade point</returns>
        public bool IncreaseScore(int value, bool increase = true)
        {
            int tempScore = score;
            bool gainedPoint = false;
            if (increase)
                score += value;
            else
                score = value;
            gvars?.AddProperty(Id, ItemProperties.Score, score);
            this.BaseSpeed -= (Math.Log10(0.01 * (score + 2)) - Math.Log10(0.01 * (tempScore + 2)));
            while (score >= NextLevel)
            {
                PrevLevel = NextLevel;
                NextLevel = (int)(NextLevel * 2 + Constants.NEXT_LEVEL_INCRESE);
                gainedPoint = true;
                UpgradePoints++;
                PointsGained++;
            }
            return gainedPoint;
        }
        //number of upgrade points
        public int UpgradePoints { get; set; } = 0;
        public List<int> Upgrades { get; set; } = new List<int>();

        /// <summary>
        /// Sum of all upgrade points gained so far
        /// </summary>
        public int PointsGained { get; set; } = 0; 
        public Ability AbilityQ { get; set; }
        public Ability AbilityE { get; set; }
        //score needed to reach next level
        public int NextLevel { get; private set; } = Constants.FIRST_LEVEL;
        public int PrevLevel { get; private set; } = 0;
        public bool MovementEnabled { get; set; } = true;
        public string ConnectionId { get; set; }
        public string Name { get; set; }


        public Player() { }
        public Player(string name, string connectionId, Gvars gvars, double x, double y, Shape shape, Mask mask, IMovement defaultMovement, double movementSpeed, double acceleration, double friction, double lives, double regeneration, WeaponTree.WeaponNode weapon, double shield, bool isVisible = true)
            : base(gvars, x, y, shape, mask, movementSpeed, acceleration, friction, lives, regeneration, weapon, false, shield, defaultMovement, isVisible)
        {
            this.Name = name;
            SetPlayer(gvars, connectionId);
        }

        public Player(string name, string connectionId, Gvars gvars, double x, double y, Shape shape, IMovement defaultMovement, double movementSpeed, double acceleration, double friction, double lives, double regeneration, WeaponTree.WeaponNode weapon, double shield, bool isVisible = true)
            : base(gvars, x, y, shape, new Mask(shape.ImageWidth, shape.ImageHeight, shape.Geometry), movementSpeed, acceleration, friction, lives, regeneration, weapon, false, shield, defaultMovement, isVisible)
        {
            this.Name = name;
            SetPlayer(gvars, connectionId);
        }

        private void SetPlayer(Gvars gvars, string connectionId)
        {
            IncreaseScore(0, false);
            this.ConnectionId = connectionId;
            Solid = true;
            gvars.ItemsPlayers.Add(this.Id, this);
            /*this.AddControlledMovement(new ConstantMovement(BaseSpeed, Math.PI / 2), "up");
            this.AddControlledMovement(new ConstantMovement(BaseSpeed, 0), "right");
            this.AddControlledMovement(new ConstantMovement(BaseSpeed, 3 * Math.PI / 2), "down");
            this.AddControlledMovement(new ConstantMovement(BaseSpeed, Math.PI), "left");*/
            this.AddControlledMovement(new AcceleratedMovement(0, Math.PI / 2, this.Acceleration, BaseSpeed), "up");
            this.AddControlledMovement(new AcceleratedMovement(0, 0, this.Acceleration, BaseSpeed), "right");
            this.AddControlledMovement(new AcceleratedMovement(0, 3 * Math.PI / 2, this.Acceleration, BaseSpeed), "down");
            this.AddControlledMovement(new AcceleratedMovement(0, Math.PI, this.Acceleration, BaseSpeed), "left");
            this.AddAction(gvars, new ItemAction("outsideArena", 1, ItemAction.ExecutionType.EveryTime), "outsideArena");
            for (int i = 0; i < ToolsGame.upgrades.Count; i++)
            {
                this.Upgrades.Add(0);
            }
        }
        public override void SetItemFromClient(Gvars gvars)
        {
            base.SetItemFromClient(gvars);
            if (!gvars.ItemsPlayers.ContainsKey(Id))
            {
                gvars.ItemsPlayers.Add(this.Id, this);
            }
        }
        public void IncreaseStatPoint(int statNum)
        {
            Upgrades[statNum]++;
            ToolsGame.upgrades[statNum].OnIncrease(this);
        }

        public override void Death()
        {
            //base.Death(gvars);
            ToolsGame.EndGame();
        }
        public override void Dispose()
        {
            base.Dispose();
        }
        public override int Bounty()
        {
            if (GetScore() < 100)
                return 100;
            else if (GetScore() < 2000)
                return GetScore();
            else
                return 2000;
        }

        public override void CollideServer(Item collider, double angle)
        {
            base.CollideServer(collider, angle);
            if (collider is Coin c)
            {
                this.IncreaseScore(c.Value);
            }
            if (collider is Enemy e)
            {
                ReceiveDamage(2, e);
            }
        }
        /*public override void Collide(Item collider, double angle, params Globals.ActionsAtCollision[] actionsNotToPerform)
        {
            base.Collide(collider, angle, actionsNotToPerform);
            if (collider is Enemy enemy)
            {
                Lives -= enemy.attack;
                AddAction((Item item) =>
                {
                }, "PlayerCollision", 12,
                (Item item) =>
                {
                    Player myPlayer = item as Player;
                    PartialMovement pm = myPlayer.movement.GetCurrentMovement();
                    if(Double.IsNaN(pm.angle))
                    {
                        pm.angle = 0;
                    }
                    PartialMovement epm = enemy.movement.GetCurrentMovement();
                    myPlayer.movementEnabled = false;
                    double newAngle;
                    newAngle = 2 * angle - pm.angle - Math.PI;
                    double xspeed = (double)(pm.movementSpeed * Math.Sin(newAngle));
                    double zspeed = (double)(pm.movementSpeed * Math.Cos(newAngle));
                    myPlayer.movement = new DirectionalMovement();
                    if (Math.Abs(angle - epm.angle) < Math.PI / 2 || Math.Abs(angle - epm.angle) > 3 * Math.PI / 2)
                    {
                        xspeed += (double)(epm.movementSpeed * Math.Sin(epm.angle));
                        zspeed += (double)(epm.movementSpeed * Math.Cos(epm.angle));
                    }
                    newAngle = ToolsMath.GetAngleFromLengts(xspeed, zspeed);
                    myPlayer.movement.AddMovement("rebounce", myPlayer.baseSpeed + 1f, newAngle);
                },
                (Item item) =>
                {
                    (item as Player).movement = new ConstantMovement();
                    (item as Player).movementEnabled = true;
                    (item as Player).movement.StopAllMovement();
                    for (int i = 0; i < Globals.keys.Count; i++)
                    {
                        if ((Globals.keys[i].key.ToString() == "W" || Globals.keys[i].key.ToString() == "S") && Globals.keys[i].pressed)
                        {
                            Globals.keys[i].keyDown();
                        }
                    }
                });
            }*/
    }
}
