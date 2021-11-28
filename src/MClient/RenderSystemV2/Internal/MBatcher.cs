using System;
using DuckGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = DuckGame.Color;
using Matrix = DuckGame.Matrix;

namespace MClient.RenderSystemV2.Internal
{
    /// <summary>
    /// This class is the core of the custom rendering, and is what actually handles drawing the polygons.
    /// </summary>
    public sealed class MBatcher : IDisposable
    {
        private readonly VertexPositionColor[] _vertices = new VertexPositionColor[1000];
        
        private int _positionInBuffer;
        private readonly BasicEffect _basicEffect;
        private readonly GraphicsDevice _device;
        private PrimitiveType _primitiveType;
        private int _numVertsPerPrimitive;
        private bool _hasBegun;
        private bool _isDisposed;

        private static readonly RasterizerState RasterState = new RasterizerState()
        {
            CullMode = CullMode.None,
            //TODO - Scissor regions
        };

        public MBatcher(GraphicsDevice graphicsDevice, int bufferSize = 1000)
        {
            if (graphicsDevice == null) return;
            _device = graphicsDevice;
            _basicEffect = new BasicEffect(graphicsDevice) {VertexColorEnabled = true, LightingEnabled = false};
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing || _isDisposed)
                return;
            _basicEffect?.Dispose();
            _isDisposed = true;
        }

        public void Begin(PrimitiveType primitiveType, BlendState blendState)
        {
            if (_hasBegun)
                throw new InvalidOperationException("End must be called before Begin can be called again.");
            _primitiveType = primitiveType;
            _device.BlendState = blendState;
            _numVertsPerPrimitive = NumVertsPerPrimitive(primitiveType);
            _basicEffect.CurrentTechnique.Passes[0].Apply();
            _hasBegun = true;
        }

        public void AddVertex(Vector2 vertex, Color color)
        {
            if (!_hasBegun)
                throw new InvalidOperationException("Begin must be called before AddVertex can be called.");
            if (_positionInBuffer % _numVertsPerPrimitive == 0 && _positionInBuffer + _numVertsPerPrimitive >= _vertices.Length)
                Flush();
            _vertices[_positionInBuffer].Position = new Vector3(vertex, 0.0f);
            _vertices[_positionInBuffer].Color = color;
            ++_positionInBuffer;
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
            if (!_hasBegun)
                throw new InvalidOperationException("Begin must be called before End can be called.");
            Flush();
            _hasBegun = false;
        }

        private void Flush()
        {
            if (!_hasBegun)
                throw new InvalidOperationException("Begin must be called before Flush can be called.");
            if (_positionInBuffer == 0)
                return;
            int primitiveCount = 0;
            switch (_primitiveType)
            {
                case PrimitiveType.TriangleList:
                    primitiveCount = _positionInBuffer / 3;
                    break;
                case PrimitiveType.TriangleStrip:
                    primitiveCount = _positionInBuffer - 2;
                    break;
                case PrimitiveType.LineList:
                    primitiveCount = _positionInBuffer / 2;
                    break;
                case PrimitiveType.LineStrip:
                    primitiveCount = _positionInBuffer - 1;
                    break;
            }
            
            _device.DrawUserPrimitives<VertexPositionColor>(_primitiveType, _vertices, 0, primitiveCount);
            
            _positionInBuffer = 0;
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
            return _device;
        }

        public BasicEffect GetEffect()
        {
            return _basicEffect;
        }
    }
}
