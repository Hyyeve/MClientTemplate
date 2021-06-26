using System;

namespace MClient.Core.EventSystem.Events.Drawing.World
{
    /// <summary>
    /// MEvent that is called after the Parallax layer is drawn, with the renderer set to use World/Game positions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MEventWorldDrawParallax : MEvent
    {
    }
}