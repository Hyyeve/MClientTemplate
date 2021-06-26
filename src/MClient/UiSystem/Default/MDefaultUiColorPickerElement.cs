using System;
using System.Reflection;
using DuckGame;
using MClient.Core.Utils;
using MClient.InputSystem;
using MClient.Render;
using MClient.UiSystem.Internal.Components.Elements;
using MClient.Utils;
using MClientCore.MClient.Core;

namespace MClient.UiSystem.Default
{
    public class MDefaultUiColorPickerElement : MUiColorPickerElement
    {
        private readonly SpriteMap box = new SpriteMap(Mod.GetPath<MModClass>("Ui/UiElementBox"), 8, 8);
        private readonly SpriteMap picker = new SpriteMap(Mod.GetPath<MModClass>("Ui/UiPicker"), 16, 16);
        
        private Color TRCol;
        private Color MMCol;
        private Color RECol;
        private Color CCol;

        /// <inheritdoc />
        public MDefaultUiColorPickerElement(Vec2 pos, FieldInfo field) : base(pos, new Vec2(64), field)
        {
            
        }

        /// <inheritdoc />
        public override void Draw()
        {
            box.color = BaseColor;
            
            Vec2 texSize = new Vec2(box.width, box.height) * UiScale;
            float xRep = Size.x / texSize.x;
            float yRep = Size.y / texSize.y;
            Vec2 texXSize = new Vec2(texSize.x, 0f);
            Vec2 texYSize = new Vec2(0f, texSize.y);
            Vec2 xOff = new Vec2(Size.x - texSize.x, 0f);
            Vec2 yOff = new Vec2(0f, Size.y - texSize.y);
            Vec2 xyOff = new Vec2(Size.x - texSize.x, texSize.y);
            Vec2 yxOff = new Vec2(texSize.x, Size.y - texSize.y);

            box.frame = 0;
            MRenderer.DrawSprite(box, Position, UiScale);
            box.frame = 1;
            MRenderer.DrawSprite(box, Position + texXSize, new Vec2((xRep - 2) * UiScale, UiScale));
            box.frame = 2;
            MRenderer.DrawSprite(box, Position + xOff, UiScale);
            box.frame = 3;
            MRenderer.DrawSprite(box, Position + texYSize, new Vec2(UiScale, (yRep - 2) * UiScale));
            box.frame = 4;
            MRenderer.DrawSprite(box, Position + texSize, new Vec2((xRep - 2) * UiScale, (yRep - 2) * UiScale));
            box.frame = 5;
            MRenderer.DrawSprite(box, Position + xyOff, new Vec2(UiScale, (yRep - 2) * UiScale));
            box.frame = 6;
            MRenderer.DrawSprite(box, Position + yOff, UiScale);
            box.frame = 7;
            MRenderer.DrawSprite(box, Position + yxOff, new Vec2((xRep - 2) * UiScale, UiScale));
            box.frame = 8;
            MRenderer.DrawSprite(box, Position + Size - texSize, UiScale);

            TRCol = new Color(MMathUtils.HsVtoRgb(new Vec3(HSV.x, 1f,1f)) / 255f);
            MMCol = TRCol / 2;
            //fix alpha
            MMCol.a = TRCol.a;
            
            RECol = new Color(RGB / 255f);
            CCol = HSV.z > 0.5 ? Color.Black : Color.White;
            
            //Background box
            MRenderer.DrawRect(SLBoxPos - Vec2.One * UiScale, HBarPos + HBarSize + Vec2.One * UiScale, TextAccentColor);
            
            Vec2 SLBoxCentre = SLBoxPos + SLBoxSize / 2f;
                
            //Saturation-Value gradient. Split into 4 tris because my gradient quad method is bad for this
            MRenderer.DrawGradTri(SLBoxPos, SLBoxCentre, SLBoxPos + new Vec2(0f, SLBoxSize.y),
                Color.White, MMCol, Color.Black);
            MRenderer.DrawGradTri(
                SLBoxPos, SLBoxCentre, SLBoxPos + new Vec2(SLBoxSize.x,0f),
                Color.White, MMCol, TRCol);
            MRenderer.DrawGradTri(
                SLBoxPos + SLBoxSize, SLBoxCentre, SLBoxPos + new Vec2(0f, SLBoxSize.y),
                Color.Black, MMCol, Color.Black);
            MRenderer.DrawGradTri(
                SLBoxPos + SLBoxSize, SLBoxCentre, SLBoxPos + new Vec2(SLBoxSize.x, 0f),
                Color.Black, MMCol, TRCol);
            
            Vec2 centre = new Vec2(HBarSize.x / 2f, 0f);
            Vec2 sixth = new Vec2(0f, HBarSize.y / 6f);
            //Hue gradient. Line calls are easier than doing quads (they get converted to sets of triangles internally the same as quads)
            MRenderer.DrawGradLine(HBarPos + centre, HBarPos + sixth + centre, centre.x, Color.Red, Color.Yellow);
            MRenderer.DrawGradLine(HBarPos + sixth + centre, HBarPos + sixth * 2f + centre, centre.x, Color.Yellow, Color.Lime);
            MRenderer.DrawGradLine(HBarPos + sixth * 2f + centre, HBarPos + sixth * 3f + centre, centre.x, Color.Lime, Color.Cyan);
            MRenderer.DrawGradLine(HBarPos + sixth * 3f + centre, HBarPos + sixth * 4f + centre, centre.x, Color.Cyan, Color.Blue);
            MRenderer.DrawGradLine(HBarPos + sixth * 4f + centre, HBarPos + sixth * 5f + centre, centre.x, Color.Blue, Color.Magenta);
            MRenderer.DrawGradLine(HBarPos + sixth * 5f + centre, HBarPos + sixth * 6f + centre, centre.x, Color.Magenta, Color.Red);

            MRenderer.DrawText(Title, TitlePos, TextColor, UiScale);

            float localSliderPos = Maths.Clamp(HSliderPos, HBarPos.y + UiScale, HBarPos.y + HBarSize.y - UiScale);
            MRenderer.DrawLine(new Vec2(HBarPos.x, localSliderPos), new Vec2(HBarPos.x + HBarSize.x, localSliderPos), 1f * UiScale, TextColor);

            picker.color = CCol;
            Vec2 localPickerPos = Vec2.Clamp(SLPickerPos, SLBoxPos + Vec2.One * UiScale, SLBoxPos + SLBoxSize - Vec2.One * UiScale);
            MRenderer.DrawPoint(localPickerPos, 2.5f * UiScale, RECol);
            MRenderer.DrawSprite(picker, localPickerPos - new Vec2(8f) * UiScale, UiScale);


        }
    }
}