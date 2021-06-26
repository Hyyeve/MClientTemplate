using System;

namespace MClient.Core.EventSystem.Events.Drawing.Screen
{
    /// <summary>
    /// MEvent that is called after drawing the Lighting layer, with the renderer set to use Screen positions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MEventScreenDrawLighting : MEvent
    {
    }
}