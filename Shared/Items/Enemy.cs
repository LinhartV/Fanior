﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class Enemy : Character
    {
        [JsonProperty]
        private int scoreToReturn;
        public IEnemyAI ai;
        public Enemy() : base() { }
        public Enemy(Gvars gvars, double x, double y, Shape shape, IMovement defaultMovement, double movementSpeed, double acceleration, double friction, double lives, double regeneration, Weapon weapon, int bounty, IEnemyAI ai, bool setAngle, double shield = 0, bool isVisible = true)
            : base(gvars, x, y, shape, new Mask(shape.ImageWidth, shape.ImageHeight, shape.Geometry), movementSpeed, acceleration, friction, lives, regeneration, weapon, setAngle, shield, defaultMovement, isVisible)
        {
            scoreToReturn = bounty;
            gvars.CountOfItems[ToolsGame.Counts.enemies]++;
            this.ai = ai;
            this.AddAction(gvars, new ItemAction("enemyAI", 1, ItemAction.ExecutionType.EveryTime, true));
        }

        public override int Bounty()
        {
            return scoreToReturn;
        }
        public override void Dispose()
        {
            base.Dispose();
            gvars.CountOfItems[ToolsGame.Counts.enemies]--;
        }

        public override void CollideClient(Item collider, double angle)
        {
        }

        public override void CollideServer(Item collider, double angle)
        {
            base.CollideServer(collider, angle);
        }
        public override void Death()
        {
            gvars.AddAction(gvars, new ItemAction("createBoss", ToolsGame.random.Next(3000, 15000), ItemAction.ExecutionType.OnlyFirstTime));
            base.Death();
        }
    }
}
