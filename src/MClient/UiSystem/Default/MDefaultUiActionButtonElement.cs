using System.Reflection;
using DuckGame;
using MClient.Core;
using MClient.RenderSystem;
using MClient.UiSystem.Internal.Components.Elements;

namespace MClient.UiSystem.Default
{
    /// <summary>
    /// Default UI Action Button. Intended for use with the AutoUI system.
    /// </summary>
    public class MDefaultUiActionButtonElement : MUiActionButtonElement
    {
        private readonly SpriteMap _box = new SpriteMap(Mod.GetPath<MModClass>("Ui/UiElementBox"), 8, 8);
        
        public MDefaultUiActionButtonElement(Vec2 pos, MethodInfo method) : base(pos, new Vec2(16f), method)
        {
        }
        
        public override void Draw()
        {
            _box.color = BaseColor;
            
            var texSize = new Vec2(_box.width, _box.height) * UiScale;
            float xRep = Size.x / texSize.x;
            var texXSize = new Vec2(texSize.x, 0f);
            var xOff = new Vec2(Size.x - texSize.x, 0f);
            var yOff = new Vec2(0f, Size.y - texSize.y);
            var yxOff = new Vec2(texSize.x, Size.y - texSize.y);

            _box.frame = 0;
            MRenderer.DrawSprite(_box, Position, UiScale);
            _box.frame = 1;
            MRenderer.DrawSprite(_box, Position + texXSize, new Vec2((xRep - 2f) * UiScale, UiScale));
            _box.frame = 2;
            MRenderer.DrawSprite(_box, Position + xOff, UiScale);
            _box.frame = 6;
            MRenderer.DrawSprite(_box, Position + yOff, UiScale);
            _box.frame = 7;
            MRenderer.DrawSprite(_box, Position + yxOff, new Vec2((xRep - 2f) * UiScale, UiScale));
            _box.frame = 8;
            MRenderer.DrawSprite(_box, Position + Size - texSize, UiScale);

            MRenderer.DrawText(Title, Position + new Vec2(Padding * 4f), Pressed ? TextAccentColor : TextColor, UiScale);
        }
    }
}