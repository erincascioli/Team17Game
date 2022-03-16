using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    /// <summary>
    /// A crossbow, which points towards the mouse and can fire
    /// an arrow. Inherits from GameObject.
    /// Written by Leo Schinder-Gerendasi
    /// </summary>
    public class CrossBow : GameObject, IShoot
    {
        // ~~~ FIELDS ~~~
        private Player player;

        /// <summary>
        /// The time since the bow was last shot.
        /// </summary>
        private float timeSinceShot;

        /// <summary>
        /// The cooldown per shot.
        /// </summary>
        private float shotCoolDown;

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
                return timeSinceShot < shotCoolDown;
            }
        }
        /// <summary>
        /// Returns the direction that the crossbow is facing.
        /// </summary>
        public float Direction
        {
            get
            { 
                // Formula used for calculations: 
              // Cos(A) = (b^2 + c^2 - a^2) / 2bc
              // A = arccos the formula above
                double horizDist = Mouse.GetState().X - position.X;
                double vertDist = Mouse.GetState().Y - position.Y;
                float totalDist = (float)Math.Sqrt(Math.Pow(vertDist, 2) +
                    Math.Pow(horizDist, 2));
                double cosA = (Math.Pow(totalDist, 2) + Math.Pow(horizDist, 2) - Math.Pow(vertDist, 2))
                    / (2 * horizDist * totalDist);
                if (double.IsNaN(cosA))
                    cosA = 0;
                if (Mouse.GetState().Y < position.Y)
                    return (float)Math.Acos(cosA) * -1;
                return (float)Math.Acos(cosA);
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
        /// <param name="shotCoolDown">The cooldown per shot.</param>
        /// <param name="playerReference">A reference to the player object</param>
        public CrossBow(Texture2D sprite, Rectangle rectangle, float shotCoolDown, Player playerReference) : base(sprite, rectangle)
        {
            this.shotCoolDown = shotCoolDown;
            player = playerReference;
            isLoaded = true;
            timeSinceShot = 0f;
            color = Color.White;
        }

        // ~~~ METHODS ~~~
        /// <summary>
        /// If not on cooldown, shoots the bow, which sends
        /// a projectile forward and resets the cooldown.
        /// </summary>
        public void Shoot(Projectile projectile)
        {
            if (timeSinceShot >= shotCoolDown && isLoaded)
            {
                timeSinceShot = 0f;
                isLoaded = false;
                projectile.ChangeVelocity(
                    DrawnPosition,
                    Direction,
                    360f);

                // Makes the projectile appear from the bow instead of behind the player.
                projectile.Position += (projectile.Velocity / projectile.Velocity.Length()) * 10;
            }
        }
           
        public void PickUpArrow()
        {
            isLoaded = true;
        }

        /// <summary>
        /// Calculates the angle between the crossbow and the mouse cursor,
        /// and returns that in degrees.
        /// </summary>
        /// <returns>The angle between the crossbow and the mouse cursor, in degrees.</returns>
        public float FollowCursor()
        {
            // Formula used for calculations: 
            // Cos(A) = (b^2 + c^2 - a^2) / 2bc
            // A = arccos the formula above
            double horizDist = Mouse.GetState().X - position.X;
            double vertDist = Mouse.GetState().Y - position.Y;
            float totalDist = (float)Math.Sqrt(Math.Pow(vertDist, 2) +
                Math.Pow(horizDist, 2));
            double cosA = (Math.Pow(totalDist, 2) + Math.Pow(horizDist, 2) - Math.Pow(vertDist, 2))
                / (2 * horizDist * totalDist);
            if (double.IsNaN(cosA))
                cosA = 0;
            if (Mouse.GetState().Y < position.Y)
                return (float)Math.Acos(cosA) * -1;
            return (float)Math.Acos(cosA);
        }

        /// <summary>
        /// Draws the sprite using the active SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">The active SpriteBatch.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isLoaded && !IsOnCooldown)
                color = Color.PaleTurquoise;
            else
                color = Color.White;

            float rotation = Direction;

            spriteBatch.Draw(sprite,                                              // Texture2D
                             new Rectangle(                                       // Rectangle
                                 (int)(position.X + 60*Math.Cos(rotation)),       // Rectangle X
                                 (int)(position.Y + 60*Math.Sin(rotation)),       // Rectangle Y
                                 sprite.Width/2,                                  // Rectangle width
                                 sprite.Height/2),                                // Rectangle height
                             null,                                                // Nullable rectangle
                             color,                                                // Color
                             rotation,                                            // Rotation
                             new Vector2(                                         // Origin
                                 sprite.Width / 2,                                // Origin X
                                 sprite.Height / 2),                              // Origin Y
                             SpriteEffects.None,                                  // SpriteEffects
                             1f);                                                 // Layer depth
        }

        /// <summary>
        /// Moves the bow to the player's position, centered in the middle
        /// of the sprite.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            this.position = player.Position + player.Size.ToVector2() / 2;
            timeSinceShot += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
