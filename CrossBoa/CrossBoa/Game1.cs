using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
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

        public static bool boolThatAlternatesEveryFrame;

        private static KeyboardState kbState;
        private static KeyboardState previousKBState;
        public static MouseState MState;
        public static MouseState PreviousMState;

        // Assets
        #region Asset Field Declarations
        public static Texture2D whiteSquareSprite;
        public static Texture2D blackSquareSprite;
        public static Texture2D skullSpriteSheet;
        public static Texture2D beastSprite;
        public static Texture2D playerArrowSprite;
        public static Texture2D fireballSpritesheet;
        public static Texture2D slimeSpritesheet;
        public static Texture2D slimeDeathSpritesheet;
        public static Texture2D xpSprite;
        public static Texture2D targetSprite;
        public static Texture2D healthRecoverySprite;
        public static Texture2D leftRightDoorSprite;
        public static Texture2D bottomTopDoorSprite;
        public static Texture2D floorSprite;
        public static Texture2D sideWallSprite;
        public static Texture2D wallSprite;
        public static Texture2D topDoorBottomHalfSprite;
        public static Texture2D topExitBottomHalfSprite;

        private Texture2D snakeSpriteSheet;
        private Texture2D crossbowSprite;
        private Texture2D hitBox;
        private Texture2D arrowHitBox;
        private Texture2D playHoverSprite;
        private Texture2D playPressedSprite;
        private Texture2D settingsHoverSprite;
        private Texture2D settingsPressedSprite;
        private Texture2D creditsHoverSprite;
        private Texture2D creditsPressedSprite;
        private Texture2D mainMenuHoverSprite;
        private Texture2D mainMenuPressedSprite;
        private Texture2D playAgainHoverSprite;
        private Texture2D playAgainPressedSprite;
        private Texture2D pauseHoverSprite;
        private Texture2D pausePressedSprite;
        private Texture2D checkboxFilled;
        private Texture2D checkboxUnfilled;
        private Texture2D emptyHeart;
        private Texture2D fullHeart;
        private Texture2D menuBGSheet;
        private Texture2D pauseText;
        private Texture2D collectibleSprite;
        private Texture2D crosshairSprite;
        private Texture2D flashSprite;

        public static Texture2D UpgradeBloodOrb;

        private SpriteFont arial32;
        private static SpriteFont pressStart;
        #endregion

        // Objects
        private Rectangle[] menuBGLayers;
        private List<UIElement> playerHealthBar;
        public static List<UIElement> UIElementsList;
        private UIElement crosshair;
        private static CrossBow crossbow;
        private static Player player;
        public static List<PlayerArrow> playerArrowList;
        private static List<Collectible> collectibles;

        RenderTarget2D menuBGTarget;

        // Text Elements
        private TextElement splashText;
        private TextElement titleText;
        private TextElement gameOverText;
        private TextElement FPSCounter;
        private TextElement settingsTitle;
        private TextElement controlsTitle;
        private TextElement optionsTitle;
        private TextElement debugText;
        private TextElement godModeText;
        private TextElement settingsTextLine1;
        private TextElement settingsTextLine2;
        private TextElement settingsTextLine3;
        private TextElement creditsTitle;
        private TextElement developersText;
        private TextElement developersTitle;
        private TextElement thanksTitle;
        private TextElement thanksText;
        private TextElement specialThanksTitle;
        private TextElement specialThanksText;

        // Buttons
        private Button playButton;
        private Button playAgainButton;
        private Button creditsButton;
        private Button settingsButton;
        private Button mainMenuButton;
        private Button pauseButton;
        private Button debugButton;
        private Button godModeButton;
        private Button gameOverButton;
        private Button[] upgradeButtons;

        // Stuff for Upgrade State
        private TextElement levelUpText;
        private TextElement selectAnUpgradeText;
        private TextElement upgradeName;
        private TextElement upgradeDescription;
        private Upgrade[] upgradeChoices;
        private int prevUpgradeButtonHovered;

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

            _graphics.SynchronizeWithVerticalRetrace = false;

            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            gameObjectList = new List<GameObject>();
            menuBGLayers = new Rectangle[8];
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
            menuBGTarget = new RenderTarget2D(GraphicsDevice, 1920, 1080);
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
            skullSpriteSheet = Content.Load<Texture2D>("TotemSpriteSheet");
            beastSprite = Content.Load<Texture2D>("BeastSpriteSheet");
            slimeSpritesheet = Content.Load<Texture2D>("FacelessSlimeSpritesheet");
            slimeDeathSpritesheet = Content.Load<Texture2D>("FacelessSlimeDeathSpritesheet-sheet");
            xpSprite = Content.Load<Texture2D>("XPOrb");
            emptyHeart = Content.Load<Texture2D>("Empty Heart");
            fullHeart = Content.Load<Texture2D>("Full Heart");
            snakeSpriteSheet = Content.Load<Texture2D>("SnakeSpritesheet");
            crossbowSprite = Content.Load<Texture2D>("Crossbow");
            hitBox = Content.Load<Texture2D>("Hitbox");
            arrowHitBox = Content.Load<Texture2D>("White Pixel");
            playerArrowSprite = Content.Load<Texture2D>("arrow2");
            fireballSpritesheet = Content.Load<Texture2D>("FireballSpriteSheet");
            pauseText = Content.Load<Texture2D>("PauseText");
            collectibleSprite = Content.Load<Texture2D>("LifePot");
            crosshairSprite = Content.Load<Texture2D>("Crosshair");
            menuBGSheet = Content.Load<Texture2D>("bg-sheet");
            flashSprite = Content.Load<Texture2D>("RecoveryFlash");
            healthRecoverySprite = Content.Load<Texture2D>("HealthCollectible");
            targetSprite = Content.Load<Texture2D>("TargetSprite");
            leftRightDoorSprite = Content.Load<Texture2D>("VeryCrackedWallLeftRight");
            bottomTopDoorSprite = Content.Load<Texture2D>("VeryCrackedWallBottomTop");
            floorSprite = Content.Load<Texture2D>("Floor");
            sideWallSprite = Content.Load<Texture2D>("SideWall");
            wallSprite = Content.Load<Texture2D>("Wall");
            topDoorBottomHalfSprite = Content.Load<Texture2D>("Wall2");
            topExitBottomHalfSprite = Content.Load<Texture2D>("VeryCrackedWallBottomHalf");
            checkboxFilled = Content.Load<Texture2D>("CheckboxFilled");
            checkboxUnfilled = Content.Load<Texture2D>("CheckboxUnfilled");
            blackSquareSprite = Content.Load<Texture2D>("Shadow");

            // Upgrade sprites
            UpgradeBloodOrb = Content.Load<Texture2D>("Upgrade_BloodOrb");

            arial32 = Content.Load<SpriteFont>("Arial32");
            pressStart = Content.Load<SpriteFont>("Fonts/PressStart6");

            // Load Player
            player = new Player(
                snakeSpriteSheet,
                new Rectangle(gameRenderTarget.Bounds.Center, new Point(48)),
                DefaultPlayerMovementForce,
                DefaultPlayerMaxSpeed,
                DefaultPlayerFriction,
                DefaultPlayerHealth,
                DefaultPlayerDodgeCooldown,
                DefaultPlayerDodgeLength,
                DefaultPlayerDodgeSpeed,
                flashSprite
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
            mainMenuHoverSprite = Content.Load<Texture2D>("MainMenuReguar");
            mainMenuPressedSprite = Content.Load<Texture2D>("MainMenuPressed");
            pauseHoverSprite = Content.Load<Texture2D>("PauseRegular");
            pausePressedSprite = Content.Load<Texture2D>("PausePressed");
            creditsHoverSprite = Content.Load<Texture2D>("CreditsRegular");
            creditsPressedSprite = Content.Load<Texture2D>("CreditsPressed");
            playAgainHoverSprite = Content.Load<Texture2D>("PlayAgainRegular");
            playAgainPressedSprite = Content.Load<Texture2D>("PlayAgainPressed");

            //splashText = new TextElement("A quick brown fox\njumps over the lazy dog", ScreenAnchor.Center, new Point(0, 50));

            FPSCounter = new TextElement("", ScreenAnchor.BottomRight, new Point(-10, -6));

            titleText = new TextElement("CROSSBOA", ScreenAnchor.TopCenter, new Point(0, 45), 4f);

            gameOverText = new TextElement("GAME OVER", ScreenAnchor.TopCenter, new Point(0, 45), 4f);

            settingsTitle = new TextElement("SETTINGS", ScreenAnchor.TopCenter, new Point(0, 20), 3f);

            controlsTitle = new TextElement("CONTROLS", ScreenAnchor.LeftCenter, new Point(65, -40), 1.5f);

            settingsTextLine1 = new TextElement("WASD to Move", ScreenAnchor.LeftCenter, new Point(75, -10), 1.2f);
            settingsTextLine2 = new TextElement("Left Click to Shoot", ScreenAnchor.LeftCenter, new Point(109, 10), 1.2f);
            settingsTextLine3 = new TextElement("Spacebar to Dodge", ScreenAnchor.LeftCenter, new Point(99, 30), 1.2f);

            optionsTitle = new TextElement("OPTIONS", ScreenAnchor.RightCenter, new Point(-65, -40), 1.5f);

            debugText = new TextElement("Debug Mode:", ScreenAnchor.RightCenter, new Point(-95, -10), 1.25f);

            godModeText = new TextElement("God Mode:", ScreenAnchor.RightCenter, new Point(-85, 5), 1.25f);

            creditsTitle = new TextElement("CREDITS", ScreenAnchor.TopCenter, new Point(0, 20), 3f);

            developersTitle = new TextElement("DEVELOPERS", ScreenAnchor.TopCenter, new Point(0, 55), 1.5f);

            developersText = new TextElement("Justin Baez\n\nIan Knecht\n\nLeo Schindler-Gerendasi\n\nDonovan Scullion",
                ScreenAnchor.TopCenter, new Point(0, 100), 1.2f);

            thanksTitle = new TextElement("THANKS TO", ScreenAnchor.TopCenter, new Point(0, 155), 1.5f);

            thanksText = new TextElement("adwitr\n\nansimuz\n\nGameSupplyGuy\n\nIrmandito\n\nJesse Munguia\n\nPixel Archipel\n\nReff Pixels\n\nzrghr",
                ScreenAnchor.TopCenter, new Point(0, 230), 1.2f);

            specialThanksTitle = new TextElement("SPECIAL THANKS TO", ScreenAnchor.TopCenter, new Point(0, 330), 1.5f);

            specialThanksText = new TextElement("Erin Cascioli\n\nRyan Keller\n\nRyan Ress\n\nOur Playtesters\n\nAnd You!",
                ScreenAnchor.TopCenter, new Point(0, 385), 1.2f);

            // Load menu background layers
            Point bgSize = new Point(1920, 1080);
            for (int i = 0; i < 8; i++)
            {
                menuBGLayers[i] = new Rectangle(new Point(
                    i % 2 == 0 ? 0 : -bgSize.X,   // Every other layer is placed 800 pixels to the left
                    0),
                    bgSize);
            }

            // Play Button
            playButton = new Button(playHoverSprite, playPressedSprite, true,
                ScreenAnchor.Center, Point.Zero, playHoverSprite.Bounds.Size * new Point(2) / new Point(5));

            // Pause Button
            pauseButton = new Button(pausePressedSprite, pauseHoverSprite, true,
                ScreenAnchor.TopRight, new Point(-14, 12), pauseHoverSprite.Bounds.Size / new Point(4));

            // Credits Button
            creditsButton = new Button(creditsPressedSprite, creditsHoverSprite, true,
                ScreenAnchor.Center, new Point(0, 60), creditsHoverSprite.Bounds.Size * new Point(7) / new Point(20));

            // Settings Button
            settingsButton = new Button(settingsPressedSprite, settingsHoverSprite, true,
                ScreenAnchor.Center, new Point(0, 30), settingsHoverSprite.Bounds.Size * new Point(7) / new Point(20));

            // Main Menu Button
            mainMenuButton = new Button(mainMenuPressedSprite, mainMenuHoverSprite, true,
                ScreenAnchor.BottomLeft, new Point(37, -15), mainMenuHoverSprite.Bounds.Size * new Point(7) / new Point(20));

            // Play Again Button
            playAgainButton = new Button(playAgainPressedSprite, playAgainHoverSprite, true,
                ScreenAnchor.Center, Point.Zero, playAgainHoverSprite.Bounds.Size * new Point(7) / new Point(20));

            // Debug Button
            debugButton = new Button(checkboxUnfilled, checkboxUnfilled, true,
                ScreenAnchor.RightCenter, new Point(-30, -5), checkboxUnfilled.Bounds.Size * new Point(2,2));

            debugButton.OnClick += ToggleDebug;

            // God mode Button
            godModeButton = new Button(checkboxUnfilled, checkboxUnfilled, true,
                ScreenAnchor.RightCenter, new Point(-30, 10), checkboxUnfilled.Bounds.Size * new Point(2, 2));

            godModeButton.OnClick += ToggleGodMode;

            // Game Over Button
            gameOverButton = new Button(mainMenuPressedSprite, mainMenuHoverSprite, true,
                ScreenAnchor.Center, new Point(0, 30), mainMenuHoverSprite.Bounds.Size * new Point(7) / new Point(20));

            // Upgrade UI Stuff
            levelUpText = new TextElement("LEVEL UP!", ScreenAnchor.TopCenter, new Point(0, 40), 2f);
            selectAnUpgradeText = new TextElement("Select an upgrade", ScreenAnchor.TopCenter, new Point(0, 60), 1.5f);

            upgradeName = new TextElement("", ScreenAnchor.BottomCenter, new Point(0, -70), 1.5f);
            upgradeDescription = new TextElement("", ScreenAnchor.BottomCenter, new Point(0, -50));

            upgradeButtons[0] =
                new Button(null, null, true, ScreenAnchor.Center, new Point(-65, 0), new Point(32));
            upgradeButtons[1] =
                new Button(null, null, true, ScreenAnchor.Center, new Point(0, 0), new Point(32));
            upgradeButtons[2] =
                new Button(null, null, true, ScreenAnchor.Center, new Point(65, 0), new Point(32));

            // Create player health bar
            for (int i = 0; i < DefaultPlayerHealth; i++)
            {
                playerHealthBar.Add(new UIElement(fullHeart, ScreenAnchor.TopLeft, new Point(12 + i * 20, 10), new Point(20)));
            }

            // Create crosshair
            crosshair = new UIElement(crosshairSprite, ScreenAnchor.TopLeft, Point.Zero, crosshairSprite.Bounds.Size)
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
            MState = Mouse.GetState();

            boolThatAlternatesEveryFrame = !boolThatAlternatesEveryFrame;

            // Check for fullscreen press
            if (WasKeyPressed(Keys.F11))
                ToggleFullscreen();

            // Get the position of the mouse for the d
            crosshair.Position = MState.Position.ToVector2();

            switch (gameState)
            {
                // Main Menu
                case GameState.MainMenu:
                    // Update
                    UpdateMainMenu(gameTime);

                    // Start game
                    if (playButton.HasBeenPressed())
                    {
                        LoadDefaultLevel();
                        gameState = GameState.Game;

                        // Reposition Debug and God Mode buttons for pause menu
                        debugText.Anchor = ScreenAnchor.RightCenter;

                        debugButton.Position = new Vector2(-12, -32);
                        debugButton.Anchor = ScreenAnchor.BottomRight;

                        debugText.Position = new Vector2(-77, -37);
                        debugText.Anchor = ScreenAnchor.BottomRight;

                        godModeButton.Position = new Vector2(-12, -12);
                        godModeButton.Anchor = ScreenAnchor.BottomRight;

                        godModeText.Position = new Vector2(-67, -17);
                        godModeText.Anchor = ScreenAnchor.BottomRight;

                        // Stops current song
                        MediaPlayer.Stop();
                    }

                    // Go to credits
                    if (creditsButton.HasBeenPressed())
                    {
                        ResetCredits();
                        gameState = GameState.Credits;
                    }

                    // Go to settings
                    if (settingsButton.HasBeenPressed())
                    {
                        gameState = GameState.Settings;

                        // Reposition debug buttons
                        debugButton.Position = new Vector2(-30, -5);
                        debugButton.Anchor = ScreenAnchor.RightCenter;

                        debugText.Position = new Vector2(-95, -10);
                        debugText.Anchor = ScreenAnchor.RightCenter;

                        godModeButton.Position = new Vector2(-30, 20);
                        godModeButton.Anchor = ScreenAnchor.RightCenter;

                        godModeText.Position = new Vector2(-85, 15);
                        godModeText.Anchor = ScreenAnchor.RightCenter;
                    }

                    break;

                // Game
                case GameState.Game:

                    // Update
                    UpdateGame(gameTime);

                    // --- Check state changes ---
                    // Upgrade
                    // TEMPORARY TEST CODE TO SWITCH TO UPGRADE STATE
                    if (WasKeyPressed(Keys.M))
                    {
                        gameState = GameState.Upgrading;
                        DisplayUpgradeChoices();
                    }

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
                    UpdateSettings(gameTime);
                    break;

                // Credits
                case GameState.Credits:

                    UpdateCredits(gameTime);

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
            PreviousMState = MState;

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

                    break;

                // Game State
                case GameState.Game:
                    DrawGame();
                    DrawGameUI();

                    break;

                // Upgrading screen
                case GameState.Upgrading:
                    DrawGame();
                    DrawUpgradeUI();
                    break;

                // Pause Menu
                case GameState.Pause:

                    // Draw the game behind the pause menu
                    DrawGame();
                    DrawPauseUI();
                    break;

                // Settings Menu 
                case GameState.Settings:
                    DrawSettings();
                   
                    break;

                // Credits menu
                case GameState.Credits:
                    DrawCredits();
                    break;

                // Game Over
                case GameState.GameOver:
                    DrawGameOver();
                    break;
            }

            // Draw the crosshair
            _spriteBatch.Begin(samplerState: SamplerState.PointWrap);
            crosshair.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        // Main Menu
        /// <summary>
        /// Updates the main menu
        /// </summary>
        /// <param name="gameTime">A reference to the GameTime</param>
        private void UpdateMainMenu(GameTime gameTime)
        {
            AnimateMainMenuBG(gameTime, false);

            playButton.Update(gameTime);

            settingsButton.Update(gameTime);

            creditsButton.Update(gameTime);
        }

        /// <summary>
        /// Animates the main menu with parallax
        /// </summary>
        private void AnimateMainMenuBG(GameTime gameTime, bool isHalfSpeed)
        {
            int frame = (int)(gameTime.TotalGameTime.TotalSeconds * 60);

            if(!isHalfSpeed)
            {
                // Layer 1
                if (frame % 3 == 0)
                {
                    menuBGLayers[0].Location += new Point(1, 0);
                    menuBGLayers[1].Location += new Point(1, 0);
                }

                // Layer 2
                if (frame % 2 == 0)
                {
                    menuBGLayers[2].Location += new Point(1, 0);
                    menuBGLayers[3].Location += new Point(1, 0);
                }

                // Layer 3
                if (frame % 3 == 0 || frame % 3 == 1)
                {
                    menuBGLayers[4].Location += new Point(1, 0);
                    menuBGLayers[5].Location += new Point(1, 0);
                }

                // Layer 4
                if (frame % 1 == 0)
                {
                    menuBGLayers[6].Location += new Point(1, 0);
                    menuBGLayers[7].Location += new Point(1, 0);
                }
            }
            else
            {
                // Layer 1
                if (frame % 6 == 0)
                {
                    menuBGLayers[0].Location += new Point(1, 0);
                    menuBGLayers[1].Location += new Point(1, 0);
                }

                // Layer 2
                if (frame % 4 == 0)
                {
                    menuBGLayers[2].Location += new Point(1, 0);
                    menuBGLayers[3].Location += new Point(1, 0);
                }

                // Layer 3
                if (frame % 3 == 0)
                {
                    menuBGLayers[4].Location += new Point(1, 0);
                    menuBGLayers[5].Location += new Point(1, 0);
                }

                // Layer 4
                if (frame % 2 == 0)
                {
                    menuBGLayers[6].Location += new Point(1, 0);
                    menuBGLayers[7].Location += new Point(1, 0);
                }
            }

            // Wrap image around the screen after it goes off the edge
            for (int i = 0; i < menuBGLayers.Length; i++)
            {
                if (menuBGLayers[i].Location.X > menuBGLayers[0].Width)
                {
                    menuBGLayers[i].Location -= new Point(menuBGLayers[0].Width * 2, 0);
                }
            }
        }

        /// <summary>
        /// Draws the main menu
        /// </summary>
        private void DrawMainMenu()
        {
            // Draw main menu background to a smaller target, then scale up to reduce lag
            GraphicsDevice.SetRenderTarget(menuBGTarget);
            GraphicsDevice.Clear(new Color(174, 222, 203));
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            for (int i = 0; i < 6; i++)
            {
                _spriteBatch.Draw(menuBGSheet, menuBGLayers[i], new Rectangle((i / 2) * 384, 0, 384, 216), Color.White);
            }

            _spriteBatch.End();
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            for (int i = 6; i < 8; i++)
            {
                _spriteBatch.Draw(menuBGSheet, menuBGLayers[i], new Rectangle((i / 2) * 384, 0, 384, 216), Color.White);
            }

            _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            // Draw the main menu
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            // Determine background size
            Point menuBGSize;

            if (outputAspectRatio <= preferredAspectRatio)
            {
                // output is taller than it is wider, set size to window height
                menuBGSize = new Point((int) MathF.Round(windowHeight * preferredAspectRatio), windowHeight);
            }
            else
            {
                // output is wider than it is tall, set size to window width
                menuBGSize = new Point(windowWidth, (int) MathF.Round(windowWidth / preferredAspectRatio));
            }
            
            // Draw background
            _spriteBatch.Draw(menuBGTarget, new Rectangle(Point.Zero, menuBGSize), Color.White);

            // Title Text
            titleText.Draw(_spriteBatch);

            playButton.Draw(_spriteBatch);

            settingsButton.Draw(_spriteBatch);

            creditsButton.Draw(_spriteBatch);

            // Splash Text
            //splashText.Draw(_spriteBatch);

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
            Camera.Update(gameTime);

            for (int i = 0; i < gameObjectList.Count; i++)
            {
                // Fixes crossbow moving off of player character
                if (gameObjectList[i] is CrossBow)
                {
                    continue;
                }

                // Fires a skull's arrow if the cooldown time reaches 0.
                if (gameObjectList[i] is Skull {IsAlive: true, ReadyToFire: true} skull)
                {
                    Projectile newTotemProjectile = new Projectile(fireballSpritesheet,
                        new Rectangle(-100,
                                      -100,
                                      48,
                                      48),
                        new Vector2(0, 0));

                    CollisionManager.AddProjectile(newTotemProjectile);
                    gameObjectList.Add(newTotemProjectile);

                    skull.Shoot(newTotemProjectile);
                }

                // Removes all inactive projectiles from play.
                if (gameObjectList[i] is Projectile {IsActive: false})
                {
                    gameObjectList.RemoveAt(i);
                    i--;
                }

                // ~~~~~ DO ALL EXTERNAL GAMEOBJECT MODIFICATION ABOVE THIS CODE ~~~~~
                // Delete enemies from lists after they die
                if (gameObjectList[i] is Enemy {IsAlive: false})
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
            if (player.CanMove && MState.LeftButton == ButtonState.Pressed && PreviousMState.LeftButton == ButtonState.Released
                && !pauseButton.IsMouseOver())
            {
                crossbow.Shoot();
            }

            // Update all player arrows
            for (int i = 0; i < playerArrowList.Count; i++)
            {
                playerArrowList[i].Update(gameTime);
                if (playerArrowList[i].FlaggedForDeletion)
                {
                    playerArrowList.Remove(playerArrowList[i]);
                    i--;
                }
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
                        if (e is Enemy enemy)
                        {
                            enemy.TakeDamage(1000);
                        }
                    }
                }

                // TEST CODE TO UNLOCK UPGRADES
                /*
                if (WasKeyPressed(Keys.M))
                    UpgradeManager.UnlockUpgrade("Multishot");
                if (WasKeyPressed(Keys.V))
                    UpgradeManager.UnlockUpgrade("Vampirism");*/
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

            _spriteBatch.End();
        }

        // Settings
        private void UpdateSettings(GameTime gameTime)
        {
            AnimateMainMenuBG(gameTime, false);
            mainMenuButton.Update(gameTime);
            debugButton.Update(gameTime);
            godModeButton.Update(gameTime);

            if (mainMenuButton.HasBeenPressed())
            {
                gameState = GameState.MainMenu;
            }
        }

        /// <summary>
        /// Toggles the debug mode
        /// </summary>
        private void ToggleDebug()
        {
            if (!isDebugActive)
            {
                isDebugActive = true;
                debugButton.Sprite = checkboxFilled;
                debugButton.HoverTexture = checkboxFilled;
            }
            else
            {
                isDebugActive = false;
                debugButton.Sprite = checkboxUnfilled;
                debugButton.HoverTexture = checkboxUnfilled;
            }
        }

        /// <summary>
        /// Toggles godmode
        /// </summary>
        private void ToggleGodMode()
        {
            if (!isGodModeActive)
            {
                isGodModeActive = true;
                godModeButton.Sprite = checkboxFilled;
                godModeButton.HoverTexture = checkboxFilled;
            }
            else
            {
                isGodModeActive = false;
                godModeButton.Sprite = checkboxUnfilled;
                godModeButton.HoverTexture = checkboxUnfilled;
            }
        }

        private void DrawSettings()
        {
            // Draw main menu background to a smaller target, then scale up to reduce lag
            GraphicsDevice.SetRenderTarget(menuBGTarget);
            GraphicsDevice.Clear(new Color(174, 222, 203));
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            for (int i = 0; i < 6; i++)
            {
                _spriteBatch.Draw(menuBGSheet, menuBGLayers[i], new Rectangle((i / 2) * 384, 0, 384, 216), Color.White);
            }

            _spriteBatch.End();
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            for (int i = 6; i < 8; i++)
            {
                _spriteBatch.Draw(menuBGSheet, menuBGLayers[i], new Rectangle((i / 2) * 384, 0, 384, 216), Color.White);
            }

            _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            // Draw the main menu
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            // Determine background size
            Point menuBGSize;

            if (outputAspectRatio <= preferredAspectRatio)
            {
                // output is taller than it is wider, set size to window height
                menuBGSize = new Point((int)MathF.Round(windowHeight * preferredAspectRatio), windowHeight);
            }
            else
            {
                // output is wider than it is tall, set size to window width
                menuBGSize = new Point(windowWidth, (int)MathF.Round(windowWidth / preferredAspectRatio));
            }

            // Draw background
            _spriteBatch.Draw(menuBGTarget, new Rectangle(Point.Zero, menuBGSize), Color.White);

            settingsTitle.Draw(_spriteBatch);

            settingsTextLine1.Draw(_spriteBatch);
            settingsTextLine2.Draw(_spriteBatch);
            settingsTextLine3.Draw(_spriteBatch);

            controlsTitle.Draw(_spriteBatch);
            optionsTitle.Draw(_spriteBatch);

            mainMenuButton.Draw(_spriteBatch);
            debugText.Draw(_spriteBatch);
            debugButton.Draw(_spriteBatch);

            godModeText.Draw(_spriteBatch);
            godModeButton.Draw(_spriteBatch);

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
                Color.White);
            
            // Draw play button
            playButton.Draw(_spriteBatch);

            mainMenuButton.Draw(_spriteBatch);

            debugButton.Draw(_spriteBatch);
            debugText.Draw(_spriteBatch);
            godModeText.Draw(_spriteBatch);
            godModeButton.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        /// <summary>
        /// Update Pause UI
        /// </summary>
        /// <param name="gameTime"> Gametime </param>
        private void UpdatePauseMenu(GameTime gameTime)
        {
            playButton.Update(gameTime);
            mainMenuButton.Update(gameTime);
            debugButton.Update(gameTime);
            godModeButton.Update(gameTime);

            // Resume if player presses pause key or escape
            if (playButton.HasBeenPressed() ||
                WasKeyPressed(Keys.Escape))
                gameState = GameState.Game;

            if (mainMenuButton.HasBeenPressed())
            {
                GameOver();
                gameState = GameState.MainMenu;
            }
        }

        // Upgrades
        /// <summary>
        /// Updates upgrade screen
        /// </summary>
        /// <param name="gameTime"> Gametime </param>
        private void UpdateUpgradeScreen(GameTime gameTime)
        {
            for (int i = 0; i < upgradeButtons.Length; i++)
            {
                upgradeButtons[i].Update(gameTime);
                
                // If the player hovers over a button, display that upgrade's text
                if (upgradeButtons[i].IsMouseOver() && prevUpgradeButtonHovered != i)
                {
                    prevUpgradeButtonHovered = i;
                    upgradeName.Text = upgradeChoices[i].Name;
                    upgradeDescription.Text = upgradeChoices[i].Description;
                }

                // If the player clicks on an upgrade, unlock it
                if (upgradeButtons[i].HasBeenPressed())
                {
                    UpgradeManager.UnlockUpgrade(upgradeChoices[i].Name);
                    prevUpgradeButtonHovered = -1;
                    upgradeName.Text = "";
                    upgradeDescription.Text = "";
                    gameState = GameState.Game;
                }
            }
        }

        /// <summary>
        /// Draws upgrade UI
        /// </summary>
        private void DrawUpgradeUI()
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Draw dark overlay over the game
            _spriteBatch.Draw(whiteSquareSprite, new Rectangle(Point.Zero, new Point(windowWidth, windowHeight)), new Color(Color.Black, 160));

            // Draw buttons
            foreach (Button button in upgradeButtons)
            {
                button.Draw(_spriteBatch);
            }

            // Draw upgrade text
            levelUpText.Draw(_spriteBatch);
            selectAnUpgradeText.Draw(_spriteBatch);

            upgradeName.Draw(_spriteBatch);
            upgradeDescription.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        // Game Over
        /// <summary>
        /// Updates the game over screen
        /// </summary>
        /// <param name="gameTime"> Gametime </param>
        private void UpdateGameOver(GameTime gameTime)
        {
            AnimateMainMenuBG(gameTime, true);

            gameOverButton.Update(gameTime);

            playAgainButton.Update(gameTime);

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

            if (playAgainButton.HasBeenPressed())
            {
                LoadDefaultLevel();

                gameState = GameState.Game;

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
            GraphicsDevice.Clear(new Color(174, 222, 203));

            // Draw main menu background to a smaller target, then scale up to reduce lag
            GraphicsDevice.SetRenderTarget(menuBGTarget);
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            for (int i = 0; i < 8; i++)
            {
                _spriteBatch.Draw(menuBGSheet, menuBGLayers[i], new Rectangle((i / 2) * 384, 0, 384, 216), new Color(88, 73, 180));
            }

            _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            // Draw the main menu
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            // Determine background size
            Point menuBGSize;
            if (outputAspectRatio <= preferredAspectRatio)
            {
                // output is taller than it is wider, set size to window height
                menuBGSize = new Point((int)MathF.Round(windowHeight * preferredAspectRatio), windowHeight);
            }
            else
            {
                // output is wider than it is tall, set size to window width
                menuBGSize = new Point(windowWidth, (int)MathF.Round(windowWidth / preferredAspectRatio));
            }

            // Draw background
            _spriteBatch.Draw(menuBGTarget, new Rectangle(Point.Zero, menuBGSize), Color.White);

            gameOverText.Draw(_spriteBatch);

            gameOverButton.Draw(_spriteBatch);

            playAgainButton.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        // Credits
        /// <summary>
        /// Updates the credits
        /// </summary>
        private void UpdateCredits(GameTime gameTime)
        {
            AnimateMainMenuBG(gameTime, false);

            // Move Credits Text Up
            creditsTitle.Position += new Vector2(creditsTitle.Position.X, -0.25f);
            developersTitle.Position += new Vector2(developersTitle.Position.X, -0.25f);
            developersText.Position += new Vector2(developersText.Position.X, -0.25f);
            thanksTitle.Position += new Vector2(thanksText.Position.X, -0.25f);
            thanksText.Position += new Vector2(thanksText.Position.X, -0.25f);
            specialThanksTitle.Position += new Vector2(specialThanksTitle.Position.X, -0.25f);
            specialThanksText.Position += new Vector2(specialThanksText.Position.X, -0.25f);

            mainMenuButton.Update(gameTime);

            // Check if the main menu button has been pressed
            if (mainMenuButton.HasBeenPressed())
            {
                gameState = GameState.MainMenu;
            }
        }

        /// <summary>
        /// Resets the credits position
        /// </summary>
        private void ResetCredits()
        {
            // Set the credits text to its original position
            creditsTitle.Position = new Vector2(0, 20);
            developersTitle.Position = new Vector2(0, 75);
            developersText.Position = new Vector2(0, 100);
            thanksTitle.Position = new Vector2(0, 205);
            thanksText.Position = new Vector2(0, 230);
            specialThanksTitle.Position = new Vector2(0, 400);
            specialThanksText.Position = new Vector2(0, 425);
            
        }

        /// <summary>
        /// Draws the credits screen
        /// </summary>
        private void DrawCredits()
        {
            // Draw main menu background to a smaller target, then scale up to reduce lag
            GraphicsDevice.SetRenderTarget(menuBGTarget);
            GraphicsDevice.Clear(new Color(174, 222, 203));
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            for (int i = 0; i < 6; i++)
            {
                _spriteBatch.Draw(menuBGSheet, menuBGLayers[i], new Rectangle((i / 2) * 384, 0, 384, 216), Color.White);
            }

            _spriteBatch.End();
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            for (int i = 6; i < 8; i++)
            {
                _spriteBatch.Draw(menuBGSheet, menuBGLayers[i], new Rectangle((i / 2) * 384, 0, 384, 216), Color.White);
            }

            _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            // Draw the main menu
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            // Determine background size
            Point menuBGSize;

            if (outputAspectRatio <= preferredAspectRatio)
            {
                // output is taller than it is wider, set size to window height
                menuBGSize = new Point((int)MathF.Round(windowHeight * preferredAspectRatio), windowHeight);
            }
            else
            {
                // output is wider than it is tall, set size to window width
                menuBGSize = new Point(windowWidth, (int)MathF.Round(windowWidth / preferredAspectRatio));
            }

            // Draw background
            _spriteBatch.Draw(menuBGTarget, new Rectangle(Point.Zero, menuBGSize), Color.White);

            //Draw all the credits text
            creditsTitle.Draw(_spriteBatch);
            developersTitle.Draw(_spriteBatch);
            developersText.Draw(_spriteBatch);
            thanksTitle.Draw(_spriteBatch);
            thanksText.Draw(_spriteBatch);
            specialThanksTitle.Draw(_spriteBatch);
            specialThanksText.Draw(_spriteBatch);
            mainMenuButton.Draw(_spriteBatch);

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

            // Wipe any extra arrows on the screen
            PlayerArrow mainArrow = playerArrowList[0];
            playerArrowList.Clear();
            playerArrowList.Add(mainArrow);

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

            // Resets upgrade buttons back to their original positions
            for (int i = 0; i < upgradeButtons.Length; i++)
            {
                upgradeButtons[i].Position = new Vector2(upgradeButtons[i].Position.X, 0);
            }

            UpgradeManager.ResetUpgrades();
        }

        /// <summary>
        /// Run this when the player should unlock an upgrade
        /// </summary>
        public void DisplayUpgradeChoices()
        {
            upgradeChoices = UpgradeManager.GenerateUpgradeChoices();

            for (int i = 0; i < upgradeChoices.Length; i++)
            {
                upgradeButtons[i].Sprite = upgradeChoices[i].Sprite;
            }

            // Disable extra buttons if there aren't enough upgrades
            if (upgradeChoices.Length < 3)
            {
                for (int i = 2; i > upgradeChoices.Length - 1; i--)
                {
                    upgradeButtons[i].Position = new Vector2(upgradeButtons[i].Position.X, upgradeButtons[i].Position.Y + 1000);
                }
            }

            // If there are no upgrades left, bring the player back to the game
            if (upgradeChoices.Length <= 0)
            {
                gameState = GameState.Game;
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

            // --- No longer necessary ---
            //// Update the size and position of all the background layers
            //for (int i = 0; i < 10; i++)
            //{
            //    if (outputAspectRatio <= preferredAspectRatio)
            //    {
            //        // output is taller than it is wider, set size to window height
            //        menuBGLayers[i].Size = new Point((int)MathF.Round(windowHeight * preferredAspectRatio), windowHeight);
            //    }
            //    else
            //    {
            //        // output is wider than it is tall, set size to window width
            //        menuBGLayers[i].Size = new Point(windowWidth, (int)MathF.Round(windowWidth / preferredAspectRatio));
            //    }
            //
            //    // Update the position of all the background layers
            //    menuBGLayers[i].Position = i % 2 == 0 ? Vector2.Zero : new Vector2(-(menuBGLayers[i].Size.X), 0);
            //}
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
            LevelManager.CurrentLevel = "Level5";
            LevelManager.LoadLevel();

            // Temp enemy spawns for starting level
            SpawnManager.SpawnTarget(new Point(64 * 8, 64 * 4));
            SpawnManager.SpawnTarget(new Point(64 * 16, 64 * 4));
            SpawnManager.SpawnTarget(new Point(64 * 8, 64 * 10));
            SpawnManager.SpawnTarget(new Point(64 * 16, 64 * 10));
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
