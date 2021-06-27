using System.Collections.Generic;
using DuckGame;
using MClient.Core;
using MClient.InputSystem;
using MClient.RenderSystem;
using MClient.UiSystem.Internal.Components;

namespace MClient.UiSystem.Default
{
    /// <summary>
    /// Default UI Container. Intended for use with the AutoUI system.
    /// </summary>
    public class MDefaultUiContainer : MUiContainer
    {

        private readonly SpriteMap _texture = new SpriteMap(Mod.GetPath<MModClass>("Ui/UiContainerBackground"), 8, 8);
        
        public MDefaultUiContainer(Vec2 position, Vec2 size) : base(position, size)
        {
        }
        
        public override void Draw()
        {
            _texture.color = BaseColor;
            
            var texSize = new Vec2(_texture.width, _texture.height) * UiScale;
            float xRep = Size.x / texSize.x;
            float yRep = Size.y / texSize.y;
            var texXSize = new Vec2(texSize.x, 0f);
            var texYSize = new Vec2(0f, texSize.y);
            var xOff = new Vec2(Size.x - texSize.x, 0f);
            var yOff = new Vec2(0f, Size.y - texSize.y);
            var xyOff = new Vec2(Size.x - texSize.x, texSize.y);
            var yxOff = new Vec2(texSize.x, Size.y - texSize.y);
            
                _texture.frame = 0;
                MRenderer.DrawSprite(_texture, Position, UiScale);
                _texture.frame++;
                MRenderer.DrawSprite(_texture, Position + texXSize, new Vec2((xRep - 2) * UiScale, UiScale));

                _texture.frame += IsOverlappingCloseButton(MInputHandler.MousePositionGame) ? 2 : 1;
                MRenderer.DrawSprite(_texture, Position + xOff, UiScale);

                _texture.frame = 4;
                MRenderer.DrawSprite(_texture, Position + texYSize, new Vec2(UiScale, (yRep - 2) * UiScale));
                _texture.frame++;
                MRenderer.DrawSprite(_texture, Position + texSize, new Vec2((xRep - 2) * UiScale, (yRep - 2) * UiScale));
                _texture.frame++;
                MRenderer.DrawSprite(_texture, Position + xyOff, new Vec2(UiScale, (yRep - 2) * UiScale));
                _texture.frame = 8;
                MRenderer.DrawSprite(_texture, Position + yOff, UiScale);
                _texture.frame++;
                MRenderer.DrawSprite(_texture, Position + yxOff, new Vec2((xRep - 2) * UiScale, UiScale));
                _texture.frame++;
                MRenderer.DrawSprite(_texture, Position + Size - texSize, UiScale);
        }


        /// <inheritdoc />
        public override bool IsOverlappingCloseButton(Vec2 pos)
        {
            var closeButtonPos = Position + new Vec2(Size.x - 8 * UiScale, 0f);
            var closeButtonSize = new Vec2(8, 8) * UiScale;
                
            return pos.x > closeButtonPos.x && pos.x < closeButtonPos.x + closeButtonSize.x &&
                   pos.y > closeButtonPos.y && pos.y < closeButtonPos.y + closeButtonSize.y;
        }

    }
}