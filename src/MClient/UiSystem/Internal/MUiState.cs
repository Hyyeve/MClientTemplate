using System;
using DuckGame;
using MClient.Core.EventSystem.Events.Input;
using MClient.UiSystem.Internal.Attributes;
using MClient.UiSystem.Internal.Components;

namespace MClient.UiSystem.Internal
{
    public class MUiState
    {
        private MUiContainer Panel;
        private bool active;
        public bool Active => active;

        public Color BaseCol = Color.White;
        public Color BaseAccentCol = Color.White;
        public Color TextCol = Color.Black;
        public Color TextAccentCol = new Color(70,70,70);

        public readonly string id;
        public MUiState(string id, MUiContainer panel)
        {
           Panel = panel;
           this.id = id;
           Panel.SetOwningState(this);
           Panel.UpdateCols();
        }

        public MUiState(string id, MUiContainer panel, Type uiClassRef)
        {
            Panel = panel;
            this.id = id;
            UpdateColours(uiClassRef);
            Panel.SetOwningState(this);
            Panel.UpdateCols();
        }
        
        public void DisablePanel()
        {
            active = false;
        }

        public void EnablePanel()
        {
            active = true;
        }

        public void UpdatePanel()
        {
            if (!active) return;
            Panel.HandleUiUpdate();
        }

        public void HandleKeyEvent(MEventKeyTyped e)
        {
            if (!active) return;
            Panel.HandleKeyTypedEvent(e);
        }

        public void HandleMouseEvent(MEventMouseAction e)
        {
            if (!active) return;
            Panel.HandleMouseEvent(e);
        }

        public bool IsOverlapping(Vec2 pos)
        {
            return Panel.IsOverlapping(pos);
        }

        public void UpdateColours(Type uiClassRef)
        {
            foreach (MUiColorAttribute attribute in uiClassRef.GetCustomAttributes(typeof(MUiColorAttribute), false))
            {
                switch (attribute.ColorArea)
                {
                    case UiColorArea.Base:
                        BaseCol = attribute.Color;
                        break;
                    case UiColorArea.BaseAccent:
                        BaseAccentCol = attribute.Color;
                        break;
                    case UiColorArea.Text:
                        TextCol = attribute.Color;
                        break;
                    case UiColorArea.TextAccent:
                        TextAccentCol = attribute.Color;
                        break;
                } ;
                Panel.UpdateCols();
            }
        }

        public void SetCol(UiColorArea area, Color col)
        {
            switch (area)
            {
                case UiColorArea.Base:
                    BaseCol = col;
                    break;
                case UiColorArea.BaseAccent:
                    BaseAccentCol = col;
                    break;
                case UiColorArea.Text:
                    TextCol = col;
                    break;
                case UiColorArea.TextAccent:
                    TextAccentCol = col;
                    break;
            }
            Panel.UpdateCols();
        }
    }
}