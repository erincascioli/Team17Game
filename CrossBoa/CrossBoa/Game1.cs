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
        private SpriteFont arial32;

        // Objects
        private Button testButton;
        private CrossBow crossbow;
        private Player player;
        private Slime slime;
        private CollisionManager manager;

        private List<GameObject> gameObjectList;

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

            crossbow = new CrossBow(
                tempCbSprite,
                tempCbSprite.Bounds,
                1,
                player,
                whiteSquareSprite);

            slime = new Slime(
                3,
                whiteSquareSprite,
                new Rectangle(400, 400, 64, 64),
                20000f,
                500f,
                2500f,
                player);

            // TODO: TEST CODE
            testButton = new Button(whiteSquareSprite, tempCbSprite, true, new Rectangle(1000, 800, 250, 50));

            // Add all GameObjects to GameObject list
            gameObjectList.Add(slime);
            gameObjectList.Add(player);
            gameObjectList.Add(crossbow); 
            gameObjectList.Add(testButton);

            

            manager = new CollisionManager(player, crossbow);
            manager.AddEnemy(slime);

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

            // Update all GameObjects
            Camera.Update(kbState);

            foreach (GameObject gameObject in gameObjectList)
            {
                gameObject.Update(gameTime);
            }

            // Fires the bow on click.
            if (mState.LeftButton == ButtonState.Pressed && previousMState.LeftButton == ButtonState.Released)
            {
                manager.PlayerArrow = crossbow.Shoot();
            }

            if (manager.PlayerArrow != null)
               manager.PlayerArrow.Update(gameTime);

            

            // TEST CODE THAT MAKES THE PROJECTILE FOLLOW THE MOUSE
            // if (testProjectile != null) testProjectile.Position = mState.Position.ToVector2();
            
            previousKBState = kbState;
            previousMState = mState;

            // CollisionManager checks for collisions
            manager.CheckCollision();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Camera.Matrix);

            //Level
            LevelManager.Draw(_spriteBatch);

            // Draw all GameObjects
            foreach (GameObject gameObject in gameObjectList)
            {
                gameObject.Draw(_spriteBatch);
            }

            if (manager.PlayerArrow != null)
                manager.PlayerArrow.Draw(_spriteBatch);

            // ~~~ Draws the crossbow's timeSinceShot timer
             _spriteBatch.DrawString(arial32, "" + crossbow.TimeSinceShot, new Vector2(0, 0), Color.Black);

            // Shows working hitboxes that don't use points
            manager.Draw(_spriteBatch, hitBox, arrowHitBox);

            _spriteBatch.End();

                base.Draw(gameTime);
        }
    }
}
