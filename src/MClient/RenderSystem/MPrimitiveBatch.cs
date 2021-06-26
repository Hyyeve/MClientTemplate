using System;
using DuckGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = DuckGame.Color;
using Matrix = DuckGame.Matrix;

//-------------------------------------------DISCLAIMER-------------------------------------------------------------------------
//This class was taken from a freely distributed helper library and modified to work for this mod. Most of this code is not mine.

namespace MClient.Render
{
    public sealed class MPrimitiveBatch : IDisposable
    {
        private VertexPositionColor[] vertices = new VertexPositionColor[1000];
        public int bufferSize;
        private int positionInBuffer;
        private BasicEffect basicEffect;
        private GraphicsDevice device;
        private PrimitiveType primitiveType;
        private int numVertsPerPrimitive;
        private bool hasBegun;
        private bool isDisposed;
        private Layer currentLayer;

        private static readonly RasterizerState RasterState = new RasterizerState()
        {
            CullMode = CullMode.None,
            ScissorTestEnable = true
        };

        public MPrimitiveBatch(GraphicsDevice graphicsDevice, int bufferSize, BlendState blendState)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException(graphicsDevice.ToString());
            device = graphicsDevice;
            basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.LightingEnabled = false;
            this.bufferSize = bufferSize;
        }

        public MPrimitiveBatch(GraphicsDevice graphicsDevice, int bufferSize) : this(graphicsDevice, bufferSize, BlendState.Additive
        )
        {
        }

        public MPrimitiveBatch(GraphicsDevice graphicsDevice, BlendState blendState) : this(graphicsDevice, 500, blendState)
        {
        }

        public MPrimitiveBatch(GraphicsDevice graphicsDevice) : this(graphicsDevice, 500) { }

        
        public void UpdateState(Layer current)
        {
            this.currentLayer = current;
            Graphics.screen.End();
            Matrix cam = current.camera?.getMatrix() ?? Matrix.Identity;
            Graphics.screen.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterState, null, cam);
            basicEffect.View = Level.activeLevel.camera.getMatrix();
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0f, Graphics.viewport.Width, Graphics.viewport.Height, 0f, 0.0f, -1f);
        }

        public void ResetState()
        {
            basicEffect.View = Matrix.Identity;
        }
        

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing || isDisposed)
                return;
            if (basicEffect != null)
                basicEffect.Dispose();
            isDisposed = true;
        }

        public void Begin(PrimitiveType primitiveType, BlendState blendState)
        {
            if (hasBegun)
                throw new InvalidOperationException("End must be called before Begin can be called again.");
            this.primitiveType = primitiveType;
            device.BlendState = blendState;
            numVertsPerPrimitive = NumVertsPerPrimitive(primitiveType);
            basicEffect.CurrentTechnique.Passes[0].Apply();
            hasBegun = true;
        }

        public void AddVertex(Vector2 vertex, Color color)
        {
            if (!hasBegun)
                throw new InvalidOperationException("Begin must be called before AddVertex can be called.");
            if (positionInBuffer % numVertsPerPrimitive == 0 && positionInBuffer + numVertsPerPrimitive >= vertices.Length)
                Flush();
            vertices[positionInBuffer].Position = new Vector3(vertex, 0.0f);
            vertices[positionInBuffer].Color = color;
            ++positionInBuffer;
        }

        public void AddVertex(int x, int y, Color color)
        {
            AddVertex(new Vector2((float)x, (float)y), color);
        }

        public void AddVertex(float x, float y, Color color)
        {
            AddVertex(new Vector2(x, y), color);
        }

        public void End()
        {
            if (!hasBegun)
                throw new InvalidOperationException("Begin must be called before End can be called.");
            Flush();
            hasBegun = false;
            if (currentLayer is null) return;
            UpdateState(currentLayer);
        }

        private void Flush()
        {
            if (!hasBegun)
                throw new InvalidOperationException("Begin must be called before Flush can be called.");
            if (positionInBuffer == 0)
                return;
            int primitiveCount = 0;
            switch (primitiveType)
            {
                case PrimitiveType.TriangleList:
                    primitiveCount = positionInBuffer / 3;
                    break;
                case PrimitiveType.TriangleStrip:
                    primitiveCount = positionInBuffer - 2;
                    break;
                case PrimitiveType.LineList:
                    primitiveCount = positionInBuffer / 2;
                    break;
                case PrimitiveType.LineStrip:
                    primitiveCount = positionInBuffer - 1;
                    break;
            }
            
            device.DrawUserPrimitives<VertexPositionColor>(primitiveType, vertices, 0, primitiveCount);
            
            positionInBuffer = 0;
        }
        

        private static int NumVertsPerPrimitive(PrimitiveType primitive)
        {
            switch (primitive)
            {
                case PrimitiveType.TriangleList:
                case PrimitiveType.TriangleStrip:
                    return 3;
                case PrimitiveType.LineList:
                case PrimitiveType.LineStrip:
                    return 2;
                default:
                    throw new InvalidOperationException("primitive is not valid");
            }
        }


        public GraphicsDevice GetDevice()
        {
            return device;
        }

        public Effect GetEffect()
        {
            return basicEffect;
        }
    }
}
