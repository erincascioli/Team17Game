using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    /// <summary>
    /// Author:  TacNayn
    /// <para>Represents a projectile with a rotation and a constant movement speed</para>
    /// </summary>
    public class Projectile : PhysicsObject, ICollidable
    {
        private CrossBow crossbowReference;
        private Player playerReference;

        private float direction;
        private bool isActive;
        private bool isInAir;
        private bool isPlayerArrow;
        private float timeUntilDespawn;
        private bool flashFrames;

        private const float PlayerArrowDespawn = 30f;
        private const float TimeBeforePickup = 0.5f;

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
                // Places hitbox at the tip of the arrow
                if (IsInAir)
                    return new Rectangle(position.ToPoint(), Point.Zero);
                
                // Repositions hitbox to center of arrow
                else
                    return new Rectangle((position - (MathHelper.GetNormalVector(direction) * size.X / 2)).ToPoint(), Point.Zero);
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

        /// <summary>
        /// A reference to the player for the player arrow
        /// </summary>
        public Player PlayerReference
        {
            set
            {
                if (isPlayerArrow)
                    playerReference = value;
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
                if (isActive)
                {
                    timeUntilDespawn -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (!IsInAir)
                    {
                        // Move to player if arrow is nearby and arrow is on ground
                        if (timeUntilDespawn < PlayerArrowDespawn - TimeBeforePickup)
                            GetSuckedIntoPlayer(80, 5000);
                    }

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
                    else if (timeUntilDespawn <= 0 && timeUntilDespawn > -1f)
                    {
                        GetSuckedIntoPlayer(8000, 7500);

                        if (MathHelper.DistanceSquared(this.Hitbox.Center, playerReference.Hitbox.Center) < 10000)
                        {
                            color = Color.White;
                            crossbowReference.PickUpArrow();
                            GetPickedUp();
                        }
                    }

                    // If the arrow somehow misses, give it back automatically
                    else if (timeUntilDespawn <= -1f)
                    {
                        color = Color.White;
                        crossbowReference.PickUpArrow();
                        GetPickedUp();
                    }
                }
            }

            base.Update(gameTime);
        }

        public void ChangeVelocity(Vector2 position, float direction, float magnitude)
        {
            timeUntilDespawn = PlayerArrowDespawn;
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
        public void GetPickedUp()
        {
            timeUntilDespawn = 0;
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
        /// Moves the arrow toward the player if they are near it
        /// </summary>
        /// <param name="distance">The distance to go toward the player from</param>
        /// <param name="force">How much force the arrow should return to the player with</param>
        public void GetSuckedIntoPlayer(int distance, float force)
        {
            Point playerCenter = playerReference.Hitbox.Center;
            Point arrowCenter = this.Hitbox.Center;

            if (MathHelper.DistanceSquared(playerCenter, arrowCenter) < MathF.Pow(distance, 2))
            {
                // Update the velocity to point towards the player
                VelocityAngle = MathHelper.DirectionBetween(arrowCenter, playerCenter);

                // Apply more velocity
                ApplyForce(MathHelper.DirectionBetween(arrowCenter, playerCenter), force);
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
