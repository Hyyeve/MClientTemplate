using System;

namespace MClient.Core.EventSystem.Events.Game
{
    /// <summary>
    /// MEvent that is called just after the game has finished running its main update loop.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MEventPostGameUpdate : MEvent
    {
    }
}