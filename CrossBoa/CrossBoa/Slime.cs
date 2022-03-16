using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    enum EnemyState
    {
        Alive,
        Dead
    }

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
    public class Slime : PhysicsObject, IEnemy, ICollidable
    {
        // ~~~ FIELDS ~~~
        private const float timeBetweenJumps = 2.5f;
        private const float movementForce = 45000f;
        private const float frictionForce = 2000f;

        private Player player;

        /// <summary>
        /// The health of the slime.
        /// </summary>
        private int health;

        /// <summary>
        /// The X position of the target.
        /// </summary>
        private int targetX;

        /// <summary>
        /// The Y position of the target.
        /// </summary>
        private int targetY;

        /// <summary>
        /// The last time the slime made a push towards the player.
        /// </summary>
        private float timeSinceMove;

        private float hurtFlashTime;
            
        private EnemyState isAlive;
        private SlimeAnimState animationState;

        // ~~~ PROPERTIES ~~~
        /// <summary>
        /// The health of the slime. Get/set property.
        /// </summary>
        public int Health
        {
            get
            {
                return health;
            }
            set
            {
                health = value;
            }
        }

        public Rectangle Hitbox
        {
            get { return new Rectangle(Rectangle.X + 4, Rectangle.Y + 12, 56, 56); }
        }

        /// <summary>
        /// Color of the slime ; test code
        /// </summary>
        Color IEnemy.CurrentColor
        {
            get { return color; }
            set { color = value; }
        }

        // ~~~ CONSTRUCTORS ~~~
        public Slime(int health, Texture2D spriteSheet, Rectangle rectangle, Player playerReference) :
            base(spriteSheet, rectangle, null, frictionForce)
        {
            player = playerReference;
            this.health = health;
            timeSinceMove = 0.25f;
            color = Color.White;
            isAlive = EnemyState.Alive;
            animationState = SlimeAnimState.Resting;
        }

        /// <summary>
        /// Moves the slime towards the player.
        /// </summary>
        public void Move()
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
        /// Deals damage to the player that is currently in contact
        /// with the slime.
        /// </summary>
        /// <param name="player">The player to damage.</param>
        public void DealContactDamage(Player player)
        {
            player.TakeDamage(1);
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                isAlive = EnemyState.Dead;
                position = new Vector2(-1000, -1000);
            }
            hurtFlashTime = 0.1f;
        }

        /// <summary>
        /// Draws the slime.
        /// </summary>
        /// <param name="sb">The active SpriteBatch.</param>
        public override void Draw(SpriteBatch sb)
        {
            if (isAlive == EnemyState.Alive)
                switch (animationState)
                {
                    case SlimeAnimState.Resting:
                        sb.Draw(sprite, Rectangle, new Rectangle(0, 0, 16, 16), color);
                        break;

                    case SlimeAnimState.Jumping:
                        sb.Draw(sprite, Rectangle, new Rectangle(16, 0, 16, 16), color);
                        break;

                    case SlimeAnimState.Falling:
                        sb.Draw(sprite, Rectangle, new Rectangle(32, 0, 16, 16), color);
                        break;

                    case SlimeAnimState.Squished:
                        sb.Draw(sprite, Rectangle, new Rectangle(48, 0, 16, 16), color);
                        break;
                }
        }

        public override void Update(GameTime gameTime)
        {
            if (isAlive == EnemyState.Alive)
            {
                // Update the timer
                float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                timeSinceMove += totalSeconds;

                // If it's been at least 1 second since the last slime movement,
                // push the slime towards the player.
                if (timeSinceMove >= timeBetweenJumps)
                {
                    targetX = (int)player.Position.X;
                    targetY = (int)player.Position.Y;
                    timeSinceMove -= timeBetweenJumps;
                    Move();
                }
                ApplyFriction(gameTime);
                UpdatePhysics(gameTime);
                UpdateAnimations(gameTime);
            }
        }

        public void UpdateAnimations(GameTime gameTime)
        {
            // Movement animation state
            if (timeSinceMove < 0.25f)
                animationState = SlimeAnimState.Jumping;
            if (timeSinceMove >= 0.25f && timeSinceMove < 0.4f)
                animationState = SlimeAnimState.Falling;
            if (timeSinceMove >= 0.4f && timeSinceMove < 0.5f)
                animationState = SlimeAnimState.Squished;
            if (timeSinceMove >= 0.5f && timeSinceMove < timeBetweenJumps - 0.2f)
                animationState = SlimeAnimState.Resting;
            if (timeSinceMove >= timeBetweenJumps - 0.2f)
                animationState = SlimeAnimState.Squished;

            // Hurt time
            if (hurtFlashTime > 0)
            {
                color = Color.Red;
                hurtFlashTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                hurtFlashTime = 0;
                color = Color.White;
            }
        }
    }
}
