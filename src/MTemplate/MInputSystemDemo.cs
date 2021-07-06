using DuckGame;
using Harmony;
using MClient.Core.EventSystem.Events.Input;
using MClient.InputSystem;
using MClient.UiSystem.Internal;

namespace MClient.MTemplate
{
    /// <summary>
    /// Demonstration class for the input system!
    /// </summary>
    public class MInputSystemDemo
    {
        /*
        The input system is a very simple way to access useful information about input.
        
        There's three MEvents that come with the system that you can use for running
        methods that take input. The first one, here, is the MouseAction event.
        To do much with this event, you need to take it as a parameter in your method.
        */

        [MEventMouseAction]
        public static void HandleMouseEvent(MEventMouseAction e)
        {
            /*
            Then, we can access some useful values from it.
            
            The Scroll value stores how much the mouse was scrolled
            this frame - it's 0 while not scrolling, and positive
            or negative an amount while scrolling, depending on 
            the direction and speed.
            */
            
            float scroll = e.Scroll;
            
            /*
            It doesn't store separate values for click actions,
            instead it has a single Action variable that we can
            look at to see what kind of click action it was.
            
            (I've added brackets and the ?: operator to this line to make it easier
            to understand, but they aren't needed. The commented line below would
            function identically)
            */
            
              bool leftClicked = (e.Action == MMouseAction.LeftPressed ? true : false); 
            //bool leftClicked = e.Action == MMouseAction.LeftPressed;
            
            /*
            You might have noticed though, that with this setup, you have to manually check every action.
            Well,to make that easier, the event has some useful property variables to check what kind
            of action it is, so you don't have to manually work it out yourself.
            */

            bool isClickEvent = e.IsClickAction;
            bool isReleaseEvent = e.IsReleaseAction;
            bool isScrollEvent = e.IsScrollAction;
            
            /*
            And you can also access the mouse position directly through the
            event, for simplicity. This position is the World/Game space position
            of the mouse.
            */

            var mousePos = e.MousePosGame;
        }

        /*
        There's also a couple of events for keyboard input, too. 
        This one is for key presses, and just like the mouse input
        one, you need to take it as a parameter for it to be much use.
        */
        [MEventKeyPressed]
        public static void OnKeyPress(MEventKeyPressed e)
        {
            /*
            This event holds key presses. This is a
            important difference from keys typed, as I'll
            explain shortly. 
            
            This event though, holds a Keys object with, well,
            they key that was pressed.
            */

            var key = e.Key;
            
            /*
            And you can then check what kind of key it is, if you need to.
            (Again, ?: operator added for clarity)
            */

            bool keyIsF = e.Key == Keys.F ? true : false;
            
            /*
            If you're thinking of doing keybinds/hotkeys like this,
            don't! The input system includes a keybind handler too,
            which I'll go over a bit later in this demo class!
            */
        }

        /*
        Crucially different from the KeyPressed event, this is
        the KeyTyped event. Don't forget the difference!
        */
        [MEventKeyTyped]
        public static void EventKeyTyped(MEventKeyTyped e)
        {
            /*
            KeyTyped events hold the actual character that was typed
            on the keyboard, instead of just the ID of what key was
            pressed.Useful if you want to make custom text input somewhere. 
            */

            char key = e.Key;
            
            /*
            We can't easily compare a char to a specific key, but what
            we can do is use it to record actual typed input! 
            */

            string typedChar = "Char Typed Was: " + e.Key;
        }

        public static void GetInputState()
        {
            /*
            Alternatively, if you don't want it to be event-based, you
            can access the mouse - and keyboard - states via the
            MInputHandler class, using one of the methods or variables
            it provides.
            */

            float scroll = MInputHandler.MouseScroll;

            bool fPressed = MInputHandler.KeyPressed(Keys.F);
            
            /*
            Additionally, it provides some extra things that the events don't. 
            */

            //This one is the screen-space pixel position
            var mousePosScreen = MInputHandler.MousePositionScreen;
            
            //This one is world/game space and snapped to the in-game pixel grid
            var mousePosGameSnapped = MInputHandler.MousePositionGameSnapped;
            
            /*
            You can also set the mouse position if you want!
            Be careful with this though, if you happen to create a
            loop that constantly sets the mouse position, then you
            might struggle to actually close or stop the game... at
            worst, you might even have to restart your PC because
            you can't move the mouse.
            */
            MInputHandler.MousePositionScreen = Vec2.Zero;
        }

        /*
        And, as promised, here's the keybind system too!
        It's pretty simple to use, you just add the attribute like
        with events and other auto-attributes, pass it an array of the 
        keys you want in the keybinding, along with the requirements for
        how they should be pressed and in what order. 
        
        Keybinds are auto-attributes, so the methods must be public and static,
        and the keybinds will be applied on load.
        */
        [MInputBinding(new []{Keys.LeftShift, Keys.A}, MBindPressReq.AllPressed, MBindOrderReq.InOrder)]
        public static void BoundToKeys()
        {
            /*
            This will be called every time LeftShift and then A are pressed down, 
            in order, and without letting go of the previous keys (in this case just LeftShift)
            
            If you want to access the binding to enable and disable it, you can use the GetBind
            method to find your keybinding from the class and method it's attached to.
            */

            var binding = MInputBindingHandler.GetBind(typeof(MInputSystemDemo), nameof(BoundToKeys));

            /*
            And then to disable/enable it:
            */

            binding.Deactivate();
            
            binding.Activate();
            
            /*
            To demonstrate this & the UI system functioning,
            we will also open the UI that was created in the
            MUiSystemDemo here. Feel free to try it out! 
            */
            
            MUiHandler.Open("DemoPanel");
        }
        
        public static void ManualBoundToKeys()
        {
            /*
            Alternatively, if you want to create them manually, you can use the
            MInputBindingHandler. This also gives you easier control over enabling
            and disabling the keybinds!
            
            We need the method info object for this, so we use AccessTools, which is
            looked at more in the PatchSystemDemo.
            */

            var method = AccessTools.Method(typeof(MInputSystemDemo), nameof(ManualBoundToKeys));

            /*
            Then we create the binding.  
            
            By default, bindings are activated, so if we only want it to trigger
            later, we need to immediately deactivate it, like is done here.
            */
            
            var binding = MInputBindingHandler.CreateBind(new[] {Keys.F, Keys.K}, MBindPressReq.EachPressed, MBindOrderReq.AnyOrder, method);

            binding.Deactivate();
            
        }
    }
}