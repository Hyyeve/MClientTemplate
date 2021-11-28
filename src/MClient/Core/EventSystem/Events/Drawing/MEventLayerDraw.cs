using System;
using DuckGame;

namespace MClient.Core.EventSystem.Events.Drawing
{
    /// <summary>
    /// MEvent used for core draw calls. If you are using the RenderSystem,
    /// you should use the more specific draw events provided with it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MEventLayerDraw : MEvent
    {
        public Layer Layer;
        
        public static MEventLayerDraw Get(Layer layer)
        {
            var temp = new MEventLayerDraw {Layer = layer};
            return temp;
        }
    }
}