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
    public class CrossBow : GameObject//, IShoot
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

        /// <summary>
        /// The sprite used to represent the fired arrow.
        /// </summary>
        private Texture2D boltSprite;

        /// <summary>
        /// The current projectile that can get fired.
        /// </summary>
        private Projectile arrow;

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
        /// Returns the direction that the crossbow is facing.
        /// </summary>
        public float Direction
        {
            get { return FollowCursor(); }
        }

        // ~~~ CONSTRUCTOR ~~~
        /// <summary>
        /// Creates a CrossBow.
        /// </summary>
        /// <param name="sprite">The sprite that the crossbow uses.</param>
        /// <param name="rectangle">The rectangle that represents the crossbow's hitbox.</param>
        /// <param name="shotCoolDown">The cooldown per shot.</param>
        /// <param name="playerReference">A reference to the player object</param>
        public CrossBow(Texture2D sprite, Rectangle rectangle, float shotCoolDown, Player playerReference,
            Texture2D boltSprite) : base(sprite, rectangle)
        {
            this.shotCoolDown = shotCoolDown;
            player = playerReference;
            isLoaded = true;
            timeSinceShot = 0f;
            this.boltSprite = boltSprite;
        }

        // ~~~ METHODS ~~~
        /// <summary>
        /// If not on cooldown, shoots the bow, which sends
        /// a projectile forward and resets the cooldown.
        /// </summary>
        public Projectile Shoot()
        {
            if (timeSinceShot >= shotCoolDown)
            {
                timeSinceShot = 0f;
                arrow = new Projectile(
                boltSprite,                 // Sprite
                Position,                   // Position
                new Point(50, 15),          // Size
                Direction,                  // Direction
                5,                          // Velocity
                true);                      // IsPlayerArrow

                // Makes the projectile appear from the bow instead of behind the player.
                arrow.Position += arrow.Velocity * 10;
            }
            return arrow;
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
            spriteBatch.Draw(sprite,                    // Texture2D
                             new Rectangle(             // Rectangle
                                 (int)position.X,       // Rectangle X
                                 (int)position.Y,       // Rectangle Y
                                 sprite.Width,          // Rectangle width
                                 sprite.Height),        // Rectangle height
                             null,                      // Nullable rectangle
                             Color.White,               // Color
                             FollowCursor(),            // Rotation
                             new Vector2(               // Origin
                                 sprite.Width / 2,      // Origin X
                                 sprite.Height / 2),    // Origin Y
                             SpriteEffects.None,        // SpriteEffects
                             1f);                       // Layer depth
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
