using System;
using System.Reflection;
using DuckGame;
using MClient.Core.EventSystem.Events.Input;
using MClient.InputSystem;
using MClient.UiSystem.Internal.Attributes;

namespace MClient.UiSystem.Internal.Components.Elements
{
    public abstract class MUiSliderElement : MUiFieldElement
    {
        protected readonly string Title;
        protected readonly double Max;
        protected readonly double Min;
        protected double Value;
        protected double Percent;
        protected string ValueString;

        private bool dragging;
        private readonly ValueType valueType;

        /// <inheritdoc />
        protected MUiSliderElement(Vec2 pos, Vec2 size, FieldInfo field) : base(pos, size, field)
        {
            Max = GetMax(field);
            Min = GetMin(field);
            valueType = GetValueType(field);
            Title = GetName(field);
            size = size * UiScale;
            size.x = Graphics.GetStringWidth(Title + "  " + $"{Max:0.0}") * UiScale;
            SetSize(size, true);
            Value = GetValue();
        }

        /// <inheritdoc />
        protected override void Arrange() { }

        protected override void Update()
        {
            Value = GetValue();
            ValueString = $"{Value:0.0}";
            Percent = (Value - Min) / (Max - Min);
            
            if (!dragging) return;
            
            float MouseX = MInputHandler.MousePositionGame.x;
            float MinX = Position.x + 1f;
            float MaxX = Position.x + Size.x - 1f;
            
            if (MouseX < MinX)
            {
                Percent = 0f;
                SetValue(Min);
                return;
            }
            if (MouseX > MaxX)
            {
                Percent = 1f;
                SetValue(Max);
                return;
            }

            float MousePercent = (MouseX - MinX) / (MaxX - MinX);
            double value = MousePercent * (Max - Min) + Min;
            SetValue(value);
            Value = GetValue();
            ValueString = $"{Value:0.0}";
            Percent = MousePercent;
        }
        
        /// <inheritdoc />
        public override void HandleMouseEvent(MEventMouseAction e)
        {
            dragging = e.Action switch
            {
                MMouseAction.LeftPressed when IsOverlapping(e.MousePosGame) => true,
                MMouseAction.LeftReleased => false,
                _ => dragging
            };
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
            MUiSliderAttribute att = (MUiSliderAttribute) Attribute.GetCustomAttribute(field,typeof(MUiSliderAttribute));
            return att != null ? att.TitleOverride == "" ? field.Name : att.TitleOverride : field.Name;
        }

        private double GetMin(FieldInfo field)
        {
            MUiSliderAttribute att = (MUiSliderAttribute) Attribute.GetCustomAttribute(field,typeof(MUiSliderAttribute));
            return att is null ? 0f : att.Min;
        }

        private double GetMax(FieldInfo field)
        {
            MUiSliderAttribute att = (MUiSliderAttribute) Attribute.GetCustomAttribute(field,typeof(MUiSliderAttribute));
            return att is null ? 1f : att.Max;
        }
    }

    internal enum ValueType
    {
        Int,Float,Double, Invalid
    }
}