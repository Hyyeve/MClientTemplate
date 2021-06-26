using MClient.Core.Utils;
using MClient.Utils;
using Microsoft.Xna.Framework.Graphics;

namespace MClient.Render.RenderTasks
{
    public class MRenderTask
    {

        private readonly PrimitiveType type;
        private readonly MVec2Col[] points;

        public MRenderTask(PrimitiveType type, MVec2Col[] points)
        {
            this.type = type;
            this.points = points;
        }

        public void Run() 
        {
            MRenderer.DrawArray(points, type);
        }

    }
}
