using System;
using DuckGame;

namespace MClient.Core.Utils
{
    /// <summary>
    /// A collection of functions to convert between various coordinate systems.
    /// </summary>
    public static class MPositionConversionUtil
    {
        /// <summary>
        /// Converts UV coordinates to Screen coordinates.
        /// </summary>
        /// <param name="uv">The UV coordinates to convert</param>
        /// <returns>The screen coordinates that the given UV coordinates represent</returns>
        /// <remarks>
        /// This function may not always behave as expected, as it uses DuckGame.Resolution for the screen size,
        /// which is known to behave strangely with non 16/9 aspect ratios.
        /// </remarks>
        public static Vec2 UvCoordsToScreenPos(Vec2 uv)
        {
            return uv * Resolution.size;
        }

        /// <summary>
        /// Converts Screen coordinates to UV coordinates.
        /// </summary>
        /// <param name="pos">The screen coordinates to convert</param>
        /// <returns>The UV coordinates that the given screen coordinates represent</returns>
        /// <remarks>
        /// This function may not always behave as expected, as it uses DuckGame.Resolution for the screen size,
        /// which is known to behave strangely with non 16/9 aspect ratios.
        /// </remarks>
        public static Vec2 ScreenPosToUvCoords(Vec2 pos)
        {
            return pos / Resolution.size;
        }

        /// <summary>
        /// Converts UV coordinates to Screen coordinates, with 0,0 being the centre of the screen.
        /// </summary>
        /// <param name="uv">The UV coordinates to convert</param>
        /// <returns>The screen coordinates that the given UV coordinates represent</returns>
        /// <remarks>
        /// This function may not always behave as expected, as it uses DuckGame.Resolution for the screen size,
        /// which is known to behave strangely with non 16/9 aspect ratios.
        /// </remarks>
        public static Vec2 CentredUvCoordsToScreenPos(Vec2 uv)
        {
            return uv * Resolution.size.y + 0.5f * Resolution.size;
        }

        /// <summary>
        /// Converts Screen coordinates to UV coordinates, with the centre of the screen being 0,0
        /// </summary>
        /// <param name="pos">The screen coordinates to convert</param>
        /// <returns>The UV coordinates that the given screen coordinates represent</returns>
        /// <remarks>
        /// This function may not always behave as expected, as it uses DuckGame.Resolution for the screen size,
        /// which is known to behave strangely with non 16/9 aspect ratios.
        /// </remarks>
        public static Vec2 ScreenPosToCentredUvCoords(Vec2 pos)
        {
            return (pos - 0.5f * Resolution.size) / Resolution.size.y;
        }
        
        /// <summary>
        /// Converts a Game/World space position to a screen space position.
        /// </summary>
        /// <param name="pos">The position to convert</param>
        /// <returns>The screen position equivalent to the given position</returns>
        /// <remarks>This method uses the HUD layer to convert between positions</remarks>
        public static Vec2 GameToScreenPos(Vec2 pos)
        {
            return Layer.HUD.camera.transformWorldVector(pos);
        }

        /// <summary>
        /// Converts a screen space position to a Game/World space position.
        /// </summary>
        /// <param name="pos">The position to convert</param>
        /// <returns>The Game/World position equivalent to the given position</returns>
        /// <remarks>This method uses the HUD layer to convert between positions</remarks>
        public static Vec2 ScreenToGamePos(Vec2 pos)
        {
            return Layer.HUD.camera.transformScreenVector(pos);
        }

        /// <summary>
        /// Converts a Game/World space position to a screen space position, using the given layer
        /// </summary>
        /// <param name="pos">The position to convert</param>
        /// <param name="gameLayer">The layer to use for the conversion</param>
        /// <returns>The screen position equivalent to the given position</returns>
        public static Vec2 GameToScreenPos(Vec2 pos, Layer gameLayer)
        {
            return gameLayer.camera.transformWorldVector(pos);
        }

        /// <summary>
        /// Converts a screen space position to a Game/World space position, using the given layer
        /// </summary>
        /// <param name="pos">The position to convert</param>
        /// <param name="gameLayer">The layer to use for the conversion</param>
        /// <returns>The Game/World position equivalent to the given position</returns>
        public static Vec2 ScreenToGamePos(Vec2 pos, Layer gameLayer)
        {
            return gameLayer.camera.transformScreenVector(pos);
        }

        /// <summary>
        /// Clamps the given rectangles position such that it is entirely on-screen.
        /// </summary>
        /// <param name="pos">The position of the rectangle</param>
        /// <param name="size">The size of the rectangle</param>
        /// <param name="area">The area that the origin of the rectangle is located at</param>
        /// <param name="gamePos">Whether to assume the positions given are in Game/World space or not</param>
        /// <returns>A position such that the rectangle is entirely on-screen</returns>
        public static Vec2 ClampToScreen(Vec2 pos, Vec2 size, MQuadrantArea area, bool gamePos = true)
        {
            Vec2 offsetX;
            Vec2 offsetY;
            var screen = Resolution.size;
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