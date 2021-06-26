using System;
using DuckGame;

namespace MClient.Core.EventSystem.Events.Input
{
    /// <summary>
    /// MEvent that is called whenever a key is pressed.
    /// </summary>
    /// <remarks>
    /// This event should be used for handling input, as it holds
    /// a <c>Keys</c> object. If you want to handle typing or character
    /// input instead, use the MEventKeyTyped.
    /// 
    /// This event relies on some Duck Game systems, and might not
    /// always be called in cases where Duck Game is blocking input.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class MEventKeyPressed : MEvent
    {
        public Keys Key;

        public static MEventKeyPressed Get(Keys key)
        {
            var temp = new MEventKeyPressed {Key = key};
            return temp;
        }
    }
}