using System;
using System.Reflection;
using DuckGame;
using MClient.Render;
using MClient.UiSystem.Internal.Components.Elements;
using MClientCore.MClient.Core;

namespace MClient.UiSystem.Default
{
    public class MDefaultUiSliderElement : MUiSliderElement
    {

        private readonly SpriteMap box = new SpriteMap(Mod.GetPath<MModClass>("Ui/UiElementBox"), 8, 8);
        private readonly SpriteMap bar = new SpriteMap(Mod.GetPath<MModClass>("Ui/UiSliderBar"), 16, 16);
        
        /// <inheritdoc />
        public MDefaultUiSliderElement(Vec2 pos, FieldInfo field) : base(pos, new Vec2(16f), field)
        {

        }

        /// <inheritdoc />
        public override void Draw()
        {
            box.color = BaseColor;
            bar.color = BaseAccentColor;
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

            float xBarLength = (float) ((((Position.x + Size.x) - Position.x) - (4 * UiScale)) * Percent);
            xBarLength = Math.Max(1f * UiScale, xBarLength);
            float xBarRep = xBarLength / bar.width;

            bar.frame = 0;
            MRenderer.DrawSprite(bar, Position + Vec2.Unitx * UiScale, new Vec2(xBarRep, 1f * UiScale));
            bar.frame = 1;
            MRenderer.DrawSprite(bar, Position + new Vec2(xBarLength, 0f), UiScale);
            
            MRenderer.DrawText(Title, Position + new Vec2(2f,4f) * UiScale, TextColor, UiScale);
            MRenderer.DrawText(ValueString, Position + new Vec2(2f + Graphics.GetStringWidth(Title + " "), 4f) * UiScale , TextAccentColor, UiScale);
        }
    }
}