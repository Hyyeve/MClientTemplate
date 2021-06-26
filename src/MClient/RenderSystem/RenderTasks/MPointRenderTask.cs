using DuckGame;
using MClient.Utils;
using Microsoft.Xna.Framework.Graphics;

namespace MClient.Render.RenderTasks
{
    public class MPointRenderTask : MRenderTask
    {
        public MPointRenderTask(Vec2 pos, int size, Color col) : base(PrimitiveType.TriangleStrip, new MVec2Col[4]
        {
            new MVec2Col(pos + new Vec2(-size, -size), col),
            new MVec2Col(pos + new Vec2(-size, size), col),
            new MVec2Col(pos + new Vec2(size, -size), col),
            new MVec2Col(pos + new Vec2(size, size), col),
        })
        {
        }
    }
}