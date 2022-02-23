using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    public class Slime : PhysicsObject, IEnemy
    {
        // ~~~ FIELDS ~~~
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

        // ~~~ CONSTRUCTORS ~~~
        public Slime(int health, Texture2D sprite, Rectangle rectangle, float movementForce, float maxSpeed, float friction) :
            base(sprite, rectangle, movementForce, maxSpeed, friction)
        {
            this.health = health;
            timeSinceMove = 0;
        }

        public void Move()
        {
            float direction;
            // Formula used for calculations: 
            // Cos(A) = (b^2 + c^2 - a^2) / 2bc
            // A = arccos the formula above
            double horizDist = targetX - position.X;
            double vertDist = targetY - position.Y;
            float totalDist = (float)Math.Sqrt(Math.Pow(vertDist, 2) +
                Math.Pow(horizDist, 2));
            double cosA = (Math.Pow(totalDist, 2) + Math.Pow(horizDist, 2) - Math.Pow(vertDist, 2))
                / (2 * horizDist * totalDist);
            if (targetY < position.Y)
                 direction = (float)Math.Acos(cosA) * -1;
            direction = (float)Math.Acos(cosA);

            ApplyForce(direction, 2.5f);
        }

        /// <summary>
        /// if the enemy is within the vicinity of the player,
        /// deals damage to them.
        /// </summary>
        /// <param name="player">The player to check the collision of.</param>
        public void DealContactDamage(Player player)
        {
            if (player.Rectangle.Intersects(this.Rectangle))
                return;
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(sprite,
                Rectangle,
                Color.Green);
        }

        public void Update(GameTime gameTime, Player player)
        {
            // Update the timer
            float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeSinceMove += totalSeconds;

            // If it's been at least 1 second since the last slime movement,
            // move the slime
            if (timeSinceMove >= 1)
            {
                targetX = (int)player.Position.X;
                targetY = (int)player.Position.Y;
                timeSinceMove = 0;
                Move();
            }
            ApplyFriction(gameTime);

            // Deal damage to the player if they're touching the slime
            DealContactDamage(player);
        }
    }
}
