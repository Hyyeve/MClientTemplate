using MClient.Core.EventSystem.Events;

namespace MClient.MTemplate.EventSystemDemo
{
    
    /// <summary>
    /// A super simple template event.
    /// All that's needed to create custom events without parameters is a empty class like this which extends MEvent.
    /// It can then be used as shown in <see cref="MEventSystemDemo"/>
    /// </summary>
    public class MEventTemplate : MEvent
    {
    }
}