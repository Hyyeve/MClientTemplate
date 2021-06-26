using System;
using System.Reflection;
using DuckGame;
using MClient.Core.EventSystem.Events.Input;
using MClient.UiSystem.Internal.Attributes;

namespace MClient.UiSystem.Internal.Components.Elements
{
    /// <summary>
    /// Base class for Ui Toggles. Contains all basic functionality.
    /// </summary>
    public abstract class MUiToggleElement : MUiFieldElement
    {
        
        protected Vec2 ToggleBoxSize;
        protected Vec2 ToggleBoxPosition;
        protected readonly float Padding;
        protected readonly string Title;
        protected bool Toggled;
        
        /// <inheritdoc />
        protected MUiToggleElement(Vec2 pos, Vec2 size, FieldInfo field) : base(pos, size, field)
        {
            Padding = 1f * UiScale;
            Title = GetName(field);
            size *= UiScale;
            size.x = Graphics.GetStringWidth(Title) * UiScale + size.y + Padding;
            SetSize(size, true);
            Toggled = (bool) AttatchedField.GetValue(null);
        }

        /// <inheritdoc />
        protected MUiToggleElement(Vec2 pos, Vec2 size, FieldInfo field, float toggleBoxPadding) : base(pos, size, field)
        {
            Padding = toggleBoxPadding * UiScale;
            Title = GetName(field);
            size *= UiScale;
            size.x = Graphics.GetStringWidth(Title) * UiScale + size.y + Padding;
            SetSize(size, true);
            Toggled = (bool) AttatchedField.GetValue(null);
        }

        private static string GetName(FieldInfo field)
        {
            var att = Attribute.GetCustomAttribute(field,typeof(MUiToggleAttribute));
            return att != null ? ((MUiToggleAttribute) att).TitleOverride == "" ? field.Name : ((MUiToggleAttribute) att).TitleOverride : field.Name;
        }

        /// <inheritdoc />
        protected override void VerifyFieldInfo(FieldInfo fieldInfo)
        {
            if (fieldInfo.FieldType != typeof(bool)) throw new Exception("UiToggleElement field was not a boolean! " 
                                                                         + fieldInfo.DeclaringType?.Name + "." + fieldInfo.Name);
            if (!fieldInfo.IsStatic)
                throw new Exception("UiToggleElement field isn't static!" + fieldInfo.DeclaringType?.Name +
                                    "." + fieldInfo.Name);
        }

        protected override void Arrange()
        {
            if (!NeedsArranging) return;
            ToggleBoxSize = new Vec2(Size.y - Padding * 2f);
            ToggleBoxPosition = Position + new Vec2(Padding);
            NeedsArranging = false;
        }

        /// <inheritdoc />
        public override void HandleMouseEvent(MEventMouseAction e)
        {
            if (e.Action != MMouseAction.LeftPressed) return;
            if (!IsOverlapping(e.MousePosGame)) return;
            FlipToggle();
        }

        protected override void Update()
        {
            Toggled = (bool) AttatchedField.GetValue(null);
        }

        public void FlipToggle()
        {
            bool toggled = (bool) AttatchedField.GetValue(null);
            AttatchedField.SetValue(null, !toggled);
            Toggled = !toggled;
        }

        /// <inheritdoc />
        public override void HandleKeyTypedEvent(MEventKeyTyped e) {}

    }
}