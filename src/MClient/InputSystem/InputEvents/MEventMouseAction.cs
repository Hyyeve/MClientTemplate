using System;
using DuckGame;

namespace MClient.Core.EventSystem.Events.Input
{
    /// <summary>
    /// MEvent that is called for all types of mouse input.
    /// </summary>
    /// <remarks>
    /// This event is called for both clicking and scrolling.
    /// Make sure to check what kind it is before accessing its variables!
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class MEventMouseAction : MEvent
    {
        public MMouseAction Action;
        public float Scroll;
        public Vec2 MousePosGame;

        public static MEventMouseAction Get(MMouseAction action, Vec2 mousePos, float scroll = 0f)
        {
            var temp = new MEventMouseAction {Action = action, Scroll = scroll, MousePosGame = mousePos};
            return temp;
        }

        public bool IsClickAction => Action == MMouseAction.LeftPressed || Action == MMouseAction.MiddlePressed || Action == MMouseAction.RightPressed;

        public bool IsReleaseAction => Action == MMouseAction.LeftReleased || Action == MMouseAction.MiddleReleased || Action == MMouseAction.RightReleased;

        public bool IsScrollAction => Action == MMouseAction.Scrolled;
    }

    public enum MMouseAction
    {
        LeftPressed,LeftReleased,RightPressed,RightReleased,MiddlePressed,MiddleReleased,Scrolled
    }
}