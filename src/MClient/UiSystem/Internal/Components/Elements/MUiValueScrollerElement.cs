using System;
using System.Reflection;
using DuckGame;
using MClient.Core.EventSystem.Events.Input;
using MClient.InputSystem;
using MClient.RenderSystem;
using MClient.UiSystem.Internal.Attributes;

namespace MClient.UiSystem.Internal.Components.Elements
{
    /// <summary>
    /// Base class for Ui Value Scrollers. Contains all basic functionality.
    /// </summary>
    public abstract class MUiValueScrollerElement : MUiFieldElement
    {
        protected readonly string Title;
        protected double Value;
        protected string ValueString;

        private bool _dragging;
        private Vec2 _mouseZero;
        private readonly MValueType _valueType;
        private double _oldVal;
        private Vec2 _accumulatedOffset = Vec2.Zero;


        protected MUiValueScrollerElement(Vec2 pos, Vec2 size, FieldInfo field) : base(pos, size, field)
        {
            _valueType = GetValueType(field);
            Title = GetName(field);
            size *= UiScale;
            Value = GetValue();
            size.x = MRenderer.GetStringWidth(Title + "  " + $"{Value:0.0}") * UiScale;
            SetSize(size, true);
        }



  
        protected override void Arrange() { }

        protected override void Update()
        {
            Value = GetValue();
            string newValueString = $"{Value:0.0}";
            if (ValueString is null || newValueString.Length != ValueString.Length)
            {
                SetSize(new Vec2(MRenderer.GetStringWidth(Title + "  " + $"{Value:0.0}") * UiScale, Size.y), true);
            }

            ValueString = $"{Value:0.0}";

            if (!_dragging) return;
            
            var offset = MInputHandler.MousePositionGame - _mouseZero;
            Value = _oldVal + (offset.x + offset.y) / 10;
            SetValue(Value);
            MInputHandler.MousePositionGame = _mouseZero;
            _oldVal = Value;
            Value = GetValue();

            ValueString = newValueString;
        }

        
        public override void HandleMouseEvent(MEventMouseAction e)
        {
            switch (e.Action)
            {
                case MMouseAction.LeftPressed when IsOverlapping(e.MousePosGame):
                    _mouseZero = e.MousePosGame;
                    _oldVal = Value;
                    _dragging = true;
                    MUiHandler.HideMouse();
                    break;
                case MMouseAction.LeftReleased:
                    _dragging = false;
                    _accumulatedOffset = Vec2.Zero;
                    MUiHandler.ShowMouse();
                    break;
                case MMouseAction.Scrolled when IsOverlapping (e.MousePosGame):
                    if (_dragging) break;
                    SetValue(GetValue() + Math.Sign(e.Scroll) * (_valueType == MValueType.Int ? -1f : -0.1f));
                    break;
            }
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

        private string GetName(FieldInfo field)
        {
            var att = (MUiValueScrollerAttribute) Attribute.GetCustomAttribute(field, typeof(MUiValueScrollerAttribute));
            return att != null ? att.TitleOverride == "" ? field.Name : att.TitleOverride : field.Name;
        }
        
    }
}