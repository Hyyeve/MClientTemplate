using System;
using System.Reflection;
using DuckGame;
using MClient.Core.Utils;
using MClient.UiSystem.Internal.Attributes;

namespace MClient.UiSystem.Internal.Components.Elements
{
    /// <summary>
    /// Base class for all Ui Elements that bind to methods. Contains some basic functionality.
    /// </summary>
    public abstract class MUiMethodElement : MAmUi
    {
        private MUiState _state;

        protected Vec2 Position;
        protected Vec2 Size;

        protected Color BaseColor;
        protected Color BaseAccentColor;
        protected Color TextColor;
        protected Color TextAccentColor;

        protected float UiScale => MUiHandler.GlobalUiScale;
        protected bool NeedsArranging;
        
        protected readonly MethodInfo AttatchedMethod;

        protected MUiMethodElement(Vec2 pos, Vec2 size, MethodInfo method)
        {
            VerifyMethodInfo(method);
            AttatchedMethod = method;
            Position = pos * UiScale;
            Size = size * UiScale;
            NeedsArranging = true;
            UpdateCols();
        }

        private Color GetCol(MUiColorArea area)
        {
            MUiColorAttribute[] attributes = (MUiColorAttribute[])AttatchedMethod.GetCustomAttributes(typeof(MUiColorAttribute), false);
            foreach (var att in attributes)
            {
                if (att.ColorArea == area)
                {
                    return att.Color;
                }
            }
            
            if(_state is null) return Color.White;

            return area switch
            {
                MUiColorArea.Base => _state.BaseCol,
                MUiColorArea.BaseAccent => _state.BaseAccentCol,
                MUiColorArea.Text => _state.TextCol,
                MUiColorArea.TextAccent => _state.TextAccentCol,
                _ => throw new ArgumentOutOfRangeException(nameof(area), area, null)
            };
        }
        
        public override void HandleUiUpdate()
        {
            Arrange();
            Update();
            Draw();
        }

        public override void UpdateCols()
        {
            BaseColor = GetCol(MUiColorArea.Base);
            BaseAccentColor = GetCol(MUiColorArea.BaseAccent);
            TextColor = GetCol(MUiColorArea.Text);
            TextAccentColor = GetCol(MUiColorArea.TextAccent);
        }

        protected abstract void VerifyMethodInfo(MethodInfo methodInfo);
        protected abstract void Arrange();
        protected abstract void Update();

        public override Vec2 GetPos()
        {
            return Position;
        }


        public override Vec2 GetSize()
        {
            return Size;
        }

        public override void SetPos(Vec2 pos)
        {
            Position = MPositionConversionUtil.ClampToScreen(pos, GetSize(), MQuadrantArea.TopLeft);
            NeedsArranging = true;
            base.SetPos(pos);
        }


        public override void SetSize(Vec2 size, bool scaled)
        {
            Size = size * (scaled ? 1f : UiScale);
            NeedsArranging = true;
            base.SetSize(size, scaled);
        }

     
        public override bool IsOverlapping(Vec2 pos)
        {
            return pos.x > Position.x && pos.x < Position.x + Size.x && pos.y > Position.y &&
                   pos.y < Position.y + Size.y;
        }


        public override void SetOwningState(MUiState state)
        {
            _state = state;
        }


        public override MUiState GetOwningState()
        {
            return _state;
        }
    }
}