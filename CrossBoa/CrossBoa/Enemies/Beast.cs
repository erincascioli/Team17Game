using System;
using CrossBoa.Interfaces;
using CrossBoa.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace CrossBoa.Enemies
{
    /// <summary>
    /// The current charging state of the beast.
    /// Unnoticed: The beast is idle.
    /// Readying: The beast vibrates.
    /// Charging: The beast moves towards the player fast.
    /// Tired: The beast moves towards the player slower.
    /// </summary>
    enum ChargingState
    {
        Unnoticed,
        Readying,
        Charging,
        Tired
    }

    public enum BeastAnimState
    {
        FacingDown,
        FacingUp,
        FacingLeft,
        FacingRight
    }


    /// <summary>
    /// A Beast (formerly a skeleton). It stands still until you enter
    /// its provoke radius, at which point it will charge at you constantly.
    /// Written by: Leo Schindler-Gerendasi.
    /// </summary>
    class Beast : Enemy
    {
        // ~~~ FIELDS ~~~
        private const float MovementForce = 1000f;
        private const float FrictionForce = 100f;
        private const float MovementMaxSpd = 250f;
        private const float KnockbackMaxSpd = 450f;

        // Movement fields
        private Player target;
        private float knockbackTimer;
        private float chargeTimer;
        private float provokeRadius;
        private ChargingState chargingState;
        private bool hasCollided;
        //private float chargeDirection;

        // Other fields
        private Rectangle drawRect;

        // Animation Fields
        private BeastAnimState animationState;
        private int currentAnimationFrame;
        private double previousFrameChangeTime;

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
                return chargingState == ChargingState.Charging || chargingState == ChargingState.Tired;
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
            maxSpeed = MovementMaxSpd;
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
            float movForce = MovementForce;
            if (chargingState == ChargingState.Tired)
                movForce *= 0.85f;
            
            ApplyForce(Helper.DirectionBetween(
                                new Point((int)position.X + Width / 2, (int)position.Y + Height / 2),
                                new Point(target.Rectangle.Center.X, target.Rectangle.Center.Y)), 
                       movForce);
            //velocity *= 5f;
            /*Point player = new Point(target.Rectangle.Center.X, target.Rectangle.Center.Y);

            if (Math.Abs(Helper.Distance(new Point((int)position.X + Width / 2, (int)position.Y + Height / 2), player)) > 35)
            {
            }*/
        }

        public override void GetKnockedBack(ICollidable other, float force)
        {
            knockbackTimer = 0;
            maxSpeed = KnockbackMaxSpd;
            if (other is Projectile && !(other is PlayerArrow))
                chargingState = ChargingState.Unnoticed;
            base.GetKnockedBack(other, force * 1000);
        }

        public override void TakeDamage(int damage)
        {
            // Cool feature to increase the beast's provoke radius
            // the more damage it takes
            provokeRadius += 50f;

            // Provokes the beast if it's not yet provoked, 
            // and un-tireds[sic] the beast if it is.
            if (chargingState == ChargingState.Unnoticed || chargingState == ChargingState.Charging)
            {
                if (chargingState == ChargingState.Unnoticed)
                {
                    SoundManager.beastCharge.Play(.2f, 0f, 0f);
                }

                chargingState = ChargingState.Readying;
                chargeTimer = 0f;
            }
            else
            {
                chargingState = ChargingState.Charging;
                chargeTimer = 2f;
            }
            base.TakeDamage(damage);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Knockback stuff
            if (knockbackTimer < 0.25)
            {
                if (velocity != Vector2.Zero)
                {
                    velocity /= 1.1f;
                    ApplyFriction(gameTime);
                }
                drawRect = Rectangle;
                knockbackTimer += totalSeconds;
            }
            else if (maxSpeed != MovementMaxSpd)
                maxSpeed = MovementMaxSpd;
            else if (Math.Abs(DistanceBetween) < provokeRadius && chargingState == ChargingState.Unnoticed)
            {
                SoundManager.beastCharge.Play(.6f, 0f, 0f);
                chargingState = ChargingState.Readying;
                chargeTimer = 0;
            }
            else
            {
                // State machine to deal with beast's current movement state
                // in all cases: increase the charge timer, as that is the basis for this chunk of code
                // from 0 - 0.5 seconds: ready the attack (vibrate)
                // from 0.5 - 3 seconds: charge (move towards the player fast)
                //      also check for collisions. If the beast runs into a wall, set charge timer
                //      to 3 seconds immediately (stop the charge) and knock the beast back
                // from 3 seconds onwards: charge towards the player at a slower speed until they 
                if (chargingState != ChargingState.Unnoticed)
                    chargeTimer += totalSeconds;

                switch (chargingState)
                {
                    case ChargingState.Readying:
                        Point vibrationMod =
                            new Point(Game1.RNG.Next(-1, 1), Game1.RNG.Next(-1, 1));
                        drawRect = new Rectangle(
                        new Point(Rectangle.X + vibrationMod.X, Rectangle.Y + vibrationMod.Y),
                        Rectangle.Size);
                        if (chargeTimer >= 0.5f)
                        {
                            /*chargeDirection = Helper.DirectionBetween(
                                new Point((int)position.X + Width / 2, (int)position.Y + Height / 2),
                                new Point(target.Rectangle.Center.X, target.Rectangle.Center.Y));*/
                            chargingState = ChargingState.Charging;
                            //SoundManager.beastCharge.Play(.2f, 0f, 0f);
                        }

                        break;

                    case ChargingState.Charging:
                        Move();
                        if (hasCollided)
                        {
                            hasCollided = false;
                            chargeTimer = 1.5f;
                        }
                        /*if (chargeTimer >= 3f && Math.Abs(DistanceBetween) < provokeRadius * 1.5)
                        {
                            chargeTimer = 3f;
                            chargingState = ChargingState.Tired;
                        }*/
                        break;

                    case ChargingState.Tired:
                        Move();
                        if (Math.Abs(DistanceBetween) < provokeRadius)
                        {
                            chargingState = ChargingState.Charging;
                        }
                        else if (chargeTimer >= 5f && Math.Abs(DistanceBetween) < provokeRadius * 2)
                            chargingState = ChargingState.Unnoticed;
                        break;
                }
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

            UpdateAnimations(gameTime);
        }

        /// <summary>
        /// Updates the beast's animations based on the current state
        /// </summary>
        private void UpdateAnimations(GameTime gameTime)
        {
            // Only face the player and move directions when charging
            if (chargingState == ChargingState.Charging)
            {
                // Change frame every 0.12 seconds
                if (gameTime.TotalGameTime.TotalSeconds > previousFrameChangeTime + 0.12)
                {
                    previousFrameChangeTime = gameTime.TotalGameTime.TotalSeconds;

                    currentAnimationFrame++;
                    if (currentAnimationFrame > 3)
                        currentAnimationFrame = 0;
                }
            }
            
            // If not charging, reset frame to 0
            else
            {
                currentAnimationFrame = 0;
            }

            // Look at the player if readying or charging
            if (chargingState != ChargingState.Readying && chargingState != ChargingState.Charging) return;
            Point differenceBetweenPlayer = Game1.Player.Rectangle.Center - this.Rectangle.Center;

            // Enemy should face left or right
            if (MathF.Abs(differenceBetweenPlayer.X) > MathF.Abs(differenceBetweenPlayer.Y))
            {
                animationState = differenceBetweenPlayer.X > 0 ?
                    BeastAnimState.FacingRight :
                    BeastAnimState.FacingLeft;
            }

            // Enemy should face up or down
            else
            {
                animationState = differenceBetweenPlayer.Y > 0 ?
                    BeastAnimState.FacingDown :
                    BeastAnimState.FacingUp;
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isAlive)
            {
                spriteBatch.Draw(sprite,
                    drawRect,
                    new Rectangle((int)animationState * 16, currentAnimationFrame * 16, 16, 16),
                    color);
            }
        }

        /// <summary>
        /// A helper method to convert a Vector2 to a Point,
        /// usable by the Helper method DirectionBetween().
        /// </summary>
        /// <param name="position">The Vector2 in question.</param>
        /// <returns>A point equivalent to that Vector2.</returns>
        private static Point ToPoint(Vector2 position)
        {
            return new Point((int)position.X, (int)position.Y);
        }
    }
}
