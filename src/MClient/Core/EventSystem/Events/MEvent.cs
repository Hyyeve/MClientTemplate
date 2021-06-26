using System;

namespace MClient.EventSystem.Events
{
    [AttributeUsage(AttributeTargets.Method)] 
    public abstract class MEvent : Attribute
    {
        
    }
}