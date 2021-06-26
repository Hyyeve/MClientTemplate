using DuckGame;

namespace MClient.Core.Utils
{
    /// <summary>
    /// Helper class used in the render system. Wraps a position and colour.
    /// </summary>
    public class MVec2Col
    {
        public Vec2 Vec;
        public Color Col;

        public MVec2Col(Vec2 vec2, Color color)
        {
            Vec = vec2;
            Col = color;
        }

    }
}
