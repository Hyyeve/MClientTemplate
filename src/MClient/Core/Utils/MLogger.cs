using System;
using System.Text.RegularExpressions;
using DuckGame;

namespace MClient.Core
{
    /// <summary>
    /// Custom logging class for ease of use.
    /// </summary>
    public static class MLogger
    {

        private static string _modName;
        private static bool _initialised;
        private static string _prevMessage;

        /// <summary>
        /// Initialises the logger.
        /// </summary>
        public static void Initialise()
        {
            if (_initialised) return;
            string name = MModClass.Config.displayName;
            if (name.Length > 6)
            {
                var regex = new Regex("[^A-Z]");
                name = regex.Replace(name, "");
            }
            _modName = name;
            _initialised = true;
        }
        
        /// <summary>
        /// Logs a message to the in-game dev console.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="logType">The type of message</param>
        /// <param name="logSection">The section/prefix to apply to the log</param>
        public static void Log(string message, MLogType logType = MLogType.Info, MLogSection logSection = MLogSection.None)
        {
            if (!_initialised)
            {
                Initialise();
                return;
            }
            
            //if (message == _prevMessage) return;

            _prevMessage = message;

            string sect = logSection == MLogSection.None ? string.Empty : "." + logSection.ToString().ToUpper();
            DevConsole.Log(DCSection.General,  FormatColor(MDuckGameColours.MenuOrange) + _modName + sect + ": " + LogTypeToColor(logType) + message);
        }

        /// <summary>
        /// Logs an exception to the in-game dev console.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="exe">The exception</param>
        public static void Log(string message, Exception exe)
        {
            Log(message + " [" + exe.GetType().Name + "]", MLogType.Error, MLogSection.Excp);
        }

        /// <summary>
        /// Gets the corresponding color for a log type
        /// </summary>
        /// <param name="type">The log type to get the color for</param>
        /// <returns>The corresponding color for a log type</returns>
        private static string LogTypeToColor(MLogType type)
        {
            return type switch
            {
                MLogType.Info => FormatColor(MDuckGameColours.LightGray),
                MLogType.Warning => FormatColor(MDuckGameColours.DgYellow),
                MLogType.Error => FormatColor(MDuckGameColours.Red),
                MLogType.Unique => FormatColor(MDuckGameColours.Purple),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        /// <summary>
        /// Formats a MDuckGameColours color into a standard colour escape string
        /// </summary>
        /// <param name="col">The colour to convert to a string</param>
        /// <returns>The color converted to a standard color escape string</returns>
        public static string FormatColor(MDuckGameColours col)
        {
            return "|" + col.ToString().ToUpper() + "|";
        }

        public enum MLogType
        {
            Info, Warning, Error, Unique
        }

        public enum MLogSection
        {
            None, Excp, Asmb, Evnt, Ptch, Core, Rndr, Save, UsrI
        }

        /// <summary>
        /// The default chat colours that duck game accepts via color escape codes in text.
        /// </summary>
        public enum MDuckGameColours
        {
            Aqua, Red, White, Black,
            Darkness, Blue, DgBlue,
            DgRed, DgReddd, DgGreen,
            DgReenn, DgYellow, DgOrange, 
            Orange, MenuOrange,
            Yellow, Green, Timeline,
            Gray, LightGray, CreditsGray,
            BlueGray, Pink, Purple,
            DgPurple, CBronze, CSilver, 
            CGold, CPlatinum, CDev, 
            DuckColor1, DuckColor2, 
            DuckColor3, DuckColor4, 
            Rbow1, Rbow2, Rbow3,
            Rbow4, Rbow5, Rbow6, 
            Rbow7
        }
    }
}