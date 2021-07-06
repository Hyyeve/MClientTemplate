using MClient.Core.EventSystem;
using MClient.Core.EventSystem.Events.Drawing.Screen;
using MClient.Core.EventSystem.Events.Drawing.World;
using MClient.Core.EventSystem.Events.Helper;
using MClient.Core.EventSystem.Events.Input;

namespace MClient.MTemplate.EventSystemDemo
{
    /// <summary>
    /// Demonstration class for the event system and custom events!
    /// </summary>
    
    /*
     This attribute marks a class to have all events automatically
     registered on load, which means you don't need to manually register
     them, and gives you an additional entry point hook if you need it.
     
     Be aware that only static event methods will be registered
     with this, since the event system cannot create an instance 
     of the class for you.
     
     This attribute can only go on the class itself.
    */
    [MAutoRegisterEvents]
    public class MEventSystemDemo
    {
        
        /*
        All default event names are prefixed with MEvent, and can be
        applied to methods like shown here. For the event to
        be registered by the AutoRegisterEvents, the method must be
        static. All event methods must be public, otherwise the
        event handler can't access them.
        */
        [MEventWorldDrawGame]
        public static void OnDrawGame()
        {
            /*
            This will be called every time the
            Game layer is drawn.
            */
        }
        
        /*  
        The event method may also take a instance of the
        event as a parameter. This allows you to pass
        extra information along with the event, such as in
        this case, a Keys enum of exactly what key was
        pressed.
        
        All event methods must either have no parameters,
        or a single parameter that is the event type.
        */
        [MEventKeyPressed]
        public static void OnKeyPress(MEventKeyPressed e)
        {
            /*
            This will be called every time a key is pressed.
            */
            var key = e.Key;
        }

        /*
        The event system also supports multiple attributes on
        a method! However, the method will be unable to take
        any of the events as parameters, so it's generally
        more useful to have separate methods.
        */
        [MEventScreenDrawBackground]
        [MEventScreenDrawBlocks]
        public static void OnDrawMultiple()
        {
            //This will be called for both Background and Blocks layer draw events.
        }


        /*
        There are also some special events that do not require
        you to register the class first - Init, EarlyInit, and LateInit.
        These will be called on all classes no matter what, subject to the
        same requirements of the method being public and static.
        
        (Static is always required for init events, 
        as they are never called for instances)
        */
        [MEventInit]
        public static void Init()
        {
            /*
            This will be called once, after classes have been
            auto-registered, but before any patching is done.
            
            EarlyInit and LateInit are called directly before and
            after this event, however you should only use those
            ones if you know what you're doing and really need them.
            */
            
            
            /*
            We can use the MEventHandler to manually register or de-register classes
            and instances, which lets you dynamically control whether your class or
            object receives events or not.
            
            Here, we're de-registering this class, meaning that none of the event
            methods will actually be called when running the mod. Init events 
            are called just after auto-registering, but before any events are 
            actually called, so we can de-register everything here before anything is called.
            
            The first parameter is the type of the class you want to de-register,
            and the second parameter - which we're setting to null here, for demonstration
            purposes - is the instance that you want to de-register. Since this is all
            static, there isn't an instance, and we just pass null (though you can
            also just leave out the parameter entirely).
            
            To register classes and instances, the syntax is exactly the same, just
            with the MEventHandler.Register() method instead.
            */
            MEventHandler.DeRegister(typeof(MEventSystemDemo), null);
            
            
            /*
            Next up, custom events. These are pretty easy to set up and use.
            
            All you need is a custom event class that extends MEvent, then you
            can just do this call here, and it'll all be handled for you.
            
            Have a look at the MEventTemplate class for some more info.
            */
            MEventHandler.Call(new MEventTemplate());
            
            /*
            Alternatively, if we want to call an event with parameters, we
            create a custom event with some kind of Get() method - again,
            take a look at the template class (in this case, MEventTemplateWithParams)
            */
            MEventHandler.Call(MEventTemplateWithParams.Get("hi!"));
        }
        
        /*
        And then to use it, it's as simple as adding it as an attribute! 
        */
        [MEventTemplate]
        public static void OnEventTemplate() { }

        /*
        And of course, the one with params: 
        */
        [MEventTemplateWithParams]
        public static void OnEventTemplateWithParams(MEventTemplateWithParams e)
        {
            /*
            To be clear, the params inside the template event do not
            need to be "object", you can set them to whatever you want.
            Object is just used for demonstration, which is why we have to
            cast back to string here. 
            */
            string value = (string) e.Param;
        }
    }
}