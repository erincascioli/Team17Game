using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    class Totem : GameObject, IEnemy, IShoot
    {
        // ~~~ FIELDS ~~~
        private int health;
        private Color currentColor;
        private float timeSinceShot;
        private bool isAlive;
        private Projectile projectile;

        private const float cooldownTimer = 2f;

        // ~~~ PROPERTIES ~~~
        /// <summary>
        /// The totem's current health. Get/set property.
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

        public bool IsAlive
        {
            get { return isAlive; }
        }

        /// <summary>
        /// The current color of the totem. Get/set property.
        /// </summary>
        public Color CurrentColor
        {
            get
            {
                return currentColor;
            }
            set
            {
                currentColor = value;
            }
        }

        /// <summary>
        /// The hitbox of the totem. Get-only property.
        /// </summary>
        public Rectangle Hitbox
        {
            get
            {
                return Rectangle;
            }
        }

        /// <summary>
        /// The projectile fired by the totem. Get-only property.
        /// </summary>
        public Projectile TotemProjectile
        {
            get
            {
                return projectile;
            }
        }
        // ~~~ CONSTRUCTOR ~~~
        public Totem(Texture2D sprite, Rectangle rectangle, Texture2D projectileSprite) :
            base(sprite, rectangle)
        {
            timeSinceShot = 0f;
            health = 3;
            projectile = new Projectile(projectileSprite, 
                new Rectangle(-100,
                              -100,
                              30,
                              30),
                new Vector2(0,0),
                false);
            isAlive = true;
        }

        // ~~~ METHODS ~~~
        /// <summary>
        /// This enemy doesn't move, hence this method does nothing.
        /// </summary>
        public void Move()
        {

        }

        /// <summary>
        /// This enemy can't get knocked back, hence this method does nothing.
        /// </summary>
        /// <param name="other">The other object interacting with this object.</param>
        /// <param name="force">The amount of force to move back by.</param>
        public void GetKnockedBack(ICollidable other, float force)
        {

        }

        /// <summary>
        /// Deal damage on contact with the player.
        /// </summary>
        /// <param name="player">The player to deal damage to.</param>
        public void DealContactDamage(Player player)
        {
            if (isAlive)
                player.TakeDamage(1);
        }

        /// <summary>
        /// Takes damage. If this would result in the totem's
        /// health being ≤ 0, destroy it.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                // Placeholder to kill the object immediately until its defeated animation is available
                isAlive = false;
            }
        }

        public void Shoot(Projectile projectile)
        {
            timeSinceShot = 0f;
            projectile.ChangeVelocity(
                new Vector2(Rectangle.X + Width/2,
                            Rectangle.Y + Height/2),
                (float)(Math.PI/2), 500);
        }

        /// <summary>
        /// Draws the object. No animation yet.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isAlive)
                base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeSinceShot += totalSeconds;
            if (timeSinceShot >= cooldownTimer)
                Shoot(projectile);
        }
    }
}
