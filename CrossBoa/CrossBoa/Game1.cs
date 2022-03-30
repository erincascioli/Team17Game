using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using CrossBoa.Enemies;
using CrossBoa.Managers;
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
        public static Random RNG = new Random();

        private const int DefaultPlayerMovementForce = 5000;
        private const int DefaultPlayerMaxSpeed = 300;
        private const int DefaultPlayerFriction = 2500;
        private const int DefaultPlayerHealth = 5;
        private const float DefaultPlayerInvulnerabilityFrames = 3.5f;
        private const float DefaultPlayerDodgeCooldown = 2;
        private const float DefaultPlayerDodgeLength = 0.25f;
        private const float DefaultPlayerDodgeSpeed = 2f;

        public const int ScreenWidth = 1600;
        public const int ScreenHeight = 900;

        private bool isDebugActive;
        private bool isGodModeActive = false; // Default

        private KeyboardState kbState;
        private KeyboardState previousKBState;
        private MouseState mState;
        private MouseState previousMState;

        // Assets
        public static Texture2D whiteSquareSprite;
        public static Texture2D playerArrowSprite;
        public static Texture2D slimeSpritesheet;
        public static Texture2D slimeDeathSpritesheet;
        private Texture2D snakeSprite;
        private Texture2D crossbowSprite;
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
        private Texture2D collectibleSprite;
        private Texture2D crosshairSprite;

        private SpriteFont arial32;

        // Objects
        private GameObject[] menuBGLayers;
        private List<GameObject> playerHealthBar;
        private GameObject crosshair;
        private CrossBow crossbow;
        private static Player player;
        private PlayerArrow playerArrow;

        // Buttons
        private Button playButton;
        private Button pauseButton;
        private Button debugButton;
        private Button gameOverButton;

        private List<GameObject> gameObjectList;

        private GameState gameState;

        /// <summary>
        /// A reference to the player object
        /// </summary>
        public static Player Player
        {
            get { return player; }
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            gameObjectList = new List<GameObject>();
            menuBGSpriteList = new Texture2D[5];
            menuBGLayers = new GameObject[10];
            playerHealthBar = new List<GameObject>();


            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
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
            slimeDeathSpritesheet = Content.Load<Texture2D>("FacelessSlimeDeathSpritesheet-sheet");
            emptyHeart = Content.Load<Texture2D>("Empty Heart");
            fullHeart = Content.Load<Texture2D>("Full Heart");
            snakeSprite = Content.Load<Texture2D>("snake");
            crossbowSprite = Content.Load<Texture2D>("Crossbow");
            arial32 = Content.Load<SpriteFont>("Arial32");
            hitBox = Content.Load<Texture2D>("Hitbox");
            arrowHitBox = Content.Load<Texture2D>("White Pixel");
            playerArrowSprite = Content.Load<Texture2D>("arrow2");
            titleText = Content.Load<Texture2D>("TitleText");
            pauseText = Content.Load<Texture2D>("PauseText");
            gameOverText = Content.Load<Texture2D>("GameOverText");
            collectibleSprite = Content.Load<Texture2D>("LifePot");
            crosshairSprite = Content.Load<Texture2D>("Crosshair");


            for (int i = 0; i < 5; i++)
            {
                menuBGSpriteList[i] = Content.Load<Texture2D>("bg" + (i + 1));
            }

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

            for (int i = 0; i < DefaultPlayerHealth; i++)
            {
                playerHealthBar.Add(new GameObject(fullHeart, new Rectangle(5 + (i * 80), 0, 80, 80)));
            }

            crossbow = new CrossBow(
                crossbowSprite,
                crossbowSprite.Bounds);


            CollisionManager.AddCollectible(new Collectible(collectibleSprite, collectibleSprite.Bounds, false));

            // Load menu background layers
            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                {
                    menuBGLayers[i] = new GameObject(menuBGSpriteList[i / 2], new Rectangle(0, 0, ScreenWidth + 10, ScreenHeight));
                }
                else
                {
                    menuBGLayers[i] = new GameObject(menuBGSpriteList[i / 2], new Rectangle(-(ScreenWidth + 10), 0, ScreenWidth + 10, ScreenHeight));
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
                new Rectangle(ScreenWidth / 2 - playHoverSprite.Width * 3 / 4,
                    ScreenHeight / 2 - playHoverSprite.Height * 3 / 4 + 50, playHoverSprite.Width * 3 / 2, playHoverSprite.Height * 3 / 2));

            // Pause Button
            pauseButton = new Button(settingsPressedSprite, settingsHoverSprite, true,
                new Rectangle(ScreenWidth - settingsPressedSprite.Width - 5, 5, settingsHoverSprite.Width,
                    settingsHoverSprite.Height));

            // Debug Button
            debugButton = new Button(settingsPressedSprite, settingsHoverSprite, true,
                new Rectangle(ScreenWidth - 100, ScreenHeight - 100, settingsHoverSprite.Width,
                    settingsHoverSprite.Height));

            // Game Over Button
            gameOverButton = new Button(playHoverSprite, playPressedSprite, true,
                new Rectangle(ScreenWidth / 2 - playHoverSprite.Width * 3 / 4,
                    ScreenHeight / 2 - playHoverSprite.Height * 3 / 4 + 50, playHoverSprite.Width * 3 / 2, playHoverSprite.Height * 3 / 2));


            // Add all GameObjects to GameObject list
            gameObjectList.Add(player);
            gameObjectList.Add(crossbow);

            SpawnManager.GameObjectList = gameObjectList;
            LevelManager.LContent = Content;
            LevelManager.LoadLevel("TestingFile");


            // Temp enemy spawns for starting level
            SpawnManager.SpawnSlime(new Point(400, 400));
            SpawnManager.SpawnSlime(new Point(1280, 448));
            SpawnManager.SpawnSlime(new Point(64 * 12, 64 * 9));
            SpawnManager.SpawnTotem(new Point(50, 100));
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            kbState = Keyboard.GetState();
            mState = Mouse.GetState();

            //Get the position of the mouse for the crosshair
            crosshair = new GameObject(crosshairSprite, new Rectangle(mState.X - crosshairSprite.Width, mState.Y - crosshairSprite.Height, crosshairSprite.Width * 2, crosshairSprite.Height * 2));

            switch (gameState)
            {
                // Main Menu
                case GameState.MainMenu:

                    // Update
                    UpdateMainMenu(gameTime);

                    // Check state changes
                    if (playButton.HasBeenPressed())
                        gameState = GameState.Game;

                    break;

                // Game
                case GameState.Game:

                    // Update
                    UpdateGame(gameTime);

                    // Check state changes
                    if (player.CurrentHealth <= 0)
                        GameOver();

                    break;

                // Settings - NOT YET IMPLEMENTED
                case GameState.Settings:

                    break;

                // Credits - NOT YET IMPLEMENTED
                case GameState.Credits:

                    break;

                // Pause
                case GameState.Pause:

                    UpdatePauseMenu(gameTime);

                    break;

                // Game Over
                case GameState.GameOver:

                    UpdateGameOver(gameTime);

                    break;
            }

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
                    DrawMainMenu();
                    break;

                // Game State
                case GameState.Game:
                    DrawGame();
                    DrawGameUI();
                    
                    break;

                // Pause Menu
                case GameState.Pause:

                    // Draw the game behind the pause menu
                    DrawGame();
                    DrawPauseUI();
                    break;

                // Settings Menu - NOT IMPLEMENTED YET
                case GameState.Settings:

                    _spriteBatch.DrawString(arial32, "Settings", new Vector2(ScreenWidth - 175, ScreenHeight / 2f),
                        Color.White);

                    break;

                // Credits menu - NOT IMPLEMENTED YET
                case GameState.Credits:
                    DrawCredits();
                    break;

                // Game Over
                case GameState.GameOver:
                    DrawGameOver();
                    break;
            }

            base.Draw(gameTime);
        }

        // Update and Draw methods
        #region Update and Draw Methods

        // Main Menu
        /// <summary>
        /// Updates the main menu
        /// </summary>
        /// <param name="gameTime">A reference to the GameTime</param>
        void UpdateMainMenu(GameTime gameTime)
        {
            AnimateMainMenuBG();

            playButton.Update(gameTime);

        }

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
                if (layer.Position.X > ScreenWidth)
                {
                    layer.Position -= new Vector2(menuBGLayers[0].Width * 2, 0);
                }
            }
        }

        /// <summary>
        /// Draws the main menu
        /// </summary>
        void DrawMainMenu()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            GraphicsDevice.Clear(new Color(174, 222, 203));

            foreach (GameObject background in menuBGLayers)
            {
                background.Draw(_spriteBatch);
            }

            _spriteBatch.Draw(titleText, new Vector2(0, 0), Color.White);

            playButton.Draw(_spriteBatch);

            crosshair.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        // Game
        /// <summary>
        /// Updates the game
        /// </summary>
        /// <param name="gameTime">A reference to the GameTime</param>
        private void UpdateGame(GameTime gameTime)
        {
            // Update all GameObjects
            Camera.Update(kbState, gameTime);

            for (int i = 0; i < gameObjectList.Count; i++)
            {
                // Fixes crossbow moving off of player character
                if (gameObjectList[i] is CrossBow)
                {
                    // CollisionManager checks for collisions
                    CollisionManager.CheckCollision(isGodModeActive);
                }

                // Fires a totem's arrow if the cooldown time reaches 0.
                Totem totem;
                if ((totem = gameObjectList[i] as Totem) != null && totem.IsAlive
                    && totem.ReadyToFire)
                {
                    Arrow newTotemArrow = new Arrow(playerArrowSprite,
                        new Rectangle(-100,
                                      -100,
                                      30,
                                      30),
                        new Vector2(0, 0));

                    CollisionManager.AddProjectile(newTotemArrow);
                    gameObjectList.Add(newTotemArrow);

                    totem.Shoot(newTotemArrow);
                }

                // Removes all inactive projectiles from play.
                Arrow arrow;
                if ((arrow = gameObjectList[i] as Arrow) != null && !arrow.IsActive)
                {
                    gameObjectList.RemoveAt(i);
                    i--;
                }

                // ~~~~~ DO ALL EXTERNAL    GAMEOBJECT MODIFICATION ABOVE THIS CODE ~~~~~
                // Delete enemies from lists after they die
                Enemy enemy;
                if ((enemy = gameObjectList[i] as Enemy) != null && !enemy.IsAlive)
                {
                    gameObjectList.RemoveAt(i);
                    i--;
                }
                else if (!(gameObjectList[i] == player && (!player.CanMove && !player.InDodge)))
                {
                    gameObjectList[i].Update(gameTime);
                }
            }

            

            // Fires the bow on click.
            if (mState.LeftButton == ButtonState.Pressed && previousMState.LeftButton == ButtonState.Released
                && !pauseButton.IsMouseOver())
            {
                // Prevents arrow from zooming onto the screen if 
                // the player doesn't shoot within the first 30 seconds of starting
                if (playerArrow == null)
                {
                    playerArrow = new PlayerArrow(
                playerArrowSprite,
                new Rectangle(-100, -100, 60, 60),
                0f,
                0);

                    // Pass-in References
                    playerArrow.CrossbowReference = crossbow;
                    CollisionManager.PlayerArrow = playerArrow;
                }
                crossbow.Shoot(playerArrow);
            }

            // Update the player arrow
            if (playerArrow != null)
                playerArrow.Update(gameTime);

            // Pause if player presses pause key or escape
            pauseButton.Update(gameTime);
            if (pauseButton.HasBeenPressed() ||
                WasKeyPressed(Keys.Escape))
                gameState = GameState.Pause;

            // DEBUG
            if (isDebugActive)
            {
                // Spawn slimes when pressing E
                if (WasKeyPressed(Keys.E))
                {
                    SpawnManager.SpawnSlime(mState.Position);
                }

                // Shake the screen if the player presses Enter while debug is active
                if (kbState.IsKeyDown(Keys.Enter))
                {
                    Camera.ShakeScreen(20);
                }

                // Kills every enemy if the player presses N while debug is active
                // NOTE: Will crash if the the player moves to the next room having 
                // never fired an arrow; for obvious reasons this is a non-issue for now
                if (WasKeyPressed(Keys.N))
                {
                    foreach (GameObject e in gameObjectList)
                    {
                        if (e is Enemy)
                            ((Enemy)e).TakeDamage(1000);
                    }
                }
            }

            if (isDebugActive && WasKeyPressed(Keys.F))
                isGodModeActive = !isGodModeActive;
            if (!isDebugActive)
                isGodModeActive = false;

            if (LevelManager.Exit.IsOpen || (!player.CanMove && !player.InDodge))
            {
                //Camera.FollowPlayer(player);
                if (player.Rectangle.Intersects(LevelManager.Exit.Rectangle) || (!player.CanMove && !player.InDodge))
                {
                    player.InDodge = false;
                    LevelManager.LevelTransition(player, crossbow, gameTime);
                    //player.CanMove = false; // Prevents premature end
                }
            }
            else
            {
                Camera.Center();
            }
        }

        /// <summary>
        /// Includes all of the Draw code for Game GameState
        /// </summary>
        void DrawGame()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Camera.Matrix);

            // Change background color
            GraphicsDevice.Clear(Color.Black);

            // Draw Level
            LevelManager.Draw(_spriteBatch);

            // Draw all GameObjects
            foreach (GameObject gameObject in gameObjectList)
            {
                gameObject.Draw(_spriteBatch);
            }

            if (playerArrow != null)
            playerArrow.Draw(_spriteBatch);

            // DEBUG
            if (isDebugActive)
            {
                // Shows working hitboxes that don't use points
                CollisionManager.Draw(_spriteBatch, hitBox, arrowHitBox);

                // TEST CODE TO DRAW ARROW RECTANGLE
                // _spriteBatch.Draw(whiteSquareSprite, playerArrow.Rectangle, Color.Tan);
            }

            _spriteBatch.End();
        }

        /// <summary>
        /// Includes all of the Draw code for the GameState.Game UI
        /// </summary>
        void DrawGameUI()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

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

            // DEBUG
            if (isDebugActive)
            {
                // ~~~ Draws the crossbow's timeSinceShot timer
                _spriteBatch.DrawString(arial32, "" + crossbow.TimeSinceShot, new Vector2(10, ScreenHeight - 50), Color.White);

                // Draws the crossbow's rotation
                _spriteBatch.DrawString(arial32, "" + crossbow.Direction, new Vector2(10, ScreenHeight - 100), Color.White);
            }

            crosshair.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        // Pause
        /// <summary>
        /// Includes all of the Draw code for the GameState.Pause UI
        /// </summary>
        void DrawPauseUI()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            // Draw dark overlay over the game
            _spriteBatch.Draw(whiteSquareSprite, new Rectangle(Point.Zero, new Point(ScreenWidth, ScreenHeight)), new Color(Color.Black, 160));
            
            // Draw PAUSED text
            _spriteBatch.Draw(pauseText, new Vector2(0, 0), Color.White);
            
            // Draw play button
            playButton.Draw(_spriteBatch);

            // Draw debug button
            _spriteBatch.DrawString(arial32, isDebugActive ? "Disable Debug:" : "Enable Debug:",
                new Vector2(ScreenWidth - 400, ScreenHeight - 100), isDebugActive ? Color.Red : Color.Green);
            debugButton.Draw(_spriteBatch);

            crosshair.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        private void UpdatePauseMenu(GameTime gameTime)
        {
            playButton.Update(gameTime);
            debugButton.Update(gameTime);

            // Resume if player presses pause key or escape
            if (playButton.HasBeenPressed() ||
                WasKeyPressed(Keys.Escape))
                gameState = GameState.Game;

            // Enables debug if player presses debug button
            if (debugButton.HasBeenPressed())
                isDebugActive = !isDebugActive;
        }

        // Game Over
        private void UpdateGameOver(GameTime gameTime)
        {
            AnimateMainMenuBG();

            gameOverButton.Update(gameTime);

            if (gameOverButton.HasBeenPressed())
            {
                gameState = GameState.MainMenu;

                foreach (GameObject i in playerHealthBar)
                {
                    i.Sprite = fullHeart;
                }
            }
        }

        /// <summary>
        /// Draw the game over screen
        /// </summary>
        private void DrawGameOver()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            GraphicsDevice.Clear(new Color(174, 222, 203));

            foreach (GameObject background in menuBGLayers)
            {
                background.Draw(_spriteBatch);
            }

            _spriteBatch.Draw(gameOverText, new Vector2(0, 0), Color.White);

            gameOverButton.Draw(_spriteBatch);

            crosshair.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        // Credits
        /// <summary>
        /// Draws the credits screen
        /// </summary>
        private void DrawCredits()
        {
            _spriteBatch.Begin();

            _spriteBatch.DrawString(arial32, "Credits",
                new Vector2(ScreenWidth - 175,
                    ScreenHeight / 2f), Color.White);

            crosshair.Draw(_spriteBatch);

            _spriteBatch.End();
        }
        #endregion

        #region Helper Methods
        // Helper Methods
        /// <summary>
        /// Run this when the game should end
        /// </summary>
        public void GameOver()
        {
            gameState = GameState.GameOver;
            player.CurrentHealth = DefaultPlayerHealth;
        }

        /// <summary>
        /// Checks if this was the first frame a key was pressed
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if the key was pressed this frame but not pressed last frame; false otherwise</returns>
        public bool WasKeyPressed(Keys key)
        {
            return kbState.IsKeyDown(key) && previousKBState.IsKeyUp(key);
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
    }
}
