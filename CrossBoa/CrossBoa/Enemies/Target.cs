using System;
using CrossBoa.Enemies;
using CrossBoa.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace CrossBoa
{
    /// <summary>
    /// A Target. It does literally nothing and its sole purpose is to be destroyed.
    /// Author: Leo Schindler-Gerendasi
    /// </summary>
    class Target : Enemy
    {
        // ~~~ THIS OBJECT HAS NO UNIQUE FIELDS OR PROPERTIES ~~~

        // ~~~ CONSTRUCTOR ~~~
        public Target(Texture2D sprite, Rectangle rectangle) :
            base(sprite, rectangle, 1, null, 0)
        {
            isAlive = true;
            color = Color.White;
            expReward = new List<Collectible>(0);
        }

        // ~~~ METHODS ~~~
        /// <summary>
        /// Overrides DealContactDamage to deal no damage to the player.
        /// </summary>
        /// <param name="player">The player to not deal damage to.</param>
        public override void DealContactDamage(Player player)
        {
            float dir = Helper.DirectionBetween(
                            new Point(Rectangle.X, Rectangle.Y),
                            new Point(Game1.Player.Rectangle.X, Game1.Player.Rectangle.Y));
            player.TakeDamage(this, 0, dir);
        }

        /// <summary>
        /// Handles knockback when this object is hit.
        /// </summary>
        /// <param name="other">The object that it is colliding with.</param>
        /// <param name="force">The force to be pushed back by.</param>
        public override void GetKnockedBack(ICollidable other, float force)
        {
            // This enemy can't get knocked back, so this method doesn't call base
        }
    }
}
