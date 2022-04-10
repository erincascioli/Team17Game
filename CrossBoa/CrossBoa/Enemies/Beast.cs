using System;
using CrossBoa.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrossBoa.Enemies
{
    /// <summary>
    /// A skeleton (whether or not it turns out to actually
    /// be a skeleton will be decided). It charges at the player constantly.
    /// Written by: Leo Schindler-Gerendasi.
    /// </summary>
    class Beast : Enemy, ICollidable
    {
        // ~~~ FIELDS ~~~
        private const float MovementForce = 1000f;
        private const float FrictionForce = 50f;

        // Movement fields
        private Player target;
        private float knockbackTimer;

        // ~~~ PROPERTIES ~~~
        /// <summary>
        /// The skeleton's hitbox.
        /// </summary>
        public override Rectangle Hitbox
        {
            get 
            {
                return Rectangle; // new Rectangle(Rectangle.X + 4, Rectangle.Y + 12, 56, 56); 
            }
        }

        // ~~~ CONSTRUCTORS ~~~
        /// <summary>
        /// Creates a new Beast.
        /// </summary>
        /// <param name="spriteSheet">The sprite sheet of the skeleton.</param>
        /// <param name="deathSheet">The death animation of the skeleton
        /// (no death animation is programmed yet, so just throw anything in there).</param>
        /// <param name="health">The max health of the skeleton.</param>
        /// <param name="rectangle">The rectangular hitbox of the skeleton.</param>
        public Beast(Texture2D spriteSheet, Texture2D deathSheet, int health, Rectangle rectangle) :
            base(spriteSheet, rectangle, health, null, FrictionForce)
        {
            color = Color.White;
            isAlive = true;
            target = Game1.Player;
            maxSpeed = 200f;
            knockbackTimer = 0.25f;
        }

        // ~~~ METHODS ~~~
        public override void Move()
        {
            Point player = new Point(target.Rectangle.Center.X, target.Rectangle.Center.Y);

            if (Math.Abs(Helper.Distance(new Point((int)position.X + Width / 2, (int)position.Y + Height / 2), player)) > 35)
            {
                ApplyForce(Helper.DirectionBetween(
                    new Point((int)position.X + Width / 2, (int)position.Y + Height / 2), player),
                    MovementForce);
            }
        }

        public override void GetKnockedBack(ICollidable other, float force)
        {
            knockbackTimer = 0;
            base.GetKnockedBack(other, force * 3);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (knockbackTimer < 0.15)
                knockbackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
                Move();
        }
    }
}
