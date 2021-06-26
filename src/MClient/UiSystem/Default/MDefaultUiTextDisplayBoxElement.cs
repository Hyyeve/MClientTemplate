using System.Reflection;
using DuckGame;
using MClient.Core;
using MClient.Render;
using MClient.UiSystem.Internal.Components.Elements;

namespace MClient.UiSystem.Default
{
    public class MDefaultUiTextDisplayBoxElement : MUiTextDisplayBoxElement
    {
        private readonly SpriteMap box = new SpriteMap(Mod.GetPath<MModClass>("Ui/UiElementBox"), 8, 8);
        
        /// <inheritdoc />
        public MDefaultUiTextDisplayBoxElement(Vec2 pos, Vec2 size, FieldInfo field) : base(pos, size, field)
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

            Vec2 textPos = TextStartPos;
            
            MRenderer.DrawText(Title, TitlePos, TextColor, UiScale);

            foreach (string text in DrawList)
            {
                if (text.StartsWith(LinePrefix))
                {
                    MRenderer.DrawText(LinePrefix, textPos, TextAccentColor, UiScale * TextScale);
                    MRenderer.DrawText(text.Substring(LinePrefix.Length), textPos + new Vec2(Graphics.GetStringWidth(LinePrefix) * UiScale * TextScale,0f), TextColor, UiScale * TextScale);
                }
                else
                {
                    MRenderer.DrawText(text, textPos, TextColor, UiScale * TextScale);
                }
                
                textPos.y += Graphics.GetStringHeight(text) * UiScale * TextScale + Padding;
            }
        }
    }
}