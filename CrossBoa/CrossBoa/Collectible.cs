using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using CrossBoa.Enemies;
using CrossBoa.Interfaces;

namespace CrossBoa
{
    /// <summary>
    /// Represents a collectible objecct
    /// </summary>
    public class Collectible : PhysicsObject, ICollidable
    {
        private bool isActive;
        private bool isAssigned;

        /// <summary>
        /// Whether or not this collectible is currently active
        /// </summary>
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        /// <summary>
        /// Whether or not this collectible is currently assigned to an enemy
        /// </summary>
        public bool IsAssigned
        {
            get { return isAssigned; }
            set { isAssigned = value; }
        }

        /// <summary>
        /// The hitbox of this collectible
        /// </summary>
        public Rectangle Hitbox
        {   
            get { return Rectangle; }
        }

        /// <summary>
        /// Constructs a collectible object
        /// </summary>
        /// <param name="asset">The texture for this collectible</param>
        /// <param name="size">The size of this collectible</param>
        public Collectible(Texture2D asset, Point size) : 
            base(asset, new Rectangle(new Point(-500), size), null, 800)
        {
            isActive = true;
            isAssigned = false;
        }

        /// <summary>
        /// Updates this object's physics and friction
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            GetSuckedIntoPlayer(100, 6000);

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isActive)
            {
                spriteBatch.Draw(Sprite, Rectangle, Color.White);
            }
        }

        /// <summary>
        /// Moves this collectible toward the player if they are near it
        /// </summary>
        /// <param name="distance">The distance to go toward the player from</param>
        /// <param name="force">How much force the collectible should be pulled toward the player with</param>
        public void GetSuckedIntoPlayer(int distance, float force)
        {
            if(isActive)
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

        /// <summary>
        /// Reactivates this collectible and fires it out randomly after an enemy's death
        /// </summary>
        /// <param name="enemy">A reference to the enemy this collectible has spawned from</param>
        public void Spawn(Enemy enemy)
        {
            isActive = true;
            
            // Set the position to the enemy
            position = (enemy.Hitbox.Center - Size / new Point(2)).ToVector2();
            
            // Generate a random direction and velocity
            float direction = (float) ((Game1.RNG.NextDouble() * 2 - 1) * Math.PI);
            float magnitude = Game1.RNG.Next(8000, 18000);

            ApplyForce(direction, magnitude);
        }

        /// <summary>
        /// Performs all necessary interactions when the player collects this
        /// </summary>
        public void GetCollected()
        {
            if(isActive)
            {
                isActive = false;
                isAssigned = false;
                Game1.Exp++;
            }
        }
    }
}
