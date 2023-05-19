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
        public Action Action { get; private set; }
        //whether the action will be repeated and if so, how often
        private int repeat;
        //Item the action is assigned to
        public Item Item { get; private set; }

        public ItemAction(Action action, int repeat, Item item)
        {
            Action = action;
            Repeat = repeat;
            Item = item;
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