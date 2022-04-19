using System;
using System.Collections.Generic;
using CrossBoa.Enemies;
using Microsoft.Xna.Framework;

namespace CrossBoa.Upgrades
{
    /// <summary>
    /// The quality of this upgrade
    /// </summary>
    public enum UpgradeQuality
    {
        Bronze,
        Silver,
        Gold
    }

    /// <summary>
    /// The type of upgrade
    /// </summary>
    public enum UpgradeType
    {
        OnShot,
        OnKill,
        StatBoost
    }

    public static class UpgradeManager
    {
        private static Dictionary<string, Upgrade> lockedUpgrades = new Dictionary<string, Upgrade>()
        {
            {"Multishot", new Upgrade("Multishot", "Fires two additional arrows every few seconds", Multishot, UpgradeType.OnShot, Game1.playerArrowSprite)},
            {"Vampirism", new Upgrade("Vampirism", "5% Chance to heal when killing an enemy", Vampirism, UpgradeType.OnKill, Game1.whiteSquareSprite)},
            {"Placeholder1", new Upgrade("Placeholder1", "5% Chance to heal when killing an enemy", Vampirism, UpgradeType.OnKill, Game1.whiteSquareSprite)},
            {"Placeholder2", new Upgrade("Placeholder2", "5% Chance to heal when killing an enemy", Vampirism, UpgradeType.OnKill, Game1.whiteSquareSprite)},
            {"Placeholder3", new Upgrade("Placeholder3", "5% Chance to heal when killing an enemy", Vampirism, UpgradeType.OnKill, Game1.whiteSquareSprite)},
            {"Placeholder4", new Upgrade("Placeholder4", "5% Chance to heal when killing an enemy", Vampirism, UpgradeType.OnKill, Game1.whiteSquareSprite)},
            {"Placeholder5", new Upgrade("Placeholder5", "5% Chance to heal when killing an enemy", Vampirism, UpgradeType.OnKill, Game1.whiteSquareSprite)},
        };

        // Fields required to store behavior for upgrades
        public static PlayerArrow[] multishotArrows = null;

        /// <summary>
        /// Chooses 3 random upgrades to display to the player
        /// </summary>
        public static Upgrade[] GenerateUpgradeChoices()
        {
            // Put all the locked upgrade names into a list
            List<string> lockedUpgradeNames = new List<string>(lockedUpgrades.Count);
            lockedUpgradeNames.AddRange(lockedUpgrades.Keys);

            // Shuffle the locked upgrade names list
            for (int i = lockedUpgradeNames.Count - 1; i >= 1; i--)
            {
                // Generate a random number
                int j = Game1.RNG.Next(0, i + 1);

                // Swap this element with the random chosen element
                (lockedUpgradeNames[i], lockedUpgradeNames[j]) = (lockedUpgradeNames[j], lockedUpgradeNames[i]);
            }

            // Make the output array, and return it
            Upgrade[] output = new[]
            {
                lockedUpgrades[lockedUpgradeNames[0]],
                lockedUpgrades[lockedUpgradeNames[1]],
                lockedUpgrades[lockedUpgradeNames[2]]
            };

            return output;
        }

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
                case UpgradeType.OnKill:
                    Enemy.OnKill += upgrade.Effect;
                    break;
            }

            // Remove this upgrade from the locked upgrades list
            lockedUpgrades.Remove(upgradeName);
        }

        #region Upgrade Delegate Methods
        /// <summary>
        /// Shoot 3 arrows every shot
        /// </summary>
        public static void Multishot()
        {
            if (multishotArrows == null || multishotArrows[0].FlaggedForDeletion)
            {
                // Spawn 2 arrows facing 15 degrees from the center
                multishotArrows = new[]
                {
                    new PlayerArrow(Game1.playerArrowSprite, new Point(48), false) {DirectionOffset = -0.261799388f},
                    new PlayerArrow(Game1.playerArrowSprite, new Point(48), false) {DirectionOffset = 0.261799388f},
                };

                // Subscribe the new arrows to the crossbow shoot event so they get shot
                foreach (PlayerArrow arrow in multishotArrows)
                {
                    Game1.Crossbow.FireArrows += arrow.GetShot;
                }

                // Add the arrows to the main list in Game1
                Game1.playerArrowList.AddRange(multishotArrows);
            }
        }

        /// <summary>
        /// 5% Chance to heal when killing an enemy
        /// </summary>
        public static void Vampirism()
        {
            if(Game1.RNG.NextDouble() <= 0.05)
               Game1.Player.CurrentHealth++;
        }

        #endregion
    }
}
