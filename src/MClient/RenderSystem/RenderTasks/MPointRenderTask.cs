using DuckGame;
using MClient.Core.Utils;
using Microsoft.Xna.Framework.Graphics;

namespace MClient.RenderSystem.RenderTasks
{
    /// <summary>
    /// A basic RenderTask that draws a square
    /// </summary>
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