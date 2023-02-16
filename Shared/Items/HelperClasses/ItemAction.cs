using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared.Items.HelperClasses
{
    public class ItemAction
    {
        public Action Action { get; private set; }
        private int repeat;
        public Item Item { get; private set; }

        public ItemAction(Action action, int repeat, Item item)
        {
            Action = action;
            this.repeat = repeat;
            Item = item;
        }

        public int Repeat
        {
            get { return repeat; }
            set
            {
                if (value >= 0)
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