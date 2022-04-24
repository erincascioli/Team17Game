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
        public HealthCollectible(Texture2D sprite, Point size) :
            base(sprite, size)
        {

        }

        public override void GetCollected()
        {
            if (IsActive)
                Game1.Player.CurrentHealth++;
            base.GetCollected();
        }
    }
}
