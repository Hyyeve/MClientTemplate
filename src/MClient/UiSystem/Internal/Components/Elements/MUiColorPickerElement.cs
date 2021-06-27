using System;
using System.Reflection;
using DuckGame;
using MClient.Core.EventSystem.Events.Input;
using MClient.Core.Utils;
using MClient.InputSystem;
using MClient.UiSystem.Internal.Attributes;

namespace MClient.UiSystem.Internal.Components.Elements
{
    /// <summary>
    /// Base class for Ui Color Pickers. Contains all basic functionality.
    /// </summary>
    public abstract class MUiColorPickerElement : MUiFieldElement
    {
        protected Vec2 SlPickerPos = Vec2.Zero;
        protected float HSliderPos = -1;

        protected readonly float Padding;

        protected Vec2 TitlePos;
        protected Vec2 TitleSize;
        protected Vec2 SlBoxPos;
        protected Vec2 SlBoxSize;
        protected Vec2 HBarPos;
        protected Vec2 HBarSize;
        protected Vec3 Hsv;
        protected Vec3 Rgb;

        protected Color TestCol;
        
        protected readonly string Title;
        
        private bool _draggingSlPicker;
        private bool _draggingHBar;
        protected MUiColorPickerElement(Vec2 pos, Vec2 size, FieldInfo field) : base(pos, size, field)
        {
            Padding = 1f * UiScale;
            Title = GetName(field);
            size *= UiScale;
            TitleSize.x = Graphics.GetStringWidth(Title) * UiScale;
            TitleSize.y = Graphics.GetStringHeight(Title) * UiScale;
            size.x = Math.Max(TitleSize.x + Padding * 2f, size.y * 1.35f - Padding * 8f);
            SetSize(size, true);
        }
        
        protected MUiColorPickerElement(Vec2 pos, Vec2 size, FieldInfo field, float padding) : base(pos, size, field)
        {
            Padding = padding * UiScale;
            Title = GetName(field);
            TitleSize.x = Graphics.GetStringWidth(Title) * UiScale;
            TitleSize.y = Graphics.GetStringHeight(Title) * UiScale;
            size.x = Math.Max(TitleSize.x + Padding * 2f, size.y * 1.35f - Padding * 8f);
            SetSize(size, true);
        }
        

        protected override void Update()
        {
            var rgb = GetVal();
            if (!MMathUtils.Compare3(rgb, MMathUtils.Floor3(Rgb)))
            {
                Rgb = rgb;
                Hsv = MMathUtils.RgbToHsv(rgb);
                UpdatePickerPositions();
                return;
            }

            if (!_draggingSlPicker && !_draggingHBar) return;
            
            var mousePosition = MInputHandler.MousePositionGame;
            mousePosition.x = Maths.Clamp(mousePosition.x, SlBoxPos.x, SlBoxPos.x + SlBoxSize.x);
            mousePosition.y = Maths.Clamp(mousePosition.y, SlBoxPos.y, SlBoxPos.y + SlBoxSize.y);

            if (_draggingSlPicker)
            {
                SlPickerPos = mousePosition;
                //prevent offset being too small
                var offset = MMathUtils.Max2(mousePosition - SlBoxPos, new Vec2(0.001f));
                var percent = offset / SlBoxSize;
                    
                Hsv.y = percent.x;
                //invert v
                Hsv.z = 1f - percent.y;
            }

            if (_draggingHBar)
            {
                HSliderPos = mousePosition.y;
                //prevent offset being too small
                float offset = MMathUtils.Max(mousePosition.y - HBarPos.y, 0.001f);
                float percent = offset / HBarSize.y;
                Hsv.x = percent;
            }

            Rgb = MMathUtils.HsVtoRgb(Hsv);
                
            SetVal(Rgb);
        }

        protected override void Arrange()
        {
            if (!NeedsArranging) return;
            TitlePos = Position + new Vec2(Padding * 4f);
            SlBoxSize = new Vec2(Size.y - TitleSize.y - Padding * 10f);
            SlBoxPos = TitlePos + new Vec2(0f, TitleSize.y + Padding);
            HBarPos = SlBoxPos + new Vec2(SlBoxSize.x + Padding * 2f, 0f);
            HBarSize = new Vec2(Size.y * 0.35f - Padding * 2f, Size.y -  TitleSize.y - Padding * 10f);
            UpdatePickerPositions();
            NeedsArranging = false;
        }

        private void UpdatePickerPositions()
        {
            HSliderPos = HBarPos.y + HBarSize.y * Maths.Clamp( Hsv.x, 0.001f, 0.999f);
            SlPickerPos = SlBoxPos + SlBoxSize * new Vec2(Maths.Clamp(Hsv.y, 0.001f, 0.999f), 1f - Maths.Clamp(Hsv.z, 0.001f, 0.999f));
        }

        private Vec3 GetVal()
        {
            return ((Color)AttatchedField.GetValue(null)).ToVector3() * 255;
        }

        private void SetVal(Vec3 val)
        {
            AttatchedField.SetValue(null, new Color((int)val.x, (int)val.y, (int)val.z));
        }

        private static string GetName(FieldInfo field)
        {
            var att = Attribute.GetCustomAttribute(field, typeof(MUiColorPickerAttribute));
            return att != null
                ? ((MUiColorPickerAttribute) att).TitleOverride == "" ? field.Name : ((MUiColorPickerAttribute) att).TitleOverride
                : field.Name;
        }

        protected bool IsOverlappingPickerBox(Vec2 pos)
        {
            return pos.x > SlBoxPos.x && pos.x < SlBoxPos.x + SlBoxSize.x && pos.y > SlBoxPos.y &&
                   pos.y < SlBoxPos.y + SlBoxSize.y;
        }

        protected bool IsOverlappingHueBar(Vec2 pos)
        {
            return pos.x > HBarPos.x && pos.x < HBarPos.x + HBarSize.x && pos.y > HBarPos.y &&
                   pos.y < HBarPos.y + HBarSize.y;
        }
        
        public override void HandleMouseEvent(MEventMouseAction e)
        {
            switch (e.Action)
            {
                case MMouseAction.LeftPressed:
                    if (IsOverlappingPickerBox(e.MousePosGame))
                    {
                        _draggingSlPicker = true;
                        break;
                    }
                    if (IsOverlappingHueBar(e.MousePosGame))
                    {
                        _draggingHBar = true;
                    }
                    break;
                case MMouseAction.LeftReleased:
                    _draggingHBar = false;
                    _draggingSlPicker = false;
                    break;
            }
        }
        
        public override void HandleKeyTypedEvent(MEventKeyTyped e) { }

        /// <inheritdoc />
        protected override void VerifyFieldInfo(FieldInfo fieldInfo)
        {
            if (fieldInfo.FieldType != typeof(Color))
                throw new Exception("UiColorPickerElement field was not a DuckGame.Color! " 
                                    + fieldInfo.DeclaringType?.Name + "." + fieldInfo.Name);
            if (!fieldInfo.IsStatic)
                throw new Exception("UiColorPickerElement field isn't static!" + fieldInfo.DeclaringType?.Name +
                                    "." + fieldInfo.Name);
        }
    }
}