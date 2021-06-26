using System.Collections.Generic;
using DuckGame;
using MClient.Core.EventSystem;
using MClient.Core.EventSystem.Events;
using MClient.Core.EventSystem.Events.Drawing.Screen;
using MClient.Core.EventSystem.Events.Drawing.World;
using MClient.Core.EventSystem.Events.Helper;

namespace MClient.RenderSystem
{
    /// <summary>
    /// Internal class that helps with calling render events.
    /// </summary>
    public static class MRenderEventHelper
    {
        private static Dictionary<Layer, MEvent> _layerToScreenDrawEvent;
        private static Dictionary<Layer, MEvent> _layerToWorldDrawEvent;

        [MEventInit]
        public static void SetupDictionaries()
        {
            _layerToScreenDrawEvent = new Dictionary<Layer, MEvent>();
            _layerToWorldDrawEvent = new Dictionary<Layer, MEvent>();
            
            _layerToScreenDrawEvent.Add(Layer.PreDrawLayer, new MEventScreenDrawPre());
            _layerToScreenDrawEvent.Add(Layer.Parallax, new MEventScreenDrawParallax());
            _layerToScreenDrawEvent.Add(Layer.Virtual, new MEventScreenDrawVirtual());
            _layerToScreenDrawEvent.Add(Layer.Background, new MEventScreenDrawBackground());
            _layerToScreenDrawEvent.Add(Layer.Game, new MEventScreenDrawGame());
            _layerToScreenDrawEvent.Add(Layer.Blocks, new MEventScreenDrawBlocks());
            _layerToScreenDrawEvent.Add(Layer.Glow, new MEventScreenDrawGlow());
            _layerToScreenDrawEvent.Add(Layer.Lighting, new MEventScreenDrawLighting());
            _layerToScreenDrawEvent.Add(Layer.Foreground, new MEventScreenDrawForeground());
            _layerToScreenDrawEvent.Add(Layer.HUD, new MEventScreenDrawHud());
            _layerToScreenDrawEvent.Add(Layer.Console, new MEventScreenDrawConsole());

            _layerToWorldDrawEvent.Add(Layer.PreDrawLayer, new MEventWorldDrawPre());
            _layerToWorldDrawEvent.Add(Layer.Parallax, new MEventWorldDrawParallax());
            _layerToWorldDrawEvent.Add(Layer.Virtual, new MEventWorldDrawVirtual());
            _layerToWorldDrawEvent.Add(Layer.Background, new MEventWorldDrawBackground());
            _layerToWorldDrawEvent.Add(Layer.Game, new MEventWorldDrawGame());
            _layerToWorldDrawEvent.Add(Layer.Blocks, new MEventWorldDrawBlocks());
            _layerToWorldDrawEvent.Add(Layer.Glow, new MEventWorldDrawGlow());
            _layerToWorldDrawEvent.Add(Layer.Lighting, new MEventWorldDrawLighting());
            _layerToWorldDrawEvent.Add(Layer.Foreground, new MEventWorldDrawForeground());
            _layerToWorldDrawEvent.Add(Layer.HUD, new MEventWorldDrawHud());
            _layerToWorldDrawEvent.Add(Layer.Console, new MEventWorldDrawConsole());
        }

        public static void CallWorldDrawLayerEvent(Layer drawLayer)
        {
            if(_layerToWorldDrawEvent.ContainsKey(drawLayer)) MEventHandler.Call(_layerToWorldDrawEvent[drawLayer]);
        }

        public static void CallScreenDrawLayerEvent(Layer drawLayer)
        {
            if (_layerToScreenDrawEvent.ContainsKey(drawLayer)) MEventHandler.Call(_layerToScreenDrawEvent[drawLayer]);
        }
    }
}