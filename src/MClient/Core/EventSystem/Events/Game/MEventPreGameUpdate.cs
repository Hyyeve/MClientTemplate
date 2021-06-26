using System;

namespace MClient.Core.EventSystem.Events.Game
{
    /// <summary>
    /// MEvent that is called just before the game runs its main update loop.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MEventPreGameUpdate : MEvent
    {

    }
}