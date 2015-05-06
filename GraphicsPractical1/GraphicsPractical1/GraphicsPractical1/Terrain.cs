using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraphicsPractical1
{
    class Terrain
    {
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;

        private int width;
        private int height;

        private short[] indices;
        private VertexPositionColorNormal[] vertices;

        private HeightMap heightmapV;

        public int Width
        {
            get { return this.width; }
        }

        public int Height
        {
            get { return this.height; }
        }

        public Terrain(HeightMap heightMap, float heightScale, GraphicsDevice device)
        {
            // Set the width and the height of the terrain.
            this.width = heightMap.Width;
            this.height = heightMap.Height;

            heightmapV = heightMap;

            // Create an array with vertices and calculate the corresponding indices.
            this.vertices = this.loadVertices(heightMap, heightScale);
            this.SetupIndices();

            // Calculate normals for all the vertices.
            this.calculateNormals();

            // Create buffers for the Graphics Device.
            this.copyToBuffers(device);
        }

        private VertexPositionColorNormal[] loadVertices(HeightMap heightMap, float heightScale)
        {   // This method creates vertices from the height map.
            VertexPositionColorNormal[] vertices = new VertexPositionColorNormal[this.width * this.height];
            
            // Set the position, height and the color of the vertices.
            for (int x = 0; x < this.width; x++)
                for (int y = 0; y < this.height; y++)
                {
                    int v = x + y * this.width;
                    float h = heightMap[x, y] * heightScale;

                    vertices[v].Position = new Vector3(x, h, -y);
                    vertices[v].Color = Color.Green;
                }

            return vertices;
        }

        private void SetupIndices()
        {   // This method turns the vertices created from the height map into vertices for the terrain.

            // Create an array with the terrain vertices. Every vertex/pixel from the height map/terrain,
            // save for the most lower and most right ones, needs two triangles, with three vertices each.
            this.indices = new short[(this.width - 1) * (this.height - 1) * 6];

            int counter = 0;

            // For every point in the terrain:
            for (int x = 0; x < this.width - 1; x++)
                for (int y = 0; y < this.height - 1; y++)
                {
                    // Get the indices of the four vertices that will make two triangles.
                    int lowerLeft = x + y * this.width;
                    int lowerRight = (x + 1) + y * this.width;
                    int topLeft = x + (y + 1) * this.width;
                    int topRight = (x + 1) + (y + 1) * this.width;

                    // Create the two triangles from the indices.
                    this.indices[counter++] = (short) topLeft;
                    this.indices[counter++] = (short) lowerRight;
                    this.indices[counter++] = (short) lowerLeft;

                    this.indices[counter++] = (short) topLeft;
                    this.indices[counter++] = (short) topRight;
                    this.indices[counter++] = (short) lowerRight;
                }
        }

        private void calculateNormals()
        {
            // For every triangle:
            for (int i = 0; i < this.indices.Length / 3; i++)
            {
                // Get the three indices of the triangle.
                short i1 = this.indices[i * 3];
                short i2 = this.indices[i * 3 + 1];
                short i3 = this.indices[i * 3 + 2];

                // First use the indices to find the sides of the triangle and calculate a normal.
                Vector3 side1 = this.vertices[i3].Position - this.vertices[i1].Position;
                Vector3 side2 = this.vertices[i2].Position - this.vertices[i1].Position;
                Vector3 normal = Vector3.Cross(side1, side2);
                normal.Normalize();

                // Add the normal to all the normals of the vertices meaning multiple normals on a single vertex will be added.
                this.vertices[i1].Normal += normal;
                this.vertices[i2].Normal += normal;
                this.vertices[i3].Normal += normal;
            }

            // Adding the normals changed their lengths, every vertex is normalized again.
            for (int i = 0; i < this.vertices.Length; i++)
                this.vertices[i].Normal.Normalize();
        }

        public void Draw(GraphicsDevice device)
        {
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.vertices.Length, 0, this.indices.Length / 3);
        }

        private void copyToBuffers(GraphicsDevice device)
        {
            // Specify what kind of data the buffer has to store and how much so it can reserve space.
            this.vertexBuffer = new VertexBuffer(device, VertexPositionColorNormal.VertexDeclaration, this.vertices.Length, BufferUsage.WriteOnly);

            // Load the vertices data into the buffer.
            this.vertexBuffer.SetData(this.vertices);

            // Specify what kind of data the buffer has to store and how much so it can allocate space.
            this.indexBuffer = new IndexBuffer(device, typeof(short), this.indices.Length, BufferUsage.WriteOnly);

            // Load the indices data into the buffer.
            this.indexBuffer.SetData(this.indices);

            // Tells the graphics device to read from its own internal memory.
            device.Indices = this.indexBuffer;
            device.SetVertexBuffer(this.vertexBuffer);
        }

        public Vector3 clipEye(Vector3 eye)
        {
            eye.Y = heightmapV[(int)eye.X + 64, (int)eye.Z + 64] * 0.2f + 3;
            Console.WriteLine("HM Coords: X: " + (int)eye.X + 64 + " Y: " + (int)eye.Z + 64);
            return eye;
        }
    }
}
