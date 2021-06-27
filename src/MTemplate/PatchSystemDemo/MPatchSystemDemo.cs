using Harmony;
using MClient.Core.PatchSystem.AutoPatcher;

namespace MClient.MTemplate.PatchSystemDemo
{
    /// <summary>
    /// Demonstration class for the patch system!
    /// </summary>
    public static class MPatchSystemDemo
    {

        /*
        This is the easy system I've implemented to do patching. It simply
        uses an attribute on the method you want to patch in, with info about
        what method you want to patch and what kind of patch you want to do.
        
        The first parameter of the attribute specifies the type - class - that has
        the method you want to patch. The second specifies the method in that class
        which you want to patch onto, and the third specifies what kind of patch you want
        to do.
        
        All standard Harmony patching features should be available, 
        see https://harmony.pardeike.net/articles/patching.html for 
        the Harmony patching documentation.
        */
        [MAutoPatch(typeof(MPatchSystemDemo), nameof(PatchReceiverMethod), MPatchType.Prefix)]
        public static void PatchMethod()
        {
            /*
            The code in this method will be added onto the start of the PatchReceiverMethod,
            which, for demonstration purposes, is right here in this class too.
            */
        }

        /*
        This method will run the PatchMethod code before it's own
        code, since we added the PatchMethod code onto it as a Prefix.
        If we were to use a Postfix instead, the PatchMethod code would
        be run after the code of this method.
        
        Transpilers are a much more advanced way of patching, and
        let you actually modify the code of the method you're patching,
        but they're too complex for me to document here. (And, to be honest,
        I don't entirely understand them myself. They should still
        be supported by my patch system, though!)
        */
        public static void PatchReceiverMethod()
        {
            
        }

        /*
        Alternatively, it's possible to manually do patches directly
        through Harmony. For compatibility between mods, you should
        always use the HarmonyLoader.Loader, as shown here, and not
        create your own harmony instance directly, as that could
        cause conflicts between different mods. 
        */
        public static void ManualPatchMethod()
        {
            /*
            We can use the AccessTools class to easily get the objects we need to do a manual patch.
            Do note how I'm using "nameof()" to get the names of the methods, rather than typing it in
            as a string, even though the parameter is really just a string. Using "nameof()" instead
            makes sure that you get it right and don't cause any errors from misspellings or similar issues.
            
            For this example, I'm patching the code of this method onto the other, so were it to actually
            run, it would cause a loop of patching itself again and again and again. This is just to
            make the demonstration code easier, and in real patches, the "methodToPatchWith" should
            never be the method that's actually doing the patch.
            */

            var methodToPatchOnto = AccessTools.Method(typeof(MPatchSystemDemo), nameof(ManualPatchReceiverMethod));
            
            var methodToPatchWith = AccessTools.Method(typeof(MPatchSystemDemo), nameof(ManualPatchMethod));

            /*
            This will patch the code of this method - the ManualPatchMethod - onto the end of the
            ManualPatchReceiverMethod. 
            
            There's only one overload for this method, which takes the original method - the method we want to
            modify - and the methods we want to use for a prefix, postfix, and transpiler. Generally, you'll
            only be using one of them at a time though, so to do just a postfix, like here, we can just pass 
            null to the other two. It works the same if you want to just do a prefix or transpiler, too.
            
            That said, I recommend using the auto-patch system instead, wherever possible, as this way is
            more complex and can give you more issues. But it is here if you need to use it!
            */
            HarmonyLoader.Loader.Patch(methodToPatchOnto, null, methodToPatchWith, null);
        }

        /*
        Just like the other PatchReceiverMethod, this one will run the code
        we patched onto it as well as it's own code. In this example, it will run
        the code of ManualPatchMethod after it's own code.
        */
        public static void ManualPatchReceiverMethod()
        {
            
        }
        
        
    }
}