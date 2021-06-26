using DuckGame;

namespace MClient.EventSystem.Events.Input
{
    public class MEventKeyPressed : MEvent
    {
        public Keys Key;

        public static MEventKeyPressed Get(Keys key)
        {
            MEventKeyPressed temp = new MEventKeyPressed();
            temp.Key = key;
            return temp;
        }
    }
}