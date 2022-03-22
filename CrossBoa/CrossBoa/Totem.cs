using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    class Totem : Enemy, IShoot
    {
        // ~~~ FIELDS ~~~
        private double timeSinceShot;
        private Projectile projectile;

        private const double TimePerShot = 1f;

        // ~~~ CONSTRUCTOR ~~~
        public Totem(Texture2D sprite, Rectangle rectangle, int health) :
            base(sprite, rectangle, health, null, 0)
        {
            timeSinceShot = 0f;
            projectile = new Projectile(sprite, 
                new Rectangle(-100,
                              -100,
                              30,
                              30),
                new Vector2(0,0),
                false);
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

        public void Shoot(Projectile projectile)
        {
            timeSinceShot = 0f;
            projectile.ChangeVelocity(
                position, 0, 500);
        }

        public override void Update(GameTime gameTime)
        {
            timeSinceShot += gameTime.ElapsedGameTime.TotalSeconds;
            if (timeSinceShot >= TimePerShot)
                Shoot(projectile);
        }
    }
}
