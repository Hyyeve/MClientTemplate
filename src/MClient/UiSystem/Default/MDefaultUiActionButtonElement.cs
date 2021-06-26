using System.Reflection;
using DuckGame;
using MClient.Core;
using MClient.Render;
using MClient.UiSystem.Internal.Components.Elements;

namespace MClient.UiSystem.Default
{
    public class MDefaultUiActionButtonElement : MUiActionButtonElement
    {
        private readonly SpriteMap box = new SpriteMap(Mod.GetPath<MModClass>("Ui/UiElementBox"), 8, 8);
        
        /// <inheritdoc />
        public MDefaultUiActionButtonElement(Vec2 pos, MethodInfo method) : base(pos, new Vec2(16f), method)
        {
        }
        

        /// <inheritdoc />
        public override void Draw()
        {
            box.color = BaseColor;
            
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
            MRenderer.DrawSprite(box, Position + Size - texSize, UiScale);

            MRenderer.DrawText(Title, Position + new Vec2(Padding * 4f), Pressed ? TextAccentColor : TextColor, UiScale);
        }
    }
}