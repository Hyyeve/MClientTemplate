using System;

namespace MClient.SettingsSystem
{
    /// <summary>
    /// Auto-Attribute that marks a field to be automatically saved and reloaded.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)] 
    public class MSerializeSettingAttribute : Attribute
    {
        
    }
}