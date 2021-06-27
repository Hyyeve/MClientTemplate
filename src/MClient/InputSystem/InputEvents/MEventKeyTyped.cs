using System;

namespace MClient.Core.EventSystem.Events.Input
{
    /// <summary>
    /// MEvent that is called whenever a key is typed.
    /// </summary>
    /// <remarks>
    /// This event should be used for handling typing and character input, as it holds
    /// a <c>char</c> object. If you want to handle key input
    /// instead, use the MEventKeyPressed.
    /// 
    /// This event relies on some Duck Game systems, and might not
    /// always be called in cases where Duck Game is blocking input.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class MEventKeyTyped : MEvent
    {
        public char Key;

        public static MEventKeyTyped Get(Char key)
        {
            var temp = new MEventKeyTyped {Key = key};
            return temp;
        }
    }
}