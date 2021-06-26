using DuckGame;

namespace MClient.EventSystem.Events.Input
{
    public class MEventMouseAction : MEvent
    {
        public MouseAction Action;
        public float Scroll;
        public Vec2 MousePosGame;

        public static MEventMouseAction Get(MouseAction action, Vec2 mousepos, float scroll = 0f)
        {
            MEventMouseAction temp = new MEventMouseAction();
            temp.Action = action;
            temp.Scroll = scroll;
            temp.MousePosGame = mousepos;
            return temp;
        }

        public bool IsClickAction()
        {
            return (Action == MouseAction.LeftPressed || Action == MouseAction.MiddlePressed || Action == MouseAction.RightPressed);
        }

        public bool IsReleaseAction()
        {
            return (Action == MouseAction.LeftReleased || Action == MouseAction.MiddleReleased || Action == MouseAction.RightReleased);
        }
    }

    public enum MouseAction
    {
        LeftPressed,LeftReleased,RightPressed,RightReleased,MiddlePressed,MiddleReleased,Scrolled
    }
}