using System;

namespace MClient.Core.EventSystem.Events
{
    /// <summary>
    /// Core MEvent that all other standard events are derived from.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)] 
    public abstract class MEvent : Attribute
    {
        
    }
}