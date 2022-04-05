using System;
using System.Collections.Generic;
using System.Text;
using CrossBoa.UpgradeTypes;
using Microsoft.Xna.Framework;

namespace CrossBoa.Managers
{
    public static class UpgradeManager
    {
        private static List<OnShotUpgrade> unlockedOnShotUpgrades = new List<OnShotUpgrade>();

        private static Dictionary<string, Upgrade> lockedUpgrades = new Dictionary<string, Upgrade>()
        {
            {"Multishot", new OnShotUpgrade("Multishot", "Fires two additional arrows", Multishot, Game1.whiteSquareSprite)}
        };

        /// <summary>
        /// Unlocks the specified upgrade
        /// </summary>
        /// <param name="upgradeName">The upgrade to unlock</param>
        public static void UnlockUpgrade(string upgradeName)
        {
            // Add the upgrade to its corresponding unlocked list
            unlockedUpgrades.Add(lockedUpgrades[upgradeName]);

            // Remove this upgrade from the locked upgrades list
            lockedUpgrades.Remove(upgradeName);
        }

        #region Upgrade Delegate Methods
        /// <summary>
        /// OnShotUpgrade that shoots multiple arrows
        /// </summary>
        public static void Multishot()
        {
            // Spawn 2 arrows facing 15 degrees from the center
            PlayerArrow[] newArrows =
            {
                new PlayerArrow(Game1.playerArrowSprite, new Point(40), false) {DirectionOffset = -0.261799388f},
                new PlayerArrow(Game1.playerArrowSprite, new Point(40), false) {DirectionOffset = 0.261799388f},
            };

            foreach (PlayerArrow arrow in newArrows)
            {
                Game1.Crossbow.FireArrows += arrow.GetShot;
            }

            Game1.playerArrowList.AddRange(newArrows);
        }
        #endregion
    }
}
