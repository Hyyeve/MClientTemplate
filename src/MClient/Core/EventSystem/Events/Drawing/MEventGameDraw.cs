using System;
using DuckGame;

namespace MClient.Core.EventSystem.Events.Drawing
{
    /// <summary>
    /// MEvent used internally for draw calls. Not recommended for custom use.
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