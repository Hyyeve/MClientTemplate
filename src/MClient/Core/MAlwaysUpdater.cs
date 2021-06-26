using System;
using DuckGame;
using MClient.Core.EventSystem;
using MClient.Core.EventSystem.Events.Game;
using MClient.Core.EventSystem.Events.Helper;

namespace MClient.Core
{

    /// <summary>
    /// Core class that handles some special functionality.
    /// </summary>
    public static class MAlwaysUpdater
    {
        [MEventInit]
        public static void Init()
        {
            MEventHandler.Register(typeof(MAlwaysUpdater));
        }
        
        [MEventPostGameUpdate]
        private static void DoLobbyStuff()
        {
            if (!ModLoader.modsEnabled || !(Level.current is TeamSelect2) || Steam.lobby == null || Steam.lobby.id == 0UL)
            {
                _updateLobby = true;
                return;
            }
            string text;
            if (!(Level.current is TeamSelect2) || !_updateLobby ||
                string.IsNullOrEmpty(text = Steam.lobby.GetLobbyData("mods"))) return;
            int num = text.IndexOf(MModClass.ReplaceData, StringComparison.Ordinal);
            if (num < 0)
            {
                _updateLobby = false;
                return;
            }
            text = text.Remove(num, MModClass.ReplaceData.Length).Trim('|').Replace("||", "|");
            Steam.lobby.SetLobbyData("mods", text);
            _updateLobby = false;
        }
        
        private static bool _updateLobby;
    }
}
