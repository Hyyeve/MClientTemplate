using System.Reflection;
using DuckGame;
using MClient.Core;
using MClient.RenderSystem;
using MClient.UiSystem.Internal.Components.Elements;

namespace MClient.UiSystem.Default
{
    /// <summary>
    /// Default UI Text Display Box. Intended for use with the AutoUI system.
    /// </summary>
    public class MDefaultUiTextDisplayBoxElement : MUiTextDisplayBoxElement
    {
        private readonly SpriteMap _box = new SpriteMap(Mod.GetPath<MModClass>("Ui/UiElementBox"), 8, 8);
        
        public MDefaultUiTextDisplayBoxElement(Vec2 pos, Vec2 size, FieldInfo field) : base(pos, size, field)
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

            var textPos = TextStartPos;
            
            MRenderer.DrawText(Title, TitlePos, TextColor, UiScale);

            foreach (string text in DrawList)
            {
                if (text.StartsWith(LinePrefix))
                {
                    MRenderer.DrawText(LinePrefix, textPos, TextAccentColor, UiScale * TextScale);
                    MRenderer.DrawText(text.Substring(LinePrefix.Length), textPos + new Vec2(MRenderer.GetStringWidth(LinePrefix) * UiScale * TextScale,0f), TextColor, UiScale * TextScale);
                }
                else
                {
                    MRenderer.DrawText(text, textPos, TextColor, UiScale * TextScale);
                }
                
                textPos.y += MRenderer.GetStringHeight(text) * UiScale * TextScale + Padding;
            }
        }
    }
}