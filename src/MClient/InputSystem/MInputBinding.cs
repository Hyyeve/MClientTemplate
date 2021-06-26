using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DuckGame;
using MClient.EventSystem;
using MClient.EventSystem.Events.Input;

namespace MClient.InputSystem
{
    public class MInputBinding
    {
        public readonly Keys[] Keys;
        private List<Keys> current;
        private int currentIndex;
        private bool hasInvoked;
        public readonly MBindOrderReq OrderReq;
        public readonly MBindPressReq PressReq;
        public readonly MethodInfo Method;


        public MInputBinding(Keys[] bind, MBindPressReq pressReq, MBindOrderReq orderReq, MethodInfo action)
        {
            Keys = bind;
            PressReq = pressReq;
            OrderReq = orderReq;
            Method = action;
            current = new List<Keys>();
            currentIndex = 0;
        }

        public void Activate()
        {
            MEventHandler.Register(typeof(MInputBinding), this);
        }

        public void Deactivate()
        {
            MEventHandler.DeRegister(typeof(MInputBinding), this);
        }
        
        public void UpdateBind(Keys key)
        {

            if (!Keys.Contains(key))
            {
                Reset();
                return;
            }
            
            switch (OrderReq)
            {
                case MBindOrderReq.AnyOrder:
                {
                    switch (PressReq)
                    {
                        case MBindPressReq.AllPressed:
                        {
                            if (Keys.All(Keyboard.Down))
                            {
                                if (hasInvoked) return;
                                Invoke();
                                hasInvoked = true;
                            }
                            else
                            {
                                Reset();
                            }

                            break;
                        }
                        case MBindPressReq.EachPressed:
                        {
                            if (!current.Contains(key) && (current.Count == 0 || !current.All(Keyboard.Down)))
                            {
                                current.Add(key);
                                if (current.Count == Keys.Length)
                                {
                                    if (hasInvoked)
                                    {
                                        Reset();
                                        return;
                                    };
                                    Invoke();
                                    hasInvoked = true;
                                }
                            }
                            else
                            {
                                Reset();
                            }
                            break;
                        }
                    }

                    break;
                }
                case MBindOrderReq.InOrder:
                {
                    switch (PressReq)
                    {
                        case MBindPressReq.AllPressed:
                        {
                            if (Keys[currentIndex] == key && current.All(Keyboard.Down))
                            {
                                currentIndex++;
                                current.Add(key);
                                if (currentIndex >= Keys.Length)
                                {
                                    if (hasInvoked) return;
                                    Invoke();
                                    hasInvoked = true;
                                }
                            }
                            else
                            {
                                Reset();
                            }

                            break;
                        }
                        case MBindPressReq.EachPressed:
                        {
                            if (currentIndex < Keys.Length && Keys[currentIndex] == key && (current.Count == 0 || !current.All(Keyboard.Down)))
                            {
                                currentIndex++;
                                current.Add(key);
                                if (currentIndex >= Keys.Length)
                                {
                                    if (hasInvoked)
                                    {
                                        Reset();
                                        return;
                                    }
                                    Invoke();
                                    hasInvoked = true;
                                }
                            }
                            else
                            {
                                Reset();
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        }

        private void Reset()
        {
            hasInvoked = false;
            currentIndex = 0;
            current.Clear();
        }

        [MEventKeyPressed]
        public void OnKeyPressed(MEventKeyPressed e)
        {
            UpdateBind(e.Key);
        }

        private void Invoke()
        {
            Method.Invoke(null, null);
        }
    }

    public enum MBindPressReq
    {
        AllPressed,EachPressed
    }

    public enum MBindOrderReq
    {
        InOrder, AnyOrder
    }
}