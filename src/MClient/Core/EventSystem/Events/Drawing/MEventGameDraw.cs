using System;
using DuckGame;

namespace MClient.Core.EventSystem.Events.Drawing
{
    /// <summary>
    /// MEvent used for core draw calls. If you are using the RenderSystem,
    /// you should use the more specific draw events provided with it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MEventGameDraw : MEvent
    {
        public Layer Layer;
        
        public static MEventGameDraw Get(Layer layer)
        {
            var temp = new MEventGameDraw {Layer = layer};
            return temp;
        }
    }
}