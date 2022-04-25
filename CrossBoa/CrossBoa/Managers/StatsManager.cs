using System;
using System.Collections.Generic;
using System.Text;

namespace CrossBoa.Managers
{
    /// <summary>
    /// A manager that contains properties for all of the player's stats within the game
    /// </summary>
    public static class StatsManager
    {
        // Base stats for additive upgrades
        public const float BasePlayerInvulnerabilityTime = 1.5f;
        public const float BaseArrowVelocity = 400f;
        public const float BaseArrowDespawnTime = 20f;

        /// <summary>
        /// The speed that the player's arrow will fire at
        /// </summary>
        public static float PlayerInvulnerabilityTime { get; set; } = 1.5f;

        /// <summary>
        /// The speed that the player's arrow will fire at
        /// </summary>
        public static float ArrowVelocity { get; set; } = 400f;

        /// <summary>
        /// The time before the player arrow will return automatically
        /// </summary>
        public static float ArrowDespawnTime { get; set; } = 20f;

        /// <summary>
        /// Resets all stats that were modified
        /// </summary>
        public static void ResetStats()
        {
            PlayerInvulnerabilityTime = BasePlayerInvulnerabilityTime;
            ArrowVelocity = BaseArrowVelocity;
            ArrowDespawnTime = BaseArrowDespawnTime;
        }
    }
}
