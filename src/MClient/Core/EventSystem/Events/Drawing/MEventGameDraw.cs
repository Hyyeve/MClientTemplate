using DuckGame;

namespace MClient.EventSystem.Events.Drawing
{
    public class MEventGameDraw : MEvent
    {
        public Layer layer;

        public MEventGameDraw()
        {

        }

        public static MEventGameDraw Get(Layer layer)
        {
            MEventGameDraw temp = new MEventGameDraw();
            temp.layer = layer;
            return temp;
        }
    }
}