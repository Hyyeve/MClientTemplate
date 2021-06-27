using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DuckGame;
using MClient.Core.EventSystem;
using MClient.Core.EventSystem.Events.Input;

namespace MClient.InputSystem
{
    /// <summary>
    /// A class for handling binding method calls to sets of inputs. Mostly intended to be used internally - the <see cref="MInputBindingAttribute"/> wraps this system.
    /// </summary>
    public class MInputBinding
    {
        public readonly Keys[] Keys;
        private readonly List<Keys> _current;
        private int _currentIndex;
        private bool _hasInvoked;
        public readonly MBindOrderReq OrderReq;
        public readonly MBindPressReq PressReq;
        public readonly MethodInfo Method;


        public MInputBinding(Keys[] bind, MBindPressReq pressReq, MBindOrderReq orderReq, MethodInfo action)
        {
            Keys = bind;
            PressReq = pressReq;
            OrderReq = orderReq;
            Method = action;
            _current = new List<Keys>();
            _currentIndex = 0;
        }

        /// <summary>
        /// "Enables" this binding, by registering it with the event handler
        /// </summary>
        public void Activate()
        {
            MEventHandler.Register(typeof(MInputBinding), this);
        }

        /// <summary>
        /// "Disables" this binding, by de-registering it from the event handler
        /// </summary>
        public void Deactivate()
        {
            MEventHandler.DeRegister(typeof(MInputBinding), this);
        }
        
        /// <summary>
        /// Updates the state of the binding, given a key press. Primarily an internal call, but can be used for custom purposes.
        /// </summary>
        /// <param name="key">The key that has been pressed</param>
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
                                if (_hasInvoked) return;
                                Invoke();
                                _hasInvoked = true;
                            }
                            else
                            {
                                Reset();
                            }

                            break;
                        }
                        case MBindPressReq.EachPressed:
                        {
                            if (!_current.Contains(key) && (_current.Count == 0 || !_current.All(Keyboard.Down)))
                            {
                                _current.Add(key);
                                if (_current.Count == Keys.Length)
                                {
                                    if (_hasInvoked)
                                    {
                                        Reset();
                                        return;
                                    };
                                    Invoke();
                                    _hasInvoked = true;
                                }
                            }
                            else
                            {
                                Reset();
                            }
                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                }
                case MBindOrderReq.InOrder:
                {
                    switch (PressReq)
                    {
                        case MBindPressReq.AllPressed:
                        {
                            if (Keys[_currentIndex] == key && _current.All(Keyboard.Down))
                            {
                                _currentIndex++;
                                _current.Add(key);
                                if (_currentIndex >= Keys.Length)
                                {
                                    if (_hasInvoked) return;
                                    Invoke();
                                    _hasInvoked = true;
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
                            if (_currentIndex < Keys.Length && Keys[_currentIndex] == key && (_current.Count == 0 || !_current.All(Keyboard.Down)))
                            {
                                _currentIndex++;
                                _current.Add(key);
                                if (_currentIndex >= Keys.Length)
                                {
                                    if (_hasInvoked)
                                    {
                                        Reset();
                                        return;
                                    }
                                    Invoke();
                                    _hasInvoked = true;
                                }
                            }
                            else
                            {
                                Reset();
                            }
                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Reset()
        {
            _hasInvoked = false;
            _currentIndex = 0;
            _current.Clear();
        }
        
        /// <summary>
        /// Internal event call
        /// </summary>
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