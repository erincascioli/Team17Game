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
        private static float arrowVelocity = 360f;
        private static float arrowDespawnTime = 30f;

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
