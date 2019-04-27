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

        int windowWidthPx = 1010;
        int windowHeightPx = 510;

        int tileWidthPx = 5;
        int tileHeightPx = 5;

        int stageXOffset = 1;
        int stageYOffset = 1;

        private Texture2D grassTileSprite;
        private Texture2D treeTileSprite;
        private Texture2D waterTileSprite;
        private Texture2D elfSprite;

        public GameState game;

        public AOGame()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = windowWidthPx;
            graphics.PreferredBackBufferHeight = windowHeightPx;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            game = new GameState();
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
            treeTileSprite = Content.Load<Texture2D>("Images/tree");
            waterTileSprite = Content.Load<Texture2D>("Images/water");
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
            GraphicsDevice.Clear(new Color(12, 33, 23));

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
            Texture2D tileTexture = grassTileSprite;

            for (int x = 0; x < stage.width; x++)
            {
                for (int y = 0; y < stage.height; y++)
                {
                    tile = stage.tiles[x, y];
                    if (tile is GrassTile)
                        tileTexture = grassTileSprite;
                    else if (tile is TreeTile)
                        tileTexture = treeTileSprite;
                    else if (tile is WaterTile)
                        tileTexture = waterTileSprite;

                    // The Y axis is flipped when drawing sprites.
                    int xCoord = x + stageXOffset;
                    int yCoord = (stage.height - y - 1) + stageYOffset;
                    spriteBatch.Draw(tileTexture, new Rectangle(xCoord * tileWidthPx, yCoord * tileHeightPx, tileWidthPx, tileHeightPx), Color.White);
                }
            }
        }

        struct DrawComponents
        {
            public Components.Transform transform;
            public Components.Render render;
        }

        protected void DrawEntities(SpriteBatch spriteBatch)
        {
            foreach (Entity entity in game.GetEntities<DrawComponents>())
            {
                Components.Transform transform = entity.GetComponent<Components.Transform>();
                Components.Render render = entity.GetComponent<Components.Render>();

                int xCoord = transform.position.x + stageXOffset;
                int yCoord = (game.CurrentStage.height - transform.position.y) + stageYOffset;
                spriteBatch.Draw(elfSprite, new Rectangle(xCoord * tileWidthPx, yCoord * tileHeightPx, tileWidthPx, tileHeightPx), Color.White);
            }
        }
    }
}
