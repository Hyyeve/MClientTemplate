using System.Reflection;
using DuckGame;
using MClient.Render;
using MClient.UiSystem.Internal;
using MClient.UiSystem.Internal.Components.Elements;
using MClientCore.MClient.Core;

namespace MClient.UiSystem.Default
{
    public class MDefaultUiToggleElement : MUiToggleElement
    {

        private readonly SpriteMap box = new SpriteMap(Mod.GetPath<MModClass>("Ui/UiElementBox"), 8, 8);
        private readonly SpriteMap button = new SpriteMap(Mod.GetPath<MModClass>("Ui/UiToggleButton"), 12, 12);
        
        /// <inheritdoc />
        public MDefaultUiToggleElement(Vec2 pos, FieldInfo field) : base(pos, new Vec2(16f), field, 2f)
        {
        }
        

        /// <inheritdoc />
        public override void Draw()
        {
            box.color = BaseColor;
            button.color = BaseAccentColor;
            
            Vec2 texSize = new Vec2(box.width, box.height) * UiScale;
            float xRep = Size.x / texSize.x;
            Vec2 texXSize = new Vec2(texSize.x, 0f);
            Vec2 xOff = new Vec2(Size.x - texSize.x, 0f);
            Vec2 yOff = new Vec2(0f, Size.y - texSize.y);
            Vec2 yxOff = new Vec2(texSize.x, Size.y - texSize.y);
            
            box.frame = 0;
            MRenderer.DrawSprite(box, Position, UiScale);
            box.frame = 1;
            MRenderer.DrawSprite(box, Position + texXSize, new Vec2((xRep - 2f) * UiScale, UiScale));
            box.frame = 2;
            MRenderer.DrawSprite(box, Position + xOff, UiScale);
            box.frame = 6;
            MRenderer.DrawSprite(box, Position + yOff, UiScale);
            box.frame = 7;
            MRenderer.DrawSprite(box, Position + yxOff, new Vec2((xRep - 2f) * UiScale, UiScale));
            box.frame = 8;
            MRenderer.DrawSprite(box, Position + Size - texSize,UiScale);

            button.frame = Toggled ? 0 : 1;
            MRenderer.DrawSprite(button, ToggleBoxPosition, UiScale);
            
            MRenderer.DrawText(Title, ToggleBoxPosition + new Vec2(ToggleBoxSize.x + Padding, Padding), TextColor, UiScale);
        }
    }
}