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
        private const float BaseMaxSpd = 175f;
        private const float KnockbackMaxSpd = 500f;

        // Movement fields
        private Player target;
        private float knockbackTimer;
        private float provokeRadius;

        // ~~~ PROPERTIES ~~~
        /// <summary>
        /// The beast's hitbox.
        /// </summary>
        public override Rectangle Hitbox
        {
            get 
            {
                return Rectangle; // new Rectangle(Rectangle.X + 4, Rectangle.Y + 12, 56, 56); 
            }
        }

        public float DistanceBetween
        {
            get
            {
                return Helper.Distance(ToPoint(position),
                ToPoint(target.Position));
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
            maxSpeed = BaseMaxSpd;
            knockbackTimer = 0.25f;
            provokeRadius = 250f;
        }

        // ~~~ METHODS ~~~
        /// <summary>
        /// Moves the beast towards the target.
        /// </summary>
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
            maxSpeed = KnockbackMaxSpd;
            base.GetKnockedBack(other, force);
            // Cool feature to make the beast start chasing the player sooner
            // the more damage it takes
            provokeRadius += 50f; 
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (knockbackTimer < 0.15)
            {
                ApplyFriction(gameTime);
                knockbackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (knockbackTimer >= 0.15)
                    maxSpeed = BaseMaxSpd;
            }
            else if (Math.Abs(DistanceBetween) < provokeRadius)
                Move();
            else if (velocity.Length() > 0f)
            {
                velocity = velocity / 1.1f;
                ApplyFriction(gameTime);
            }
        }

        /// <summary>
        /// A helper method to convert a Vector2 to a Point,
        /// usable by the Helper method DirectionBetween().
        /// </summary>
        /// <param name="position">The Vector2 in question.</param>
        /// <returns>A point equivalent to that Vector2.</returns>
        private Point ToPoint(Vector2 position)
        {
            return new Point((int)position.X, (int)position.Y);
        }
    }
}
