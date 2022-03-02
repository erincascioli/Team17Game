using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    /// <summary>
    /// Written by Leo Schindler-Gerendasi
    /// </summary>
    public interface IEnemy
    {
        /// <summary>
        /// Property to get the current health of the enemy.
        /// </summary>
        int Health { get; set; }
        
        /// <summary>
        /// Property to get the current location of the enemy
        /// </summary>
        Rectangle Rectangle { get; }

        // Test code
        Color CurrentColor { get; set; }

        /// <summary>
        /// Method on how the enemy moves.
        /// </summary>
        void Move();

        /// <summary>
        /// Method to deal damage on contact with the player.
        /// </summary>
        void DealContactDamage(Player player);

        /// <summary>
        /// Method to have the enemy take damage, and if their health reaches 0,
        /// destroy them.
        /// </summary>
        void TakeDamage(int damage);
    }
}
