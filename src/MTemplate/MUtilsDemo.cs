using System;
using DuckGame;
using MClient.Core;
using MClient.Core.Utils;

namespace MClient.MTemplate
{
    /// <summary>
    /// Documentation class for the various utils that the template includes!
    /// </summary>
    public static class MUtilsDemo
    {
        /*
        First up, a quick explanation of the DLL system. It's completely
        automatic, so there's nothing to really go over in terms of how
        to use it, but I should explain what it does and why it's useful.
        
        The DLL system is a core system that makes loading custom DLLs easy.
        Duck Game will not load any dependencies of your mod itself, and as
        such, if your code relies on external DLLs or libraries, your mod
        will fail to load or crash.
        
        This template is already using Harmony and MoreLinq though, both of
        which are external dependencies - so how does the DLL system handle
        them? The short answer is that the DLL system catches the exceptions
        that happen when code that relies on a dependency tries to run, and
        attempts to load the dependency needed, checking several different
        options.
        
        On the user end, all you need to do to get your dependencies working
        is to make sure you include the DLL in the content/dlls folder of the
        mod, then reference them from there in your IDE.
        
        An important note about the system - it checks the already loaded
        assemblies for the dependency, which prevents Harmony clashes as
        long as all mods are using the same version of Harmony and at most,
        only one of them isn't using a similar dependency loader. However,
        this means that for your own dependencies, you must be careful that
        they are uniquely named and all mods that use them use the same version,
        otherwise the loader may attempt to load a incorrect version from a 
        different mod.
        */

        public static void LoggerUtilDemo()
        {
            /*
            The MLogger class holds various logging
            methods for ease of use and consistency
            across your mod. I'll go over the important
            features of it here.
            
            Currently, all logs go directly to the in-game
            DevConsole, which can be opened by pressing the 
            @ key (On UK layout keyboards), but I do have plans
            to expand the system in future.


            The Log method takes (up to) three parameters. The message to log, the 
            log type, and the LogSection. The first two should be self explanatory, 
            but the LogSection is important. It defines what prefix the log message
            is given - as you will notice if you run the template and open the DevConsole,
            all the logs from the initialization of the template have prefixes that
            are abbreviations of what system they are from. For example, all patching
            logs have the prefix .PTCH, and all core logs have the prefix .CORE
            
            These are passed as an enum value. There's a number of built-in sections,
            however, you should create your own custom sections for logging in your own
            code as needed.
            */
            MLogger.Log("Hey! A log!", MLogger.MLogType.Info, MLogger.MLogSection.None);
            
            /*
            There is also an overload that takes an exception instance, which will
            automatically convert itself to a Error in the Excp section, and append
            [ExceptionType] to the message. This is useful for quicker logging
            of exceptions.
            */
            MLogger.Log("Hey! An exception!", new Exception());
        }

        public static void DelayUtilDemo()
        {
            /*
            This util is designed to let you run code with specific
            timing easily.
            
            To use it, you first need to create an instance. (Which should
            be a field in real use. Declaring it inside a method
            would cause it to be re-created every time the method was
            called and thus not function. Here it's done purely for
            demonstration)
            */

            var delayer = new MDelayUtil();
            
            /*
            Once you have created an instance, it has a few functions you
            can use to create simple, timed code. 
            
            TimePassed will return true each time the specified
            number of milliseconds have passed.
            */

            if (delayer.TimePassed(1000))
            {
                //Any code here would run once every second (1000 milliseconds)
            }
            
            /*
            AbsoluteTimePassed will return true after the specified
            number of milliseconds have passed, but will continue to
            return true until Reset is called.
            */

            if (delayer.AbsoluteTimePassed(1000))
            {
                //Any code here would start running after one second.
            }

            /*
            If you use AbsoluteTimePassed, you can reset the
            DelayUtil as needed. Resetting when AbsoluteTimePassed
            is true will create the same effect as using TimePassed,
            however you can use Reset creatively to create more complex
            timing systems.
            */

            if (delayer.AbsoluteTimePassed(1000))
            {
                delayer.Reset();
                //Any code here would run once every second.
            }
            
            /*
            It's important to note that the DelayUtil cannot
            call code by itself, so the method you're using
            it in must be being called at least as often as
            the minimum amount of time you wish to use for
            delaying. For most event methods, this is 60 times/second,
            or once per frame, so the minimum amount of time you
            can delay by is 16 milliseconds. Additionally, you are
            precision-limited to increments of that amount, meaning
            that a delay of 100 milliseconds will actually be called
            at approximately 112 milliseconds for a method that's being
            called every frame. (That being the first frame after 
            100 milliseconds have passed)
            */
        }


        public static void MathUtilsDemo()
        {
            /*
            The MMathUtils class contains a variety of useful 
            mathematics functions that can be used for many
            purposes. I will not go over all of them, as they
            are all documented themselves and should be fairly
            self-explanatory, but I will highlight some specific
            ones you may wish to use.
            */

            
            float degrees = 180f;

            //This value allows you to (approximately) convert from degrees to radians via multiplication
            var radians = degrees * MMathUtils.DegToRad;

            //This value allows you to (approximately) convert from radians to degrees via multiplication
            degrees = radians * MMathUtils.RadToDeg;

            /*
            The utils include a number of simple math functions that support single floats
            all the way up to Vec4's. Additionally, the lerp functions are pure, unclamped
            lerps, unlike the ones in DuckGame.Lerp, which mainly produce clamped, normalized values.
            */
            MMathUtils.Lerp2(Vec2.Zero, Vec2.One, 0.5f);

            /*
            The utils also supply some useful intersection calculation functions. 
            */
            MMathUtils.CalcIntersection(Vec2.Zero, Vec2.One, Vec2.Unitx, Vec2.Unity);

            /*
            One particularly useful one is calculating where a line would hit an object. Used properly,
            this can replace many collision/raytracing functions at a fraction of the performance cost.
            Here we're passing null in for the object, but for actual use you'd need to pass in a DuckGame.Thing.
            */
            MMathUtils.CalcHitPoint(Vec2.Zero, Vec2.One, null);

        }

        public static void PositionConversionUtilDemo()
        {
            /*
            This util allows you to convert between various different coordinate systems
            easily. However, note that converting to and from UV positions is reliant on
            the Resolution class, which is known to sometimes not hold the true screen
            resolution for non 16/9 aspect ratios.
            
            All methods are explained fully in the class itself, however I'll show
            the basic and particularly useful ones here.
            */

            /*
            Converts a world/game position to a screen (pixel) position. 
            */
            MPositionConversionUtil.GameToScreenPos(Vec2.Zero);

            /*
            Converts a screen position to a world/game position. 
            */
            MPositionConversionUtil.ScreenToGamePos(Vec2.Zero);

            /*
            Clamps a position to the screen. This is used in the UI system to ensure you cannot drag panels
            offscreen. It takes a rectangle defined by a position and size, and requires you to specify
            where on the rectangle the position is centred. To clarify, if you had a rectangle with
            size [2,2] and position [0,0], here are the corners that would be generated with some different
            QuadrantAreas (given as [TopLeftCorner][BottomRightCorner]):
            
            For QuadrantArea.TopLeft: [0,0][2,2]
            For QuadrantArea.MiddleMiddle: [-1,-1][1,1]
            For QuadrantArea.MiddleRight: [-2,-1][0,1]
            
            This can understandably be confusing at times, and thus it's best to use what makes sense to you.
            The UI system always uses TopLeft, and if you wish to clamp the position of a DuckGame.Thing, you
            can really use any, but Centre and TopLeft are easiest with the built-in variables that Thing has.
            */
            MPositionConversionUtil.ClampToScreen(Vec2.One, Vec2.One, MQuadrantArea.TopLeft, true);
        }
    }
}