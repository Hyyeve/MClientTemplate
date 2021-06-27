using System;
using System.Linq;
using DuckGame;
using MClient.Core.EventSystem;
using MClient.Core.EventSystem.Events.Game;
using MClient.Core.EventSystem.Events.Helper;
using MClient.Core.EventSystem.Events.Input;
using MClient.Core.Utils;


namespace MClient.InputSystem
{
    
    /// <summary>
    /// Easy-to-use class for accessing various inputs
    /// </summary>
    [MAutoRegisterEvents]
    public static class MInputHandler
    {
        public static bool KeyPressed(Keys key) => Keyboard.Pressed(key);
        public static bool KeyReleased(Keys key) => Keyboard.Released(key);
        public static bool KeyDown(Keys key) => Keyboard.Down(key);

        public static bool MouseLeftPressed() => Mouse.left == InputState.Pressed;
        public static bool MouseLeftReleased() => Mouse.left == InputState.Released;
        public static bool MouseLeftDown () => Mouse.left == InputState.Down;

        public static bool MouseRightPressed() => Mouse.right == InputState.Pressed;
        public static bool MouseRightReleased() => Mouse.right == InputState.Released;
        public static bool MouseRightDown() => Mouse.right == InputState.Down;

        public static bool MouseMiddlePressed() => Mouse.middle == InputState.Pressed;
        public static bool MouseMiddleReleased() => Mouse.middle == InputState.Released;
        public static bool MouseMiddleDown() => Mouse.middle == InputState.Down;

        public static Vec2 MousePositionScreen
        {
           get => Mouse.mousePos;
           set => Mouse.position = MPositionConversionUtil.ScreenToGamePos(value);
        }

        public static Vec2 MousePositionGameSnapped => Mouse.position;

        public static Vec2 MousePositionGame
        {
            get => MPositionConversionUtil.ScreenToGamePos(MousePositionScreen);
            set => Mouse.position = value;
        }

        public static float MouseScroll => Mouse.scroll;

        /// <summary>
        /// The delay before a held-down key sends repeated key typed events
        /// </summary>
        public const float KeyRepeatDelay = 300;

        /// <summary>
        /// The delay between repeated key typed events when a key is held down
        /// </summary>
        public const float KeyRepeatSpeed = 50;
        
        
        private static string _prevKeyString = "";
        private static Keys _repeatKey = Keys.None;
        
        private static readonly MDelayUtil DelayTimer = new MDelayUtil();
        private static readonly MDelayUtil RepeatTimer = new MDelayUtil();

        /// <summary>
        /// Internal event call. Not intended for custom use!
        /// </summary>
        [MEventPreGameUpdate]
        public static void CallInputEvents()
        {
            HandleKeyboardPresses();
            
            HandleKeyboardTyping();

            HandleKeyboardRepeats();

            HandleMouseActions();
        }

        private static void HandleKeyboardRepeats()
        {
            Microsoft.Xna.Framework.Input.Keys[] pressed =
                Microsoft.Xna.Framework.Input.Keyboard.GetState().GetPressedKeys().ToArray();

            if (pressed.Length > 0) _repeatKey = (Keys) pressed[0];

            if (Keyboard.Down(_repeatKey))
            {
                if (DelayTimer.AbsoluteTimePassed(KeyRepeatDelay) && RepeatTimer.TimePassed(KeyRepeatSpeed))
                {
                    MEventHandler.Call(MEventKeyTyped.Get(Keyboard.GetCharFromKey(_repeatKey)));
                }
            }
            else
            {
                DelayTimer.Reset();
            }
        }

        private static void HandleMouseActions()
        {
            var mpg = MousePositionGame;
            
            switch (Mouse.left)
            {
                case InputState.Pressed:
                    MEventHandler.Call(MEventMouseAction.Get(MMouseAction.LeftPressed, mpg));
                    break;
                case InputState.Released:
                    MEventHandler.Call(MEventMouseAction.Get(MMouseAction.LeftReleased, mpg));
                    break;
            }

            switch (Mouse.right)
            {
                case InputState.Pressed:
                    MEventHandler.Call(MEventMouseAction.Get(MMouseAction.RightPressed, mpg));
                    break;
                case InputState.Released:
                    MEventHandler.Call(MEventMouseAction.Get(MMouseAction.RightReleased, mpg));
                    break;
            }

            switch (Mouse.middle)
            {
                case InputState.Pressed:
                    MEventHandler.Call(MEventMouseAction.Get(MMouseAction.MiddlePressed, mpg));
                    break;
                case InputState.Released:
                    MEventHandler.Call(MEventMouseAction.Get(MMouseAction.MiddleReleased, mpg));
                    break;
            }

            if (Mouse.scroll != 0)
            {
                MEventHandler.Call(MEventMouseAction.Get(MMouseAction.Scrolled, mpg, Mouse.scroll));
            }
        }

        private static void HandleKeyboardPresses()
        {
            Microsoft.Xna.Framework.Input.Keys[] pressedKeys = Microsoft.Xna.Framework.Input.Keyboard.GetState().GetPressedKeys();
            foreach (var keys in pressedKeys)
            {
                var key = (Keys) keys;
                if(Keyboard.Pressed(key)) MEventHandler.Call(MEventKeyPressed.Get(key));
            }
        }

        private static void HandleKeyboardTyping()
        {
            if (Keyboard.keyString == _prevKeyString || Keyboard.keyString == "") return;
            
            string keys = Keyboard.keyString;
            string newKeys = keys.Length < _prevKeyString.Length ? keys : keys.Substring(_prevKeyString.Length);
            foreach (var c in newKeys.Where(c => !Char.IsControl(c)))
            {
                MEventHandler.Call(MEventKeyTyped.Get(c));
            }
            _prevKeyString = Keyboard.keyString;
        }
    }
}