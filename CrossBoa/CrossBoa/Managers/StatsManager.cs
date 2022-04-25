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
        // Backing fields for stats that need additional logic
        private static float playerMaxSpeed = 300;

        // Base stats for additive upgrades
        public const float BasePlayerInvulnerabilityTime = 1.5f;
        public const float BaseArrowVelocity = 400f;
        public const float BaseArrowDespawnTime = 20f;
        public const float BasePlayerMovementForce = 5000;
        public const float BasePlayerMaxSpeed = 300;

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
        /// How quickly the player accelerates to their max speed
        /// </summary>
        public static float PlayerMovementForce { get; set; } = 5000;

        /// <summary>
        /// The maximum speed of the player
        /// </summary>
        public static float PlayerMaxSpeed
        {
            get { return playerMaxSpeed; }
            set
            {
                playerMaxSpeed = value;
                Game1.Player.MaxSpeed = value;
            }
        }

        /// <summary>
        /// Resets all stats that were modified
        /// </summary>
        public static void ResetStats()
        {
            PlayerInvulnerabilityTime = BasePlayerInvulnerabilityTime;
            ArrowVelocity = BaseArrowVelocity;
            ArrowDespawnTime = BaseArrowDespawnTime;
            PlayerMovementForce = BasePlayerMovementForce;
            PlayerMaxSpeed = BasePlayerMaxSpeed;
        }
    }
}
