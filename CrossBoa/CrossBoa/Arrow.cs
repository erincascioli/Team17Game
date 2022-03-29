﻿using System;
using CrossBoa.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrossBoa
{
    public class Arrow : PhysicsObject, ICollidable
    {
        protected float direction;
        protected bool isActive;

        /// <summary>
        /// The direction vector of the arrow
        /// </summary>
        public float Direction
        {
            get { return direction; }
        }

        /// <summary>
        /// Whether this arrow is currently being used, or if it should be pooled
        /// </summary>
        public bool IsActive
        {
            get { return isActive; }
        }

        /// <summary>
        /// The hitbox for this object. A single point at the tip of the arrow
        /// </summary>
        public virtual Rectangle Hitbox
        {
            get { return new Rectangle(position.ToPoint(), Point.Zero); }
        }

        public Arrow(Texture2D sprite, Rectangle rectangle, Vector2 velocity) : base(sprite, rectangle, null, 0)
        {
            this.velocity = velocity;
            this.direction = MathF.Atan2(velocity.Y, velocity.X);
            this.isActive = true;
        }

        public Arrow(Texture2D sprite, Rectangle rectangle, float direction, float magnitude) : base(sprite, rectangle, null, 0)
        {
            this.direction = direction;
            this.velocity = new Vector2(MathF.Cos(direction), MathF.Sin(direction)) * magnitude;
            this.isActive = true;
        }

        /// <summary>
        /// Run this whenever the arrow hits something
        /// </summary>
        public virtual void HitSomething()
        {
            // Delete projectile
            velocity = Vector2.Zero;
            isActive = false;
        }

        public virtual void GetShot(Vector2 position, float direction, float magnitude)
        {
            color = Color.White;
            isActive = true;
            this.position = position;
            this.direction = direction;
            this.velocity = new Vector2(MathF.Cos(direction), MathF.Sin(direction)) * magnitude;
            friction = 0;
            maxSpeed = null;
        }

        /// <summary>
        /// Draws this GameObject to the screen
        /// </summary>
        /// <param name="spriteBatch">A reference to the SpriteBatch</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isActive)
            {
                spriteBatch.Draw(
                    sprite,

                    // Repositions arrow draw call so the tip is on the hitbox
                    position - (MathHelper.GetNormalVector(direction) * size.X) + (MathHelper.GetNormalVector(direction - (MathF.PI * 0.5f)) * size.Y / 2),
                    null,
                    color,
                    direction,
                    new Vector2(1, 0.5f),
                    new Vector2(size.X / (float)sprite.Width, size.Y / (float)sprite.Height),
                    SpriteEffects.None,
                    0.5f);
            }
        }
    }
}