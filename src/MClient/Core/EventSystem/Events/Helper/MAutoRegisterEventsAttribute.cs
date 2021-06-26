using System;

namespace MClient.Core.EventSystem.Events.Helper
{
    /// <summary>
    /// This is a Auto-Attribute which will automatically register all static events in a class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MAutoRegisterEventsAttribute : Attribute
    {
        
    }
}