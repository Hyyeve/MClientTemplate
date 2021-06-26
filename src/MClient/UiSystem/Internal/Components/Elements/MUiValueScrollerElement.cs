using System;
using System.Reflection;
using DuckGame;
using MClient.Core.EventSystem.Events.Input;
using MClient.InputSystem;
using MClient.UiSystem.Internal.Attributes;

namespace MClient.UiSystem.Internal.Components.Elements
{
    public abstract class MUiValueScrollerElement : MUiFieldElement
    {
        protected readonly string Title;
        protected double Value;
        protected string ValueString;

        private bool dragging;
        private Vec2 mouseZero;
        private readonly ValueType valueType;
        private double oldVal;
        private Vec2 accumulatedOffset = Vec2.Zero;

        /// <inheritdoc />
        protected MUiValueScrollerElement(Vec2 pos, Vec2 size, FieldInfo field) : base(pos, size, field)
        {
            valueType = GetValueType(field);
            Title = GetName(field);
            size = size * UiScale;
            Value = GetValue();
            size.x = Graphics.GetStringWidth(Title + "  " + $"{Value:0.0}") * UiScale;
            SetSize(size, true);
        }



        /// <inheritdoc />
        protected override void Arrange() { }

        protected override void Update()
        {
            Value = GetValue();
            string newValueString = $"{Value:0.0}";
            if (ValueString is null || newValueString.Length != ValueString.Length)
            {
                SetSize(new Vec2(Graphics.GetStringWidth(Title + "  " + $"{Value:0.0}") * UiScale, Size.y), true);
            }

            ValueString = $"{Value:0.0}";

            if (!dragging) return;
            
            Vec2 offset = MInputHandler.MousePositionGame - mouseZero;
            Value = oldVal + (offset.x + offset.y) / 10;
            SetValue(Value);
            MInputHandler.MousePositionGame = mouseZero;
            oldVal = Value;
            Value = GetValue();

            ValueString = newValueString;
        }
        
        /// <inheritdoc />
        public override void HandleMouseEvent(MEventMouseAction e)
        {
            switch (e.Action)
            {
                case MMouseAction.LeftPressed when IsOverlapping(e.MousePosGame):
                    mouseZero = e.MousePosGame;
                    oldVal = Value;
                    dragging = true;
                    MUiHandler.HideMouse();
                    break;
                case MMouseAction.LeftReleased:
                    dragging = false;
                    accumulatedOffset = Vec2.Zero;
                    MUiHandler.ShowMouse();
                    break;
                case MMouseAction.Scrolled when IsOverlapping (e.MousePosGame):
                    if (dragging) break;
                    SetValue(GetValue() + Math.Sign(e.Scroll) * (valueType == ValueType.Int ? -1f : -0.1f));
                    break;
            }
        }

        /// <inheritdoc />
        public override void HandleKeyTypedEvent(MEventKeyTyped e) { }
        

        /// <inheritdoc />
        protected override void VerifyFieldInfo(FieldInfo fieldInfo)
        {
            if (GetValueType(fieldInfo) == ValueType.Invalid)
                throw new Exception("UiSliderElement field was not a valid number type! "
                                    + fieldInfo.DeclaringType.Name + "." + fieldInfo.Name);
            if (!fieldInfo.IsStatic)
                throw new Exception("UiSliderElement field isn't static!" + fieldInfo.DeclaringType.Name +
                                    "." + fieldInfo.Name);
        }

        private double GetValue()
        {
            return valueType switch
            {
                ValueType.Int => (int) AttatchedField.GetValue(null),
                ValueType.Float => (float) AttatchedField.GetValue(null),
                ValueType.Double => (double) AttatchedField.GetValue(null),
                ValueType.Invalid => double.NaN,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        private void SetValue(double value)
        {
            switch (valueType)
            {
                case ValueType.Int:
                    AttatchedField.SetValue(null, (int) Math.Round(value));
                    break;
                case ValueType.Float:
                    AttatchedField.SetValue(null, (float) value);
                    break;
                case ValueType.Double:
                    AttatchedField.SetValue(null, (double) value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private ValueType GetValueType(FieldInfo field)
        {
            if (field.FieldType == typeof(int))
            {
                return ValueType.Int;
            }

            if (field.FieldType == typeof(float))
            {
                return ValueType.Float;
            }

            if (field.FieldType == typeof(double))
            {
                return ValueType.Double;
            }

            return ValueType.Invalid;
        }

        private string GetName(FieldInfo field)
        {
            MUiValueScrollerAttribute att = (MUiValueScrollerAttribute) Attribute.GetCustomAttribute(field, typeof(MUiValueScrollerAttribute));
            return att != null ? att.TitleOverride == "" ? field.Name : att.TitleOverride : field.Name;
        }
        
    }
}