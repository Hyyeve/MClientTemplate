using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DuckGame;
using MClient.EventSystem.Events.Game;
using MClient.EventSystem.Events.Helper;
using MClientCore.MClient.Core;

namespace MClient.SettingsSystem
{
    [MAutoRegisterMEvents]
    public static class MSettingHandler
    {

        private static string _savePath;
        
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
                MLogger.Log("No save data to load!", logSection: ".SAVE");
                return;
            }
            
            List<string> saveData = File.ReadAllLines(_savePath).ToList();

            MLogger.Log("Loading save data..", logSection: ".SAVE");
            
            foreach (string data in saveData)
            {
                string[] splitData = data.Split(':');
                Type type = Type.GetType(splitData[0]);
                FieldInfo field = type.GetField(splitData[1]);
                
                try
                {
                    if (field.FieldType == typeof(string))
                    {
                        field.SetValue(null, splitData[2]);
                        continue;
                    }

                    if (field.FieldType == typeof(float))
                    {
                        field.SetValue(null, float.Parse(splitData[2]));
                        continue;
                    }

                    if (field.FieldType == typeof(bool))
                    {
                        field.SetValue(null, bool.Parse(splitData[2]));
                        continue;
                    }

                    if (field.FieldType == typeof(double))
                    {
                        field.SetValue(null, double.Parse(splitData[2]));
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
                }
                catch
                {
                    MLogger.Log("Failed to load setting " + splitData[1], MLogger.LogType.Warning, ".SAVE");
                }
            }

            MLogger.Log("Save data loaded!", logSection: ".SAVE");
            
        }

        private static void SaveAllData()
        {
            List<string> saveData = new List<string>();

            MLogger.Log("Saving all data...", logSection: ".SAVE");

            foreach (FieldInfo field in GetAllSettings())
            {

                string data = field.DeclaringType + ":" + field.Name + ":";
                
                if (field.FieldType == typeof(string))
                {
                    data += (string)field.GetValue(null);
                    saveData.Add(data);
                    continue;
                }

                if (field.FieldType == typeof(float))
                {
                    data +=  (float) field.GetValue(null);
                    saveData.Add(data);
                    continue;
                }

                if (field.FieldType == typeof(bool))
                {
                    data +=  (bool) field.GetValue(null);
                    saveData.Add(data);
                    continue;
                }

                if (field.FieldType == typeof(double))
                {
                    data +=  (double) field.GetValue(null);
                    saveData.Add(data);
                    continue;
                }
                

                if (field.FieldType == typeof(Color))
                {
                    Color col = (Color)field.GetValue(null);
                    data += col.a + "." + col.b + "." + col.g + "." + col.r;
                    saveData.Add(data);
                    continue;
                }

                if (field.FieldType == typeof(Vec2))
                {
                    Vec2 vec = (Vec2) field.GetValue(null);
                    data += vec.x + "." + vec.y;
                    saveData.Add(data);
                    continue;
                }
            }
            
            File.WriteAllLines(_savePath, saveData);

            MLogger.Log("Saved!", logSection: ".SAVE");
        }

        private static List<FieldInfo> GetAllSettings()
        {
            return Assembly.GetExecutingAssembly().GetTypes().SelectMany(x => x.GetFields()).Where(x =>
                x.GetCustomAttributes(typeof(MSerializeSettingAttribute), false).FirstOrDefault() != null).ToList();
        }
    }
}