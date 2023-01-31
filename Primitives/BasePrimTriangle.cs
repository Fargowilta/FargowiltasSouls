using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasSouls.Primitives
{
    public struct BasePrimTriangle : IVertexType
    {
        public Vector2 _position;
        public Color _color;
        public Vector2 _sideCoordinates;

        public VertexDeclaration VertexDeclaration => _vertexDeclaration;

        // This is used via the IVertexType interface for the FNA prim drawer to read the above information and properly
        // draw our primitive triangle. Each VertexElement represents how many bytes the above fields take up and what
        // they should be used for.
        private static readonly VertexDeclaration _vertexDeclaration = new(new VertexElement[]
        {
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
        });

        /// <summary>
        /// A basic struct to store data about a primitive triangle.
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="color">The color</param>
        /// <param name="sideCoordinates">The position of the side this is on</param>
        public BasePrimTriangle(Vector2 position, Color color, Vector2 sideCoordinates)
        {
            _position = position;
            _color = color;
            _sideCoordinates = sideCoordinates;
        }
    }

    public struct PrimTriangle3D : IVertexType
    {
        public Vector2 Position;
        public Color Color;
        public Vector3 SideCoordinates;

        public VertexDeclaration VertexDeclaration => _vertexDeclaration;

        private static readonly VertexDeclaration _vertexDeclaration = new(new VertexElement[]
        {
            new VertexElement(0,VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement(0, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0)
        });

        public PrimTriangle3D(Vector2 position, Color color, Vector3 sideCoordinates)
        {
            Position = position;
            Color = color;
            SideCoordinates = sideCoordinates;
        }
    }

}
