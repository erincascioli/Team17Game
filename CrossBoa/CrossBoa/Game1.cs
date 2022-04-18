using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using CrossBoa.Enemies;
using CrossBoa.Interfaces;
using CrossBoa.Managers;
using CrossBoa.Upgrades;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace CrossBoa
{
    public class Game1 : Game
    {
        // Managers
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // A render target will make the game render to a much smaller, virtual screen
        //     before scaling it up to the proper window size
        public static RenderTarget2D gameRenderTarget;
        public static Rectangle gameTargetRect;                 // A rectangle representing the whole window inside the black bars

        // Fields
        public static Random RNG = new Random();

        private const int DefaultPlayerMovementForce = 5000;
        private const int DefaultPlayerMaxSpeed = 300;
        private const int DefaultPlayerFriction = 2500;
        private const int DefaultPlayerHealth = 5;
        private const float DefaultPlayerDodgeCooldown = 2;
        private const float DefaultPlayerDodgeLength = 0.25f;
        private const float DefaultPlayerDodgeSpeed = 3f;

        private static int exp;

        public static int UIScale;
        public static int windowWidth;
        public static int windowHeight;
        public static Rectangle windowRect;
        private float outputAspectRatio;
        private float preferredAspectRatio;
        private float FPS;

        private static bool isDebugActive = false;
        public static bool isGodModeActive = false;

        private static KeyboardState kbState;
        private static KeyboardState previousKBState;
        private static MouseState mState;
        private static MouseState previousMState;

        // Assets
        #region Asset Field Declarations
        public static Texture2D whiteSquareSprite;
        public static Texture2D totemSprite;
        public static Texture2D skeletonSprite;
        public static Texture2D playerArrowSprite;
        public static Texture2D totemProjectileSprite;
        public static Texture2D slimeSpritesheet;
        public static Texture2D slimeDeathSpritesheet;
        public static Texture2D xpSprite;
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
        private static SpriteFont pressStart;
        #endregion

        // Objects
        private GameObject[] menuBGLayers;
        private List<UIElement> playerHealthBar;
        public static List<UIElement> UIElementsList;
        private UIElement crosshair;
        private static CrossBow crossbow;
        private static Player player;
        public static List<PlayerArrow> playerArrowList;
        private static List<Collectible> collectibles;

        private TextElement testText;
        private TextElement FPSCounter;

        // Buttons
        private Button playButton;
        private Button pauseButton;
        private Button debugButton;
        private Button gameOverButton;
        private Button[] upgradeButtons;

        // Stuff for Upgrade State
        private TextElement upgradeName;
        private TextElement upgradeDescription;

        // GameState Stuff
        private List<GameObject> gameObjectList;
        private GameState gameState;

        #region Static properties
        /// <summary>
        /// A reference to the player object
        /// </summary>
        public static Player Player
        {
            get { return player; }
        }

        /// <summary>
        /// A reference to the crossbow object
        /// </summary>
        public static CrossBow Crossbow
        {
            get { return crossbow; }
        }

        /// <summary>
        /// A reference to the list of all collectibles
        /// </summary>
        public static List<Collectible> Collectibles
        {
            get { return collectibles; }
        }

        /// <summary>
        /// The current experience points this player has
        /// </summary>
        public static int Exp
        {
            get { return exp; }
            set { exp = value; }
        }

        /// <summary>
        /// A static reference to the press start font
        /// </summary>
        public static SpriteFont PressStart
        {
            get { return pressStart; }
        }

        /// <summary>
        /// Whether or not debug mode is currently active
        /// </summary>
        public static bool IsDebugActive
        {
            get { return isDebugActive; }
        }

        #endregion

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // CREDITS TO: https://community.monogame.net/t/handling-user-controlled-window-resizing/7828
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;

            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            gameObjectList = new List<GameObject>();
            menuBGSpriteList = new Texture2D[5];
            menuBGLayers = new GameObject[10];
            playerHealthBar = new List<UIElement>();
            UIElementsList = new List<UIElement>(20);
            playerArrowList = new List<PlayerArrow>();
            collectibles = new List<Collectible>(100);
            upgradeButtons = new Button[3];

            // --- Prepare game rendering ---
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.ApplyChanges();

            UIScale = 4;

            // Create a render target that can be much more easily rescaled
            gameRenderTarget = new RenderTarget2D(GraphicsDevice, 1600, 900);

            // Save aspect ratio
            preferredAspectRatio = 16 / 9f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // Load textures
            whiteSquareSprite = Content.Load<Texture2D>("White Pixel");
            totemSprite = Content.Load<Texture2D>("TotemSprite");
            skeletonSprite = Content.Load<Texture2D>("BeastSprite");
            slimeSpritesheet = Content.Load<Texture2D>("FacelessSlimeSpritesheet");
            slimeDeathSpritesheet = Content.Load<Texture2D>("FacelessSlimeDeathSpritesheet-sheet");
            xpSprite = Content.Load<Texture2D>("XPOrb");
            emptyHeart = Content.Load<Texture2D>("Empty Heart");
            fullHeart = Content.Load<Texture2D>("Full Heart");
            snakeSprite = Content.Load<Texture2D>("snake");
            crossbowSprite = Content.Load<Texture2D>("Crossbow");
            hitBox = Content.Load<Texture2D>("Hitbox");
            arrowHitBox = Content.Load<Texture2D>("White Pixel");
            playerArrowSprite = Content.Load<Texture2D>("arrow2");
            totemProjectileSprite = Content.Load<Texture2D>("FireballSprite");
            titleText = Content.Load<Texture2D>("TitleText");
            pauseText = Content.Load<Texture2D>("PauseText");
            gameOverText = Content.Load<Texture2D>("GameOverText");
            collectibleSprite = Content.Load<Texture2D>("LifePot");
            crosshairSprite = Content.Load<Texture2D>("Crosshair");

            arial32 = Content.Load<SpriteFont>("Arial32");
            pressStart = Content.Load<SpriteFont>("Fonts/PressStart6");


            for (int i = 0; i < 5; i++)
            {
                menuBGSpriteList[i] = Content.Load<Texture2D>("bg" + (i + 1));
            }

            // Load Player
            player = new Player(
                snakeSprite,
                new Rectangle(gameRenderTarget.Bounds.Center, new Point(48)),
                DefaultPlayerMovementForce,
                DefaultPlayerMaxSpeed,
                DefaultPlayerFriction,
                DefaultPlayerHealth,
                DefaultPlayerDodgeCooldown,
                DefaultPlayerDodgeLength,
                DefaultPlayerDodgeSpeed
            );

            // Load Crossbow
            crossbow = new CrossBow(
                crossbowSprite,
                crossbowSprite.Bounds);

            // Load main arrow
            playerArrowList.Add(
                new PlayerArrow(
                    playerArrowSprite,
                    new Point(64),
                    true));

            // Subscribes crossbow and arrow to each others' events
            crossbow.FireArrows += playerArrowList[0].GetShot;
            playerArrowList[0].OnPickup += crossbow.PickUpArrow;

            // CollisionManager is established and receives important permanent references
            CollisionManager.Crossbow = crossbow;

            // Set up UI Elements
            playHoverSprite = Content.Load<Texture2D>("PlayPressed");
            playPressedSprite = Content.Load<Texture2D>("PlayRegular");
            settingsHoverSprite = Content.Load<Texture2D>("SettingsRegular");
            settingsPressedSprite = Content.Load<Texture2D>("SettingsPressed");

            testText = new TextElement("A\nquick\nbrown\nfox\njumps\nover\nthe\nlazy\ndog",
                ScreenAnchor.Center, new Point(0, 0));

            FPSCounter = new TextElement("", ScreenAnchor.BottomRight, new Point(-10, -6));

            // Load menu background layers
            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                {
                    menuBGLayers[i] = new GameObject(menuBGSpriteList[i / 2], new Rectangle(0, 0, windowWidth + 10, windowHeight));
                }
                else
                {
                    menuBGLayers[i] = new GameObject(menuBGSpriteList[i / 2], new Rectangle(-(windowWidth + 10), 0, windowWidth + 10, windowHeight));
                }
            }

            // Play Button
            playButton = new Button(playHoverSprite, playPressedSprite, true,
                ScreenAnchor.Center, Point.Zero, playHoverSprite.Bounds.Size * new Point(2) / new Point(5));

            // Pause Button
            pauseButton = new Button(settingsPressedSprite, settingsHoverSprite, true,
                ScreenAnchor.TopRight, new Point(-14, 12), settingsHoverSprite.Bounds.Size / new Point(4));

            // Debug Button
            debugButton = new Button(settingsPressedSprite, settingsHoverSprite, true,
                ScreenAnchor.BottomRight, new Point(-16, -14), settingsHoverSprite.Bounds.Size * new Point(2) / new Point(7));

            // Game Over Button
            gameOverButton = new Button(playHoverSprite, playPressedSprite, true,
                ScreenAnchor.Center, new Point(0, 10), playHoverSprite.Bounds.Size * new Point(2) / new Point(5));

            // Upgrade Stuff
            upgradeButtons[0] =
                new Button(null, null, true, ScreenAnchor.BottomCenter, new Point(-30, 40), new Point(16));
            upgradeButtons[1] =
                new Button(null, null, true, ScreenAnchor.BottomCenter, new Point(0, 40), new Point(16));
            upgradeButtons[2] =
                new Button(null, null, true, ScreenAnchor.BottomCenter, new Point(30, 40), new Point(16));

            upgradeName = new TextElement("", ScreenAnchor.Center, new Point(0, -30), 1.5f);
            upgradeDescription = new TextElement("", ScreenAnchor.Center, new Point(-10));

            // Create player health bar
            for (int i = 0; i < DefaultPlayerHealth; i++)
            {
                playerHealthBar.Add(new UIElement(fullHeart, ScreenAnchor.TopLeft, new Point(12 + i * 20, 10), new Point(20)));
            }

            // Create crosshair
            crosshair = new UIElement(crosshairSprite, ScreenAnchor.TopLeft, Point.Zero, crosshairSprite.Bounds.Size / new Point(2))
            {
                DoesPositionScale = false
            };

            // Add all GameObjects to GameObject list
            gameObjectList.Add(player);
            gameObjectList.Add(crossbow);

            SpawnManager.GameObjectList = gameObjectList;
            LevelManager.LContent = Content;

            // Title Track starts playing
            MediaPlayer.Play(SoundManager.titleTheme);
            MediaPlayer.IsRepeating = true;


            OnResize();
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            kbState = Keyboard.GetState();
            mState = Mouse.GetState();
            
            // TEST CODE
            if(WasKeyPressed(Keys.F11))
                ToggleFullscreen();

            // Get the position of the mouse for the crosshair
            crosshair.Position = mState.Position.ToVector2();

            switch (gameState)
            {
                // Main Menu
                case GameState.MainMenu:
                    // Update
                    UpdateMainMenu(gameTime);

                    // Check state changes
                    if (playButton.HasBeenPressed())
                    {
                        LoadDefaultLevel();
                        gameState = GameState.Game;

                        // Stops current song
                        MediaPlayer.Stop();
                    }

                    break;

                // Game
                case GameState.Game:

                    // Update
                    UpdateGame(gameTime);

                    // --- Check state changes ---
                    // Game Over
                    if (player.CurrentHealth <= 0)
                        GameOver();

                    // Pause
                    pauseButton.Update(gameTime);
                    if (pauseButton.HasBeenPressed() ||
                        WasKeyPressed(Keys.Escape))
                        gameState = GameState.Pause;

                    break;

                // Upgrading
                case GameState.Upgrading:
                    UpdateUpgradeScreen(gameTime);
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
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            switch (gameState)
            {
                // Main Menu
                case GameState.MainMenu:
                    DrawMainMenu();

                    // TEST TEXT
                    //_spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                    //string testText = "a quick brown fox jumps over the lazy dog";
                    //Vector2 stringLength = pressStart.MeasureString(testText) * 2;
                    //_spriteBatch.DrawString(pressStart, testText, new Vector2(windowWidth / 2f - stringLength.X / 2, 700), Color.White, 0, Vector2.Zero, new Vector2(2), SpriteEffects.None, 1f);
                    //_spriteBatch.End();

                    break;

                // Game State
                case GameState.Game:
                    DrawGame();
                    DrawGameUI();

                    break;

                // Upgrading screen
                case GameState.Upgrading:
                    DrawUpgradeUI();
                    break;

                // Pause Menu
                case GameState.Pause:

                    // Draw the game behind the pause menu
                    DrawGame();
                    DrawPauseUI();
                    break;

                // Settings Menu - NOT IMPLEMENTED YET
                case GameState.Settings:

                    _spriteBatch.DrawString(arial32, "Settings", new Vector2(windowWidth - 175, windowHeight / 2f),
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

        // --- Update and draw methods ---

        // Main Menu
        /// <summary>
        /// Updates the main menu
        /// </summary>
        /// <param name="gameTime">A reference to the GameTime</param>
        private void UpdateMainMenu(GameTime gameTime)
        {
            AnimateMainMenuBG();

            playButton.Update(gameTime);

        }

        /// <summary>
        /// Animates the main menu with parallax
        /// </summary>
        private void AnimateMainMenuBG()
        {
            // Layer 1 is a blank image

            // Layer 2
            menuBGLayers[2].Position += new Vector2(0.1f * UIScale, 0);
            menuBGLayers[3].Position += new Vector2(0.1f * UIScale, 0);

            // Layer 3
            menuBGLayers[4].Position += new Vector2(0.2f * UIScale, 0);
            menuBGLayers[5].Position += new Vector2(0.2f * UIScale, 0);

            // Layer 4
            menuBGLayers[6].Position += new Vector2(0.3f * UIScale, 0);
            menuBGLayers[7].Position += new Vector2(0.3f * UIScale, 0);

            // Layer 5
            menuBGLayers[8].Position += new Vector2(0.4f * UIScale, 0);
            menuBGLayers[9].Position += new Vector2(0.4f * UIScale, 0);

            // Wrap image around the screen after it goes off the edge
            foreach (GameObject layer in menuBGLayers)
            {
                if (layer.Position.X > windowWidth)
                {
                    layer.Position -= new Vector2(menuBGLayers[0].Width * 2, 0);
                }
            }
        }

        /// <summary>
        /// Draws the main menu
        /// </summary>
        private void DrawMainMenu()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            GraphicsDevice.Clear(new Color(174, 222, 203));

            foreach (GameObject background in menuBGLayers)
            {
                background.Draw(_spriteBatch);
            }

            _spriteBatch.Draw(titleText, 
                Helper.MakeRectangleFromCenter(windowRect.Center - new Point(0, UIScale * 40), titleText.Bounds.Size * new Point(UIScale * 4)), 
                Color.White);

            playButton.Draw(_spriteBatch);

            crosshair.Draw(_spriteBatch);

            // TEST TEXT
            _spriteBatch.Draw(whiteSquareSprite, testText.Rectangle, Color.Tan);
            testText.Draw(_spriteBatch);

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
                    continue;
                }

                // Fires a skull's arrow if the cooldown time reaches 0.
                Skull skull;
                if ((skull = gameObjectList[i] as Skull) != null && skull.IsAlive
                    && skull.ReadyToFire)
                {
                    Arrow newTotemArrow = new Arrow(totemProjectileSprite,
                        new Rectangle(-100,
                                      -100,
                                      48,
                                      48),
                        new Vector2(0, 0));

                    CollisionManager.AddProjectile(newTotemArrow);
                    gameObjectList.Add(newTotemArrow);

                    skull.Shoot(newTotemArrow);
                }

                // Removes all inactive projectiles from play.
                Arrow arrow;
                if ((arrow = gameObjectList[i] as Arrow) != null && !arrow.IsActive)
                {
                    gameObjectList.RemoveAt(i);
                    i--;
                }

                // ~~~~~ DO ALL EXTERNAL GAMEOBJECT MODIFICATION ABOVE THIS CODE ~~~~~
                // Delete enemies from lists after they die
                Enemy enemy;
                if ((enemy = gameObjectList[i] as Enemy) != null && !enemy.IsAlive)
                {
                    gameObjectList.RemoveAt(i);
                    i--;
                }
                else if (!(gameObjectList[i] == player && (!player.CanMove && !player.InDodge)))
                {
                    if (!(gameObjectList[i] is Enemy) || player.CanMove || player.InDodge)
                    {
                        gameObjectList[i].Update(gameTime);
                    }
                }
            } // End of GameObject Update calls

            // Update collectibles
            foreach (Collectible collectible in collectibles)
            {
                collectible.Update(gameTime);
            }

            // Fires the bow on click.
            if (player.CanMove && mState.LeftButton == ButtonState.Pressed && previousMState.LeftButton == ButtonState.Released
                && !pauseButton.IsMouseOver())
            {
                crossbow.Shoot();
            }

            // Update all player arrows
            foreach (PlayerArrow playerArrow in playerArrowList)
            {
                playerArrow.Update(gameTime);
            }

            // CollisionManager checks for collisions
            CollisionManager.CheckCollision(isGodModeActive);

            // Update crossbow (Fixes crossbow moving off of player)
            crossbow.Update(gameTime);

            // ---- DEBUG UPDATE ----
            if (isDebugActive)
            {
                // Update FPS Counter
                float newFPS = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                // If FPS changes by more than 0.5, update the text
                if (Math.Abs(FPS - newFPS) > 0.5f)
                {
                    FPS = newFPS;
                    FPSCounter.Text = $"{FPS:F0}";
                }

                // Spawn slimes when pressing E
                if (WasKeyPressed(Keys.E))
                {
                    SpawnManager.SpawnSlime(MousePositionInGame().ToPoint());
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

                // TEST CODE TO SWITCH TO UPGRADE STATE
                if (WasKeyPressed(Keys.M))
                {
                    gameState = GameState.Upgrading;
                    
                }

                    // TEST CODE TO UNLOCK UPGRADES
                /*
                if (WasKeyPressed(Keys.M))
                    UpgradeManager.UnlockUpgrade("Multishot");
                if (WasKeyPressed(Keys.V))
                    UpgradeManager.UnlockUpgrade("Vampirism");*/

                isGodModeActive = true;
            }
            else
            {
                isGodModeActive = false;
            }
            

            /* DISABLED FOR PLAYTEST
             * 
            if (isDebugActive && WasKeyPressed(Keys.F))
                isGodModeActive = !isGodModeActive;
            if (!isDebugActive)
                isGodModeActive = false;
            */

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
        private void DrawGame()
        {
            // Make the graphics device render to the smaller target
            GraphicsDevice.SetRenderTarget(gameRenderTarget);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Camera.Matrix);

            // Change background color
            GraphicsDevice.Clear(Color.Black);

            // Draw Level
            LevelManager.Draw(_spriteBatch);

            // Draw all GameObjects
            foreach (GameObject gameObject in gameObjectList)
            {
                gameObject.Draw(_spriteBatch);
            }

            // Draw arrow
            foreach (PlayerArrow playerArrow in playerArrowList)
            {
                playerArrow.Draw(_spriteBatch);
            }

            // Draw all collectibles
            foreach (Collectible collectible in collectibles)
            {
                collectible.Draw(_spriteBatch);
            }

            // ---- DEBUG DRAW ----
            if (isDebugActive)
            {
                // Shows working hitboxes that don't use points
                CollisionManager.Draw(_spriteBatch, hitBox, arrowHitBox);

                // TEST CODE TO DRAW ARROW RECTANGLE
                // _spriteBatch.Draw(whiteSquareSprite, playerArrow.Rectangle, Color.Tan)
            }

            _spriteBatch.End();

            // Render the target to the backbuffer
            GraphicsDevice.SetRenderTarget(null);


            // Add Letterboxing
            // CODE TAKEN FROM: http://www.infinitespace-studios.co.uk/general/monogame-scaling-your-game-using-rendertargets-and-touchpanel/
            if (outputAspectRatio <= preferredAspectRatio)
            {
                // output is taller than it is wider, bars on top/bottom
                int presentHeight = (int)((Window.ClientBounds.Width / preferredAspectRatio) + 0.5f);
                int barHeight = (Window.ClientBounds.Height - presentHeight) / 2;
                gameTargetRect = new Rectangle(0, barHeight, Window.ClientBounds.Width, presentHeight);
            }
            else
            {
                // output is wider than it is tall, bars left/right
                int presentWidth = (int)((Window.ClientBounds.Height * preferredAspectRatio) + 0.5f);
                int barWidth = (Window.ClientBounds.Width - presentWidth) / 2;
                gameTargetRect = new Rectangle(barWidth, 0, presentWidth, Window.ClientBounds.Height);
            }

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(gameRenderTarget, gameTargetRect, Color.White);
            _spriteBatch.End();
        }

        /// <summary>
        /// Includes all of the Draw code for the GameState.Game UI
        /// </summary>
        private void DrawGameUI()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            pauseButton.Draw(_spriteBatch);

            for (int i = 0; i < playerHealthBar.Count; i++)
            {
                // Loop through all the health the player currently has
                if (i < player.CurrentHealth)
                {
                    // Set sprite to full heart if it is not
                    if (playerHealthBar[i].Sprite != fullHeart)
                    {
                        playerHealthBar[i].Sprite = fullHeart;
                    }

                    playerHealthBar[i].Draw(_spriteBatch);
                }

                // Loop through the remaining empty hearts
                else
                {
                    // Set sprite to empty heart if it is not
                    if (playerHealthBar[i].Sprite != emptyHeart)
                    {
                        playerHealthBar[i].Sprite = emptyHeart;
                    }
                    playerHealthBar[i].Draw(_spriteBatch);
                }
            }

            // Draw score text
            _spriteBatch.DrawString(pressStart, "Exp: " + exp, 
                new Vector2(playerHealthBar[0].Rectangle.Left, playerHealthBar[0].Rectangle.Bottom) + new Vector2(3 * UIScale),
                Color.White, 0, Vector2.Zero, new Vector2(UIScale), SpriteEffects.None, 1);


            // ---- DEBUG UI ----
            if (isDebugActive)
            {
                // ~~~ Draws the crossbow's timeSinceShot timer
                _spriteBatch.DrawString(arial32, "" + crossbow.TimeSinceShot, new Vector2(10, windowHeight - 50), Color.White);

                // Draws the crossbow's rotation
                _spriteBatch.DrawString(arial32, "" + crossbow.Direction, new Vector2(10, windowHeight - 100), Color.White);

                // Draws the FPS Counter
                FPSCounter.Draw(_spriteBatch);

               
            }

            crosshair.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        // Pause
        /// <summary>
        /// Includes all of the Draw code for the GameState.Pause UI
        /// </summary>
        private void DrawPauseUI()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            // Draw dark overlay over the game
            _spriteBatch.Draw(whiteSquareSprite, new Rectangle(Point.Zero, new Point(windowWidth, windowHeight)), new Color(Color.Black, 160));

            // Draw PAUSED text
            _spriteBatch.Draw(pauseText,
                Helper.MakeRectangleFromCenter(windowRect.Center - new Point(0, UIScale * 40), pauseText.Bounds.Size * new Point(UIScale * 4)),
                Color.White); ;
            
            // Draw play button
            playButton.Draw(_spriteBatch);

            // Draw debug button
            _spriteBatch.DrawString(arial32, isDebugActive ? "Disable God Mode:" : "Enable God Mode:", // Changed for Playtest
                new Vector2(windowWidth - 500, windowHeight - 80), isDebugActive ? Color.Red : Color.Green);
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

        // Choosing Upgrade
        private void UpdateUpgradeScreen(GameTime gameTime)
        {
            foreach (Button button in upgradeButtons)
            {
                button.Update(gameTime);
            }
        }

        private void DrawUpgradeUI()
        {
            _spriteBatch.Begin();

            // Draw dark overlay over the game
            _spriteBatch.Draw(whiteSquareSprite, new Rectangle(Point.Zero, new Point(windowWidth, windowHeight)), new Color(Color.Black, 160));

            // Draw buttons
            foreach (Button button in upgradeButtons)
            {
                button.Draw(_spriteBatch);
            }

            _spriteBatch.End();
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

                // Title Track starts playing
                MediaPlayer.Play(SoundManager.titleTheme);
                MediaPlayer.IsRepeating = true;
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

            _spriteBatch.Draw(gameOverText, 
                Helper.MakeRectangleFromCenter(windowRect.Center - new Point(0, UIScale * 40), gameOverText.Bounds.Size * new Point(UIScale * 4)), 
                Color.White);

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
                new Vector2(windowWidth - 175,
                    windowHeight / 2f), Color.White);

            crosshair.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        // Helper Methods
        /// <summary>
        /// Gets the coordinates of the mouse position in the game world
        /// </summary>
        public static Vector2 MousePositionInGame()
        {
            // SCREEN SPACE
            // Capture mouse position
            (double, double) output = (Mouse.GetState().Position.X, Mouse.GetState().Position.Y);

            // Offset mouse position by the black borders
            output.Item1 -= gameTargetRect.Location.X;
            output.Item2 -= gameTargetRect.Location.Y;

            // CAMERA SPACE
            // Scale mouse position by RenderTarget difference
            //     *uses tuples to avoid float division inaccuracy
            double scale = gameRenderTarget.Bounds.Size.X / (double)gameTargetRect.Size.X;
            output = (output.Item1 * scale, output.Item2 * scale);

            // Convert tuple back to vector
            Vector2 outputVector = new Vector2((float) output.Item1, (float) output.Item2);

            // Offset mouse position by camera
            outputVector = Vector2.Transform(outputVector, Matrix.Invert(Camera.Matrix));

            return outputVector;
        }

        /// <summary>
        /// Run this when the game should end
        /// </summary>
        public void GameOver()
        {
            // Sets the game state to Game Over
            gameState = GameState.GameOver;

            // Resets the player's stats and position, and resets the LevelManager
            player.ResetPlayer(new Rectangle(gameRenderTarget.Bounds.Center, new Point(48)));
            LevelManager.GameOver();
            CollisionManager.ClearEnemiesList();
            Collectibles.Clear();
            if (playerArrowList[0].IsActive)
            {
                playerArrowList[0].GetPickedUp();
                crossbow.PickUpArrow();
            }
            exp = 0;

            // Removes every non-Player and non-Crossbow object from the GameObject list
            for (int i = 0; i < gameObjectList.Count; i++)
            {
                if (!(gameObjectList[i] is Player) && !(gameObjectList[i] is CrossBow))
                {
                    gameObjectList.RemoveAt(i);
                    i--;
                }
            }

        }

        /// <summary>
        /// Checks if this was the first frame a key was pressed
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if the key was just pressed this frame but was not pressed last frame; false otherwise</returns>
        public static bool WasKeyPressed(Keys key)
        {
            return kbState.IsKeyDown(key) && previousKBState.IsKeyUp(key);
        }

        /// <summary>
        /// Checks if this was the first frame a key was released
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True if the key was just released this frame and was pressed last frame; false otherwise</returns>
        public static bool WasKeyReleased(Keys key)
        {
            return kbState.IsKeyUp(key) && previousKBState.IsKeyDown(key);
        }

        /// <summary>
        /// Moves and resizes most screen elements when the window is resized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnResize(Object sender = null, EventArgs e = null)
        {
            // Update windowWidth and windowHeight
            windowWidth = Window.ClientBounds.Width;
            windowHeight = Window.ClientBounds.Height;

            // Update windowRect
            windowRect = new Rectangle(0, 0, windowWidth, windowHeight);

            // Update Aspect Ratio
            outputAspectRatio = Window.ClientBounds.Width / (float)Window.ClientBounds.Height;

            // Update UI scale based on shorter side of window 
            if (outputAspectRatio >= preferredAspectRatio)
                UIScale = windowHeight / 225;
            else
                UIScale = windowWidth / 400;

            if (UIScale < 1)
                UIScale = 1;

            // Update the sizes and positions of all UI Elements
            foreach (UIElement element in UIElementsList)
            {
                element.OnResize();
            }

            // Update the sizes of all the background layers
            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                {
                    menuBGLayers[i].Position = Vector2.Zero;
                    menuBGLayers[i].Size = new Point(windowWidth + 10, windowHeight);
                }
                else
                {
                    menuBGLayers[i].Position = new Vector2(-(windowWidth + 10), 0);
                    menuBGLayers[i].Size = new Point(windowWidth + 10, windowHeight);
                }
            }
        }

        /// <summary>
        /// Toggles fullscreen
        /// </summary>
        public void ToggleFullscreen()
        {
            // Enable fullscreen and toggle  
            if (!_graphics.IsFullScreen)
            {
                _graphics.IsFullScreen = true;
                _graphics.HardwareModeSwitch = false;
                _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                _graphics.ApplyChanges();
            }
            else
            {
                _graphics.IsFullScreen = false;
                _graphics.HardwareModeSwitch = true;
                _graphics.PreferredBackBufferWidth = 1600;
                _graphics.PreferredBackBufferHeight = 900;
                _graphics.ApplyChanges();
            }

            OnResize();
        }

        /// <summary>
        /// Loads the starting level.
        /// </summary>
        public void LoadDefaultLevel()
        {
            // Level layout
            LevelManager.RandomizeLevel();
            LevelManager.LoadLevel();

            // Temp enemy spawns for starting level
            //SpawnManager.SpawnTotem(new Point(50, 100));
            //SpawnManager.SpawnTotem(new Point(64, 64));
            //SpawnManager.SpawnSkeleton(new Point(400, 400));
            //SpawnManager.SpawnTarget(new Point(64, 64));
        }
    }

    public enum GameState
    {
        MainMenu,
        Game,
        Upgrading,
        Settings,
        Credits,
        Pause,
        GameOver,
    }
}
