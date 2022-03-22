using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{

    enum SlimeAnimState
    {
        Resting,
        Jumping,
        Falling,
        Squished
    }

    /// <summary>
    /// A slime. It moves towards the player in spurts
    /// and damages them on contact. Inherits from PhysicsObject.
    /// Written by: Leo Schindler-Gerendasi
    /// </summary>
    public class Slime : Enemy, ICollidable
    {
        // ~~~ FIELDS ~~~
        private const float movementForce = 45000f;
        private const float frictionForce = 2000f;

        // Movement fields
        private int targetX;
        private int targetY;
        private double timeUntilMove;
        private double totalTimeBeforeNextJump;
        private bool hasMovedYet;

        // Animation fields
        private SlimeAnimState animationState;
        private float hurtFlashTime;

        // ~~~ PROPERTIES ~~~
        public override Rectangle Hitbox
        {
            get { return new Rectangle(Rectangle.X + 4, Rectangle.Y + 12, 56, 56); }
        }

        // ~~~ CONSTRUCTORS ~~~
        public Slime(Texture2D spriteSheet, int health, Rectangle rectangle) :
            base(spriteSheet, rectangle, health, null, frictionForce)
        {
            this.health = health;
            timeUntilMove = TimeUntilNextJump();
            color = Color.White;
            isAlive = true;
            animationState = SlimeAnimState.Resting;
        }

        /// <summary>
        /// Moves the slime towards the player.
        /// </summary>
        public override void Move()
        {
            float direction = 0;
            // Formula used for calculations: 
            // Cos(A) = (b^2 + c^2 - a^2) / 2bc
            // A = arccos the formula above
            double horizDist = targetX - position.X;
            double vertDist = targetY - position.Y;
            float totalDist = (float)Math.Sqrt(Math.Pow(vertDist, 2) +
                Math.Pow(horizDist, 2));
            double cosA = (Math.Pow(totalDist, 2) + Math.Pow(horizDist, 2) - Math.Pow(vertDist, 2))
                / (2 * horizDist * totalDist);
            if (double.IsNaN(cosA))
                cosA = 0;
            direction = (float)Math.Acos(cosA);
            if (targetY < position.Y)
                direction *= -1;
            ApplyForce(direction, movementForce);

        }

        /// <summary>
        /// Handles knockback when this enemy gets hit
        /// </summary>
        /// <param name="other">The object causing this enemy to be knocked back</param>
        /// <param name="force">How much force to knock this enemy back by</param>
        public override void GetKnockedBack(ICollidable other, float force)
        {
            // If the slime is in the air, move it to the ground
            if (animationState != SlimeAnimState.Resting)
                timeUntilMove = 2f;

            // Otherwise, if the slime hasn't moved for a while, stun it.
            else if (timeUntilMove <= 1.2f)
                timeUntilMove = 1.2f;

            base.GetKnockedBack(other, force);
        }


        /// <summary>
        /// Draws the slime.
        /// </summary>
        /// <param name="sb">The active SpriteBatch.</param>
        public override void Draw(SpriteBatch sb)
        {
            // Draws from the spritesheet based on the animation state
            if (isAlive)
                sb.Draw(sprite, Rectangle, new Rectangle(16 * (int)animationState, 0, 16, 16), color);
        }

        /// <summary>
        /// Ticks this slime every frame
        /// </summary>
        /// <param name="gameTime">A reference to the GameTime</param>
        public override void Update(GameTime gameTime)
        {
            if (isAlive)
            {
                // Update the timer
                timeUntilMove -= gameTime.ElapsedGameTime.TotalSeconds;

                // If the slime is ready to jump, push the slime towards the player
                // push the slime towards the player.
                if (timeUntilMove <= 0.5 && !hasMovedYet)
                {
                    targetX = (int)Game1.Player.Position.X;
                    targetY = (int)Game1.Player.Position.Y;
                    Move();
                    hasMovedYet = true;
                }

                if (timeUntilMove <= 0)
                {
                    timeUntilMove = TimeUntilNextJump();
                    hasMovedYet = false;
                }

                UpdateAnimations(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Updates this slime's animations
        /// </summary>
        /// <param name="gameTime">A reference to the GameTime</param>
        public void UpdateAnimations(GameTime gameTime)
        {
            // Movement animation state
            animationState = SlimeAnimState.Resting;            // Rest if not doing anything else
            
            if (timeUntilMove < 0.7 && timeUntilMove >= 0.5)    // Anticipate for 0.2 seconds
                animationState = SlimeAnimState.Squished;
            if (timeUntilMove < 0.5 && timeUntilMove >= 0.25)   // Jump for 0.25 seconds
                animationState = SlimeAnimState.Jumping;
            if (timeUntilMove < 0.25 && timeUntilMove >= 0.1)   // Fall for 0.15 seconds
                animationState = SlimeAnimState.Falling;
            if (timeUntilMove < 0.1)                            // Land for 0.1 seconds
                animationState = SlimeAnimState.Squished;
        }

        /// <summary>
        /// Generates a random time for the slime to wait until it jumps again
        /// </summary>
        /// <returns>A double</returns>
        private double TimeUntilNextJump()
        {
            // Random number between 2 and 3.75
            totalTimeBeforeNextJump = Game1.RNG.NextDouble() * 1.75 + 2;
            return totalTimeBeforeNextJump;
        }
    }
}
