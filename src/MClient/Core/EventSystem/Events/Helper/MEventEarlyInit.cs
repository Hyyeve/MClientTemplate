using System;

namespace MClient.Core.EventSystem.Events.Helper
{
    /// <summary>
    /// MEvent that is called during load, just before the MEventInit.
    /// </summary>
    /// <remarks>
    /// This event is used internally for some systems and custom use may cause issues. Use with caution!
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class MEventEarlyInit : Attribute
    {
        
    }
}