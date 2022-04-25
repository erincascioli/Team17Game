using System;
using CrossBoa.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrossBoa
{
    public class Projectile : PhysicsObject, ICollidable
    {
        protected float direction;
        protected bool isActive;

        private SpriteEffects spriteFlip;
        private int currentAnimationFrame;
        private double previousFrameChange;

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
            set { isActive = value; }
        }

        /// <summary>
        /// The hitbox for this object. A single point at the tip of the arrow
        /// </summary>
        public virtual Rectangle Hitbox
        {
            get { return new Rectangle(position.ToPoint().X - (int)(Rectangle.Width * .25), position.ToPoint().Y - (int)(Rectangle.Height * .25), (int)(Rectangle.Width * .5), (int)(Rectangle.Height * .5)); }
        }

        public Projectile(Texture2D sprite, Rectangle rectangle, Vector2 velocity) : base(sprite, rectangle, null, 0)
        {
            this.velocity = velocity;
            this.direction = MathF.Atan2(velocity.Y, velocity.X);
            this.isActive = true;
            this.spriteFlip = SpriteEffects.None;
        }

        public Projectile(Texture2D sprite, Rectangle rectangle, float direction, float magnitude) : base(sprite, rectangle, null, 0)
        {
            this.direction = MathHelper.WrapAngle(direction);
            this.velocity = new Vector2(MathF.Cos(direction), MathF.Sin(direction)) * magnitude;
            this.isActive = true;
            this.spriteFlip = SpriteEffects.None;
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
            this.direction = MathHelper.WrapAngle(direction);
            this.velocity = new Vector2(MathF.Cos(direction), MathF.Sin(direction)) * magnitude;
            friction = 0;
            maxSpeed = null;
        }

        /// <summary>
        /// Updates this object's physics and friction
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            UpdateAnimations(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// Updates the animations of this projectile
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateAnimations(GameTime gameTime)
        {
            // Change frame every 0.1 seconds
            if (!(gameTime.TotalGameTime.TotalSeconds > previousFrameChange + 0.1)) return;

            previousFrameChange = gameTime.TotalGameTime.TotalSeconds;

            currentAnimationFrame++;
            if (currentAnimationFrame > 3)
                currentAnimationFrame = 0;
        }

        /// <summary>
        /// Draws this GameObject to the screen
        /// </summary>
        /// <param name="spriteBatch">A reference to the SpriteBatch</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isActive)
            {
                //double radius = Math.Sqrt(Math.Pow(Rectangle.Width, 2) + Math.Pow(Rectangle.Height, 2));

                spriteBatch.Draw(
                    sprite,

                    // Repositions arrow draw call so the tip is on the hitbox
                    //new Vector2(Rectangle.X + (float)(Rectangle.Width * Math.Sin(direction)), Rectangle.Y + (float)(Rectangle.Height / 2 * Math.Sin(direction))),
                    position,
                    new Rectangle(currentAnimationFrame * 16, 0, 16, 16),
                    color,
                    Direction,
                    new Vector2(8f, 8f),
                    new Vector2(size.X / 16f, size.Y / 16f),
                    spriteFlip,
                    0.5f);
            }
        }
    }
}