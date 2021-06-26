using System.Collections.Generic;
using DuckGame;
using MClient.Core;
using MClient.InputSystem;
using MClient.RenderSystem;
using MClient.UiSystem.Internal.Components;

namespace MClient.UiSystem.Default
{
    public class MDefaultUiContainer : MUiContainer
    {

        private readonly SpriteMap texture = new SpriteMap(Mod.GetPath<MModClass>("Ui/UiContainerBackground"), 8, 8);

        /// <inheritdoc />
        public MDefaultUiContainer(Vec2 position, Vec2 size) : base(position, size)
        {
        }

        /// <inheritdoc />
        public MDefaultUiContainer(Vec2 position, Vec2 size, Vec2 elementOffset) : base(position, size, elementOffset)
        {
        }

        /// <inheritdoc />
        public MDefaultUiContainer(Vec2 position, Vec2 size, Vec2 elementOffset, List<MAmUi> elements) : base(position,
            size, elementOffset, elements)
        {
        }

        /// <inheritdoc />
        public override void Draw()
        {
            texture.color = BaseColor;
            
            Vec2 texSize = new Vec2(texture.width, texture.height) * UiScale;
            float xRep = Size.x / texSize.x;
            float yRep = Size.y / texSize.y;
            Vec2 texXSize = new Vec2(texSize.x, 0f);
            Vec2 texYSize = new Vec2(0f, texSize.y);
            Vec2 xOff = new Vec2(Size.x - texSize.x, 0f);
            Vec2 yOff = new Vec2(0f, Size.y - texSize.y);
            Vec2 xyOff = new Vec2(Size.x - texSize.x, texSize.y);
            Vec2 yxOff = new Vec2(texSize.x, Size.y - texSize.y);
            
                texture.frame = 0;
                MRenderer.DrawSprite(texture, Position, UiScale);
                texture.frame++;
                MRenderer.DrawSprite(texture, Position + texXSize, new Vec2((xRep - 2) * UiScale, UiScale));

                texture.frame += IsOverlappingCloseButton(MInputHandler.MousePositionGame) ? 2 : 1;
                MRenderer.DrawSprite(texture, Position + xOff, UiScale);

                texture.frame = 4;
                MRenderer.DrawSprite(texture, Position + texYSize, new Vec2(UiScale, (yRep - 2) * UiScale));
                texture.frame++;
                MRenderer.DrawSprite(texture, Position + texSize, new Vec2((xRep - 2) * UiScale, (yRep - 2) * UiScale));
                texture.frame++;
                MRenderer.DrawSprite(texture, Position + xyOff, new Vec2(UiScale, (yRep - 2) * UiScale));
                texture.frame = 8;
                MRenderer.DrawSprite(texture, Position + yOff, UiScale);
                texture.frame++;
                MRenderer.DrawSprite(texture, Position + yxOff, new Vec2((xRep - 2) * UiScale, UiScale));
                texture.frame++;
                MRenderer.DrawSprite(texture, Position + Size - texSize, UiScale);
        }


        /// <inheritdoc />
        public override bool IsOverlappingCloseButton(Vec2 pos)
        {
            Vec2 closeButtonPos = Position + new Vec2(Size.x - 8 * UiScale, 0f);
            Vec2 closeButtonSize = new Vec2(8, 8) * UiScale;
                
            return pos.x > closeButtonPos.x && pos.x < closeButtonPos.x + closeButtonSize.x &&
                   pos.y > closeButtonPos.y && pos.y < closeButtonPos.y + closeButtonSize.y;
        }

    }
}