using System;
using BetfairNG.Data;

namespace BF_API.Helpers.ExtensionMethods
{
    public static class NullCheckers
    {
        public static Competition IsNull(this Competition val)
        {
            if (val == null)
            {
                return new Competition();
            }
            else
            {
                return val;
            }
        }

        public static Event IsNull(this Event val)
        {
            if (val == null)
            {
                return new Event();
            }
            else
            {
                return val;
            }
        }

        public static EventType IsNull(this EventType val)
        {
            if (val == null)
            {
                return new EventType();
            }
            else
            {
                return val;
            }
        }
    }
}
