
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class Player : Character
    {
        public bool MovementEnabled { get; set; } = true;

        public Player() { }
        public Player(Gvars gvars, float x, float y, Shape shape, Mask mask, Type defaultMovement, float movementSpeed, float lives, bool isVisible = true)
            : base(gvars, x, y, shape, mask, movementSpeed, lives, defaultMovement, isVisible)
        {
            SetPlayer(gvars);
        }

        public Player(Gvars gvars, float x, float y, Shape shape, Type defaultMovement, float movementSpeed, float lives, bool isVisible = true)
            : base(gvars, x, y, shape, new Mask(shape.ImageWidth, shape.ImageHeight, shape.Geometry), movementSpeed, lives, defaultMovement, isVisible)
        {
            SetPlayer(gvars);
        }

        private void SetPlayer(Gvars gvars)
        {
            Weapon = new BasicWeapon(true, 100, 1, 0.7f, this.Id);
            Solid = true;
            gvars.ItemsPlayers.Add(this.Id, this);
        }

        public override void Death()
        {
            ToolsGame.EndGame();
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
                    float xspeed = (float)(pm.movementSpeed * Math.Sin(newAngle));
                    float zspeed = (float)(pm.movementSpeed * Math.Cos(newAngle));
                    myPlayer.movement = new DirectionalMovement();
                    if (Math.Abs(angle - epm.angle) < Math.PI / 2 || Math.Abs(angle - epm.angle) > 3 * Math.PI / 2)
                    {
                        xspeed += (float)(epm.movementSpeed * Math.Sin(epm.angle));
                        zspeed += (float)(epm.movementSpeed * Math.Cos(epm.angle));
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
