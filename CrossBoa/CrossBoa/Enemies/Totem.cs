using System;
using CrossBoa.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrossBoa.Enemies
{
    class Totem : Enemy, IShoot
    {
        // ~~~ FIELDS ~~~
        private double timeSinceShot;
        private Arrow projectile;

        private const double TimePerShot = 1f;

        /// <summary>
        /// The projectile fired by the totem. Get-only property.
        /// </summary>
        public Arrow TotemProjectile
        {
            get
            {
                return projectile;
            }
        }

        // ~~~ CONSTRUCTOR ~~~
        public Totem(Texture2D sprite, Rectangle rectangle, int health, Texture2D projectileSprite) :
            base(sprite, rectangle, health, null, 0)
        {
            timeSinceShot = 0f;
            projectile = new Arrow(projectileSprite, 
                new Rectangle(-100,
                              -100,
                              30,
                              30),
                new Vector2(0,0));
            isAlive = true;
            color = Color.White;
        }

        // ~~~ METHODS ~~~
        /// <summary>
        /// Handles knockback when this enemy gets hit
        /// </summary>
        /// <param name="other">The object causing this enemy to be knocked back</param>
        /// <param name="force">How much force to knock this enemy back by</param>
        public override void GetKnockedBack(ICollidable other, float force)
        {
            // This enemy does not get knocked back, therefore this override does not call base
        }

        public void Shoot(Arrow projectile)
        {
            timeSinceShot = 0f;
            projectile.GetShot(
                new Vector2(Rectangle.X + Width/2,
                            Rectangle.Y + Height/2),
                (float)(Math.PI/2), 500);
        }

        public override void Update(GameTime gameTime)
        {
            timeSinceShot += gameTime.ElapsedGameTime.TotalSeconds;
            if (timeSinceShot >= TimePerShot)
                Shoot(projectile);
        }
    }
}
