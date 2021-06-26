using System;
using System.Collections.Generic;
using DuckGame;
using MClient.Core;
using MClient.Core.EventSystem;
using MClient.Core.EventSystem.Events.Drawing;
using MClient.Core.EventSystem.Events.Helper;
using MClient.Core.Utils;
using MClient.Render.RenderTasks;
using MClient.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = DuckGame.Color;
using Matrix = DuckGame.Matrix;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MClient.Render
{
    public static class MRenderer
    {

        private static MPrimitiveBatch _batcher;
        private static bool _initialised;
        private static readonly List<MRenderTask> Tasks = new List<MRenderTask>();
        private static bool _doMssa;
        private static readonly GraphicsDevice LocDevice = Graphics.device;

        public static bool DoMsaa => _doMssa;

        [MEventInit]
        public static void InitEvent()
        {
            Init();
        }
        
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

        [MEventGameDraw]
        public static void Draw(MEventGameDraw e)
        {
            UpdateState(e.Layer);

            MRenderEventHelper.CallWorldDrawLayerEvent(e.Layer);

            RunTasks();
            ResetState();

            MRenderEventHelper.CallScreenDrawLayerEvent(e.Layer);

        }

        public static void Clear()
        {
            MEventHandler.DeRegister(typeof(MRenderer));
            _initialised = false;
            _batcher = null;
            Tasks.Clear();
        }

        /// <summary>
        /// MUST BE CALLED FROM MAIN THREAD - ONLY USE INSIDE EVENT OR PATCH METHODS
        /// </summary>
        /// <param name="msaa">Whether to enable or disable MultiSamplingAntiAliasing</param>
        /// <param name="msaaLevel">The amount of multi sampling to apply</param>
        public static void SetAntiAliasing(bool msaa, int msaaLevel)
        {
            Graphics._manager.PreferMultiSampling = msaa;
            Graphics.device.PresentationParameters.MultiSampleCount = msaaLevel;
            Graphics._manager.ApplyChanges();
            _doMssa = msaa;
        }


        public static void AddVertex(Vec2 vec, Color col, bool allowAntiAlias = true)
        {
            _batcher.AddVertex(vec, col);
        }

        public static void Begin(PrimitiveType type, BlendState blendState)
        {
            _batcher.Begin(type, blendState);
        }

        public static void End()
        {
            _batcher.End();
        }

        private static void UpdateState(Layer layer)
        {
            
            _batcher.UpdateState(layer);
        }

        private static void ResetState()
        {
            _batcher.ResetState();
        }

        private static void RunTasks()
        {
            foreach (MRenderTask task in Tasks)
            {
                task.Run();
            }
            Tasks.Clear();
        }

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
            Vec2 p1P2 = end - start;
            Vec2 perp = MMathUtils.CalcPerpendicularCw(p1P2).normalized * thickness;
            Vec2 a = start + perp;
            Vec2 b = start - perp;
            Vec2 c = end + perp;
            Vec2 d = end - perp;
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
