using System.Reflection;
using DuckGame;
using MClient.Core;
using MClient.Core.Utils;
using MClient.RenderSystem;
using MClient.UiSystem.Internal.Components.Elements;

namespace MClient.UiSystem.Default
{
    /// <summary>
    /// Default UI Color Picker. Intended for use with the AutoUI system.
    /// </summary>
    public class MDefaultUiColorPickerElement : MUiColorPickerElement
    {
        private readonly SpriteMap _box = new SpriteMap(Mod.GetPath<MModClass>("Ui/UiElementBox"), 8, 8);
        private readonly SpriteMap _picker = new SpriteMap(Mod.GetPath<MModClass>("Ui/UiPicker"), 16, 16);
        
        private Color _trCol;
        private Color _mmCol;
        private Color _reCol;
        private Color _cCol;
        
        public MDefaultUiColorPickerElement(Vec2 pos, FieldInfo field) : base(pos, new Vec2(64), field)
        {
            
        }
        
        public override void Draw()
        {
            _box.color = BaseColor;
            
            var texSize = new Vec2(_box.width, _box.height) * UiScale;
            float xRep = Size.x / texSize.x;
            float yRep = Size.y / texSize.y;
            var texXSize = new Vec2(texSize.x, 0f);
            var texYSize = new Vec2(0f, texSize.y);
            var xOff = new Vec2(Size.x - texSize.x, 0f);
            var yOff = new Vec2(0f, Size.y - texSize.y);
            var xyOff = new Vec2(Size.x - texSize.x, texSize.y);
            var yxOff = new Vec2(texSize.x, Size.y - texSize.y);

            _box.frame = 0;
            MRenderer.DrawSprite(_box, Position, UiScale);
            _box.frame = 1;
            MRenderer.DrawSprite(_box, Position + texXSize, new Vec2((xRep - 2) * UiScale, UiScale));
            _box.frame = 2;
            MRenderer.DrawSprite(_box, Position + xOff, UiScale);
            _box.frame = 3;
            MRenderer.DrawSprite(_box, Position + texYSize, new Vec2(UiScale, (yRep - 2) * UiScale));
            _box.frame = 4;
            MRenderer.DrawSprite(_box, Position + texSize, new Vec2((xRep - 2) * UiScale, (yRep - 2) * UiScale));
            _box.frame = 5;
            MRenderer.DrawSprite(_box, Position + xyOff, new Vec2(UiScale, (yRep - 2) * UiScale));
            _box.frame = 6;
            MRenderer.DrawSprite(_box, Position + yOff, UiScale);
            _box.frame = 7;
            MRenderer.DrawSprite(_box, Position + yxOff, new Vec2((xRep - 2) * UiScale, UiScale));
            _box.frame = 8;
            MRenderer.DrawSprite(_box, Position + Size - texSize, UiScale);

            _trCol = new Color(MMathUtils.HsVtoRgb(new Vec3(Hsv.x, 1f,1f)) / 255f);
            _mmCol = _trCol / 2;
            //fix alpha
            _mmCol.a = _trCol.a;
            
            _reCol = new Color(Rgb / 255f);
            _cCol = Hsv.z > 0.5 ? Color.Black : Color.White;
            
            //Background box
            MRenderer.DrawRect(SlBoxPos - Vec2.One * UiScale, HBarPos + HBarSize + Vec2.One * UiScale, TextAccentColor);
            
            var slBoxCentre = SlBoxPos + SlBoxSize / 2f;
                
            //Saturation-Value gradient. Split into 4 tris because my gradient quad method is bad for this
            MRenderer.DrawGradTri(SlBoxPos, slBoxCentre, SlBoxPos + new Vec2(0f, SlBoxSize.y),
                Color.White, _mmCol, Color.Black);
            MRenderer.DrawGradTri(
                SlBoxPos, slBoxCentre, SlBoxPos + new Vec2(SlBoxSize.x,0f),
                Color.White, _mmCol, _trCol);
            MRenderer.DrawGradTri(
                SlBoxPos + SlBoxSize, slBoxCentre, SlBoxPos + new Vec2(0f, SlBoxSize.y),
                Color.Black, _mmCol, Color.Black);
            MRenderer.DrawGradTri(
                SlBoxPos + SlBoxSize, slBoxCentre, SlBoxPos + new Vec2(SlBoxSize.x, 0f),
                Color.Black, _mmCol, _trCol);
            
            var centre = new Vec2(HBarSize.x / 2f, 0f);
            var sixth = new Vec2(0f, HBarSize.y / 6f);
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

            _picker.color = _cCol;
            var localPickerPos = Vec2.Clamp(SlPickerPos, SlBoxPos + Vec2.One * UiScale, SlBoxPos + SlBoxSize - Vec2.One * UiScale);
            MRenderer.DrawPoint(localPickerPos, 2.5f * UiScale, _reCol);
            MRenderer.DrawSprite(_picker, localPickerPos - new Vec2(8f) * UiScale, UiScale);


        }
    }
}