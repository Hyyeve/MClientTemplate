using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using MClient.PatchSystem.AutoPatcher;
using MClientCore.MClient.Core;

namespace MClientCore.MClient.PatchSystem.AutoPatcher
{
    public static class MAutoPatchHandler
    {
        public static void Patch()
        {

            HarmonyInstance harmony = HarmonyLoader.Loader.harmonyInstance;
            
            MLogger.Log("AutoPatcher started", logSection: ".PTCH");

            foreach (MethodInfo origInfo in GetAllAutoPatches())
            {
                List<MAutoPatchAttribute> attributes = origInfo.GetCustomAttributes(typeof(MAutoPatchAttribute),false).Cast<MAutoPatchAttribute>().ToList();

                foreach (MAutoPatchAttribute attribute in attributes)
                {
                    MethodInfo mPatch = AccessTools.DeclaredMethod(attribute.type, attribute.method);

                    if (mPatch is null)
                    {
                        MLogger.Log("Failed to find specified method: " + attribute.method + ". on type of: " + attribute.type.Name, MLogger.LogType.Warning, ".PTCH");
                    }
                    else
                    {
                        switch (attribute.patchType)
                        {
                            case PatchType.Prefix:
                                harmony.Patch(mPatch, new HarmonyMethod(origInfo));
                                break;
                            case PatchType.Postfix:
                                harmony.Patch(mPatch, null, new HarmonyMethod(origInfo));
                                break;
                            case PatchType.Transpiler:
                                harmony.Patch(mPatch, null, null, new HarmonyMethod(origInfo));
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        MLogger.Log("Patched method " + origInfo.DeclaringType.Name + "." + origInfo.Name + " Onto " + attribute.type.Name + "." + attribute.method, logSection:".PTCH");
                    }
                }
            }

            MLogger.Log("AutoPatcher finished", logSection:".PTCH");
        }

        private static List<MethodInfo> GetAllAutoPatches()
        {
            return Assembly.GetExecutingAssembly().GetTypes().SelectMany(x => x.GetMethods()).Where(x =>
                x.GetCustomAttributes(typeof(MAutoPatchAttribute), false).FirstOrDefault() != null).ToList();
        }
    }
}