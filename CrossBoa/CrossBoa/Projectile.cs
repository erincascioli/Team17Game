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
        private CrossBow crossbowReference;

        private float direction;
        private bool isActive;
        private bool isInAir;
        private bool isPlayerArrow;
        private float timeUntilDespawn;
        private bool flashFrames;

        private const float PlayerArrowDespawn = 15f;

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
        /// Whether the arrow is currently able to hit anything, or if it is on the ground
        /// </summary>
        public bool IsInAir
        {
            get { return isInAir; }
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
                if (IsInAir)
                    return new Rectangle(position.ToPoint(), Point.Zero);
                else
                    return new Rectangle(position.ToPoint(), new Point(10, 10));
            }
        }

        /// <summary>
        /// A reference to the crossbow for the player arrow
        /// </summary>
        public CrossBow CrossbowReference
        {
            set
            {
                if (isPlayerArrow)
                    crossbowReference = value;
                else
                    throw new Exception("Enemy arrows should not collect a reference to the crossbow.");
            }
        }

        /*public Point PickupHitbox
        {
            get {return position}
        }*/

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
            if (isPlayerArrow)
            {
                ApplyFriction(gameTime);

                // If it's on the ground, tick down the despawn time
                if (!isInAir)
                {
                    timeUntilDespawn -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    
                    // Begin flashing when arrow is about to despawn
                    if (timeUntilDespawn <= 2.5f && timeUntilDespawn > 0)
                    {
                        flashFrames = !flashFrames;
                        if (flashFrames)
                            color = new Color(Color.Black, 60);
                        else
                            color = Color.White;
                    }

                    // If there's no time left on the despawn timer, give it back to the player
                    else if (timeUntilDespawn <= 0)
                    {
                        color = Color.White;
                        Disable();
                        crossbowReference.PickUpArrow();
                    }
                }
            }

            base.Update(gameTime);
        }

        public void ChangeVelocity(Vector2 position, float direction, float magnitude)
        {
            color = Color.White;
            isActive = true;
            isInAir = true;
            this.position = position;
            this.direction = direction;
            this.velocity = new Vector2(MathF.Cos(direction), MathF.Sin(direction)) * magnitude;
            friction = 0;
            maxSpeed = null;
        }

        /// <summary>
        /// Disables this arrow and moves it offscreen for later reuse
        /// </summary>
        public void Disable()
        {
            isActive = false;
            position = new Vector2(-1000, -1000);
        }

        /// <summary>
        /// Run this whenever the arrow hits something
        /// </summary>
        public void HitSomething()
        {
            if (isPlayerArrow)
            {
                // If it's the player's arrow, bounce off the wall
                velocity *= -1;
                friction = 1000;
                isInAir = false;
                timeUntilDespawn = PlayerArrowDespawn;
            }
            else
            {
                // If it's an enemy arrow, delete projectile
                velocity = Vector2.Zero;
                isActive = false;
            }
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
                    position - (MathHelper.GetNormalVector(direction) * size.X) + (MathHelper.GetNormalVector(direction - (MathF.PI * 0.5f)) * size.Y),
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
