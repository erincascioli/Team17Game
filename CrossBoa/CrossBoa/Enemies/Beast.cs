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

    enum ChargingState
    {
        Unnoticed,
        Readying,
        Charging,
        Resting
    }

    class Beast : Enemy, ICollidable
    {
        // ~~~ FIELDS ~~~
        private const float MovementForce = 5000f;
        private const float FrictionForce = 50f;

        // Movement fields
        private Player target;
        private float knockbackTimer;
        private float chargeTimer;
        private float provokeRadius;
        private ChargingState chargingState;
        private bool hasCollided;
        private float chargeDirection;

        // Other fields
        private Rectangle drawRect;

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

        

        /// <summary>
        /// The distance between the the Beast and the player.
        /// </summary>
        public float DistanceBetween
        {
            get
            {
                return Helper.Distance(ToPoint(position),
                ToPoint(target.Position));
            }
        }

        // ~~~ PROPERTIES INTENDED ONLY FOR USE WITHIN COLLISION MANAGER ~~~
        /// <summary>
        /// Whether or not the beast has collided with something. Setting to 
        /// true prematurely ends the beast's charge. Get/set method.
        /// </summary>
        public bool HasCollided
        {
            get
            {
                return hasCollided;
            }
            set
            {
                hasCollided = value;
            }

        }

        /// <summary>
        /// Whether or not the beast is currently charging. Get-only property.
        /// </summary>
        public bool InCharge
        {
            get
            {
                return chargingState == ChargingState.Charging;
            }
        }

        // ~~~ CONSTRUCTORS ~~~
        /// <summary>
        /// Creates a new Beast.
        /// </summary>
        /// <param name="spriteSheet">The sprite sheet of the skeleton.</param>
        /// <param name="deathSheet">The death animation of the beast
        /// (no death animation is programmed yet, so just throw anything in there).</param>
        /// <param name="health">The max health of the skeleton.</param>
        /// <param name="rectangle">The rectangular hitbox of the skeleton.</param>
        public Beast(Texture2D spriteSheet, Texture2D deathSheet, int health, Rectangle rectangle) :
            base(spriteSheet, rectangle, health, null, FrictionForce)
        {
            color = Color.White;
            isAlive = true;
            target = Game1.Player;
            maxSpeed = 450f;
            knockbackTimer = 0.25f;
            provokeRadius = 250f;
            chargeTimer = 0;
            drawRect = Rectangle;
            chargingState = ChargingState.Unnoticed;
            hasCollided = false;
        }

        // ~~~ METHODS ~~~
        /// <summary>
        /// Moves the beast towards the target.
        /// </summary>
        public override void Move()
        {
            ApplyForce(chargeDirection, MovementForce);
            //velocity *= 5f;
            /*Point player = new Point(target.Rectangle.Center.X, target.Rectangle.Center.Y);

            if (Math.Abs(Helper.Distance(new Point((int)position.X + Width / 2, (int)position.Y + Height / 2), player)) > 35)
            {
            }*/
        }

        public override void GetKnockedBack(ICollidable other, float force)
        {
            knockbackTimer = 0;
            base.GetKnockedBack(other, force * 1000);
        }

        public override void TakeDamage(int damage)
        {
            // Cool feature to increase the beast's provoke radius
            // the more damage it takes
            provokeRadius += 50f;

            // Resets the beast's charging state.
            chargingState = ChargingState.Resting;
            chargeTimer = 2f;
            base.TakeDamage(damage);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Knockback stuff
            if (knockbackTimer < 0.15)
            {
                ApplyFriction(gameTime);
                knockbackTimer += totalSeconds;
            }
            else if (Math.Abs(DistanceBetween) < provokeRadius && chargingState == ChargingState.Unnoticed)
            {
                chargingState = ChargingState.Readying;
                chargeTimer = 0;
            }

            // State machine to deal with beast's current movement state
            // in all cases: increase the charge timer, as that is the basis for this chunk of code
            // from 0 - 1 seconds: ready the attack (vibrate)
            // from 1 - 1.5 seconds: charge (move forward very very fast)
            //      also check for collisions. If the beast runs into a wall, set charge timer
            //      to 1.5 seconds immediately (stop the charge) and knock the beast back
            // from 1.5 - 3 seconds: do nothing lmao
            if (chargeTimer <= 3 && chargingState != ChargingState.Unnoticed)
                chargeTimer += totalSeconds;

            switch (chargingState)
            {
                case ChargingState.Readying:
                    Point vibrationMod =
                        new Point(Game1.RNG.Next(-1, 1), Game1.RNG.Next(-1, 1));
                    drawRect = new Rectangle(
                    new Point(Rectangle.X + vibrationMod.X, Rectangle.Y + vibrationMod.Y),
                    Rectangle.Size);
                    if (chargeTimer >= 1f)
                    {
                        chargeDirection = Helper.DirectionBetween(
                            new Point((int)position.X + Width / 2, (int)position.Y + Height / 2),
                            new Point(target.Rectangle.Center.X, target.Rectangle.Center.Y));
                        chargingState = ChargingState.Charging;
                    }
                    break;

                case ChargingState.Charging:
                    Move();
                    if (hasCollided)
                    {
                        hasCollided = false;
                        chargeTimer = 1.5f;
                    }
                    if (chargeTimer >= 1.5f)
                        chargingState = ChargingState.Resting;
                    break;

                case ChargingState.Resting:
                    if (chargeTimer >= 3f)
                        chargingState = ChargingState.Unnoticed;
                    break;
            }

            // Other modifications based on charge state:
            // drawRect must be set to the regular Rectangle if the beast isn't vibrating
            // Friction must be applied when the beast isn't charging
            if (chargingState != ChargingState.Readying)
                drawRect = Rectangle;
            if (chargingState != ChargingState.Charging)
            {
                if (velocity != Vector2.Zero)
                {
                    velocity /= 1.075f;
                    ApplyFriction(gameTime);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            
            if (isAlive)
            {
                spriteBatch.Draw(sprite, drawRect, color);
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
