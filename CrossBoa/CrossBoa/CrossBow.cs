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
    /// </summary>
    public class CrossBow : GameObject
    {
        // ~~~ FIELDS ~~~
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
        /// The cooldown per shot. Get/set property.
        /// </summary>
        public float ShotCoolDown
        {
            get
            {
                return shotCoolDown;
            }
            set
            {
                shotCoolDown = value;
            }
        }

        // ~~~ CONSTRUCTOR ~~~
        /// <summary>
        /// Creates a CrossBow.
        /// </summary>
        /// <param name="sprite">The sprite that the crossbow uses.</param>
        /// <param name="rectangle">The rectangle that represents the crossbow's hitbox.</param>
        /// <param name="shotCoolDown">The cooldown per shot.</param>
        public CrossBow(Texture2D sprite, Rectangle rectangle, float shotCoolDown) :
            base(sprite, rectangle)
        {
            this.shotCoolDown = shotCoolDown;
            isLoaded = true;
            timeSinceShot = 0f;
        }

        // ~~~ METHODS ~~~
        /// <summary>
        /// If not on cooldown, shoots the bow, which sends
        /// a projectile forward and resets the cooldown.
        /// Can't be worked on since we don't have a
        /// Projectile class yet.
        /// </summary>
        /// <param name="arrow">The projectile tp shoot forward.</param>
        // public void Shoot(Projectile arrow)


        /// <summary>
        /// Calculates the angle between the crossbow and the mouse cursor,
        /// and returns that.
        /// </summary>
        /// <returns>The anle between the crossbow and the mouse cursor,</returns>
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
        public void Update(Player player)
        {
            this.position.X = player.Position.X + player.Width / 2;
            this.position.Y = player.Position.Y + player.Height / 2;
        }
    }
}
