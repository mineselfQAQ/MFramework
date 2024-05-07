using System;

namespace MFramework
{
    public class MEvent
    {
        public Action action;

        public MEvent() { }
        public MEvent(Action action)
        {
            this.action = action;
        }
    }
}