using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    /// <summary>
    /// Author: Ian Knecht
    /// <para>Represents a projectile with a rotation and a constant movement speed</para>
    /// </summary>
    public class Projectile : PhysicsObject, ICollidable
    {
        private float direction;
        private bool isActive;
        private bool isInAir;
        private bool isPlayerArrow;

        /// <summary>
        /// The velocity vector of the arrow
        /// </summary>
        public Vector2 Velocity
        {
            get { return velocity; }
        }

        /// <summary>
        /// The direction vector of the arrow
        /// </summary>
        public float Direction
        {
            get { return direction; }
        }

        /// <summary>
        /// Whether this arrow is currently active or not
        /// </summary>
        public bool IsActive
        {
            get { return isActive; }
        }

        /// <summary>
        /// Whether the arrow is currently able to hit anything, or if it is on the ground
        /// </summary>
        public bool IsInAir
        {
            get { return isInAir; }
            set { isInAir = value; }
        }

        /// <summary>
        /// Flag used to differentiate the player arrow
        /// </summary>
        public bool IsPlayerArrow
        {
            get { return isPlayerArrow; }
        }

        /// <summary>
        /// A single point at the tip of this arrow, represented as a rectangle
        /// </summary>
        public Rectangle Hitbox
        {
            get
            {
                return new Rectangle(position.ToPoint(), Point.Zero);
            }
        }

        /// <summary>
        /// Constructs a Projectile
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="rectangle">a Rectangle containing this GameObject's position and size</param>
        /// <param name="velocity">The direction that the projectile will move in</param>
        /// <param name="isPlayerArrow">Set to true if this is the player's arrow</param>
        public Projectile(Texture2D sprite, Rectangle rectangle, Vector2 velocity, bool isPlayerArrow) :
            base(sprite, rectangle, null, 0)
        {
            this.velocity = velocity;
            this.isPlayerArrow = isPlayerArrow;
            direction = MathF.Atan2(velocity.Y, velocity.X);
            this.isActive = true;
            this.isInAir = true;
        }

        /// <summary>
        /// Constructs a Projectile
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="position">The GameObject's position</param>
        /// <param name="size">The GameObject's size in pixels</param>
        /// <param name="velocity">The direction that the projectile will move in</param>
        /// <param name="isPlayerArrow">Set to true if this is the player's arrow</param>
        public Projectile(Texture2D sprite, Vector2 position, Point size, Vector2 velocity, bool isPlayerArrow) :
            base(sprite, position, size, null, 0)
        {
            this.velocity = velocity;
            this.isPlayerArrow = isPlayerArrow;
            direction = MathF.Atan2(velocity.Y, velocity.X);
            this.isActive = true;
            this.isInAir = true;
        }

        /// <summary>
        /// Constructs a Projectile
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="rectangle">a Rectangle containing this GameObject's position and size</param>
        /// <param name="direction">The direction that the projectile will move in</param>
        /// <param name="magnitude">How quickly the projectile will move in that direction</param>
        /// <param name="isPlayerArrow">Set to true if this is the player's arrow</param>
        public Projectile(Texture2D sprite, Rectangle rectangle, float direction, float magnitude, bool isPlayerArrow) :
            base(sprite, rectangle, null, 0)
        {
            this.direction = direction;
            this.isPlayerArrow = isPlayerArrow;
            this.velocity = new Vector2(MathF.Cos(direction), MathF.Sin(direction)) * magnitude;
            this.isActive = true;
            this.isInAir = true;
        }

        /// <summary>
        /// Constructs a Projectile
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="position">The GameObject's position</param>
        /// <param name="size">The GameObject's size in pixels</param>
        /// <param name="direction">The direction that the projectile will move in</param>
        /// <param name="magnitude">How quickly the projectile will move in that direction</param>
        /// <param name="isPlayerArrow">Set to true if this is the player's arrow</param>
        public Projectile(Texture2D sprite, Vector2 position, Point size, float direction, float magnitude, bool isPlayerArrow) :
            base(sprite, position, size, null, 0)
        {
            this.direction = direction;
            this.isPlayerArrow = isPlayerArrow;
            this.velocity = new Vector2(MathF.Cos(direction), MathF.Sin(direction)) * magnitude;
            this.isActive = true;
            this.isInAir = true;
        }

        /// <summary>
        /// Put this in Update() in Game1
        /// </summary>
        /// <param name="gameTime">A reference to the GameTime</param>
        public override void Update(GameTime gameTime)
        {
            Move();
            base.Update(gameTime);
        }

        /// <summary>
        /// Updates the arrow's movement
        /// </summary>
        public void Move()
        {
            if (isActive)
            {
                position += velocity;
            }
        }

        public void ChangeVelocity(Vector2 position, float direction, float magnitude)
        {
            isActive = true;
            this.position = position;
            this.direction = direction;
            this.velocity = new Vector2(MathF.Cos(direction), MathF.Sin(direction)) * magnitude;
        }

        /// <summary>
        /// Run this whenever the arrow hits something
        /// </summary>
        public void HitSomething()
        {
            isActive = false; // Stops projectile

            // Code specifically for the player's arrow
            if (isPlayerArrow)
            {
                direction *= -1;

            }
        }

        /// <summary>
        /// Draws this GameObject to the screen
        /// </summary>
        /// <param name="spriteBatch">A reference to the SpriteBatch</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                sprite,
                position,
                null,
                Color.White,
                direction,
                new Vector2(1, 0.5f),
                size.ToVector2(),
                SpriteEffects.None,
                0.5f);
        }
    }
}
