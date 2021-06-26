using System;

namespace MClient.Core.EventSystem.Events.Helper
{
    /// <summary>
    /// MEvent that is called on load.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)] 
    public class MEventInit : System.Attribute
    {
    }
}