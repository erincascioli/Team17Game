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
        public const float BaseArrowVelocity = 450f;
        public const float BaseArrowDespawnTime = 20f;
        public const float BaseArrowBounceForce = 400f;
        public const float BaseArrowReturnRadius = 80;
        public const float BasePlayerMovementForce = 5000;
        public const float BasePlayerMaxSpeed = 300;

        // Backing fields for stats that need additional logic
        private static float playerMaxSpeed = BasePlayerMaxSpeed;

        /// <summary>
        /// The speed that the player's arrow will fire at
        /// </summary>
        public static float PlayerInvulnerabilityTime { get; set; } 
            = BasePlayerInvulnerabilityTime;

        /// <summary>
        /// The speed that the player's arrow will fire at
        /// </summary>
        public static float ArrowSpeed { get; set; } = BaseArrowVelocity;

        /// <summary>
        /// The time before the player arrow will return automatically
        /// </summary>
        public static float ArrowDespawnTime { get; set; } = BaseArrowDespawnTime;

        /// <summary>
        /// How much the arrow will bounce when it hits an object
        /// </summary>
        public static float ArrowBounceForce { get; set; } = BaseArrowBounceForce;

        /// <summary>
        /// The radius that the arrow will return to the player from
        /// </summary>
        public static float ArrowReturnRadius { get; set; } = BaseArrowReturnRadius;

        /// <summary>
        /// How quickly the player accelerates to their max speed
        /// </summary>
        public static float PlayerMovementForce { get; set; } = BasePlayerMovementForce;

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
            ArrowSpeed = BaseArrowVelocity;
            ArrowDespawnTime = BaseArrowDespawnTime;
            ArrowBounceForce = BaseArrowBounceForce;
            ArrowReturnRadius = BaseArrowReturnRadius;
            PlayerMovementForce = BasePlayerMovementForce;
            PlayerMaxSpeed = BasePlayerMaxSpeed;
        }
    }
}
