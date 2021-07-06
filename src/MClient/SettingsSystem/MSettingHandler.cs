using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DuckGame;
using MClient.Core;
using MClient.Core.EventSystem.Events.Game;
using MClient.Core.EventSystem.Events.Helper;

namespace MClient.SettingsSystem
{
    
    /// <summary>
    /// Settings handler class that saves and loads data automatically.
    /// </summary>
    [MAutoRegisterEvents]
    public static class MSettingHandler
    {
        /// <summary>
        /// The save path. Currently all data is saved to a single file - I intend to improve this in future.
        /// </summary>
        private static string _savePath;
        
        [MEventEarlyInit]
        public static void Initialise()
        {
            _savePath = MModClass.Config.directory + "/config.quack";
            
            LoadAllData();
        }

        [MEventGameExit]
        public static void Finalise(MEventGameExit e)
        {
            SaveAllData();
        }
        
        
        private static void LoadAllData()
        {
            if (!File.Exists(_savePath))
            {
                MLogger.Log("No save data to load!", logSection: MLogger.MLogSection.Save);
                return;
            }
            
            List<string> saveData = File.ReadAllLines(_savePath).ToList();

            MLogger.Log("Loading save data..", logSection: MLogger.MLogSection.Save);
            
            foreach (string data in saveData)
            {
                string[] splitData = data.Split(':');
                var type = Type.GetType(splitData[0]);
                if (type is null) continue;
                var field = type.GetField(splitData[1]);
                
                try
                {

                    //------------------------------------------------------------------------------------------
                    //THIS IS WHERE DATA IS PARSED FOR LOADING. EDIT THIS TO ADD SUPPORT FOR MORE TYPES OF VALUE
                    
                    if (field.FieldType == typeof(string))
                    {
                        field.SetValue(null, splitData[2]);
                        continue;
                    }

                    if (field.FieldType == typeof(int))
                    {
                        field.SetValue(null, int.Parse(splitData[2]));
                        continue;
                    }

                    if (field.FieldType == typeof(float))
                    {
                        field.SetValue(null, float.Parse(splitData[2]));
                        continue;
                    }

                    if (field.FieldType == typeof(double))
                    {
                        field.SetValue(null, double.Parse(splitData[2]));
                        continue;
                    }

                    if (field.FieldType == typeof(bool))
                    {
                        field.SetValue(null, bool.Parse(splitData[2]));
                        continue;
                    }

                    if (field.FieldType == typeof(Color))
                    {
                        string[] splitSplitData = splitData[2].Split('.');
                        byte a = byte.Parse(splitSplitData[0]);
                        byte b = byte.Parse(splitSplitData[1]);
                        byte g = byte.Parse(splitSplitData[2]);
                        byte r = byte.Parse(splitSplitData[3]);
                        field.SetValue(null, new Color(r, g, b, a));
                    }

                    if (field.FieldType == typeof(Vec2))
                    {
                        string[] splitSplitData = splitData[2].Split('.');
                        float x = float.Parse(splitSplitData[0]);
                        float y = float.Parse(splitSplitData[1]);
                        field.SetValue(null, new Vec2(x, y));
                    }

                    //------------------------------------------------------------------------------------------
                }
                catch
                {
                    MLogger.Log("Failed to load setting " + splitData[1], MLogger.MLogType.Warning,
                        MLogger.MLogSection.Save);
                }
            }

            MLogger.Log("Save data loaded!", logSection: MLogger.MLogSection.Save);
            
        }

        private static void SaveAllData()
        {
            List<string> saveData = new List<string>();

            MLogger.Log("Saving all data...", logSection: MLogger.MLogSection.Save);

            foreach (var field in GetAllSettings())
            {

                string data = field.DeclaringType + ":" + field.Name + ":";

                //------------------------------------------------------------------------------------------
                //THIS IS WHERE DATA IS PARSED FOR SAVING. EDIT THIS TO ADD SUPPORT FOR MORE TYPES OF VALUE
                
                if (field.FieldType == typeof(string))
                {
                    data += (string)field.GetValue(null);
                    saveData.Add(data);
                    continue;
                }

                if (field.FieldType == typeof(int))
                {
                    data += (int) field.GetValue(null);
                    saveData.Add(data);
                    continue;
                }

                if (field.FieldType == typeof(float))
                {
                    data +=  (float) field.GetValue(null);
                    saveData.Add(data);
                    continue;
                }

                if (field.FieldType == typeof(double))
                {
                    data += (double) field.GetValue(null);
                    saveData.Add(data);
                    continue;
                }

                if (field.FieldType == typeof(bool))
                {
                    data +=  (bool) field.GetValue(null);
                    saveData.Add(data);
                    continue;
                }

                if (field.FieldType == typeof(Color))
                {
                    var col = (Color)field.GetValue(null);
                    data += col.a + "." + col.b + "." + col.g + "." + col.r;
                    saveData.Add(data);
                    continue;
                }

                if (field.FieldType == typeof(Vec2))
                {
                    var vec = (Vec2) field.GetValue(null);
                    data += vec.x + "." + vec.y;
                    saveData.Add(data);
                    continue;
                }

                //------------------------------------------------------------------------------------------
            }
            
            File.WriteAllLines(_savePath, saveData);

            MLogger.Log("Saved!", logSection: MLogger.MLogSection.Save);
        }

        private static IEnumerable<FieldInfo> GetAllSettings()
        {
            return Assembly.GetExecutingAssembly().GetTypes().SelectMany(x => x.GetFields()).Where(x =>
                x.GetCustomAttributes(typeof(MSerializeSettingAttribute), false).FirstOrDefault() != null);
        }
    }
}