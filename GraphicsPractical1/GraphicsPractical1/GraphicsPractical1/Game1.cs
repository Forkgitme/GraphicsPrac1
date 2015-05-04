using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GraphicsPractical1
{

    public class Game1 : Game
    {
        private FrameRateCounter frameRateCounter;
        private BasicEffect effect;

        private Camera camera;

        private Terrain terrain;

        private float angle;
        private float[,] heightData;
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Add the frame rate counter.
            this.frameRateCounter = new FrameRateCounter(this);
            this.Components.Add(this.frameRateCounter);
        }


        protected override void Initialize()
        {
            // Set some properties of the window.
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 600;
            this.graphics.IsFullScreen = false;
            this.graphics.SynchronizeWithVerticalRetrace = false;
            this.graphics.ApplyChanges();

            this.IsFixedTimeStep = false;

            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create a new Effect and enable colors and lighting.
            this.effect = new BasicEffect(this.GraphicsDevice);
            this.effect.VertexColorEnabled = true;
            this.effect.LightingEnabled = true;

            // Set some properties of the lighting.
            this.effect.DirectionalLight0.Enabled = true;
            this.effect.DirectionalLight0.DiffuseColor = Color.White.ToVector3();
            this.effect.DirectionalLight0.Direction = new Vector3(0, -1, 0);
            this.effect.AmbientLightColor = new Vector3(0.3f);

            // Load the heightmap and the terrain.
            Texture2D map = Content.Load<Texture2D>("heightmap");
            this.terrain = new Terrain(new HeightMap(map), 0.2f, this.GraphicsDevice);

            // Create the camera.
            this.camera = new Camera(new Vector3(60, 80, -80), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            float timeStep = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float deltaAngle = 0;
            KeyboardState kbState = Keyboard.GetState();

            // Add a value, dependent on the elapsed time since the last update and the pressed key, to the angle.
            if (kbState.IsKeyDown(Keys.Left))
                deltaAngle += -3 * timeStep;
            if (kbState.IsKeyDown(Keys.Right))
                deltaAngle += 3 * timeStep;

            // Use the angle to rotate the camera's eye around the origin.
            if (deltaAngle != 0)
                this.camera.Eye = Vector3.Transform(this.camera.Eye, Matrix.CreateRotationY(deltaAngle));

            // Set the title of the window to also include the frame rate.
            this.Window.Title = "Graphics Tutorial | FPS: " + this.frameRateCounter.FrameRate;

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            // Color the whole screen.
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Set the rasterizer so that nothing gets culled and the triangles are filled.
            this.GraphicsDevice.RasterizerState = new RasterizerState
            {
                CullMode = CullMode.None,
                FillMode = FillMode.Solid
            };

            // Calculate the matrices that are used to draw everything on the screen.
            Vector3 rotAxis = new Vector3(3 * this.angle, this.angle, 2 * this.angle);
            rotAxis.Normalize();
            Matrix translation = Matrix.CreateTranslation(-0.5f * this.terrain.Width,0, 0.5f * this.terrain.Width);
            Matrix rotation = Matrix.CreateFromAxisAngle(rotAxis, this.angle);

            // Set the matices in the Effect.
            this.effect.Projection = this.camera.ProjectionMatrix;
            this.effect.View = this.camera.ViewMatrix;
            this.effect.World = translation;

            // Draw the terrain.
            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.terrain.Draw(this.GraphicsDevice);
            }

            base.Draw(gameTime);
        }

        //private void loadHeightData()
        //{
        //    this.heightData = new float[4, 3];

        //    this.heightData[0, 0] = 0;
        //    this.heightData[1, 0] = 0;
        //    this.heightData[2, 0] = 0;
        //    this.heightData[3, 0] = 0;
            
        //    this.heightData[0, 1] = 0.5f;
        //    this.heightData[1, 1] = 0;
        //    this.heightData[2, 1] = -1.0f;
        //    this.heightData[3, 1] = 0.2f;

        //    this.heightData[0, 2] = 1.0f;
        //    this.heightData[1, 2] = 1.2f;
        //    this.heightData[2, 2] = 0.8f;
        //    this.heightData[3, 2] = 0;
        //}

    }
}
