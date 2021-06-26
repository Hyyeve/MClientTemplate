using System;

namespace MClient.Core.EventSystem.Events.Drawing.World
{
    /// <summary>
    /// MEvent that is called after the Glow layer is drawn, with the renderer set to use World/Game positions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MEventWorldDrawGlow : MEvent
    {
    }
}