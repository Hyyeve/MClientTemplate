using System;
using System.Text.RegularExpressions;
using DuckGame;

namespace MClientCore.MClient.Core
{
    public static class MLogger
    {

        private static string _modName;
        private static bool _initialised;
        
        public static void Initialise()
        {
            if (_initialised) return;
            string name = MModClass.Config.displayName;
            if (name.Length > 6)
            {
                Regex regex = new Regex("[^A-Z]");
                name = regex.Replace(name, "");
            }
            _modName = name;
            _initialised = true;
        }
        
        public static void Log(string message, LogType logType = LogType.Info, string logSection = "")
        {
            if (!_initialised)
            {
                Initialise();
                return;
            }
            DevConsole.Log(DCSection.General,  "|MENUORANGE|" + _modName + logSection + ": " + LogTypeToColor(logType) + message);
        }

        public static void Log(string message, Exception exe)
        {
            Log(message + " [" + exe.GetType().Name + "]", LogType.Error, ".EXCPTION");
        }

        private static string LogTypeToColor(LogType type)
        {
            switch (type)
            {
                case LogType.Info:
                    return "|LIGHTGRAY|";
                case LogType.Warning:
                    return "|DGYELLOW|";
                case LogType.Error:
                    return "|RED|";
                case LogType.Unique:
                    return "|PURPLE|";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public enum LogType
        {
            Info, Warning, Error, Unique
        }

        public enum DuckGameColours
        {
            AQUA, RED, WHITE, BLACK,
            DARKNESS, BLUE, DGBLUE,
            DGRED, DGREDDD, DGGREEN,
            DGREENN, DGYELLOW, DGORANGE, 
            ORANGE, MENUORANGE,
            YELLOW, GREEN, TIMELINE,
            GRAY, LIGHTGRAY, CREDITSGRAY,
            BLUEGRAY, PINK, PURPLE,
            DGPURPLE, CBRONZE, CSILVER, 
            CGOLD, CPLATINUM, CDEV, 
            DUCKCOLOR1, DUCKCOLOR2, 
            DUCKCOLOR3, DUCKCOLOR4, 
            RBOW_1, RBOW_2, RBOW_3,
            RBOW_4, RBOW_5, RBOW_6, 
            RBOW_7
        }
    }
}