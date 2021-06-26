using System;
using System.Collections.Generic;
using DuckGame;
using MClient.Core;
using MClient.Core.EventSystem;
using MClient.Core.EventSystem.Events.Drawing;
using MClient.Core.EventSystem.Events.Helper;
using MClient.Core.Utils;
using MClient.RenderSystem.RenderTasks;
using Microsoft.Xna.Framework.Graphics;
using Color = DuckGame.Color;

namespace MClient.RenderSystem
{
    
    /// <summary>
    /// This is the main custom rendering class, which contains many functions for drawing polygons, sprites, and text.
    /// </summary>
    public static class MRenderer
    {

        private static MPrimitiveBatch _batcher;
        private static bool _initialised;
        private static readonly List<MRenderTask> Tasks = new List<MRenderTask>();
        private static readonly GraphicsDevice LocDevice = Graphics.device;

        [MEventInit]
        public static void InitEvent()
        {
            Init();
        }
        
        /// <summary>
        /// Initialises the renderer with a given vertex buffer size
        /// </summary>
        /// <param name="bufferSize">The buffer size</param>
        /// <exception cref="Exception">The renderer has already been initialised</exception>
        public static void Init(int bufferSize = 500)
        {
            if (_initialised == false)
            {
                MEventHandler.Register(typeof(MRenderer));
                _batcher = new MPrimitiveBatch(LocDevice, bufferSize);
                _initialised = true;
            }
            else
            {
                throw new Exception("Already initialised!");
            }
        }

        /// <summary>
        /// Internal draw call that updates rendering state and calls render events
        /// </summary>
        [MEventGameDraw]
        public static void Draw(MEventGameDraw e)
        {
            UpdateState(e.Layer);

            MRenderEventHelper.CallWorldDrawLayerEvent(e.Layer);
            
            if(e.Layer == Layer.HUD) RunTasks();
            ResetState();

            MRenderEventHelper.CallScreenDrawLayerEvent(e.Layer);
        }

        /// <summary>
        /// Clears and un-initialises the renderer.
        /// </summary>
        public static void Clear()
        {
            MEventHandler.DeRegister(typeof(MRenderer));
            _initialised = false;
            _batcher = null;
            Tasks.Clear();
        }

        /// <summary>
        /// Allows you to enable or disable MultiSampleAntiAliasing
        /// </summary>
        /// <param name="msaa">Whether to enable or disable MultiSamplingAntiAliasing</param>
        /// <param name="msaaLevel">The amount of multi sampling to apply</param>
        /// <remarks>
        /// This must be called from the main thread, otherwise the graphics device will throw an exception.
        /// Only call inside built-in event or patch methods.
        /// </remarks>
        public static void SetAntiAliasing(bool msaa, int msaaLevel)
        {
            Graphics._manager.PreferMultiSampling = msaa;
            Graphics.device.PresentationParameters.MultiSampleCount = msaaLevel;
            Graphics._manager.ApplyChanges();
        }

        /// <summary>
        /// Adds a vertex to the buffer
        /// </summary>
        /// <param name="vec">The vertex position</param>
        /// <param name="col">The vertex color</param>
        public static void AddVertex(Vec2 vec, Color col)
        {
            _batcher.AddVertex(vec, col);
        }

        /// <summary>
        /// Opens the render state for drawing. Must be called before doing any draw calls.
        /// </summary>
        /// <param name="type">The type of primitives to be drawn</param>
        /// <param name="blendState">The blend state for the primitives</param>
        /// <remarks>Normally you shouldn't need to worry about calling this yourself, as it's automatically handled in most cases.</remarks>
        public static void Begin(PrimitiveType type, BlendState blendState)
        {
            _batcher.Begin(type, blendState);
        }

        /// <summary>
        /// Closes the render state and flushes all polygons to the screen to be displayed.
        /// </summary>
        public static void End()
        {
            _batcher.End();
        }

        /// <summary>
        /// Updates the transform and camera matrices based on the given layer
        /// </summary>
        /// <param name="layer"></param>
        public static void UpdateState(Layer layer)
        {
            _batcher.UpdateState(layer);
        }

        /// <summary>
        /// Resets the transform and camera matrices
        /// </summary>
        public static void ResetState()
        {
            _batcher.ResetState();
        }
        
        private static void RunTasks()
        {
            foreach (var task in Tasks)
            {
                task.Run();
            }
            Tasks.Clear();
        }

        /// <summary>
        /// Adds a render task to the queue. 
        /// </summary>
        /// <param name="task">The task to add</param>
        ///<remarks>
        /// The render task system is designed for visual debugging,
        /// it allows you to send render calls at times when the renderer is not active,
        /// and those calls will then be drawn the next time when the renderer is active.
        /// Note that all tasks assume the positions are in Game/World space and are drawn after the HUD layer.
        /// </remarks>
        public static void AddRenderTask(MRenderTask task)
        {
            Tasks.Add(task);
        }

        #region PrimitiveDrawing

        public static void DrawLine(Vec2 start, Vec2 end, float thickness, Color col) => DrawLine(start, end, thickness, col, BlendState.AlphaBlend);
        public static void DrawLine(Vec2 start, Vec2 end, float thickness, Color col, BlendState blendState) => DrawGradLine(start, end, thickness, col, col, blendState);
        
        public static void DrawGradLine(Vec2 start, Vec2 end, float thickness, Color colStart, Color colEnd) => DrawGradLine(start, end, thickness, colStart, colEnd, BlendState.AlphaBlend);
        public static void DrawGradLine(Vec2 start, Vec2 end, float thickness, Color colStart, Color colEnd, BlendState blendState)
        {
            var p1P2 = end - start;
            var perpendicular = MMathUtils.CalcPerpendicularCw(p1P2).normalized * thickness;
            var a = start + perpendicular;
            var b = start - perpendicular;
            var c = end + perpendicular;
            var d = end - perpendicular;
            DrawGradTri(a, b, c, colStart, colStart, colEnd, blendState);
            DrawGradTri(b, d, c, colStart, colEnd, colEnd, blendState);
        }
        
        public static void DrawTri(Vec2 pos1, Vec2 pos2, Vec2 pos3, Color col, BlendState blendState) => DrawGradTri(pos1, pos2, pos3, col, col, col, blendState);
        public static void DrawTri(Vec2 pos1, Vec2 pos2, Vec2 pos3, Color col) => DrawGradTri(pos1, pos2, pos3, col, col, col);
       
        public static void DrawGradTri(Vec2 pos1, Vec2 pos2, Vec2 pos3, Color col1, Color col2, Color col3) => DrawGradTri(pos1, pos2, pos3, col1, col2,col3, BlendState.AlphaBlend);
        public static void DrawGradTri(Vec2 pos1, Vec2 pos2, Vec2 pos3, Color col1, Color col2, Color col3, BlendState blendState)
        {
            Begin(PrimitiveType.TriangleList, blendState);
            AddVertex(pos1, col1);
            AddVertex(pos2, col2);
            AddVertex(pos3, col3);
            End();
        }
       
        public static void DrawQuad(Vec2 upLeft, Vec2 downLeft, Vec2 upRight, Vec2 downRight, Color col) => DrawQuad(upLeft, downLeft, upRight, downRight, col, BlendState.AlphaBlend);
        public static void DrawQuad(Vec2 upLeft, Vec2 downLeft, Vec2 upRight, Vec2 downRight, Color col, BlendState blendState) => DrawGradQuad(upLeft, downLeft, upRight, downRight, col, col, col, col, blendState);
       
        public static void DrawRect(Vec2 upLeft, Vec2 downRight, Color col) => DrawRect(upLeft, downRight, col, BlendState.AlphaBlend);
        public static void DrawRect(Vec2 upLeft, Vec2 downRight, Color col, BlendState blendState) => DrawGradQuad(upLeft, new Vec2(upLeft.x, downRight.y), new Vec2(downRight.x, upLeft.y), downRight, col, col, col, col, blendState);

        public static void DrawGradQuad(Vec2 upLeft, Vec2 downLeft, Vec2 upRight, Vec2 downRight, Color colUpLeft, Color colDownLeft, Color colUpRight, Color colDownRight) => DrawGradQuad(upLeft, downLeft, upRight, downRight, colUpLeft, colDownLeft, colUpRight, colDownRight, BlendState.AlphaBlend);
        public static void DrawGradQuad(Vec2 upLeft, Vec2 downLeft, Vec2 upRight, Vec2 downRight, Color colUpLeft, Color colDownLeft, Color colUpRight, Color colDownRight, BlendState blendState)
        {
            Begin(PrimitiveType.TriangleStrip, blendState);
            AddVertex(upLeft, colUpLeft);
            AddVertex(downLeft, colDownLeft);
            AddVertex(upRight, colUpRight);
            AddVertex(downRight, colDownRight);
            End();
        }
        
        public static void DrawArray(MVec2Col[] vertices, PrimitiveType type) => DrawArray(vertices, type, BlendState.AlphaBlend);
        public static void DrawArray(MVec2Col[] vertices, PrimitiveType type, BlendState blendState)
        {
            int index = 0;

            if (vertices == null)
            {
                MLogger.Log("Draw Array was passed a null list, skipping!", MLogger.MLogType.Warning, MLogger.MLogSection.Rndr);
                return;
            }
            
            Begin(type, blendState);
            while (index < vertices.Length)
            {
                if (vertices[index] == null)
                {
                    MLogger.Log("Draw Array encountered a null vertex, skipping!", MLogger.MLogType.Warning,
                        MLogger.MLogSection.Rndr);
                }
                else
                {
                    AddVertex(vertices[index].Vec, vertices[index].Col);    
                }
                index++;
            }
            End();
        }

        public static void DrawArray(Vec2[] vertices, Color color, PrimitiveType type) => DrawArray(vertices, color, type, BlendState.AlphaBlend);
        public static void DrawArray(Vec2[] vertices, Color color, PrimitiveType type, BlendState blendState)
        {
            int index = 0;

            if (vertices == null)
            {
                MLogger.Log("Draw Array was passed a null list, skipping!", MLogger.MLogType.Warning,
                    MLogger.MLogSection.Rndr);
                return;
            }
            
            Begin(type, blendState);
            while (index < vertices.Length)
            {
                AddVertex(vertices[index], color);
                index++;
            }
            End();
        }

        public static void DrawPoint(Vec2 pos, float size, Color col) => DrawPoint(pos, size, col, BlendState.AlphaBlend);
        public static void DrawPoint(Vec2 pos, float size, Color col, BlendState blendState)
        {
            DrawQuad(pos + new Vec2(-size, size),
                pos + new Vec2(-size, -size),
                pos + new Vec2(size, size),
                pos + new Vec2(size, -size),
                col, blendState
            ); 
        }

        public static void DrawText(string text, Vec2 pos, Color col, float scale = 1f, bool gamePos = true, BitmapFont font = null, Layer gameLayer = null)
        {
            if(!gamePos) pos = gameLayer?.camera.transformScreenVector(pos) ?? Layer.HUD.camera.transformScreenVector(pos);
            if (font != null)
            {
                font.Draw(text, pos, col, 0f);
            }
            else
            {
                Graphics.DrawString(text, pos, col, scale: scale);
            }
        }
        public static void DrawOutlineText(string text, Vec2 pos, Color col, Color outline, float scale = 1f, bool gamePos = true, BitmapFont font = null, Layer gameLayer = null)
        {
            if (!gamePos) pos = gameLayer?.camera.transformScreenVector(pos) ?? Layer.HUD.camera.transformScreenVector(pos);
            if (font != null)
            {
                font.Draw(text, pos, col);
            }
            else
            {
                Graphics.DrawStringOutline(text, pos, col, outline, scale: scale);
            }
        }
        
        public static void DrawSprite(SpriteMap sprite, Vec2 pos, Vec2 scale, Layer drawLayer, bool adjustPosition = false)
        {
            sprite.scale = scale;
            sprite.position = adjustPosition ? drawLayer.camera.transformScreenVector(pos) : pos;
            sprite.Draw();
        }
        public static void DrawSprite(SpriteMap sprite, Vec2 pos, float scale = 1f, bool adjustPosition = false)
        {
            DrawSprite(sprite,pos,new Vec2(scale), Layer.HUD, adjustPosition);
        }
        public static void DrawSprite(SpriteMap sprite, Vec2 pos, Layer drawLayer, float scale = 1f, bool adjustPosition = false)
        {
            DrawSprite(sprite, pos, new Vec2(scale),drawLayer, adjustPosition);
        }
        public static void DrawSprite(SpriteMap sprite, Vec2 pos, Vec2 scale, bool adjustPosition = false)
        {
            DrawSprite(sprite, pos, scale, Layer.HUD, adjustPosition);
        }

        #endregion

    }


}
