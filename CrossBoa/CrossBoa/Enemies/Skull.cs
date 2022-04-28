using System;
using System.Xml.Schema;
using CrossBoa.Interfaces;
using CrossBoa.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrossBoa.Enemies
{
    public enum SkullAnimState
    {
        FacingDown,
        FacingUp,
        FacingLeft,
        FacingRight
    }

    /// <summary>
    /// A skull. It fires shots towards the player.
    /// Written by: Leo S-G
    /// </summary>
    class Skull : Enemy
    {
        // ~~~ FIELDS ~~~
        private double timeSinceShot;
        private Player target;

        private SkullAnimState animationState;
        private int currentAnimationFrame;
        private double previousFrameChangeTime;

        private const double TimePerShot = 2.5f;

        // ~~~ PROPERTIES ~~~
        public bool ReadyToFire
        {
            get
            {
                return timeSinceShot > TimePerShot;
            }
        }

        // ~~~ CONSTRUCTOR ~~~
        public Skull(Texture2D sprite, Rectangle rectangle, int health, (int, int) expRange) :
            base(sprite, rectangle, health, expRange, null, 0)
        {
            if (!Game1.isHardModeActive)
                timeSinceShot = 0f;
            else
                timeSinceShot = 0.75f;
            isAlive = true;
            color = Color.White;
            target = Game1.Player;
            animationState = SkullAnimState.FacingDown;
            currentAnimationFrame = 0;
        }

        // ~~~ METHODS ~~~
        /// <summary>
        /// Handles knockback when this enemy gets hit
        /// </summary>
        /// <param name="other">The object causing this enemy to be knocked back</param>
        /// <param name="force">How much force to knock this enemy back by</param>
        public override void GetKnockedBack(ICollidable other, float force)
        {
            // This enemy does not get knocked back, therefore this override does not call base
        }

        public void Shoot(Projectile projectile)
        {
            if (!Game1.isHardModeActive)
                timeSinceShot = 0f;
            else
                timeSinceShot = 0.75f;
            projectile.GetShot(
                new Vector2(Rectangle.X + Width/2,
                            Rectangle.Y + Height/2),
                Helper.DirectionBetween(Rectangle.Location, target.Rectangle.Location),
                350);

            // Play Sound Effect
            SoundManager.fireShoot.Play(.6f, 0, 0);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            timeSinceShot += gameTime.ElapsedGameTime.TotalSeconds;

            UpdateAnimations(gameTime);
        }

        /// <summary>
        /// Draws this GameObject to the screen
        /// </summary>
        /// <param name="spriteBatch">A reference to the SpriteBatch</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isAlive)
                spriteBatch.Draw(sprite, Rectangle, new Rectangle((int)animationState * 16, currentAnimationFrame * 16, 16, 16), color);
        }

        /// <summary>
        /// Updates the skull's animations based on the current state
        /// </summary>
        private void UpdateAnimations(GameTime gameTime)
        {
            // Change frame every 0.15 seconds
            if (gameTime.TotalGameTime.TotalSeconds > previousFrameChangeTime + 0.15)
            {
                previousFrameChangeTime = gameTime.TotalGameTime.TotalSeconds;

                currentAnimationFrame++;
                if (currentAnimationFrame > 3)
                    currentAnimationFrame = 0;
            }

            Point differenceBetweenPlayer = Game1.Player.Rectangle.Center - this.Rectangle.Center;

            // Enemy should face left or right
            if (MathF.Abs(differenceBetweenPlayer.X) > MathF.Abs(differenceBetweenPlayer.Y))
            {
                animationState = differenceBetweenPlayer.X > 0 ? 
                    SkullAnimState.FacingRight : 
                    SkullAnimState.FacingLeft;
            }

            // Enemy should face up or down
            else
            {
                animationState = differenceBetweenPlayer.Y > 0 ? 
                    SkullAnimState.FacingDown : 
                    SkullAnimState.FacingUp;
            }
        }

        /// <summary>
        /// Method to have the enemy take damage, and if their health reaches 0,
        /// kill them.
        /// </summary>
        public override void TakeDamage(int damage)
        {
            if (health > 1)
            {
                // Random pitch from -0.25 to 0.25
                SoundManager.skullDamage.Play(.8f, (float)(-1), 0);
            }
            base.TakeDamage(damage);
        }
    }
}
