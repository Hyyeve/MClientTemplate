using DuckGame;
using MClient.Core.EventSystem.Events;

namespace MClient.RenderSystemV2.Events
{
    public class MEventRenderLayer : MEvent
    {
        public Layer Layer;

        public MEventRenderLayer Get(Layer layer)
        {
            return new MEventRenderLayer() {Layer = layer};
        }
    }
}