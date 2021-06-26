using System;
using DuckGame;

namespace MClient.InputSystem
{
    /// <summary>
    /// Auto-Attribute for binding method calls to keypresses
    /// </summary>
    public class MInputBindingAttribute : Attribute
    {
        public readonly Keys[] Bind;
        public readonly MBindPressReq PressReq;
        public readonly MBindOrderReq OrderReq;

        public MInputBindingAttribute(Keys[] bind, MBindPressReq pressReq, MBindOrderReq orderReq)
        {
            Bind = bind;
            PressReq = pressReq;
            OrderReq = orderReq;
        }
    }
}