using System;
using DuckGame;

namespace MClient.InputSystem
{
    public class MInputBindingAttribute : Attribute
    {
        public readonly Keys[] bind;
        public readonly MBindPressReq PressReq;
        public readonly MBindOrderReq OrderReq;

        public MInputBindingAttribute(Keys[] bind, MBindPressReq pressReq, MBindOrderReq orderReq)
        {
            this.bind = bind;
            this.PressReq = pressReq;
            this.OrderReq = orderReq;
        }
    }
}