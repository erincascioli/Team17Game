using System;
using System.Collections.Generic;
using System.Text;
using CrossBoa.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    enum CBAnimState
    {
        Loaded,
        Transition,
        Empty
    }

    /// <summary>
    /// A crossbow, which points towards the mouse and can fire
    /// an arrow. Inherits from GameObject.
    /// Written by Leo Schinder-Gerendasi
    /// </summary>
    public class CrossBow : GameObject, IShoot
    {
        // ~~~ FIELDS ~~~
        private SpriteEffects spriteEffects;
        private float direction;
        private CBAnimState animationState;
        private float timeSinceShot;
        private float timeSincePickup;

        private const float arrowShotSpeed = 360f;
        private const float shotCoolDown = 0.2f;

        /// <summary>
        /// Whether or not the crossbow has an arrow
        /// in it and is ready to fire.
        /// </summary>
        private bool isLoaded;

        // ~~~ PROPERTIES ~~~
        /// <summary>
        /// The time since the bow was last shot. Get-only.
        /// </summary>
        public float TimeSinceShot
        {
            get
            {
                return timeSinceShot;
            }
        }

        /// <summary>
        /// Whether or not the crossbow has an arrow
        /// in it and is ready to fire. Get-only.
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                return isLoaded;
            }
        }

        /// <summary>
        /// The cooldown per shot. Get-only property.
        /// </summary>
        public float ShotCoolDown
        {
            get
            {
                return shotCoolDown;
            }
        }

        /// <summary>
        /// Whether or not the crossbow is currently on cooldown.
        /// Get-only property.
        /// </summary>
        public bool IsOnCooldown
        {
            get
            {
                return timeSincePickup < shotCoolDown;
            }
        }
        /// <summary>
        /// Returns the direction that the crossbow is facing.
        /// </summary>
        public float Direction
        {
            get { return direction; }
        }

        public Vector2 DrawnPosition
        {
            get
            {
                float rotation = Direction;
                return new Vector2((int)(position.X + 60 * Math.Cos(rotation)),      // Vector X
                                 (int)(position.Y + 60 * Math.Sin(rotation)));       // Vector Y
            }
        }
        // ~~~ CONSTRUCTOR ~~~
        /// <summary>
        /// Creates a CrossBow.
        /// </summary>
        /// <param name="sprite">The sprite that the crossbow uses.</param>
        /// <param name="rectangle">The rectangle that represents the crossbow's hitbox.</param>
        /// <param name="shotCoolDown">The cooldown per shot.</param>
        /// <param name="playerReference">A reference to the player object</param>
        public CrossBow(Texture2D sprite, Rectangle rectangle) : base(sprite, rectangle)
        {
            isLoaded = true;
            timeSinceShot = 0f;
            color = Color.White;
            spriteEffects = SpriteEffects.None;
            animationState = CBAnimState.Loaded;
        }

        // ~~~ METHODS ~~~
        /// <summary>
        /// If not on cooldown, shoots the bow, which sends
        /// the player arrow forward and resets the cooldown.
        /// </summary>
        public void Shoot(Arrow playerArrow)
        {
            if (!IsOnCooldown && isLoaded)
            {
                timeSinceShot = 0f;
                isLoaded = false;
                playerArrow.GetShot(
                    DrawnPosition,
                    Direction,
                    arrowShotSpeed);

                // Makes the playerArrow appear from the bow instead of behind the player.
                playerArrow.Position += (playerArrow.Velocity / playerArrow.Velocity.Length()) * 10;

                // Shake the screen
                Camera.ShakeScreen(12);
            }
        }
           
        public void PickUpArrow()
        {
            timeSincePickup = 0f;
            isLoaded = true;
        }

        /// <summary>
        /// Draws the sprite using the active SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">The active SpriteBatch.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            /*
            // Changes color if crossbow is loaded 
            if (isLoaded && !IsOnCooldown)
                color = Color.PaleTurquoise;
            else
                color = Color.White;
            */

            float rotation = Direction;

            spriteBatch.Draw(sprite,                                              // Texture2D
                             new Rectangle(                                       // Rectangle
                                 (int)(position.X + 60*Math.Cos(rotation)),       // Rectangle X
                                 (int)(position.Y + 60*Math.Sin(rotation)),       // Rectangle Y
                                 60,                                              // Rectangle width
                                 60),                                             // Rectangle height
                             new Rectangle(8 * (int)animationState, 0, 8, 8),     // Source rectangle
                             color,                                               // Color
                             rotation,                                            // Rotation
                             new Vector2(                                         // Origin
                                 sprite.Width * 0.21f,                            // Origin X
                                 sprite.Height * 0.5f),                           // Origin Y
                             spriteEffects,                                       // SpriteEffects
                             1f);                                                 // Layer depth
        }

        /// <summary>
        /// Moves the bow to the player's position, centered in the middle
        /// of the sprite.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Update direction
            direction = MathHelper.DirectionBetween(position, Game1.MousePositionInGame());

            // Update position
            this.position = Game1.Player.Position + Game1.Player.Size.ToVector2() / 2;

            // Update timer
            timeSinceShot += (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeSincePickup += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Turn sprite around when crossbow faces other way
            //     Has small amount of leeway so sprite doesn't jitter
            if (spriteEffects == SpriteEffects.None && 
                direction > Math.PI * 0.51 || 
                direction < Math.PI * -0.51)
                spriteEffects = SpriteEffects.FlipVertically;
            if (spriteEffects == SpriteEffects.FlipVertically &&
                     direction < Math.PI * 0.49 && 
                     direction > Math.PI * -0.49)
                spriteEffects = SpriteEffects.None;
            
            UpdateAnimations(gameTime);
        }

        public void UpdateAnimations(GameTime gameTime)
        {
            // Shot animation
            if (timeSinceShot < 0.075f)
                animationState = CBAnimState.Transition;
            if (timeSinceShot >= 0.075f && timeSinceShot < 0.13f)
                animationState = CBAnimState.Empty;

            // Reload animation
            if (isLoaded && timeSincePickup < shotCoolDown)
                animationState = CBAnimState.Transition;
            if (isLoaded && timeSincePickup > shotCoolDown)
                animationState = CBAnimState.Loaded;
        }
    }
}
