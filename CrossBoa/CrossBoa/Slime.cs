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

    /// <summary>
    /// A slime. It moves towards the player in spurts
    /// and damages them on contact. Inherits from PhysicsObject.
    /// Writtern by: Leo Schindler-Gerendasi
    /// </summary>
    public class Slime : PhysicsObject, IEnemy, ICollidable
    {
        // ~~~ FIELDS ~~~
        private const float timeBetweenJumps = 1.6f;

        private Player player;

        private float movementForce;

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

        /// <summary>
        /// The color of the slime.
        /// </summary>
        private Color currentColor;

        private EnemyState isAlive;

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
            get { return Rectangle; }
        }

        /// <summary>
        /// Color of the slime ; test code
        /// </summary>
        Color IEnemy.CurrentColor 
        {
            get { return currentColor; }
            set { currentColor = value; }
        }

        // ~~~ CONSTRUCTORS ~~~
        public Slime(int health, Texture2D sprite, Rectangle rectangle, float movementForce,
            float friction, Player playerReference) :
            base(sprite, rectangle, null, friction)
        {
            player = playerReference;
            this.movementForce = movementForce;
            this.health = health;
            timeSinceMove = 0;
            currentColor = Color.Green;
            isAlive = EnemyState.Alive;
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
            player.TakeDamage(0);
            currentColor = Color.MediumVioletRed;
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                isAlive = EnemyState.Dead;

            }
        }

        /// <summary>
        /// Draws the slime.
        /// </summary>
        /// <param name="sb">The active SpriteBatch.</param>
        public override void Draw(SpriteBatch sb)
        {
            if (isAlive == EnemyState.Alive)
                sb.Draw(sprite,
                Rectangle,
                currentColor);
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
            }
        }
    }
}
