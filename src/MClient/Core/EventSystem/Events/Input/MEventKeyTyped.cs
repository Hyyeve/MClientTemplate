using System;
using DuckGame;

namespace MClient.EventSystem.Events.Input
{
    public class MEventKeyTyped : MEvent
    {
        public char Key;

        public static MEventKeyTyped Get(Char key)
        {
            MEventKeyTyped temp = new MEventKeyTyped();
            temp.Key = key;
            return temp;
        }
    }
}