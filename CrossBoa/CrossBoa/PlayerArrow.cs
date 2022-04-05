using System;
using System.Collections.Generic;
using System.Text;
using CrossBoa.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    /// <summary>
    /// Author:  TacNayn
    /// <para>Represents a playerArrow with a rotation and a constant movement speed</para>
    /// </summary>
    public class PlayerArrow : Arrow
    {
        public event ReloadCrossbow OnPickup;

        private bool isInAir;
        private float timeUntilDespawn;
        private bool flashFrames;

        private const float PlayerArrowDespawn = 30f;
        private const float TimeBeforePickup = 0.5f;

        // Upgrade-related fields
        private bool isCollectable;
        private float directionOffset;

        /// <summary>
        /// Whether the arrow is currently able to hit anything, or if it is on the ground
        /// </summary>
        public bool IsInAir
        {
            get { return isInAir; }
        }

        /// <summary>
        /// Flag for arrows that should not be directly collectible
        /// </summary>
        public bool IsCollectable
        {
            get { return isCollectable; }
        }

        /// <summary>
        /// An offset for the direction of this arrow when it gets shot
        /// <para>Mainly used for multishot arrows</para>
        /// </summary>
        public float DirectionOffset
        {
            get { return directionOffset; }
            set { directionOffset = value; }
        }

        /// <summary>
        /// A single point at the tip of this arrow, represented as a rectangle,
        /// <para>which moves to the center of the arrow after it hits something</para>
        /// </summary>
        public override Rectangle Hitbox
        {
            get
            {
                // Places hitbox at the tip of the arrow
                if (IsInAir)
                    return new Rectangle(position.ToPoint(), Point.Zero);
                
                // Repositions hitbox to center of arrow
                else
                    return new Rectangle((position - (Helper.GetNormalVector(direction) * size.X / 2)).ToPoint(), Point.Zero);
            }
        }

        /*public Point PickupHitbox
        {
            get {return position}
        }*/

        /// <summary>
        /// Constructs a PlayerArrow
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="rectangle">a Rectangle containing this GameObject's position and size</param>
        /// <param name="velocity">The direction that the playerArrow will move in</param>
        /// <param name="isCollectable">Set to true if this arrow should be the one the player must recollect</param>
        public PlayerArrow(Texture2D sprite, Rectangle rectangle, bool isCollectable) :
            base(sprite, rectangle, Vector2.Zero)
        {
            this.isInAir = true;
            this.isCollectable = isCollectable;
        }

        /// <summary>
        /// Put this in Update() in Game1
        /// </summary>
        /// <param name="gameTime">A reference to the GameTime</param>
        public override void Update(GameTime gameTime)
        {
            // If it's on the ground, tick down the despawn time
            if (isActive)
            {
                timeUntilDespawn -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (!IsInAir)
                {
                    // Move to player if arrow is nearby and arrow is on ground
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

                    if (Helper.DistanceSquared(this.Hitbox.Center, Game1.Player.Hitbox.Center) < 10000)
                    {
                        color = Color.White;

                        // Invoke OnPickup event
                        if (OnPickup != null) 
                            OnPickup();

                        GetPickedUp();
                    }
                }

                // If the arrow somehow misses, give it back automatically
                else if (timeUntilDespawn <= -1f)
                {
                    color = Color.White;

                    // Invoke OnPickup event
                    if (OnPickup != null)
                        OnPickup();

                    GetPickedUp();
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Run this whenever the arrow hits something
        /// </summary>
        public override void HitSomething()
        {
            velocity *= -1;
            friction = 1000;
            isInAir = false;
            timeUntilDespawn = PlayerArrow.PlayerArrowDespawn;
        }

        public override void GetShot(Vector2 position, float direction, float magnitude)
        {
            timeUntilDespawn = PlayerArrowDespawn;
            color = Color.White;
            isActive = true;
            isInAir = true;
            this.position = position;
            this.direction = direction + directionOffset;
            this.velocity = new Vector2(MathF.Cos(direction), MathF.Sin(direction)) * magnitude;
            friction = 0;
            maxSpeed = null;

            // Makes the playerArrow appear from the bow instead of behind the player.
            this.position += (velocity / velocity.Length()) * 5;
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
        /// Moves the arrow toward the player if they are near it
        /// </summary>
        /// <param name="distance">The distance to go toward the player from</param>
        /// <param name="force">How much force the arrow should return to the player with</param>
        public void GetSuckedIntoPlayer(int distance, float force)
        {
            Point playerCenter = Game1.Player.Hitbox.Center;
            Point arrowCenter = this.Hitbox.Center;

            if (Helper.DistanceSquared(playerCenter, arrowCenter) < MathF.Pow(distance, 2))
            {
                // Update the velocity to point towards the player
                VelocityAngle = Helper.DirectionBetween(arrowCenter, playerCenter);

                // Apply more velocity
                ApplyForce(Helper.DirectionBetween(arrowCenter, playerCenter), force);
            }
        }
    }
}
