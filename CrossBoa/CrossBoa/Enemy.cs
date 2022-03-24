﻿using System;
using System.Collections.Generic;
using System.Text;
using CrossBoa.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrossBoa
{
    public class Enemy : PhysicsObject, ICollidable
    {
        protected int health;
        protected bool isAlive;

        private double hurtFlashTime;

        /// <summary>
        /// The hitbox for this object
        /// </summary>
        public virtual Rectangle Hitbox
        {
            get { return Rectangle; }
        }

        /// <summary>
        /// Property to get and set the current health of the enemy.
        /// </summary>
        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        /// <summary>
        /// Whether this enemy is alive or not
        /// </summary>
        public bool IsAlive
        {
            get { return isAlive; }
        }

        /// <summary>
        /// Constructs an Enemy
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="rectangle">A Rectangle containing this GameObject's position and size</param>
        /// <param name="health">The health this enemy will have when it spawns</param>
        /// <param name="friction">How fast this object will stop moving.</param>
        /// <param name="maxSpeed">The maximum speed this object can reach</param>
        public Enemy(Texture2D sprite, Rectangle rectangle, int health, float? maxSpeed, float friction) : base(sprite, rectangle, maxSpeed, friction)
        {
            this.health = health;
            this.isAlive = true;
        }

        /// <summary>
        /// Updates this object's physics and friction, and turns this enemy red when hit
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (hurtFlashTime > 0)
            {
                hurtFlashTime -= gameTime.ElapsedGameTime.TotalSeconds;
                color = Color.Red;
            }
            else
            {
                color = Color.White;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws this GameObject to the screen
        /// </summary>
        /// <param name="spriteBatch">A reference to the SpriteBatch</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if(isAlive)
                base.Draw(spriteBatch);
        }

        /// <summary>
        /// Determines how the enemy moves.
        /// </summary>
        public virtual void Move()
        {

        }

        /// <summary>
        /// Deals damage on contact with the player.
        /// </summary>
        public virtual void DealContactDamage(Player player)
        {
            player.TakeDamage(1);
        }

        /// <summary>
        /// Method to have the enemy take damage, and if their health reaches 0,
        /// kill them.
        /// </summary>
        public virtual void TakeDamage(int damage)
        {
            health -= damage;
            hurtFlashTime = 0.1f;
            if (health <= 0)
                Die();
        }

        /// <summary>
        /// Pools this enemy
        /// </summary>
        public virtual void Die()
        {
            isAlive = false;
            position = new Vector2(-1000, -1000);
        }

        /// <summary>
        /// Handles knockback when this enemy gets hit
        /// </summary>
        /// <param name="other">The object causing this enemy to be knocked back</param>
        /// <param name="force">How much force to knock this enemy back by</param>
        public virtual void GetKnockedBack(ICollidable other, float force)
        {
            // Knock this enemy backwards
            velocity = Vector2.Zero;
            ApplyForce(MathHelper.DirectionBetween(other.Hitbox.Center, this.Rectangle.Center), force);
        }
    }
}
