using System;

namespace MClient.Core.EventSystem.Events.Game
{
    /// <summary>
    /// MEvent that is called when the game exits.
    /// </summary>
    /// <remarks>
    /// This event is called both on normal exit and in the crash handler,
    /// However, due to the nature of crashes, you cannot rely on it
    /// always being called on crash.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class MEventGameExit : MEvent
    {
    }
}