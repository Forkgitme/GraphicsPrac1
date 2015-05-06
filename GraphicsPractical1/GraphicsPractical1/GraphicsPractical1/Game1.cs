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

        //private Vector3 rotation;

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
            this.camera = new Camera(new Vector3(0, 80, 50), new Vector3(-0.5f, 0, 0));

        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            float timeStep = (float)gameTime.ElapsedGameTime.TotalSeconds;

            KeyboardState kbState = Keyboard.GetState();

            // Exit the program with the Esc key.
            if (kbState.IsKeyDown(Keys.Escape))
                this.Exit();

            // Set the rotation of the camera first, and then move the camera.
            this.AddMouseRotation(Mouse.GetState(), timeStep * 2);
            this.AddMovement(kbState, timeStep);
            this.camera.Eye = terrain.clipEye(this.camera.Eye);
            //Console.WriteLine("X: " + this.camera.Eye.X + "Y: " + this.camera.Eye.Y + "Z: " + this.camera.Eye.Z);

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

        public void AddMouseRotation(MouseState mouseState, float timeStep)
        {   //This method adds a value to the camera's rotation based on how the user moved his mouse.
            Vector3 rotation = this.camera.Rotation;

            // Calculate how much the user moved the mouse.
            float yDifference = mouseState.X - 400;
            float xDifference = mouseState.Y - 300;

            // Rotate the camera based on that.
            rotation.Y = (rotation.Y - timeStep * yDifference) % (float)(Math.PI * 2);
            rotation.X = MathHelper.Clamp(rotation.X - timeStep * xDifference, -(float)(Math.PI / 2), (float)(Math.PI / 2));

            // Put the mouse in the center of the screen.
            Mouse.SetPosition(400, 300);

            // Set the camera's rotation.
            this.camera.Rotation = rotation;
        }

        public void AddMovement(KeyboardState kbState, float timeStep)
        {   // This methods moves the camera (the 'eye').
            Vector3 deltaPosition = Vector3.Zero;

            const int SCALE = 10;

            // Calculate how much the camera should move were it not rotated.
            if (kbState.IsKeyDown(Keys.A))
                deltaPosition += -Vector3.UnitX * SCALE * timeStep;
            if (kbState.IsKeyDown(Keys.D))
                deltaPosition += Vector3.UnitX * SCALE * timeStep;
            if (kbState.IsKeyDown(Keys.W))
                deltaPosition += -Vector3.UnitZ * SCALE * timeStep;
            if (kbState.IsKeyDown(Keys.S))
                deltaPosition += Vector3.UnitZ * SCALE * timeStep;
            if (kbState.IsKeyDown(Keys.Q))
                deltaPosition += -Vector3.UnitY * SCALE * timeStep;
            if (kbState.IsKeyDown(Keys.E))
                deltaPosition += Vector3.UnitY * SCALE * timeStep;

            // Rotate the unrotated vector and add it to the camera's position.
            Matrix rotation = Matrix.CreateRotationY(this.camera.Rotation.Y);
            this.camera.Eye += Vector3.Transform(deltaPosition, rotation);
        }
    }
}
