using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    /// <summary>
    /// A slime. It moves towards the player in spurts
    /// and damages them on contact. Inherits from PhysicsObject.
    /// Writtern by: Leo Schindler-Gerendasi
    /// </summary>
    public class Slime : PhysicsObject, IEnemy
    {
        // ~~~ FIELDS ~~~
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

        private Color currentColor;

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

        /// <summary>
        /// Color of the slime ; test code
        /// </summary>
        Color IEnemy.CurrentColor 
        {
            get { return currentColor; }
            set { currentColor = value; }
        }

        // ~~~ CONSTRUCTORS ~~~
        public Slime(int health, Texture2D sprite, Rectangle rectangle, float movementForce, float maxSpeed,
            float friction, Player playerReference) :
            base(sprite, rectangle, maxSpeed, friction)
        {
            player = playerReference;
            this.movementForce = movementForce;
            this.health = health;
            timeSinceMove = 0;
            currentColor = Color.Green;
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
        /// If the enemy is within the vicinity of the player,
        /// deals damage to them.
        /// </summary>
        /// <param name="player">The player to check the collision of.</param>
        public void DealContactDamage(Player player)
        {
                player.TakeDamage(0);
                currentColor = Color.MediumVioletRed;
        }

        /// <summary>
        /// Draws the slime.
        /// </summary>
        /// <param name="sb">The active SpriteBatch.</param>
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(sprite,
                Rectangle,
                currentColor);
        }

        public override void Update(GameTime gameTime)
        {
            // Update the timer
            float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeSinceMove += totalSeconds;

            // If it's been at least 1 second since the last slime movement,
            // push the slime towards the player.
            if (timeSinceMove >= 1)
            {
                targetX = (int)player.Position.X;
                targetY = (int)player.Position.Y;
                timeSinceMove -= 1;
                Move();
            }
            ApplyFriction(gameTime);
            UpdatePhysics(gameTime);
        }
    }
}
