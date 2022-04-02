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
        private Player target;
        // No totem sprite yet
        // private Texture2D totemSprite;

        private const double TimePerShot = 1f;

        // ~~~ PROPERTIES ~~~
        public bool ReadyToFire
        {
            get
            {
                return timeSinceShot > TimePerShot;
            }
        }

        // ~~~ CONSTRUCTOR ~~~
        public Totem(Texture2D sprite, Rectangle rectangle, int health) :
            base(sprite, rectangle, health, null, 0)
        {
            timeSinceShot = 0f;
            isAlive = true;
            color = Color.White;
            target = Game1.Player;
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
                Helper.DirectionBetween(Rectangle.Location, target.Rectangle.Location),
                500);
        }

        public override void Update(GameTime gameTime)
        {
            timeSinceShot += gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
