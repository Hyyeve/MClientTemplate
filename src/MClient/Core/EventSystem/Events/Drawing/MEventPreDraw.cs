using System;

namespace MClient.Core.EventSystem.Events.Drawing
{
    /// <summary>
    /// MEvent that is called before drawing is started.
    /// WARNING: Renderers are not active at this point,
    /// and attempting to do rendering with this event will throw a exception.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MEventPreDraw : MEvent
    {
        
    }
}