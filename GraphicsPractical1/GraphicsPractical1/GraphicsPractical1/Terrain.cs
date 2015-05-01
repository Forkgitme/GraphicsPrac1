using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraphicsPractical1
{
    class Terrain
    {
        private int width;
        private int height;

        private VertexPositionColorNormal[] vertices;

        public int Width
        {
            get { return this.width; }
        }

        public int Height
        {
            get { return this.Height; }
        }

        public Terrain(HeightMap heightMap, float heightScale)
        {
            // Set the width and the height of the terrain.
            this.width = heightMap.Width;
            this.height = heightMap.Height;

            // Create an array with vertices and load it with values.
            VertexPositionColorNormal[] heightDataVertices = this.loadVertices(heightMap, heightScale);
            this.SetupVertices(heightDataVertices);
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

        private void SetupVertices(VertexPositionColorNormal[] heightDataVertices)
        {   // This method turns the vertices created from the height map into vertices for the terrain.

            // Create an array with the terrain vertices. Every vertex/pixel from the height map/terrain,
            // save for the most lower and most right ones, needs two triangles, with three vertices each.
            this.vertices = new VertexPositionColorNormal[(this.width - 1) * (this.height - 1) * 6];

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

                    // Create the two triangles from the four vertices.
                    this.vertices[counter++] = heightDataVertices[topLeft];
                    this.vertices[counter++] = heightDataVertices[lowerRight];
                    this.vertices[counter++] = heightDataVertices[lowerLeft];

                    this.vertices[counter++] = heightDataVertices[topLeft];
                    this.vertices[counter++] = heightDataVertices[topRight];
                    this.vertices[counter++] = heightDataVertices[lowerRight];
                }
        }

        private void calculateNormals()
        {
            // For every triangle:
            for (int i = 0; i < this.vertices.Length / 3; i++)
            {
                // Get the three vertices of the triangle.
                VertexPositionColorNormal v1 = this.vertices[i * 3];
                VertexPositionColorNormal v2 = this.vertices[i * 3 + 1];
                VertexPositionColorNormal v3 = this.vertices[i * 3 + 2];

                // Calculate the normal of the triangle.
                Vector3 side1 = v3.Position - v1.Position;
                Vector3 side2 = v2.Position - v1.Position;
                Vector3 normal = Vector3.Cross(side1, side2);
                normal.Normalize();

                // Set the normals of the vertices to the normal of the triangle.
                this.vertices[i * 3].Normal = normal;
                this.vertices[i * 3 + 1].Normal = normal;
                this.vertices[i * 3 + 2].Normal = normal;
            }
        }

        public void Draw(GraphicsDevice device)
        {
            device.DrawUserPrimitives(PrimitiveType.TriangleList, this.vertices, 0, this.vertices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
        }
    }
}
