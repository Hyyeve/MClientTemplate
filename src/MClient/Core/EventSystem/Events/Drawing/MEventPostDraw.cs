using MClient.EventSystem.Events;
using RenderTarget2D = DuckGame.RenderTarget2D;

namespace MClientCore.MClient.EventSystem.Events.Drawing
{
    public class MEventPostDraw : MEvent
    {
        public RenderTarget2D Screen;

        public static MEventPostDraw Get(RenderTarget2D screen)
        {
            MEventPostDraw temp = new MEventPostDraw();
            temp.Screen = screen;
            return temp;
        }
    }
}