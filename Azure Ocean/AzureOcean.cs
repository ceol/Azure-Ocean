using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;

namespace AzureOcean
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class AOGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int tileWidthPx = 8;
        int tileHeightPx = 8;

        private Texture2D grassTileSprite;
        private Texture2D oceanTileSprite;
        private Texture2D elfSprite;

        public AzureOcean.Game game;

        public AOGame()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 750;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            game = new AzureOcean.Game();
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
            game.Initialize();

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
            grassTileSprite = Content.Load<Texture2D>("Images/grass");
            oceanTileSprite = Content.Load<Texture2D>("Images/ocean");
            elfSprite = Content.Load<Texture2D>("Images/elf");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                game.GenerateNewWorld();
            }

            game.Update();

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

            DrawStage(spriteBatch, game.world);

            // Run RenderSystem?
            DrawEntities(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void DrawStage(SpriteBatch spriteBatch, Stage stage)
        {
            Tile tile;
            Texture2D tileTexture;

            for (int x = 0; x < stage.width; x++)
            {
                // The Y axis is flipped when drawing sprites.
                for (int y = stage.height - 1; y >= 0; y--)
                {
                    tile = stage.tiles[x, y];
                    if (tile is WaterTile)
                        tileTexture = oceanTileSprite;
                    else if (tile is GrassTile)
                        tileTexture = grassTileSprite;
                    else
                        tileTexture = grassTileSprite;

                    spriteBatch.Draw(tileTexture, new Rectangle(x * tileWidthPx, y * tileHeightPx, tileWidthPx, tileHeightPx), Color.White);
                }
            }
        }

        protected void DrawEntities(SpriteBatch spriteBatch)
        {
            Type[] filter = new Type[] { typeof(Components.Render), typeof(Components.Transform) };
            foreach (Entity entity in game.GetEntities(filter))
            {
                Components.Transform transform = entity.GetComponent<Components.Transform>();
                Components.Render render = entity.GetComponent<Components.Render>();
                spriteBatch.Draw(elfSprite, new Rectangle(transform.position.X * tileWidthPx, transform.position.Y * tileHeightPx, tileWidthPx, tileHeightPx), Color.White);
            }
        }
    }
}
