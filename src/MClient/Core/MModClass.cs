using System;
using System.Reflection;
using DuckGame;

namespace MClientCore.MClient.Core
{
    public class MModClass : ClientMod
    {

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

        public static ModConfiguration Config;

        private static readonly PropertyInfo SteamIdField =
            typeof(ModConfiguration).GetProperty("workshopID", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly PropertyInfo DisabledField =
            typeof(ModConfiguration).GetProperty("disabled", BindingFlags.Instance | BindingFlags.NonPublic);

        protected override void OnPreInitialize()
        {
            Config = configuration;
            MLogger.Initialise();
            MLogger.Log("Init started..", logSection: ".CORE");
            MCoreHandler.OnPreInit();
        }
        
        protected override void OnPostInitialize()
        {
            MCoreHandler.OnPostInit();
            MLogger.Log("Initialised!", logSection: ".CORE");
        }

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
