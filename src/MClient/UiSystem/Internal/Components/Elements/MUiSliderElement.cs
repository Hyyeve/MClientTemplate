using System;
using System.Reflection;
using DuckGame;
using MClient.Core.EventSystem.Events.Input;
using MClient.InputSystem;
using MClient.UiSystem.Internal.Attributes;

namespace MClient.UiSystem.Internal.Components.Elements
{
    /// <summary>
    /// Base class for Ui Slider Elements. Contains all basic functionality.
    /// </summary>
    public abstract class MUiSliderElement : MUiFieldElement
    {
        protected readonly string Title;
        protected readonly double Max;
        protected readonly double Min;
        protected double Value;
        protected double Percent;
        protected string ValueString;

        private bool _dragging;
        private readonly MValueType _valueType;

 
        protected MUiSliderElement(Vec2 pos, Vec2 size, FieldInfo field) : base(pos, size, field)
        {
            Max = GetMax(field);
            Min = GetMin(field);
            _valueType = GetValueType(field);
            Title = GetName(field);
            size = size * UiScale;
            size.x = Graphics.GetStringWidth(Title + "  " + $"{Max:0.0}") * UiScale;
            SetSize(size, true);
            Value = GetValue();
        }

    
        protected override void Arrange() { }

        protected override void Update()
        {
            Value = GetValue();
            ValueString = $"{Value:0.0}";
            Percent = (Value - Min) / (Max - Min);
            
            if (!_dragging) return;
            
            float mouseX = MInputHandler.MousePositionGame.x;
            float minX = Position.x + 1f;
            float maxX = Position.x + Size.x - 1f;
            
            if (mouseX < minX)
            {
                Percent = 0f;
                SetValue(Min);
                return;
            }
            if (mouseX > maxX)
            {
                Percent = 1f;
                SetValue(Max);
                return;
            }

            float MousePercent = (mouseX - minX) / (maxX - minX);
            double value = MousePercent * (Max - Min) + Min;
            SetValue(value);
            Value = GetValue();
            ValueString = $"{Value:0.0}";
            Percent = MousePercent;
        }
        
  
        public override void HandleMouseEvent(MEventMouseAction e)
        {
            _dragging = e.Action switch
            {
                MMouseAction.LeftPressed when IsOverlapping(e.MousePosGame) => true,
                MMouseAction.LeftReleased => false,
                _ => _dragging
            };
        }


        public override void HandleKeyTypedEvent(MEventKeyTyped e) { }

      
        protected override void VerifyFieldInfo(FieldInfo fieldInfo)
        {
            if (GetValueType(fieldInfo) == MValueType.Invalid)
                throw new Exception("UiSliderElement field was not a valid number type! "
                                    + fieldInfo.DeclaringType?.Name + "." + fieldInfo.Name);
            if (!fieldInfo.IsStatic)
                throw new Exception("UiSliderElement field isn't static!" + fieldInfo.DeclaringType?.Name +
                                    "." + fieldInfo.Name);
        }

        private double GetValue()
        {
            return _valueType switch
            {
                MValueType.Int => (int) AttatchedField.GetValue(null),
                MValueType.Float => (float) AttatchedField.GetValue(null),
                MValueType.Double => (double) AttatchedField.GetValue(null),
                MValueType.Invalid => double.NaN,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        private void SetValue(double value)
        {
            switch (_valueType)
            {
                case MValueType.Int:
                    AttatchedField.SetValue(null, (int) Math.Round(value));
                    break;
                case MValueType.Float:
                    AttatchedField.SetValue(null, (float) value);
                    break;
                case MValueType.Double:
                    AttatchedField.SetValue(null, (double) value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private MValueType GetValueType(FieldInfo field)
        {
            if (field.FieldType == typeof(int))
            {
                return MValueType.Int;
            }

            if (field.FieldType == typeof(float))
            {
                return MValueType.Float;
            }

            if (field.FieldType == typeof(double))
            {
                return MValueType.Double;
            }

            return MValueType.Invalid;
        }

        private static string GetName(FieldInfo field)
        {
            var att = (MUiSliderAttribute) Attribute.GetCustomAttribute(field,typeof(MUiSliderAttribute));
            return att != null ? att.TitleOverride == "" ? field.Name : att.TitleOverride : field.Name;
        }

        private static double GetMin(FieldInfo field)
        {
            var att = (MUiSliderAttribute) Attribute.GetCustomAttribute(field,typeof(MUiSliderAttribute));
            return att?.Min ?? 0f;
        }

        private static double GetMax(FieldInfo field)
        {
            var att = (MUiSliderAttribute) Attribute.GetCustomAttribute(field,typeof(MUiSliderAttribute));
            return att?.Max ?? 1f;
        }
    }

    internal enum MValueType
    {
        Int,Float,Double, Invalid
    }
}