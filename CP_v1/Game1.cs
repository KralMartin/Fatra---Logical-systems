using ContextMenu_Mono;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Xml;

namespace CP_v1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Engine engine;

        public Point Get_ScreenSize()
        {
            return new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            
            this.Window.Position = new Point(300, 30);
            graphics.ApplyChanges();
            this.Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
            engine = new Engine(this);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            engine.Close();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive == false)
                return;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
                base.UnloadContent();
                UnloadContent();
                return;
            }

            // TODO: Add your update logic here
            graphics.ApplyChanges();
            engine.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            engine.Draw(spriteBatch);
            //Texture2D t = Content.Load<Texture2D>("pic/obr1");
            //spriteBatch.Draw(t, new Vector2(10, 10), Color.White);

            //SpriteFont f = Content.Load<SpriteFont>("f1");
            //spriteBatch.DrawString(f, "QWE", new Vector2(200, 200), Color.Orange);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        Rectangle prevRect = new Rectangle();
        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            if (prevRect == Window.ClientBounds)
                return;
            prevRect = Window.ClientBounds;
            if (Window.ClientBounds.Width >= 1024)
                graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            else
                graphics.PreferredBackBufferWidth = 1024;

            if (Window.ClientBounds.Height >= 768)
                graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            else
                graphics.PreferredBackBufferHeight = 768;

            ImportantClassesCollection.ScreenSize =new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            ImportantClassesCollection.ContextMenu.Resize(new Rectangle(new Point(), ImportantClassesCollection.ScreenSize));
            engine.Resize();
        }
    }
}
