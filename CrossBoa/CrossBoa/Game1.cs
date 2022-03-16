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
        private const int DefaultPlayerHealth = 5;
        private const float DefaultPlayerInvulnerabilityFrames = 1f;
        private const float DefaultPlayerDodgeCooldown = 10;
        private const float DefaultPlayerDodgeLength = 0.35f;
        private const float DefaultPlayerDodgeSpeed = 500;

        private const int screenWidth = 1600;
        private const int screenHeight = 900;

        private bool isDebugActive;

        private KeyboardState previousKBState;
        private MouseState previousMState;

        // Assets
        private Texture2D whiteSquareSprite;
        private Texture2D playerArrowSprite;
        private Texture2D slimeSpritesheet;
        private Texture2D snakeSprite;
        private Texture2D tempCbSprite;
        private Texture2D hitBox;
        private Texture2D arrowHitBox;
        private Texture2D playHoverSprite;
        private Texture2D playPressedSprite;
        private Texture2D settingsHoverSprite;
        private Texture2D settingsPressedSprite;
        private Texture2D emptyHeart;
        private Texture2D fullHeart;
        private Texture2D[] menuBGSpriteList;
        private Texture2D titleText;
        private Texture2D pauseText;
        private Texture2D gameOverText;

        private SpriteFont arial32;

        // Objects
        private GameObject[] menuBGLayers;
        private List<GameObject> playerHealthBar;
        private CrossBow crossbow;
        private Player player;
        private Projectile playerArrow;

        // Buttons
        private Button playButton;
        private Button pauseButton;
        private Button debugButton;
        private Button gameOverButton;

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
            menuBGSpriteList = new Texture2D[5];
            menuBGLayers = new GameObject[10];
            playerHealthBar = new List<GameObject>();

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
            slimeSpritesheet = Content.Load<Texture2D>("FacelessSlimeSpritesheet");
            emptyHeart = Content.Load<Texture2D>("Empty Heart");
            fullHeart = Content.Load<Texture2D>("Full Heart");
            snakeSprite = Content.Load<Texture2D>("snake");
            arial32 = Content.Load<SpriteFont>("Arial32");
            tempCbSprite = Content.Load<Texture2D>("bow");
            hitBox = Content.Load<Texture2D>("Hitbox");
            arrowHitBox = Content.Load<Texture2D>("White Pixel");
            playerArrowSprite = Content.Load<Texture2D>("arrow2");
            titleText = Content.Load<Texture2D>("TitleText");
            pauseText = Content.Load<Texture2D>("PauseText");
            gameOverText = Content.Load<Texture2D>("GameOverText");

            for (int i = 0; i < 5; i++)
            {
                menuBGSpriteList[i] = Content.Load<Texture2D>("bg" + (i + 1));
            }

            // Load objects
            player = new Player(
                snakeSprite,
                new Rectangle(250, 250, 64, 64),
                DefaultPlayerMovementForce,
                DefaultPlayerMaxSpeed,
                DefaultPlayerFriction,
                DefaultPlayerHealth,
                DefaultPlayerInvulnerabilityFrames,
                DefaultPlayerDodgeCooldown,
                DefaultPlayerDodgeLength,
                DefaultPlayerDodgeSpeed
            );

            for (int i = 0; i < DefaultPlayerHealth; i++)
            {
                playerHealthBar.Add(new GameObject(fullHeart, new Rectangle(5 + (i * 80), 0, 80, 80)));
            }

            playerArrow = new Projectile(
                playerArrowSprite,
                new Vector2(-100, -100),
                new Point(60, 60),
                0f,
                0,
                true);

            crossbow = new CrossBow(
                tempCbSprite,
                tempCbSprite.Bounds,
                0.3f,
                player);

            SpawnSlime(new Point(400, 400));
            SpawnSlime(new Point(1280, 448));
            SpawnSlime(new Point(64 * 12, 64 * 9));


            // Load menu background layers
            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                {
                    menuBGLayers[i] = new GameObject(menuBGSpriteList[i / 2], new Rectangle(0, 0, screenWidth + 10, screenHeight));
                }
                else
                {
                    menuBGLayers[i] = new GameObject(menuBGSpriteList[i / 2], new Rectangle(-(screenWidth + 10), 0, screenWidth + 10, screenHeight));
                }

            }

            // CollisionManager is established and receives important permanent references
            CollisionManager.Player = player;
            CollisionManager.Crossbow = crossbow;
            CollisionManager.PlayerArrow = playerArrow;

            playHoverSprite = Content.Load<Texture2D>("PlayPressed");
            playPressedSprite = Content.Load<Texture2D>("PlayRegular");
            settingsHoverSprite = Content.Load<Texture2D>("SettingsRegular");
            settingsPressedSprite = Content.Load<Texture2D>("SettingsPressed");

            // Play Button
            playButton = new Button(playHoverSprite, playPressedSprite, true,
                new Rectangle(screenWidth / 2 - playHoverSprite.Width * 3 / 4,
                    screenHeight / 2 - playHoverSprite.Height * 3 / 4 + 50, playHoverSprite.Width * 3 / 2, playHoverSprite.Height * 3 / 2));

            // Pause Button
            pauseButton = new Button(settingsPressedSprite, settingsHoverSprite, true,
                new Rectangle(screenWidth - settingsPressedSprite.Width - 5, 5, settingsHoverSprite.Width,
                    settingsHoverSprite.Height));

            // Debug Button
            debugButton = new Button(settingsPressedSprite, settingsHoverSprite, true,
                new Rectangle(screenWidth - 100, screenHeight - 100, settingsHoverSprite.Width,
                    settingsHoverSprite.Height));

            // Game Over Button
            gameOverButton = new Button(playHoverSprite, playPressedSprite, true,
                new Rectangle(screenWidth / 2 - playHoverSprite.Width * 3 / 4,
                    screenHeight / 2 - playHoverSprite.Height * 3 / 4 + 50, playHoverSprite.Width * 3 / 2, playHoverSprite.Height * 3 / 2));

            // PASS-IN REFERENCES
            playerArrow.CrossbowReference = crossbow;

            // Add all GameObjects to GameObject list
            gameObjectList.Add(player);
            gameObjectList.Add(crossbow);

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

                    AnimateMainMenuBG();

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
                    
                    if (player.CurrentHealth <= 0)
                    {
                        gameState = GameState.GameOver;
                        player.CurrentHealth = DefaultPlayerHealth;
                    }

                    // Spawn slime when pressing E while debug is active
                    if (isDebugActive && kbState.IsKeyDown(Keys.E) && !previousKBState.IsKeyDown(Keys.E))
                    {
                        SpawnSlime(mState.Position);
                    }

                    // Shake the screen if the player presses space while debug is active
                    if (isDebugActive && kbState.IsKeyDown(Keys.Space))
                    {
                        Camera.ShakeScreen(20);
                    }

                    // Fires the bow on click.
                    if (mState.LeftButton == ButtonState.Pressed && previousMState.LeftButton == ButtonState.Released
                        && !pauseButton.IsMouseOver())
                    {
                        crossbow.Shoot(playerArrow);
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

                    AnimateMainMenuBG();

                    gameOverButton.Update(gameTime);

                    if (gameOverButton.HasBeenPressed())
                    {
                        gameState = GameState.MainMenu;
                    }

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
            
            switch (gameState)
            {
                // Main Menu
                case GameState.MainMenu:
                    _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
                    GraphicsDevice.Clear(new Color(174, 222, 203));

                    foreach (GameObject background in menuBGLayers)
                    {
                        background.Draw(_spriteBatch);
                    }

                    _spriteBatch.Draw(titleText, new Vector2(0, 0), Color.White);

                    /*
                    _spriteBatch.DrawString(arial32, "Crossboa",
                        new Vector2(GraphicsDeviceManager.DefaultBackBufferWidth - 94,
                            GraphicsDeviceManager.DefaultBackBufferHeight / 2), Color.White);
                    */

                    playButton.Draw(_spriteBatch);
                    

                    _spriteBatch.End();
                    break;

                // Game State
                case GameState.Game:
                    _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Camera.Matrix);
                    GraphicsDevice.Clear(Color.Black);
                    DrawGame();


                    _spriteBatch.End();
                    _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
                    DrawGameUI();
                    _spriteBatch.End();
                    break;

                // Pause Menu
                case GameState.Pause:

                    _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Camera.Matrix);
                    GraphicsDevice.Clear(Color.Black);

                    // Draws the game with a darkened overlay
                    DrawGame();
                    _spriteBatch.Draw(whiteSquareSprite, new Rectangle(Point.Zero, new Point(screenWidth, screenHeight)), new Color(Color.Black, 160));

                    _spriteBatch.Draw(pauseText, new Vector2(0, 0), Color.White);
                    /*
                    _spriteBatch.DrawString(arial32, "Pause",
                        new Vector2(GraphicsDeviceManager.DefaultBackBufferWidth - 63,
                            GraphicsDeviceManager.DefaultBackBufferHeight / 2), Color.White);
                    */

                    playButton.Draw(_spriteBatch);

                    // Debug button
                    _spriteBatch.DrawString(arial32, isDebugActive ? "Disable Debug:" : "Enable Debug:",
                        new Vector2(screenWidth - 400, screenHeight - 100), isDebugActive ? Color.Red : Color.Green);
                    debugButton.Draw(_spriteBatch);

                    _spriteBatch.End();
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

                    _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
                    GraphicsDevice.Clear(new Color(174, 222, 203));

                    foreach (GameObject background in menuBGLayers)
                    {
                        background.Draw(_spriteBatch);
                    }

                    _spriteBatch.Draw(gameOverText, new Vector2(0, 0), Color.White);

                    gameOverButton.Draw(_spriteBatch);
                    


                    _spriteBatch.End();

                    break;

                case GameState.GameWin:

                    _spriteBatch.Begin();

                    _spriteBatch.DrawString(arial32, "Game Win",
                        new Vector2(GraphicsDeviceManager.DefaultBackBufferWidth - 175,
                            GraphicsDeviceManager.DefaultBackBufferHeight / 2), Color.White);

                    _spriteBatch.End();

                    break;
            }

            

            base.Draw(gameTime);


        }

        // Helper Methods
        #region Helper Methods

        /// <summary>
        /// Animates the main menu with parallax
        /// </summary>
        void AnimateMainMenuBG()
        {
            // Layer 1 is a blank image

            // Layer 2
            menuBGLayers[2].Position += new Vector2(0.45f, 0);
            menuBGLayers[3].Position += new Vector2(0.45f, 0);

            // Layer 3
            menuBGLayers[4].Position += new Vector2(0.9f, 0);
            menuBGLayers[5].Position += new Vector2(0.9f, 0);

            // Layer 4
            menuBGLayers[6].Position += new Vector2(1.3f, 0);
            menuBGLayers[7].Position += new Vector2(1.3f, 0);

            // Layer 5
            menuBGLayers[8].Position += new Vector2(1.8f, 0);
            menuBGLayers[9].Position += new Vector2(1.8f, 0);

            // Wrap image around the screen after it goes off the edge
            foreach (GameObject layer in menuBGLayers)
            {
                if (layer.Position.X > screenWidth)
                {
                    layer.Position -= new Vector2(menuBGLayers[0].Width * 2, 0);
                }
            }
        }

        /// <summary>
        /// Includes all of the Draw code for Game GameState
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

            playerArrow.Draw(_spriteBatch);

            // DEBUG
            if (isDebugActive)
            {
                // Shows working hitboxes that don't use points
                CollisionManager.Draw(_spriteBatch, hitBox, arrowHitBox);

                // TEST CODE TO DRAW ARROW RECTANGLE
                _spriteBatch.Draw(whiteSquareSprite, playerArrow.Rectangle, Color.Tan);

                // ~~~ Draws the crossbow's timeSinceShot timer
                _spriteBatch.DrawString(arial32, "" + crossbow.TimeSinceShot, new Vector2(10, screenHeight - 50), Color.White);
            }
        }

        /// <summary>
        /// Includes all of the Draw code for the GameState.Game UI
        /// </summary>
        void DrawGameUI()
        {
            pauseButton.Draw(_spriteBatch);
            for (int i = 0; i < playerHealthBar.Count; i++)
            {
                if (i < player.CurrentHealth)
                {
                    playerHealthBar[i].Draw(_spriteBatch);
                }
                else
                {
                    if (playerHealthBar[i].Sprite != emptyHeart)
                    {
                        playerHealthBar[i].Sprite = emptyHeart;
                    }
                    playerHealthBar[i].Draw(_spriteBatch);
                }
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
                slimeSpritesheet,
                new Rectangle(position, new Point(64, 64)),
                player);
            CollisionManager.AddEnemy(newSlime);
            gameObjectList.Add(newSlime);
        }

        

        #endregion
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
