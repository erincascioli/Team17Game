using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CrossBoa.Upgrades
{
    public static class UpgradeManager
    {
        private static Dictionary<string, Upgrade> lockedUpgrades = new Dictionary<string, Upgrade>()
        {
            {"Multishot", new Upgrade("Multishot", "Fires two additional arrows", Multishot, UpgradeType.OnShot, Game1.whiteSquareSprite)}
        };

        /// <summary>
        /// Unlocks the specified upgrade
        /// </summary>
        /// <param name="upgradeName">The upgrade to unlock</param>
        public static void UnlockUpgrade(string upgradeName)
        {
            Upgrade upgrade = lockedUpgrades[upgradeName];

            // Add the upgrade to its corresponding unlocked list
            switch (upgrade.Type)
            {
                case UpgradeType.OnShot:
                    Game1.Crossbow.OnShot += upgrade.Effect;
                    break;
            }

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
                new PlayerArrow(Game1.playerArrowSprite, new Point(48), false) {DirectionOffset = -0.261799388f},
                new PlayerArrow(Game1.playerArrowSprite, new Point(48), false) {DirectionOffset = 0.261799388f},
            };

            foreach (PlayerArrow arrow in newArrows)
            {
                // Subscribe the new arrows to the crossbow shoot event so they get shot
                Game1.Crossbow.FireArrows += arrow.GetShot;

                // Subscribe the new arrows to the main arrow's recollect
                Game1.playerArrowList[0].OnPickup += arrow.Recollect;
            }

            Game1.playerArrowList.AddRange(newArrows);
        }
        #endregion
    }
}
