using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using CrossBoa.Enemies;
using CrossBoa.Interfaces;

namespace CrossBoa
{
    /// <summary>
    /// A health collectible. It occasionally replaces regular collectibles
    /// and restores health when collected by the player.
    /// </summary>
    class HealthCollectible : Collectible
    {
        // ~~~ THIS OBJECT HAS NO UNIQUE FIELDS OR PROPERTIES ~~~

        public HealthCollectible(Texture2D sprite, Point size) :
            base(sprite, size)
        {
            // This object only calls its base constructor
        }

        /// <summary>
        /// Heals the player when collected.
        /// </summary>
        public override void GetCollected()
        {
            if (IsActive)
            {
                Game1.Player.CurrentHealth++;
                Game1.Exp--;
            }
            base.GetCollected();
        }
    }
}
