using System;
using System.Collections.Generic;
using System.Text;

namespace CrossBoa
{
    interface IEnemy
    {
        /// <summary>
        /// Property to get the current health of the enemy.
        /// </summary>
        int Health { get; set; }

        /// <summary>
        /// Method on how the enemy moves.
        /// </summary>
        void Move();

        /// <summary>
        /// Method to deal damage on contact with the player.
        /// </summary>
        void DealDamage(Player player);
    }
}
