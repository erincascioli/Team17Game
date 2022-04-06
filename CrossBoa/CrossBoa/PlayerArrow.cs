using System;
using System.Collections.Generic;
using System.Text;
using CrossBoa.Interfaces;
using CrossBoa.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    /// <summary>
    /// A delegate that returns all other arrows to the player after the main one is recollected
    /// </summary>
    public delegate void ArrowReturnHandler();

    /// <summary>
    /// Author: Ian Knecht
    /// <para>Represents a playerArrow with a rotation and a constant movement speed</para>
    /// </summary>
    public class PlayerArrow : Arrow
    {
        // Events
        /// <summary>
        /// Runs every frame while the arrow returns to the player
        /// </summary>
        public event ArrowReturnHandler OnReturn;

        /// <summary>
        /// Runs when the arrow is collected by the player
        /// </summary>
        public event ArrowPickupHandler OnPickup;

        private bool isInAir;
        private float timeUntilDespawn;
        private bool flashFrames;

        // Upgrade-related fields
        private bool isMainArrow;
        private bool isReturning;
        private float directionOffset;

        //private const float TimeBeforePickup = 0.5f;


        /// <summary>
        /// Whether the arrow is currently able to hit anything, or if it is on the ground
        /// </summary>
        public bool IsInAir
        {
            get { return isInAir; }
        }

        /// <summary>
        /// Flag for special arrows that should not be directly collectible
        /// </summary>
        public bool IsMainArrow
        {
            get { return isMainArrow; }
        }

        /// <summary>
        /// Flag for when secondary arrows begin returning to the player
        /// </summary>
        public bool IsReturning
        {
            get { return isReturning; }
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

        /// <summary>
        /// Constructs a PlayerArrow that is disabled by default. Run GetShot to activate it.
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="size">The size that this arrow will be rendered as</param>
        /// <param name="isMainArrow">Set to true if this arrow should be the one the player must recollect</param>
        public PlayerArrow(Texture2D sprite, Point size, bool isMainArrow) :
            base(sprite, new Rectangle(new Point(-100), size), Vector2.Zero)
        {
            this.isActive = false;
            this.isInAir = false;
            this.isMainArrow = isMainArrow;
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

                if (!IsInAir && isMainArrow)
                {
                    // Move to player if arrow is nearby and arrow is on ground
                    GetSuckedIntoPlayer(80, 5000);
                }

                if (!isMainArrow && isReturning)
                    GetSuckedIntoPlayer(5000);

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
                    // If this arrow has been flagged as returning, send it back to the player
                    if (isMainArrow)
                        GetSuckedIntoPlayer(7500);

                    // Invoke arrow return event so other arrow are flagged as returning
                    if (OnReturn != null)
                        OnReturn();

                    // If this arrow is within 100 meters, collect it (extends pickup range so arrow doesn't overshoot)
                    if (Helper.DistanceSquared(this.Hitbox.Center, Game1.Player.Hitbox.Center) < 10000)
                    {
                        color = Color.White;
                        GetPickedUp();
                    }
                }

                // If the arrow somehow misses, give it back automatically
                else if (timeUntilDespawn <= -1f)
                {
                    color = Color.White;
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
            velocity *= -1f;
            maxSpeed = 1000;
            friction = 1000;
            isInAir = false;
            timeUntilDespawn = PlayerStats.ArrowDespawnTime;
        }

        public override void GetShot(Vector2 position, float direction, float magnitude)
        {
            timeUntilDespawn = PlayerStats.ArrowDespawnTime;
            color = Color.White;
            isActive = true;
            isInAir = true;
            this.position = position;
            this.direction = direction + directionOffset;
            this.velocity = new Vector2(MathF.Cos(this.direction), MathF.Sin(this.direction)) * magnitude;
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
            // If this is a temporary arrow, delete it from the list
            if (!isMainArrow)
                Game1.playerArrowList.Remove(this);

            timeUntilDespawn = 0;
            isActive = false;
            position = new Vector2(-1000, -1000);

            // Invoke OnPickup event
            if (OnPickup != null)
                OnPickup();
        }

        /// <summary>
        /// Moves the arrow toward the player if they are near it
        /// </summary>
        /// <param name="distance">The radius the player enter must be before the arrow is returned</param>
        /// <param name="force">How much force the arrow should return to the player with</param>
        public void GetSuckedIntoPlayer(int distance, float force)
        {
            Point playerCenter = Game1.Player.Hitbox.Center;
            Point arrowCenter = this.Hitbox.Center;

            // If the player is within the distance, return the arrow
            if (Helper.DistanceSquared(playerCenter, arrowCenter) < MathF.Pow(distance, 2))
            {
                // Update the velocity to point towards the player
                VelocityAngle = Helper.DirectionBetween(arrowCenter, playerCenter);

                // Apply more velocity
                ApplyForce(Helper.DirectionBetween(arrowCenter, playerCenter), force);

                // If this is the main player arrow, invoke the OnReturn event to flag others as returning as well
                if (OnReturn != null)
                    OnReturn();
            }
        }

        /// <summary>
        /// Moves the arrow toward the player
        /// </summary>
        /// <param name="force">How much force the arrow should return to the player with</param>
        public void GetSuckedIntoPlayer(float force)
        {
            Point playerCenter = Game1.Player.Hitbox.Center;
            Point arrowCenter = this.Hitbox.Center;

            isReturning = true;

            // Update the velocity to point towards the player
            VelocityAngle = Helper.DirectionBetween(arrowCenter, playerCenter);

            // Apply more velocity
            ApplyForce(Helper.DirectionBetween(arrowCenter, playerCenter), force);
        }

        /// <summary>
        /// Flags this arrow for return
        /// </summary>
        public void Recollect()
        {
            isReturning = true;
            HitSomething();
            Game1.Crossbow.FireArrows -= GetShot;
        }
    }
}
