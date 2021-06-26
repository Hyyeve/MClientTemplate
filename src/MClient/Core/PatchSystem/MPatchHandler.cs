using System.Reflection;
using DuckGame;
using Harmony;
using MClientCore.MClient.Core;

namespace MClient.PatchSystem.Internal
{
    public static class MPatchHandler
    {
        public static void Patch()
        {
            MLogger.Log("Patcher started", logSection: ".PTCH");

            HarmonyInstance harmony = HarmonyLoader.Loader.harmonyInstance;

            MethodBase originalMethod = AccessTools.DeclaredMethod(typeof(Level), "PostDrawLayer");
            MethodInfo patchMethod = AccessTools.DeclaredMethod(typeof(MPatches), "GameDraw");
            harmony.Patch(originalMethod, null, new HarmonyMethod(patchMethod));

            originalMethod = AccessTools.DeclaredMethod(typeof(MonoMain), "RunDraw");
            patchMethod = AccessTools.DeclaredMethod(typeof(MPatches), "PreGameDraw");
            harmony.Patch(originalMethod,new HarmonyMethod(patchMethod));
            
            originalMethod = AccessTools.DeclaredMethod(typeof(MonoMain), "RunDraw");
            patchMethod = AccessTools.DeclaredMethod(typeof(MPatches), "PostGameDraw");
            harmony.Patch(originalMethod, null, new HarmonyMethod(patchMethod));

            MLogger.Log("Patched Draw Events", logSection: ".PTCH");

            originalMethod = AccessTools.DeclaredMethod(typeof(MonoMain), "Update");
            patchMethod = AccessTools.DeclaredMethod(typeof(MPatches), "PostFixUpdate");
            harmony.Patch(originalMethod, null, new HarmonyMethod(patchMethod));

            originalMethod = AccessTools.DeclaredMethod(typeof(MonoMain), "Update");
            patchMethod = AccessTools.DeclaredMethod(typeof(MPatches), "PreFixUpdate");
            harmony.Patch(originalMethod, new HarmonyMethod(patchMethod));

            MLogger.Log("Patched Game Update Events", logSection: ".PTCH");

            originalMethod = AccessTools.DeclaredMethod(typeof(MonoMain), "OnExiting");
            patchMethod = AccessTools.DeclaredMethod(typeof(MPatches), "PreFixExit");
            harmony.Patch(originalMethod, new HarmonyMethod(patchMethod));

            originalMethod = AccessTools.DeclaredMethod(typeof(Program), "HandleGameCrash");
            patchMethod = AccessTools.DeclaredMethod(typeof(MPatches), "PreFixExit");
            harmony.Patch(originalMethod, new HarmonyMethod(patchMethod));

            MLogger.Log("Patched Game Exit Events", logSection: ".PTCH");

            MLogger.Log("Patcher finished", logSection: ".PTCH");
            
        }
    }
}