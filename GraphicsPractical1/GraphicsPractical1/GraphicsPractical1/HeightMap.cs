using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraphicsPractical1
{
    class HeightMap
    {
        private int width;
        private int height;

        private byte[,] heightData;

        public HeightMap(Texture2D heightMap)
        {
            // Set the width and the height of the map.
            this.width = heightMap.Width;
            this.height = heightMap.Height;

            // Create the height map.
            this.loadHeightData(heightMap);
        }

        private void loadHeightData(Texture2D heightMap)        
        {
            //Create a 2D array for the height data.
            this.heightData = new byte[this.width, this.height];

            // Get the colors from the texture.
            Color[] colorData = new Color[this.width * this.height];
            heightMap.GetData(colorData);

            // Set the values from the height data array to be the red values of the texture.
            for (int x = 0; x < this.width; x++)
                for (int y = 0; y < this.height; y++)
                    this.heightData[x, y] = colorData[x + y * this.width].R;
        }

        // With this, we can use our class just like a 2D array, i.e. in another class we can use heightMap[x, y],
        // with heightMap being of the type HeightMap.
        public byte this[int x, int y]
        {
            get { return this.heightData[x, y]; }
            set { this.heightData[x, y] = value; }
        }

        public int Width
        {
            get { return this.width; }
        }

        public int Height
        {
            get { return this.height; }
        }
    }
}
