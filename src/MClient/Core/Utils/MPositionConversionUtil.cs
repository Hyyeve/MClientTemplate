using System;
using DuckGame;

namespace MClient.Core.Utils
{
    public class MPositionConversionUtil
    {
        public static Vec2 UvCoordsToScreenPos(Vec2 uv)
        {
            return uv * Resolution.size;
        }

        public static Vec2 ScreenPosToUvCoords(Vec2 pos)
        {
            return pos / Resolution.size;
        }

        public static Vec2 CentredUvCoordsToScreenPos(Vec2 uv)
        {
            return uv * Resolution.size.y + 0.5f * Resolution.size;
        }

        public static Vec2 ScreenPosToCentredUvCoords(Vec2 pos)
        {
            return (pos - 0.5f * Resolution.size) / Resolution.size.y;
        }

        /// <summary>
        /// UV Coords To Screen Position
        /// </summary>
        /// <param name="pos">UV coords to convert to screen position</param>
        public static Vec2 UVTSP(Vec2 uv)
        {
            return UvCoordsToScreenPos(uv);
        }

        /// <summary>
        /// Screen Position To UV Coords.
        /// </summary>
        /// <param name="pos">Screen position to convert to UV coords.</param>
        public static Vec2 SPTUV(Vec2 pos)
        {
            return ScreenPosToUvCoords(pos);
        }

        /// <summary>
        /// Centred UV Coords To Screen Position
        /// </summary>
        /// <param name="pos">Centred UV coords to convert to screen position</param>
        public static Vec2 CUPTSP(Vec2 uv)
        {
            return CentredUvCoordsToScreenPos(uv);
        }

        /// <summary>
        /// Screen Position To Centred UV Coords.
        /// </summary>
        /// <param name="pos">Screen position to convert to UV coords.</param>
        public static Vec2 SPTCUV(Vec2 pos)
        {
            return ScreenPosToCentredUvCoords(pos);
        }

        public static Vec2 GameToScreenPos(Vec2 pos)
        {
            return Layer.HUD.camera.transformWorldVector(pos);
        }

        public static Vec2 ScreenToGamePos(Vec2 pos)
        {
            return Layer.HUD.camera.transformScreenVector(pos);
        }

        public static Vec2 GameToScreenPos(Vec2 pos, Layer gameLayer)
        {
            return gameLayer.camera.transformWorldVector(pos);
        }

        public static Vec2 ScreenToGamePos(Vec2 pos, Layer gameLayer)
        {
            return gameLayer.camera.transformScreenVector(pos);
        }

        public static Vec2 ClampToScreen(Vec2 pos, Vec2 size, MQuadrantArea area, bool gamePos = true)
        {
            Vec2 offsetX;
            Vec2 offsetY;
            Vec2 screen = Resolution.size;
            if (gamePos) screen = ScreenToGamePos(screen);
            switch (area)
            {
                case MQuadrantArea.TopLeft:
                    offsetX.x = 0f;
                    offsetX.y = size.x;
                    offsetY.x = 0f;
                    offsetY.y = size.y;
                    break;
                case MQuadrantArea.TopMiddle:
                    offsetX.x = size.x / 2f;
                    offsetX.y = size.x / 2f;
                    offsetY.x = 0f;
                    offsetY.y = size.y;
                    break;
                case MQuadrantArea.TopRight:
                    offsetX.x = size.x;
                    offsetX.y = 0f;
                    offsetY.x = 0f;
                    offsetY.y = size.y;
                    break;
                case MQuadrantArea.MiddleLeft:
                    offsetX.x = 0f;
                    offsetX.y = size.x;
                    offsetY.x = size.y / 2f;
                    offsetY.y = size.y / 2f;
                    break;
                case MQuadrantArea.MiddleMiddle:
                    offsetX.x = size.x/2f;
                    offsetX.y = size.x/2f;
                    offsetY.x = size.y/2f;
                    offsetY.y = size.y/2f;
                    break;
                case MQuadrantArea.MiddleRight:
                    offsetX.x = size.x;
                    offsetX.y = 0f;
                    offsetY.x = size.y / 2f;
                    offsetY.y = size.y / 2f;
                    break;
                case MQuadrantArea.BottomLeft:
                    offsetX.x = 0f;
                    offsetX.y = size.x;
                    offsetY.x = size.y;
                    offsetY.y = 0f;
                    break;
                case MQuadrantArea.BottomMiddle:
                    offsetX.x = size.x / 2f;
                    offsetX.y = size.x / 2f;
                    offsetY.x = size.y;
                    offsetY.y = 0f;
                    break;
                case MQuadrantArea.BottomRight:
                    offsetX.x = size.x;
                    offsetX.y = 0f;
                    offsetY.x = size.y;
                    offsetY.y = 0f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(area), area, null);
            }

            pos.x = Maths.Clamp(pos.x, offsetX.x, screen.x - offsetX.y);
            pos.y = Maths.Clamp(pos.y, offsetY.x, screen.y - offsetY.y);

            return pos;
        }
    }

    public enum MQuadrantArea
    {
        TopLeft,
        TopMiddle,
        TopRight,
        MiddleLeft,
        MiddleMiddle,
        MiddleRight,
        BottomLeft,
        BottomMiddle,
        BottomRight
    }

}