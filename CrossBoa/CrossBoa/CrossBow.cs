using System;
using System.Collections.Generic;
using System.Text;
using CrossBoa.Interfaces;
using CrossBoa.Managers;
using CrossBoa.Upgrades;
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
    /// A delegate for reloading the crossbow when the arrow is picked up
    /// </summary>
    public delegate void ArrowPickupHandler();

    /// <summary>
    /// A delegate used for firing arrows
    /// </summary>
    /// <param name="position">The position of the crossbow</param>
    /// <param name="direction">The direction of the crossbow</param>
    /// <param name="magnitude">The magnitude of the shot</param>
    public delegate void ArrowShotHandler(Vector2 position, float direction, float magnitude);

    /// <summary>
    /// A crossbow, which points towards the mouse and can fire
    /// an arrow. Inherits from GameObject.
    /// Written by Leo Schinder-Gerendasi
    /// </summary>
    public class CrossBow : GameObject
    {
        // ~~~ FIELDS ~~~
        private SpriteEffects spriteEffects;
        private float direction;
        private CBAnimState animationState;
        private float timeSinceShot;
        private float timeSincePickup;

        private const float shotCoolDown = 0.2f;

        /// <summary>
        /// Handles arrow firing when the crossbow is shot
        /// </summary>
        public event ArrowShotHandler FireArrows;

        /// <summary>
        /// Invokes upgrades that affect the player's shot
        /// </summary>
        public event UpgradeBehavior OnShot;

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
        /// Get-only property.
        /// </summary>
        public float Direction
        {
            get 
            {
                if (!float.IsNaN(direction))
                    return direction;
                else
                    throw new DivideByZeroException("Crossbow angle returned NaN:" +
                        "\nHoriz. difference: " + (position.X - Game1.MousePositionInGame().X) +
                        "\nVert difference: " + (position.Y - Game1.MousePositionInGame().Y));
            }
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
        public void Shoot()
        {
            // Shoot the arrow
            if (IsOnCooldown || !isLoaded) return;

            timeSinceShot = 0f;
            isLoaded = false;

            // Invoke shot modifiers
            OnShot?.Invoke();

            // Fire additional arrows
            FireArrows?.Invoke(DrawnPosition, Direction, PlayerStats.ArrowVelocity);

            // Handled by event now
            //playerArrow.GetShot(
            //    DrawnPosition,
            //    Direction,
            //    arrowShotSpeed);

            // Shake the screen
            Camera.ShakeScreen(12);
            SoundManager.shootBow.Play(.1f, -.5f, 0);
        }
           
        public void PickUpArrow()
        {
            if (!isLoaded)
            {
                SoundManager.arrowPickup.Play(.1f, -.9f, 0);
            }
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
            direction = Helper.DirectionBetween(position, Game1.MousePositionInGame());

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
            
            UpdateAnimations();
        }

        public void UpdateAnimations()
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
