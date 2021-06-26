using System;

namespace MClient.Core.EventSystem.Events.Helper
{
    /// <summary>
    /// MEvent that is called during load, just after the MEventInit.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MEventLateInit : Attribute
    {
        
    }
}