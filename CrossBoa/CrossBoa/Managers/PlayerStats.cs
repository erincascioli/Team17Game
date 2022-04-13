using System;
using System.Collections.Generic;
using System.Text;

namespace CrossBoa.Managers
{
    /// <summary>
    /// A manager that contains properties for all of the player's stats within the game
    /// </summary>
    public static class PlayerStats
    {
        private static float playerInvulnerabilityTime = 3f;
        private static float arrowVelocity = 400f;
        private static float arrowDespawnTime = 20f;

        /// <summary>
        /// The speed that the player's arrow will fire at
        /// </summary>
        public static float PlayerInvulnerabilityTime
        {
            get { return playerInvulnerabilityTime; }
            set { playerInvulnerabilityTime = value; }
        }

        /// <summary>
        /// The speed that the player's arrow will fire at
        /// </summary>
        public static float ArrowVelocity
        {
            get { return arrowVelocity; }
            set { arrowVelocity = value; }
        }

        /// <summary>
        /// The time before the player arrow will return automatically
        /// </summary>
        public static float ArrowDespawnTime
        {
            get { return arrowDespawnTime; }
            set { arrowDespawnTime = value; }
        }
    }
}
