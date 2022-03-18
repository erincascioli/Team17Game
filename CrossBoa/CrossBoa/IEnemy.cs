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
    public interface IEnemy : ICollidable
    {
        /// <summary>
        /// Property to get and set the current health of the enemy.
        /// </summary>
        int Health { get; set; }
        
        /// <summary>
        /// Property to get the current location of the enemy
        /// </summary>
        Rectangle Rectangle { get; }

        /// <summary>
        /// Whether this enemy is alive or not
        /// </summary>
        bool IsAlive { get; }

        // Test code
        Color CurrentColor { get; set; }

        //Vector2 Position { get; set; }

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

        /// <summary>
        /// Handles knockback when this enemy gets hit
        /// </summary>
        /// <param name="other">The object causing this enemy to be knocked back</param>
        /// <param name="force">How much force to knock this enemy back by</param>
        void GetKnockedBack(ICollidable other, float force);
    }
}
