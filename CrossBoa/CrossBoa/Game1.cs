using System;
using System.Collections.Generic;
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

        private KeyboardState previousKBState;
        private MouseState previousMState;

        // Assets
        private Texture2D whiteSquareSprite;
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
        private Slime slime;
        private Projectile arrow;
        private CollisionManager manager;
        private List<Button> buttonList;

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

            buttonList = new List<Button>();

            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.ApplyChanges();



            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            
            // Load textures
            whiteSquareSprite = Content.Load<Texture2D>("White Pixel");
            arial32 = Content.Load<SpriteFont>("Arial32");
            tempCbSprite = Content.Load<Texture2D>("Crossbow_Pull_0");
            hitBox = Content.Load<Texture2D>("Hitbox");
            arrowHitBox = Content.Load<Texture2D>("White Pixel");

            // Load objects
            player = new Player(
                whiteSquareSprite,
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

            arrow = new Projectile(
                whiteSquareSprite,
                new Vector2(-100,-100),
                new Point(50, 15),
                0f,
                0,
                true);

            crossbow = new CrossBow(
                tempCbSprite,
                tempCbSprite.Bounds,
                1,
                player);

            slime = new Slime(
                3,
                whiteSquareSprite,
                new Rectangle(400, 400, 64, 64),
                20000f,
                500f,
                2500f,
                player);

            playHoverSprite = Content.Load<Texture2D>("PlayPressed");
            playPressedSprite = Content.Load<Texture2D>("PlayRegular");
            settingsHoverSprite = Content.Load<Texture2D>("SettingsRegular");
            settingsPressedSprite = Content.Load<Texture2D>("SettingsPressed");

            // TODO: TEST CODE
            testButton = new Button(whiteSquareSprite, tempCbSprite, true, new Rectangle(1000, 800, 250, 50));

            buttonList.Add(new Button(playHoverSprite, playPressedSprite, true, new Rectangle(_graphics.PreferredBackBufferWidth/2 - playHoverSprite.Width / 2, _graphics.PreferredBackBufferHeight/ 2 - playHoverSprite.Height / 2, playHoverSprite.Width, playHoverSprite.Height)));

            buttonList.Add(new Button(settingsPressedSprite, settingsHoverSprite, true, new Rectangle(_graphics.PreferredBackBufferWidth - settingsPressedSprite.Width - 5, 5, settingsHoverSprite.Width, settingsHoverSprite.Height)));

            // Add all GameObjects to GameObject list
            gameObjectList.Add(slime);
            gameObjectList.Add(player);
            gameObjectList.Add(crossbow); 
            //gameObjectList.Add(testButton);

            

            manager = new CollisionManager(player, crossbow);
            manager.AddEnemy(slime);
            manager.PlayerArrow = arrow;

            LevelManager.LContent = Content;
            LevelManager.Collide = manager;
            LevelManager.LoadLevel("TestingFile");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            KeyboardState kbState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();

            switch (gameState)
            {
                case GameState.MainMenu:

                    buttonList[0].Update(gameTime);

                    if (buttonList[0].HasBeenPressed())
                    {
                        gameState = GameState.Game;
                    }

                    break;

                case GameState.Game:

                    // Update all GameObjects
                    Camera.Update(kbState);

                    

                    foreach (GameObject gameObject in gameObjectList)
                    {
                        // Fixes crossbow moving off of player character
                        if (gameObject is CrossBow)
                        {
                            // CollisionManager checks for collisions
                            manager.CheckCollision();
                        }
                        gameObject.Update(gameTime);
                    }

                    // Fires the bow on click.
                    if (mState.LeftButton == ButtonState.Pressed && previousMState.LeftButton == ButtonState.Released)
                    {
                        crossbow.Shoot(arrow);
                    }

                    if (manager.PlayerArrow != null)
                        manager.PlayerArrow.Update(gameTime);

                    buttonList[1].Update(gameTime);

                    if (buttonList[1].HasBeenPressed())
                    {
                        gameState = GameState.Pause;
                    }

                    break;

                case GameState.Settings:

                    break;

                case GameState.Credits:

                    break;

                case GameState.Pause:

                    buttonList[0].Update(gameTime);

                    if (buttonList[0].HasBeenPressed())
                    {
                        gameState = GameState.Game;
                    }

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

                    _spriteBatch.DrawString(arial32, "Main Menu", new Vector2(GraphicsDeviceManager.DefaultBackBufferWidth - 175, GraphicsDeviceManager.DefaultBackBufferHeight / 2), Color.White);
                    buttonList[0].Draw(_spriteBatch);

                    break;

                case GameState.Game:

                    //Level
                    LevelManager.Draw(_spriteBatch);

                    // Draw all GameObjects
                    foreach (GameObject gameObject in gameObjectList)
                    {
                        gameObject.Draw(_spriteBatch);
                    }

            if (manager.PlayerArrow != null)
            {
                manager.PlayerArrow.Draw(_spriteBatch);

                // TEST CODE TO DRAW ARROW RECTANGLE
                // _spriteBatch.Draw(whiteSquareSprite, manager.PlayerArrow.Rectangle, Color.Tan);
            }

            // ~~~ Draws the crossbow's timeSinceShot timer
             _spriteBatch.DrawString(arial32, "" + crossbow.TimeSinceShot, new Vector2(0, 0), Color.Black);

                    buttonList[1].Draw(_spriteBatch);

                    // Shows working hitboxes that don't use points
                    manager.Draw(_spriteBatch, hitBox, arrowHitBox);

                    break;

                case GameState.Settings:

                    _spriteBatch.DrawString(arial32, "Settings", new Vector2(GraphicsDeviceManager.DefaultBackBufferWidth - 175, GraphicsDeviceManager.DefaultBackBufferHeight / 2), Color.White);

                    break;

                case GameState.Credits:

                    _spriteBatch.DrawString(arial32, "Credits", new Vector2(GraphicsDeviceManager.DefaultBackBufferWidth - 175, GraphicsDeviceManager.DefaultBackBufferHeight / 2), Color.White);

                    break;

                case GameState.Pause:

                    _spriteBatch.DrawString(arial32, "Pause", new Vector2(GraphicsDeviceManager.DefaultBackBufferWidth - 175, GraphicsDeviceManager.DefaultBackBufferHeight / 2), Color.White);
                    buttonList[0].Draw(_spriteBatch);

                    break;

                case GameState.GameOver:

                    _spriteBatch.DrawString(arial32, "Game Over", new Vector2(GraphicsDeviceManager.DefaultBackBufferWidth - 175, GraphicsDeviceManager.DefaultBackBufferHeight / 2), Color.White);

                    break;

                case GameState.GameWin:

                    _spriteBatch.DrawString(arial32, "Game Win", new Vector2(GraphicsDeviceManager.DefaultBackBufferWidth - 175, GraphicsDeviceManager.DefaultBackBufferHeight / 2), Color.White);

                    break;
            }
           
            _spriteBatch.End();

                base.Draw(gameTime);
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
}
