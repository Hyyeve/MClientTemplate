using System.Collections.Generic;
using DuckGame;
using Harmony;
using MClient.UiSystem.Default;
using MClient.UiSystem.Internal;
using MClient.UiSystem.Internal.Attributes;
using MClient.UiSystem.Internal.Components;

namespace MClient.MTemplate
{
    /// <summary>
    /// Demonstration class for the UI system!
    /// </summary>

    /*
    The UI system is very complex internally, but it's up to you
    how you decide to use it. I've implemented a easy-to-use 
    attribute system for it, as well as lots of default
    functionality. You can also have more control over the UI
    by manually constructing your UI panels, which is also
    relatively easy to do.

    And, if you really feel up to it, it's possible to build on
    the UI system with your own code, to create completely
    custom visuals and elements. I also do intend to add
    more visual options for UI later, too.
    
    Anyway, let's get started! For the auto-attribute
    system, we need to start with the MAutoUi attribute, which
    goes on a class. UI classes made this way must be
    fully static.
    
    It requires a panel name, which you need to access a lot of
    UI-related functions later, and has optional parameters for
    the way the panel is auto-generated.
    */
    
    [MAutoUi("DemoPanel", 3, true, MUiArrangement.HeightDecreasing)]
    public static class MUiSystemDemo
    {
        /*
        To add elements to the UI, there's a number of
        different attributes for different things, mostly
        for fields.
        
        Of note, all UI is directly tied to the fields. Changes
        in the UI instantly and directly change the fields, and
        changes to the fields via code are instantly visible on
        the UI.
        
        A warning! - ALWAYS give UI fields default values. If
        you don't assign a value to them, you may get strange
        bugs or instant crashes when opening your UI.
        
        For numbers, there's two choices. Sliders & Scrollers.
        Slider values are as you'd expect - they create a
        slider in the UI that you can drag between the min
        and max values you pass. You also have the option
        to specify a title override - this will set the display
        name for the value. If you don't include it, it will
        just use the name of the field.
        */
        [MUiSlider(0f, 5f, "Slider!")]
        public static float Slider = 0f;

        /*
        Scroller values are similar, but don't take a min
        and max. Instead, it creates a UI element that can
        be dragged infinitely to any value. You can also
        change it in smaller increments by scrolling with the
        mouse wheel while hovered over it.
        
        Of note, these attributes work on all types of primitive
        numbers - ints, floats, and doubles.
        */
        [MUiValueScroller("Scroller!")]
        public static float Scroller = 0f;
        
        /*
        You can make simple toggles out of bools.
        I've also included a MUiColor attribute here! These
        attributes let you customize the colours of various parts
        of the UI. Ones put on UI elements will only affect that 
        element, if you put ones on the class itself, those 
        colours will be used as the defaults for everything.
        
        You can also include multiple color attributes to color
        the different "areas" that the UI is split into! (In this
        case, I'm setting the color for text)
        */
        [MUiToggle("Toggle!")]
        [MUiColor(100,0,100, MUiColorArea.Text)]
        public static bool Toggle = false;

        /*
        It also supports colour pickers!
        */
        [MUiColorPicker("Colour Picker!")]
        public static Color Colour = Color.Aqua;
        
        /*
        And "enum switchers"! These elements show 
        up as buttons that allow you to cycle forwards 
        and  backwards through the enum options by
        left and right clicking.
        */
        [MUiEnumSwitcher("Enum!")]
        public static MDemoEnum Enum = MDemoEnum.Demo;

        
        /*
        And the final (currently supported) field-based
        element is the TextDisplayBox. Since it can't
        easily define its own size like the others, it
        requires you to set a specific size, which is
        in world/game space.
        */
        [MUiTextDisplayBox(50, 50, "DisplayBox")]
        public static List<string> DisplayBox = new List<string>();

        /*
        But there's also one that attaches to methods!
        The ActionButton attribute creates a simple button
        in the UI, which will call the method it's attached to!
        The method cannot have any parameters, though.
        */
        [MUiActionButton("ActionButton!")]
        public static void Action()
        {
            
        }

        public static void ManualUi()
        {
            /*
            If you wish to create a UI manually, you can
            do that with the following functions. I'm not
            covering every single UI element here, but
            all the same ones are supported. 
            
            Note that having UI attributes on the fields you're using
            is still required, as they hold important parameters!
            */
            
            //We're going to make the panel auto-resize so we pass 0 size in here.
            var panel = new MDefaultUiContainer(Vec2.Zero, Vec2.Zero);
            
            //Enable auto-resizing and set the number of elements per row
            panel.EnableAlwaysAutoResize(3);
            
            /*
            Adding elements. Passing zero position as it will be set by the panel.
            The AccessTools are used for getting the required member info. They're
            explained properly in the MPatchSystemDemo.
            */
            panel.AddElement(new MDefaultUiSliderElement(Vec2.Zero, AccessTools.Field(typeof(MUiSystemDemo), nameof(Slider))));
            panel.AddElement(new MDefaultUiValueScrollerElement(Vec2.Zero, AccessTools.Field(typeof(MUiSystemDemo), nameof(Scroller))));
            panel.AddElement(new MDefaultUiToggleElement(Vec2.Zero, AccessTools.Field(typeof(MUiSystemDemo), nameof(Toggle))));
            
            //Auto-Sort the elements. You don't need to do this if you order your AddElement calls as you want them to appear in the UI.
            panel.AutoSortElements(MUiArrangement.WidthIncreasing);
            
            //And add the panel to the handler.
            MUiHandler.AddPanel("ManualDemoPanel",panel);
        }

        public static void UiHandling()
        {
            /*
            And once you have created your UI, there's
            a few different calls you can do to manage
            it. 
            */
            
            //You can open the panel with its ID
            MUiHandler.Open("DemoPanel");
            
            //And of course you can close it too
            MUiHandler.Close("DemoPanel");
            
            //You can also completely remove a panel if you wish
            MUiHandler.RemovePanel("DemoPanel");
            
            //And you can change the base colour for a panel
            MUiHandler.SetCol("DemoPanel", MUiColorArea.Base, Color.Aqua);
        }
    }


    public enum MDemoEnum
    {
        A,B,C,Demo
    }
}