using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    public class Game1 : Game
    {
        // Managers
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Fields
        private const int DefaultPlayerMovementForce = 5000;
        private const int DefaultPlayerMaxSpeed = 300;
        private const int DefaultPlayerFriction = 2500;
        private const int DefaultPlayerHealth = 3;
        private const float DefaultPlayerInvulnerabilityFrames = 3.5f;
        private const float DefaultPlayerDodgeCooldown = 10;
        private const float DefaultPlayerDodgeLength = 0.35f;
        private const float DefaultPlayerDodgeSpeed = 500;

        private const int screenWidth = 1600;
        private const int screenHeight = 896;

        private bool isDebugActive;

        private KeyboardState previousKBState;
        private MouseState previousMState;

        // Assets
        private Texture2D whiteSquareSprite;
        private Texture2D slimeSprite;
        private Texture2D snakeSprite;
        private Texture2D tempCbSprite;
        private Texture2D hitBox;
        private Texture2D arrowHitBox;
        private Texture2D playHoverSprite;
        private Texture2D playPressedSprite;
        private Texture2D settingsHoverSprite;
        private Texture2D settingsPressedSprite;
        private SpriteFont arial32;

        // Objects
        private Button testButton;
        private CrossBow crossbow;
        private Player player;
        private Slime testSlime;
        private Projectile arrow;

        // Buttons
        private Button playButton;
        private Button pauseButton;
        private Button debugButton;

        private List<GameObject> gameObjectList;

        private GameState gameState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            gameObjectList = new List<GameObject>();

            _graphics.PreferredBackBufferWidth = screenWidth;
            _graphics.PreferredBackBufferHeight = screenHeight;
            _graphics.ApplyChanges();



            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // Load textures
            whiteSquareSprite = Content.Load<Texture2D>("White Pixel");
            slimeSprite = Content.Load<Texture2D>("slime");
            snakeSprite = Content.Load<Texture2D>("snake");
            arial32 = Content.Load<SpriteFont>("Arial32");
            tempCbSprite = Content.Load<Texture2D>("Crossbow_Pull_0");
            hitBox = Content.Load<Texture2D>("Hitbox");
            arrowHitBox = Content.Load<Texture2D>("White Pixel");

            // Load objects
            player = new Player(
                snakeSprite,
                new Rectangle(250, 250, 48, 48),
                DefaultPlayerMovementForce,
                DefaultPlayerMaxSpeed,
                DefaultPlayerFriction,
                DefaultPlayerHealth,
                DefaultPlayerInvulnerabilityFrames,
                DefaultPlayerDodgeCooldown,
                DefaultPlayerDodgeLength,
                DefaultPlayerDodgeSpeed
            );

            arrow = new Projectile(
                whiteSquareSprite,
                new Vector2(-100, -100),
                new Point(50, 15),
                0f,
                0,
                true);

            crossbow = new CrossBow(
                tempCbSprite,
                tempCbSprite.Bounds,
                0.3f,
                player);

            testSlime = new Slime(
                3,
                slimeSprite,
                new Rectangle(400, 400, 64, 64),
                39000f,
                1900f,
                player);

            // CollisionManager is established and recieves important permanent references
            CollisionManager.Player = player;
            CollisionManager.Crossbow = crossbow;
            CollisionManager.PlayerArrow = arrow;


            playHoverSprite = Content.Load<Texture2D>("PlayPressed");
            playPressedSprite = Content.Load<Texture2D>("PlayRegular");
            settingsHoverSprite = Content.Load<Texture2D>("SettingsRegular");
            settingsPressedSprite = Content.Load<Texture2D>("SettingsPressed");

            // TODO: TEST CODE
            testButton = new Button(whiteSquareSprite, tempCbSprite, true, new Rectangle(1000, 800, 250, 50));

            // Play Button
            playButton = new Button(playHoverSprite, playPressedSprite, true,
                new Rectangle(screenWidth / 2 - playHoverSprite.Width / 2,
                    screenHeight / 2 - playHoverSprite.Height / 2, playHoverSprite.Width, playHoverSprite.Height));

            // Pause Button
            pauseButton = new Button(settingsPressedSprite, settingsHoverSprite, true,
                new Rectangle(screenWidth - settingsPressedSprite.Width - 5, 5, settingsHoverSprite.Width,
                    settingsHoverSprite.Height));

            // Debug Button
            debugButton = new Button(settingsPressedSprite, settingsHoverSprite, true,
                new Rectangle(screenWidth - 100, screenHeight - 100, settingsHoverSprite.Width,
                    settingsHoverSprite.Height));

            // PASS-IN REFERENCES
            arrow.CrossbowReference = crossbow;

            // Add all GameObjects to GameObject list
            gameObjectList.Add(testSlime);
            gameObjectList.Add(player);
            gameObjectList.Add(crossbow);


            CollisionManager.AddEnemy(testSlime);

            LevelManager.LContent = Content;
            LevelManager.LoadLevel("TestingFile");
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            KeyboardState kbState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();

            switch (gameState)
            {
                case GameState.MainMenu:

                    playButton.Update(gameTime);

                    if (playButton.HasBeenPressed())
                    {
                        gameState = GameState.Game;
                    }

                    break;

                case GameState.Game:

                    CollisionManager.UpdateLevel();

                    // Update all GameObjects
                    Camera.Update(kbState);

                    foreach (GameObject gameObject in gameObjectList)
                    {
                        // Fixes crossbow moving off of player character
                        if (gameObject is CrossBow)
                        {
                            // CollisionManager checks for collisions
                            CollisionManager.CheckCollision();
                        }

                        gameObject.Update(gameTime);
                    }

                    // Spawn slime when pressing E while debug is active
                    if (isDebugActive && kbState.IsKeyDown(Keys.E) && !previousKBState.IsKeyDown(Keys.E))
                    {
                        SpawnSlime(mState.Position);
                    }

                    // Fires the bow on click.
                    if (mState.LeftButton == ButtonState.Pressed && previousMState.LeftButton == ButtonState.Released)
                    {
                        crossbow.Shoot(arrow);
                    }

                    if (CollisionManager.PlayerArrow != null)
                        CollisionManager.PlayerArrow.Update(gameTime);

                    pauseButton.Update(gameTime);

                    // Pause if player presses pause key or escape
                    if (pauseButton.HasBeenPressed() ||
                        (kbState.IsKeyDown(Keys.Escape) && previousKBState.IsKeyUp(Keys.Escape)))
                        gameState = GameState.Pause;

                    break;

                case GameState.Settings:

                    break;

                case GameState.Credits:

                    break;

                case GameState.Pause:

                    playButton.Update(gameTime);
                    debugButton.Update(gameTime);

                    // Resume if player presses pause key or escape
                    if (playButton.HasBeenPressed() ||
                        (kbState.IsKeyDown(Keys.Escape) && previousKBState.IsKeyUp(Keys.Escape)))
                        gameState = GameState.Game;

                    // Enables debug if player presses debug button
                    if (debugButton.HasBeenPressed())
                        isDebugActive = !isDebugActive;

                    break;

                case GameState.GameOver:

                    break;

                case GameState.GameWin:

                    break;

            }

            // TEST CODE THAT MAKES THE PROJECTILE FOLLOW THE MOUSE
            // if (testProjectile != null) testProjectile.Position = mState.Position.ToVector2();

            previousKBState = kbState;
            previousMState = mState;


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Camera.Matrix);

            switch (gameState)
            {
                case GameState.MainMenu:

                    _spriteBatch.DrawString(arial32, "Main Menu",
                        new Vector2(GraphicsDeviceManager.DefaultBackBufferWidth - 175,
                            GraphicsDeviceManager.DefaultBackBufferHeight / 2), Color.White);
                    playButton.Draw(_spriteBatch);

                    break;

                case GameState.Game:

                    DrawGame();

                    break;

                case GameState.Pause:

                    // Draws the game with a darkened overlay
                    DrawGame();
                    _spriteBatch.Draw(whiteSquareSprite, new Rectangle(Point.Zero, new Point(screenWidth, screenHeight)), new Color(Color.Black, 150));

                    _spriteBatch.DrawString(arial32, "Pause",
                        new Vector2(GraphicsDeviceManager.DefaultBackBufferWidth - 175,
                            GraphicsDeviceManager.DefaultBackBufferHeight / 2), Color.White);
                    playButton.Draw(_spriteBatch);

                    // Debug button
                    _spriteBatch.DrawString(arial32, isDebugActive ? "Disable Debug:" : "Enable Debug:",
                        new Vector2(screenWidth - 400, screenHeight - 100), isDebugActive ? Color.Red : Color.Green);
                    debugButton.Draw(_spriteBatch);

                    break;

                case GameState.Settings:

                    _spriteBatch.DrawString(arial32, "Settings", new Vector2(screenWidth - 175, screenHeight / 2),
                        Color.White);

                    break;

                case GameState.Credits:

                    _spriteBatch.DrawString(arial32, "Credits",
                        new Vector2(GraphicsDeviceManager.DefaultBackBufferWidth - 175,
                            GraphicsDeviceManager.DefaultBackBufferHeight / 2), Color.White);

                    break;

                case GameState.GameOver:

                    _spriteBatch.DrawString(arial32, "Game Over",
                        new Vector2(GraphicsDeviceManager.DefaultBackBufferWidth - 175,
                            GraphicsDeviceManager.DefaultBackBufferHeight / 2), Color.White);

                    break;

                case GameState.GameWin:

                    _spriteBatch.DrawString(arial32, "Game Win",
                        new Vector2(GraphicsDeviceManager.DefaultBackBufferWidth - 175,
                            GraphicsDeviceManager.DefaultBackBufferHeight / 2), Color.White);

                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);


        }

        // HELPER METHODS

        /// <summary>
        /// Includes all of the Draw code for GameState.Game
        /// </summary>
        void DrawGame()
        {
            //Level
            LevelManager.Draw(_spriteBatch);

            // Draw all GameObjects
            foreach (GameObject gameObject in gameObjectList)
            {
                gameObject.Draw(_spriteBatch);
            }

            arrow.Draw(_spriteBatch);

            pauseButton.Draw(_spriteBatch);


            // DEBUG
            if (isDebugActive)
            {
                // ~~~ Draws the crossbow's timeSinceShot timer
                _spriteBatch.DrawString(arial32, "" + crossbow.TimeSinceShot, new Vector2(0, 0), Color.Black);

                // Shows working hitboxes that don't use points
                CollisionManager.Draw(_spriteBatch, hitBox, arrowHitBox);

                // TEST CODE TO DRAW ARROW RECTANGLE
                _spriteBatch.Draw(whiteSquareSprite, arrow.Rectangle, Color.Tan);
            }
        }

        /// <summary>
        /// Spawns a slime enemy
        /// </summary>
        /// <param name="position">The position to spawn the slime at</param>
        void SpawnSlime(Point position)
        {
            Slime newSlime = new Slime(
                3,
                slimeSprite,
                new Rectangle(position, new Point(64, 64)),
                39000f,
                1900f,
                player);
            CollisionManager.AddEnemy(newSlime);
            gameObjectList.Add(newSlime);
        }
    }

    public enum GameState
    {
        MainMenu,
        Game,
        Settings,
        Credits,
        Pause,
        GameOver,
        GameWin,
    }
}
