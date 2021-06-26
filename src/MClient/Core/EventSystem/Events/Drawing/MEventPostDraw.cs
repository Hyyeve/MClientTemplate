using System;
using RenderTarget2D = DuckGame.RenderTarget2D;

namespace MClient.Core.EventSystem.Events.Drawing
{
    /// <summary>
    /// MEvent that is called after all drawing is finished.
    /// WARNING: Renderers are not active at this point,
    /// and attempting to do rendering with this event will throw a exception.
    /// </summary>
    /// <remarks>
    /// The Screen render target is not currently used in this event and will always be null.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class MEventPostDraw : MEvent
    {
        public RenderTarget2D Screen;
        
        public static MEventPostDraw Get(RenderTarget2D screen)
        {
            var temp = new MEventPostDraw {Screen = screen};
            return temp;
        }
    }
}