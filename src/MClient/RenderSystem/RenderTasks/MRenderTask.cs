using MClient.Core.Utils;
using Microsoft.Xna.Framework.Graphics;

namespace MClient.RenderSystem.RenderTasks
{
    /// <summary>
    /// The base class for RenderTasks
    /// </summary>
    public class MRenderTask
    {

        private readonly PrimitiveType _type;
        private readonly MVec2Col[] _points;

        public MRenderTask(PrimitiveType type, MVec2Col[] points)
        {
            _type = type;
            _points = points;
        }

        public void Run() 
        {
            MRenderer.DrawArray(_points, _type);
        }

    }
}
