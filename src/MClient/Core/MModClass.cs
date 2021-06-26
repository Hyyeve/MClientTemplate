using System;
using System.Reflection;
using DuckGame;

namespace MClient.Core
{
    
    /// <summary>
    /// The very core class, the actual mod class. Handles some internal functionality and basic calls to load the rest of the mod.
    /// </summary>
    public class MModClass : ClientMod
    {

        /// <summary>
        /// The duck-game generated mod config. This holds useful information and settings about the mod.
        /// </summary>
        public static ModConfiguration Config;
        
        public static string ReplaceData
        {
            get
            {
                var result = !Config.isWorkshop ? "LOCAL" : SteamIdField.GetValue(Config, new object[0]).ToString();
                return result;
            }
        }

        public static bool Disabled
        {
            get => (bool) DisabledField.GetValue(Config, new object[0]);
            set => DisabledField.SetValue(Config, value, new object[0]);
        }

        private static readonly PropertyInfo SteamIdField =
            typeof(ModConfiguration).GetProperty("workshopID", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly PropertyInfo DisabledField =
            typeof(ModConfiguration).GetProperty("disabled", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Called by Duck Game while this mod is being loaded.
        /// </summary>
        protected override void OnPreInitialize()
        {
            Config = configuration;
            MLogger.Initialise();
            MLogger.Log("Init started..", logSection: MLogger.MLogSection.Core);
            MCoreHandler.OnPreInit();
        }
        
        /// <summary>
        /// Called by Duck Game after all mods have been loaded.
        /// </summary>
        protected override void OnPostInitialize()
        {
            MCoreHandler.OnPostInit();
            MLogger.Log("Initialised!", logSection: MLogger.MLogSection.Core);
        }

        /// <summary>
        /// OwO
        /// </summary>
        private void BecomeClientMod()
        {
            int num = 5;
            int num2 = 2;
            DevConsole.Log(((double)num + (double)num2 * Math.Sin(5.0) + (double)(36 * num / 25 % 267 * 4) > 5.0).ToString() + num.ToString(), Color.Black, 2f, -1);
            typeof(ModLoader).GetField("hiddendata", BindingFlags.GetField | BindingFlags.GetProperty);
            typeof(ModLoader).GetField("loadedmods", BindingFlags.Instance | BindingFlags.Public);
            typeof(ModLoader).GetField("fly you fools", BindingFlags.Instance | BindingFlags.Public);
            typeof(Mod).GetField("AllMods", BindingFlags.Instance | BindingFlags.Public);
            typeof(Mod).GetField("mod", BindingFlags.Instance | BindingFlags.Public);
        }
        
    }
}
