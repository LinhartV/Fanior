using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    /// <summary>
    /// Actions that can be assigned to Items - controlled by game itself.
    /// </summary>
    public class ItemAction
    {
        //Action to be executed
        public Action<Item, ItemAction> Action { get; private set; }
        //whether the action will be repeated and if so, how often
        private int repeat;
        public bool executeOnFirstTime;

        public ItemAction(Action<Item, ItemAction> action, int repeat, bool executeOnFirstTime)
        {
            Action = action;
            Repeat = repeat;
            this.executeOnFirstTime = executeOnFirstTime;
        }

        public int Repeat
        {
            get { return repeat; }
            set
            {
                if (value > 0)
                {
                    if (value <= Constants.FRAME_TIME)
                    {
                        repeat = Constants.FRAME_TIME;
                    }
                    else
                        repeat = value;
                }
                else
                {
                    repeat = 0;
                }
            }
        }

    }
}