using System;
using DuckGame;
using MClient.Core.EventSystem.Events.Input;
using MClient.UiSystem.Internal.Attributes;
using MClient.UiSystem.Internal.Components;

namespace MClient.UiSystem.Internal
{
    /// <summary>
    /// This class stores state information about each Ui panel/container
    /// </summary>
    public class MUiState
    {
        private readonly MUiContainer _panel;
        private bool _active;
        public bool Active => _active;

        public Color BaseCol = Color.White;
        public Color BaseAccentCol = Color.White;
        public Color TextCol = Color.Black;
        public Color TextAccentCol = new Color(70,70,70);

        public readonly string Id;
        public MUiState(string id, MUiContainer panel)
        {
           _panel = panel;
           Id = id;
           _panel.SetOwningState(this);
           _panel.UpdateCols();
        }

        public MUiState(string id, MUiContainer panel, Type uiClassRef)
        {
            _panel = panel;
            Id = id;
            UpdateColours(uiClassRef);
            _panel.SetOwningState(this);
            _panel.UpdateCols();
        }
        
        /// <summary>
        /// Disables this panel
        /// </summary>
        public void DisablePanel()
        {
            _active = false;
        }

        /// <summary>
        /// Enables this panel
        /// </summary>
        public void EnablePanel()
        {
            _active = true;
        }

        /// <summary>
        /// Internal call. Updates & Draws this panel
        /// </summary>
        public void UpdatePanel()
        {
            if (!_active) return;
            _panel.HandleUiUpdate();
        }

        /// <summary>
        /// Internal call. Updates this panel
        /// </summary>
        public void HandleKeyEvent(MEventKeyTyped e)
        {
            if (!_active) return;
            _panel.HandleKeyTypedEvent(e);
        }

        /// <summary>
        /// Internal call. Updates this panel
        /// </summary>
        public void HandleMouseEvent(MEventMouseAction e)
        {
            if (!_active) return;
            _panel.HandleMouseEvent(e);
        }

        /// <summary>
        /// Checks whether this panel is overlapping a position
        /// </summary>
        public bool IsOverlapping(Vec2 pos)
        {
            return _panel.IsOverlapping(pos);
        }

        /// <summary>
        /// Updates the colours for this panel. Mainly an internal call but can be used if needed.
        /// </summary>
        public void UpdateColours(Type uiClassRef)
        {
            foreach (MUiColorAttribute attribute in uiClassRef.GetCustomAttributes(typeof(MUiColorAttribute), false))
            {
                switch (attribute.ColorArea)
                {
                    case MUiColorArea.Base:
                        BaseCol = attribute.Color;
                        break;
                    case MUiColorArea.BaseAccent:
                        BaseAccentCol = attribute.Color;
                        break;
                    case MUiColorArea.Text:
                        TextCol = attribute.Color;
                        break;
                    case MUiColorArea.TextAccent:
                        TextAccentCol = attribute.Color;
                        break;
                } ;
                _panel.UpdateCols();
            }
        }

        /// <summary>
        /// Sets a default colour for this panel
        /// </summary>
        public void SetCol(MUiColorArea area, Color col)
        {
            switch (area)
            {
                case MUiColorArea.Base:
                    BaseCol = col;
                    break;
                case MUiColorArea.BaseAccent:
                    BaseAccentCol = col;
                    break;
                case MUiColorArea.Text:
                    TextCol = col;
                    break;
                case MUiColorArea.TextAccent:
                    TextAccentCol = col;
                    break;
            }
            _panel.UpdateCols();
        }
    }
}