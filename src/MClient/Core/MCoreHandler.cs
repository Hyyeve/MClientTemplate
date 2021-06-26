using System.Reflection;
using DuckGame;
using MClient.Core.DLLSystem;
using MClient.Core.EventSystem;
using MClient.Core.EventSystem.Events.Drawing;
using MClient.Core.EventSystem.Events.Game;
using MClient.Core.PatchSystem;
using MClient.Core.PatchSystem.AutoPatcher;

namespace MClientCore.MClient.Core
{
    internal static class MCoreHandler
    {
        public static void OnPreInit()
        {
            MDependencyResolver.ResolveDependencies();
        }

        public static void OnPostInit()
        {
            MEventHandler.InitialiseAll();
            MPatchHandler.Patch();
            MAutoPatchHandler.Patch();
        }
    }

    public static class MPatches
    {
        private static void GameDraw(Layer layer)
        {
            MEventHandler.Call(MEventGameDraw.Get(layer));
        }

        private static void PreGameDraw()
        {
            MEventHandler.Call(new MEventPreDraw());
        }

        private static void PostGameDraw()
        {
            MEventHandler.Call(new MEventPostDraw());
        }
        
        private static void PreFixExit()
        {
            MEventHandler.Call(new MEventGameExit());

            if (!Program.commandLine.Contains("-download")) return;
            MModClass.Disabled = false;
            typeof(ModLoader).GetMethod("DisabledModsChanged", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, new object[0]);
        }

        private static void PostFixUpdate()
        {
            MEventHandler.Call(new MEventPostGameUpdate());
        }

        private static void PreFixUpdate()
        {
            MEventHandler.Call(new MEventPreGameUpdate());
        }
    }
}