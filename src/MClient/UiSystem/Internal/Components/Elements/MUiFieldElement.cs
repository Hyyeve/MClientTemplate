using System;
using System.Reflection;
using DuckGame;
using MClient.Core.Utils;
using MClient.Render;
using MClient.UiSystem.Internal.Attributes;

namespace MClient.UiSystem.Internal.Components.Elements
{
    public abstract class MUiFieldElement : MAmUi
    {
        private MUiState State;

        protected Vec2 Position;
        protected Vec2 Size;

        protected Color BaseColor;
        protected Color BaseAccentColor;
        protected Color TextColor;
        protected Color TextAccentColor;

        protected float UiScale => MUiHandler.GlobalUiScale;
        protected bool NeedsArranging;
        
        protected FieldInfo AttatchedField;

        protected MUiFieldElement(Vec2 pos, Vec2 size, FieldInfo field)
        {
            VerifyFieldInfo(field);
            AttatchedField = field;
            Position = pos * UiScale;
            Size = size * UiScale;
            NeedsArranging = true;
            UpdateCols();
        }

        private Color GetCol(UiColorArea area)
        {
            MUiColorAttribute[] attribtes = (MUiColorAttribute[])AttatchedField.GetCustomAttributes(typeof(MUiColorAttribute), false);
            foreach (MUiColorAttribute att in attribtes)
            {
                if (att.ColorArea == area)
                {
                    return att.Color;
                }
            }

            if (State is null) return Color.White;

            return area switch
            {
                UiColorArea.Base => State.BaseCol,
                UiColorArea.BaseAccent => State.BaseAccentCol,
                UiColorArea.Text => State.TextCol,
                UiColorArea.TextAccent => State.TextAccentCol,
                _ => throw new ArgumentOutOfRangeException(nameof(area), area, null)
            };
        }

        public override void UpdateCols()
        {
            BaseColor = GetCol(UiColorArea.Base);
            BaseAccentColor = GetCol(UiColorArea.BaseAccent);
            TextColor = GetCol(UiColorArea.Text);
            TextAccentColor = GetCol(UiColorArea.TextAccent);
        }
        
        public override void HandleUiUpdate()
        {
            Arrange();
            Update();
            Draw();
        }

        protected abstract void VerifyFieldInfo(FieldInfo fieldInfo);
        protected abstract void Arrange();
        protected abstract void Update();

        /// <inheritdoc />
        public override Vec2 GetPos()
        {
            return Position;
        }

        /// <inheritdoc />
        public override Vec2 GetSize()
        {
            return Size;
        }

        /// <inheritdoc />
        public override void SetPos(Vec2 pos)
        {
            Position = MPositionConversionUtil.ClampToScreen(pos, GetSize(), MQuadrantArea.TopLeft);
            NeedsArranging = true;
            base.SetPos(pos);
        }

        /// <inheritdoc />
        public override void SetSize(Vec2 size, bool scaled)
        {
            Size = size * (scaled ? 1f : UiScale);
            NeedsArranging = true;
            base.SetSize(size, scaled);
        }

        /// <inheritdoc />
        public override bool IsOverlapping(Vec2 pos)
        {
            return pos.x > Position.x && pos.x < Position.x + Size.x && pos.y > Position.y &&
                   pos.y < Position.y + Size.y;
        }

        /// <inheritdoc />
        public override void SetOwningState(MUiState state)
        {
            State = state;
        }

        /// <inheritdoc />
        public override MUiState GetOwningState()
        {
            return State;
        }
        
    }
}