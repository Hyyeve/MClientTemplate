using MClient.ExposerSystem;

namespace MClient.MTemplate.ExposerSystemDemo
{
    /// <summary>
    /// Demonstration class for the exposer system!
    /// </summary>
    public class MExposerSystemDemo
    {
        
        public static void ExposeObject()
        {
            /*
            The exposer system is very simple, but can be a bit
            confusing to understand to start with. 
            
            The first step is to create your exposed object, in this case
            I'm creating an exposed version of the DefaultPlayer1 Duck.
            
            Also, I'm using "var" here. It's a useful keyword that means you
            don't need to write out the full type of the object you're creating,
            as it implicitly knows it from what you're setting it to. However, 
            in this case, it's important. The exposedObject is actually a
            "dynamic", and if you were to fully type it as
            "MExposedObject exposedObject", it would not work correctly. You could
            also type it as "dynamic exposedObject", though.
            */

            var exposedObject = MExposedObject.From(DuckGame.Profiles.DefaultPlayer1.duck);

            /*
            Ducks have a private variable called "maxrun", which is the maximum speed they can,
            well, run at. Normally we couldn't access it, because it's private. However,
            because we created a dynamic ExposedObject from the duck, we can!
            */
            
            float value = exposedObject.maxrun;
            
            /*
            However, be careful! If you try this yourself, you'll probably notice that
            the "exposedObject" doesn't actually show any methods or fields on it, and,
            in fact, you can actually just type anything after it and it will still
            compile...
            */
            
            object random = exposedObject.LiterallyAnythingYouWantToTypeHere(":0", null, "hey look, another parameter!");
            
            /*
            That's because MExposedObjects access the values and methods dynamically, at runtime. So,
            you have to be careful to make sure that whatever methods or values you're trying to
            access do actually exist, otherwise it will crash!
            */
        }

        public static void ExposeClass()
        {
            /*
            We just looked at the ExposedObject, which works on instances of objects.
            However, if you want to access private methods or variables that are static,
            we need to use the MExposedClass object instead.
            
            It works pretty much the same, but is used to access static variables and methods
            instead of instance ones.
            */

            var exposedClass = MExposedClass.From(typeof(DuckGame.Duck));
            
            /*
            In this case, the MonoMain class has a private static method called
            "JulianDate" which we can access via our exposedClass dynamic. 
            In this case, we also need to pass in some parameters.
            */

            int value = exposedClass.JulianDate(0, 0, 0);

            /*
            Again, since it's all done at runtime, you can just type anything,
            and you have to be careful to make sure you spell the names of methods
            and variables correctly! Here, specifically, we're passing parameters,
            and you need to be extra careful with them, since putting even one
            wrong parameter could lead to a crash or using the wrong overload of
            a method and getting strange results.
            */
        }
    }
}