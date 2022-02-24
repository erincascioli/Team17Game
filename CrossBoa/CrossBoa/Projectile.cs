﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    public class Projectile : GameObject
    {
        private Vector2 velocity;
        private float direction;
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
        /// Flag used to differentiate the player arrow
        /// </summary>
        public bool IsPlayerArrow
        {
            get { return isPlayerArrow; }
        }


        /// <summary>
        /// Constructs a Projectile
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="rectangle">a Rectangle containing this GameObject's position and size</param>
        /// <param name="velocity">The direction that the projectile will move in</param>
        /// <param name="isPlayerArrow">Set to true if this is the player's arrow</param>
        public Projectile(Texture2D sprite, Rectangle rectangle, Vector2 velocity, bool isPlayerArrow) : base(sprite, rectangle)
        {
            this.velocity = velocity;
            this.isPlayerArrow = isPlayerArrow;
            direction = MathF.Atan2(velocity.Y, velocity.X);
        }

        /// <summary>
        /// Constructs a Projectile
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="position">The GameObject's position</param>
        /// <param name="size">The GameObject's size in pixels</param>
        /// <param name="velocity">The direction that the projectile will move in</param>
        /// <param name="isPlayerArrow">Set to true if this is the player's arrow</param>
        public Projectile(Texture2D sprite, Vector2 position, Point size, Vector2 velocity, bool isPlayerArrow) : base(sprite, position, size)
        {
            this.velocity = velocity;
            this.isPlayerArrow = isPlayerArrow;
            direction = MathF.Atan2(velocity.Y, velocity.X);
        }

        /// <summary>
        /// Constructs a Projectile
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="rectangle">a Rectangle containing this GameObject's position and size</param>
        /// <param name="direction">The direction that the projectile will move in</param>
        /// <param name="magnitude">How quickly the projectile will move in that direction</param>
        /// <param name="isPlayerArrow">Set to true if this is the player's arrow</param>
        public Projectile(Texture2D sprite, Rectangle rectangle, float direction, float magnitude, bool isPlayerArrow) : base(sprite, rectangle)
        {
            this.direction = direction;
            this.isPlayerArrow = isPlayerArrow;
            this.velocity = new Vector2(MathF.Cos(direction), MathF.Sin(direction)) * magnitude;
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
        public Projectile(Texture2D sprite, Vector2 position, Point size, float direction, float magnitude, bool isPlayerArrow) : base(sprite, position, size)
        {
            this.direction = direction;
            this.isPlayerArrow = isPlayerArrow;
            this.velocity = new Vector2(MathF.Cos(direction), MathF.Sin(direction)) * magnitude;
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
            position += velocity;
        }

        /// <summary>
        /// Run this whenever the arrow hits something
        /// </summary>
        public void HitSomething()
        {

        }

        /// <summary>
        /// Draws this GameObject to the screen
        /// </summary>
        /// <param name="spriteBatch">A reference to the SpriteBatch</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite,
                position,
                null,
                Color.White,
                direction,
                Vector2.Zero,
                size.ToVector2(),
                SpriteEffects.None,
                0.5f);
        }
    }
}
