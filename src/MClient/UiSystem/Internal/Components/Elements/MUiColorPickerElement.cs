using System;
using System.Reflection;
using DuckGame;
using MClient.Core.EventSystem.Events.Input;
using MClient.InputSystem;
using MClient.UiSystem.Internal.Attributes;
using MClient.Utils;

namespace MClient.UiSystem.Internal.Components.Elements
{
    public abstract class MUiColorPickerElement : MUiFieldElement
    {
        protected Vec2 SLPickerPos = Vec2.Zero;
        protected float HSliderPos = -1;

        protected float Padding;

        protected Vec2 TitlePos;
        protected Vec2 TitleSize;
        protected Vec2 SLBoxPos;
        protected Vec2 SLBoxSize;
        protected Vec2 HBarPos;
        protected Vec2 HBarSize;
        protected Vec3 HSV;
        protected Vec3 RGB;

        protected Color TestCol;
        
        protected readonly string Title;
        
        private bool draggingSLPicker;
        private bool draggingHBar;
        

        /// <inheritdoc />
        public MUiColorPickerElement(Vec2 pos, Vec2 size, FieldInfo field) : base(pos, size, field)
        {
            Padding = 1f * UiScale;
            Title = GetName(field);
            size = size * UiScale;
            TitleSize.x = Graphics.GetStringWidth(Title) * UiScale;
            TitleSize.y = Graphics.GetStringHeight(Title) * UiScale;
            size.x = Math.Max(TitleSize.x + Padding * 2f, size.y * 1.35f - Padding * 8f);
            SetSize(size, true);
        }

        /// <inheritdoc />
        public MUiColorPickerElement(Vec2 pos, Vec2 size, FieldInfo field, float padding) : base(pos, size, field)
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
            Vec3 rgb = GetVal();
            if (!MMathUtils.Compare3(rgb, MMathUtils.Floor3(RGB)))
            {
                RGB = rgb;
                HSV = MMathUtils.RGBToHSV(rgb);
                UpdatePickerPositions();
                return;
            }

            if (draggingSLPicker || draggingHBar)
            {
                Vec2 mousePosition = MInputHandler.MousePositionGame;
                mousePosition.x = Maths.Clamp(mousePosition.x, SLBoxPos.x, SLBoxPos.x + SLBoxSize.x);
                mousePosition.y = Maths.Clamp(mousePosition.y, SLBoxPos.y, SLBoxPos.y + SLBoxSize.y);

                if (draggingSLPicker)
                {
                    SLPickerPos = mousePosition;
                    //prevent offset being too small
                    Vec2 offset = MMathUtils.Max2(mousePosition - SLBoxPos, new Vec2(0.001f));
                    Vec2 percent = offset / SLBoxSize;
                    
                    HSV.y = percent.x;
                    //invert v
                    HSV.z = 1f - percent.y;
                }

                if (draggingHBar)
                {
                    HSliderPos = mousePosition.y;
                    //prevent offset being too small
                    float offset = MMathUtils.Max(mousePosition.y - HBarPos.y, 0.001f);
                    float percent = offset / HBarSize.y;
                    HSV.x = percent;
                }

                RGB = MMathUtils.HSVtoRGB(HSV);
                
                SetVal(RGB);
            }
        }

        protected override void Arrange()
        {
            if (!NeedsArranging) return;
            TitlePos = Position + new Vec2(Padding * 4f);
            SLBoxSize = new Vec2(Size.y - TitleSize.y - Padding * 10f);
            SLBoxPos = TitlePos + new Vec2(0f, TitleSize.y + Padding);
            HBarPos = SLBoxPos + new Vec2(SLBoxSize.x + Padding * 2f, 0f);
            HBarSize = new Vec2(Size.y * 0.35f - Padding * 2f, Size.y -  TitleSize.y - Padding * 10f);
            UpdatePickerPositions();
            NeedsArranging = false;
        }

        private void UpdatePickerPositions()
        {
            HSliderPos = HBarPos.y + HBarSize.y * Maths.Clamp( HSV.x, 0.001f, 0.999f);
            SLPickerPos = SLBoxPos + SLBoxSize * new Vec2(Maths.Clamp(HSV.y, 0.001f, 0.999f), 1f - Maths.Clamp(HSV.z, 0.001f, 0.999f));
        }

        private Vec3 GetVal()
        {
            return ((Color)AttatchedField.GetValue(null)).ToVector3() * 255;
        }

        void SetVal(Vec3 val)
        {
            AttatchedField.SetValue(null, new Color((int)val.x, (int)val.y, (int)val.z));
        }

        private string GetName(FieldInfo field)
        {
            Attribute att = Attribute.GetCustomAttribute(field, typeof(MUiColorPickerAttribute));
            return att != null
                ? ((MUiColorPickerAttribute) att).TitleOverride == "" ? field.Name : ((MUiColorPickerAttribute) att).TitleOverride
                : field.Name;
        }

        protected bool IsOverlappingPickerBox(Vec2 pos)
        {
            return pos.x > SLBoxPos.x && pos.x < SLBoxPos.x + SLBoxSize.x && pos.y > SLBoxPos.y &&
                   pos.y < SLBoxPos.y + SLBoxSize.y;
        }

        protected bool IsOverlappingHueBar(Vec2 pos)
        {
            return pos.x > HBarPos.x && pos.x < HBarPos.x + HBarSize.x && pos.y > HBarPos.y &&
                   pos.y < HBarPos.y + HBarSize.y;
        }
        
        /// <inheritdoc />
        public override void HandleMouseEvent(MEventMouseAction e)
        {
            switch (e.Action)
            {
                case MMouseAction.LeftPressed:
                    if (IsOverlappingPickerBox(e.MousePosGame))
                    {
                        draggingSLPicker = true;
                        break;
                    }
                    if (IsOverlappingHueBar(e.MousePosGame))
                    {
                        draggingHBar = true;
                    }
                    break;
                case MMouseAction.LeftReleased:
                    draggingHBar = false;
                    draggingSLPicker = false;
                    break;
            }
        }

        /// <inheritdoc />
        public override void HandleKeyTypedEvent(MEventKeyTyped e) { }

        /// <inheritdoc />
        protected override void VerifyFieldInfo(FieldInfo fieldInfo)
        {
            if (fieldInfo.FieldType != typeof(Color))
                throw new Exception("UiColorPickerElement field was not a DuckGame.Color! " 
                                    + fieldInfo.DeclaringType.Name + "." + fieldInfo.Name);
            if (!fieldInfo.IsStatic)
                throw new Exception("UiColorPickerElement field isn't static!" + fieldInfo.DeclaringType.Name +
                                    "." + fieldInfo.Name);
        }
    }
}