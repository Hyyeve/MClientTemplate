using System.Reflection;
using DuckGame;
using Harmony;

namespace MClient.Core.PatchSystem
{
    /// <summary>
    /// The handler that does all patching for default events.
    /// </summary>
    public static class MPatchHandler
    {
        /// <summary>
        /// The method that does all patching for default events.
        /// </summary>
        public static void Patch()
        {
            MLogger.Log("Patcher started", logSection: MLogger.MLogSection.Ptch);

            var harmony = HarmonyLoader.Loader.harmonyInstance;

            MethodBase originalMethod = AccessTools.DeclaredMethod(typeof(Level), "PostDrawLayer");
            var patchMethod = AccessTools.DeclaredMethod(typeof(MPatches), "GameDraw");
            harmony.Patch(originalMethod, null, new HarmonyMethod(patchMethod));

            originalMethod = AccessTools.DeclaredMethod(typeof(MonoMain), "RunDraw");
            patchMethod = AccessTools.DeclaredMethod(typeof(MPatches), "PreGameDraw");
            harmony.Patch(originalMethod,new HarmonyMethod(patchMethod));
            
            originalMethod = AccessTools.DeclaredMethod(typeof(MonoMain), "RunDraw");
            patchMethod = AccessTools.DeclaredMethod(typeof(MPatches), "PostGameDraw");
            harmony.Patch(originalMethod, null, new HarmonyMethod(patchMethod));

            MLogger.Log("Patched Draw Events", logSection: MLogger.MLogSection.Ptch);

            originalMethod = AccessTools.DeclaredMethod(typeof(MonoMain), "Update");
            patchMethod = AccessTools.DeclaredMethod(typeof(MPatches), "PostFixUpdate");
            harmony.Patch(originalMethod, null, new HarmonyMethod(patchMethod));

            originalMethod = AccessTools.DeclaredMethod(typeof(MonoMain), "Update");
            patchMethod = AccessTools.DeclaredMethod(typeof(MPatches), "PreFixUpdate");
            harmony.Patch(originalMethod, new HarmonyMethod(patchMethod));

            MLogger.Log("Patched Game Update Events", logSection: MLogger.MLogSection.Ptch);

            originalMethod = AccessTools.DeclaredMethod(typeof(MonoMain), "OnExiting");
            patchMethod = AccessTools.DeclaredMethod(typeof(MPatches), "PreFixExit");
            harmony.Patch(originalMethod, new HarmonyMethod(patchMethod));

            originalMethod = AccessTools.DeclaredMethod(typeof(Program), "HandleGameCrash");
            patchMethod = AccessTools.DeclaredMethod(typeof(MPatches), "PreFixExit");
            harmony.Patch(originalMethod, new HarmonyMethod(patchMethod));

            MLogger.Log("Patched Game Exit Events", logSection: MLogger.MLogSection.Ptch);

            MLogger.Log("Patcher finished", logSection: MLogger.MLogSection.Ptch);
            
        }
    }
}