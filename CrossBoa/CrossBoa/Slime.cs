using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
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
        Squished,
        Dying
    }

    /// <summary>
    /// A slime. It moves towards the player in spurts
    /// and damages them on contact. Inherits from PhysicsObject.
    /// Written by: Leo Schindler-Gerendasi
    /// </summary>
    public class Slime : Enemy, ICollidable
    {
        // ~~~ FIELDS ~~~
        private const float MovementForce = 45000f;
        private const float FrictionForce = 2000f;

        // Movement fields
        private int targetX;
        private int targetY;
        private double timeSinceMove;
        private double totalTimeBeforeNextJump;

        // Animation fields
        private double timeSinceDeath;
        private Texture2D deathSpritesheet;
        private SlimeAnimState animationState;

        // ~~~ PROPERTIES ~~~
        public override Rectangle Hitbox
        {
            get { return new Rectangle(Rectangle.X + 4, Rectangle.Y + 12, 56, 56); }
        }

        // ~~~ CONSTRUCTORS ~~~
        public Slime(Texture2D spriteSheet, Texture2D deathSheet, int health, Rectangle rectangle) :
            base(spriteSheet, rectangle, health, null, FrictionForce)
        {
            deathSpritesheet = deathSheet;

            this.health = health;
            timeSinceMove = 0.5;
            totalTimeBeforeNextJump = TimeUntilNextJump();
            timeSinceDeath = 0;
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
            ApplyForce(direction, MovementForce);

            // Calculate time until next move
            totalTimeBeforeNextJump = TimeUntilNextJump();
            timeSinceMove = 0;
        }

        /// <summary>
        /// Handles knockback when this enemy gets hit
        /// </summary>
        /// <param name="other">The object causing this enemy to be knocked back</param>
        /// <param name="force">How much force to knock this enemy back by</param>
        public override void GetKnockedBack(ICollidable other, float force)
        {
            if (animationState != SlimeAnimState.Dying)
            {
                // If the slime is in the air, move it to the ground
                if (animationState != SlimeAnimState.Resting)
                    timeSinceMove = 1.2f;

                // Otherwise, if the slime hasn't moved for a while, stun it.
                else if (timeSinceMove >= 1.2f)
                    timeSinceMove = 1.2f;

                base.GetKnockedBack(other, force);
            }
        }

        /// <summary>
        /// Draws the slime.
        /// </summary>
        /// <param name="sb">The active SpriteBatch.</param>
        public override void Draw(SpriteBatch sb)
        {
            // Draws from the spritesheet based on the animation state
            if (isAlive && animationState != SlimeAnimState.Dying)
                sb.Draw(sprite, Rectangle, new Rectangle(16 * (int)animationState, 0, 16, 15), color);

            // If the slime is dying, play death animation
            else
            {
                Vector2 deathFrameScale = new Vector2(4, 2.5f);
                int currentFrame;

                // Play first 2 frames faster
                if (timeSinceDeath <= 0.035)
                    currentFrame = 0;
                else if (timeSinceDeath <= 0.07)
                    currentFrame = 1;
                // Play rest of frames
                else
                    // Time per frame is 0.07s
                    currentFrame = (int)Math.Floor((timeSinceDeath - 0.07) / 0.065) + 2;

                // Width of sprites in sheet is 64, Height is 40
                // Slime in first frame is positioned at 25, 28
                sb.Draw(deathSpritesheet, 
                new Rectangle((position - (new Vector2(25, 40) * deathFrameScale)).ToPoint(), (size.ToVector2() * deathFrameScale).ToPoint()), 
                new Rectangle(currentFrame * 64, 0, 64, 40), 
                Color.White);

                // Delete this slime once the death animation finishes
                if (currentFrame * 64 >= deathSpritesheet.Width)
                {
                    Destroy();
                }
            }
        }

        /// <summary>
        /// Ticks this slime every frame
        /// </summary>
        /// <param name="gameTime">A reference to the GameTime</param>
        public override void Update(GameTime gameTime)
        {
            // Update the timers
            timeSinceMove += gameTime.ElapsedGameTime.TotalSeconds;
            if (animationState == SlimeAnimState.Dying)
                timeSinceDeath += gameTime.ElapsedGameTime.TotalSeconds;

            if (isAlive && animationState != SlimeAnimState.Dying)
            {
                // If the slime is ready to jump, push the slime towards the player
                // push the slime towards the player.
                if (timeSinceMove >= totalTimeBeforeNextJump)
                {
                    targetX = (int)Game1.Player.Position.X;
                    targetY = (int)Game1.Player.Position.Y;
                    Move();
                }

                base.Update(gameTime);
            }

            UpdateAnimations(gameTime);
        }

        /// <summary>
        /// Updates this slime's animations
        /// </summary>
        /// <param name="gameTime">A reference to the GameTime</param>
        public void UpdateAnimations(GameTime gameTime)
        {
            // Movement animation state
            if (isAlive && animationState != SlimeAnimState.Dying)
            {
                

                if (timeSinceMove >= totalTimeBeforeNextJump - 0.2)     // Anticipate for 0.2 seconds
                    animationState = SlimeAnimState.Squished;
                else if (timeSinceMove < 0.25)                          // Jump for 0.25 seconds
                    animationState = SlimeAnimState.Jumping;
                else if (timeSinceMove < 0.4)                           // Fall for 0.15 seconds
                    animationState = SlimeAnimState.Falling;
                else if (timeSinceMove < 0.5)                           // Land for 0.1 seconds
                    animationState = SlimeAnimState.Squished;
                else                                                    // Rest if not doing anything else
                    animationState = SlimeAnimState.Resting;
            }
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

        /// <summary>
        /// Plays the death animation and then kills this enemy
        /// </summary>
        public override void Die()
        {
            animationState = SlimeAnimState.Dying;
        }

        /// <summary>
        /// Deletes this slime after the death animation finishes
        /// </summary>
        public void Destroy()
        {
            // Delete this slime
            animationState = SlimeAnimState.Resting;
            position = new Vector2(-1000, -1000);
            timeSinceDeath = 0;
        }

        /// <summary>
        /// Deals damage on contact with the player.
        /// </summary>
        public override void DealContactDamage(Player player)
        {
            if(animationState != SlimeAnimState.Dying)
                base.DealContactDamage(player);
        }
    }
}
