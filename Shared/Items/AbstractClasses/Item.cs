using Fanior.Shared;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;


namespace Fanior.Shared
{
    public abstract class Item : ActionHandler
    {
        private double x;
        public double X
        {
            get => x;
            set
            {
                x = value;

            }

        }
        private double y;
        public double Y { get; set; }
        public int Id { get; set; }
        public bool Solid { get; set; }
        public bool IsVisible { get; set; }
        public Mask Mask { get; set; }
        public Shape Shape { get; set; }
        /// <summary>
        /// Collision event executed both on server side and client side
        /// </summary>
        /// <param name="collider">Item that collided</param>
        /// <param name="angle">Angle of collision</param>
        /// <param name="gvars">Reference to gvars</param>
        public virtual void CollideClient(Item collider, double angle, Gvars gvars) { }
        /// <summary>
        /// Collision event executed only on server side
        /// </summary>
        /// <param name="collider">Item that collided</param>
        /// <param name="angle">Angle of collision</param>
        /// <param name="gvars">Reference to gvars</param>
        public virtual void CollideServer(Item collider, double angle, Gvars gvars) { }


        public virtual void ReceiveRandomNumbers(List<double> numbers)
        {

        }
        //Idea that actions will be reversed, then I would add pending actions and then I would execute them several times to reach current state
        //This way I would execute pending actions in the past, when the actually happened... didn't work...
        /// <summary>
        /// Invokes playerActions
        /// </summary>
        public void SetActions(double now, Gvars gvars, int delay, List<(PlayerActions.PlayerActionsEnum, bool)> actionMethodNames)
        {
            /*for (int i = 0; i < frames; i++)
            {
                foreach (var action in actionsEveryFrame.Values)
                {
                    LambdaActions.executeAntiAction(action.ActionName, gvars, this.Id);
                }
            }*/
            foreach (var action in actionMethodNames)
            {
                PlayerActions.InvokeAction(action.Item1, action.Item2, Id, gvars, delay);
            }
            /*for (int i = 0; i < frames; i++)
            {
                foreach (var action in actionsEveryFrame.Values)
                {
                    LambdaActions.executeAction(action.ActionName, gvars, this.Id);
                }
            }*/
        }


        public virtual void Dispose(Gvars gvars)
        {
            gvars.Items.Remove(this.Id);
            if (gvars.ItemsPlayers.ContainsKey(Id))
            {
                gvars.ItemsPlayers.Remove(this.Id);
            }
            if (gvars.ItemsStep.ContainsKey(Id))
            {
                gvars.ItemsStep.Remove(this.Id);
            }
            gvars.Msg.itemsToDestroy.Add(this.Id);
        }
        public virtual void SetItemFromClient(Gvars gvars)
        {
            if (!gvars.Items.ContainsKey(Id))
            {
                gvars.Items.Add(Id, this);
                gvars.Id++;
            }
            DeleteNonClientActions();
        }
        public Item() { }
        public Item(Gvars gvars, double x, double y, Shape shape, Mask mask, bool isVisible = true, bool justGraphics = false)
        {
            this.Shape = shape;
            this.Mask = mask;
            if (mask == null)
            {
                Mask = new Mask(shape.ImageWidth, shape.ImageHeight, shape.Geometry);
            }
            this.X = x;
            this.Y = y;
            VirtualX = x;
            VirtualY = y;
            this.IsVisible = isVisible;
            Solid = !justGraphics;
            this.Id = gvars.Id++;
            gvars.Items.Add(Id, this);

            if (this is not Player)
            {
                gvars.Msg.itemsToCreate.Add(this);
            }
        }
    }
}
