﻿using System;
using System.Collections.Generic;
using CrossBoa.Enemies;
using CrossBoa.Managers;
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
        OnShot,         // Runs with CrossBow.OnShot event
        OnKill,         // Runs with Enemy.OnDeath event
        StatBoost,      // Runs once on unlock and adds stats to StatsManager
        SpecialEffect   // Flags a boolean inside of a specific class
    }

    public static class UpgradeManager
    {
        private static Dictionary<string, Upgrade> allUpgrades = new Dictionary<string, Upgrade>()
        {
            {"Vampirism", new Upgrade("Vampirism", "8% Chance to heal when killing an enemy", Vampirism, UpgradeType.OnKill, Game1.UpgradeBloodOrb)},
            {"Better Fletching", new Upgrade("Better Fletching", "Arrows travel 15% faster", BetterFletching, UpgradeType.StatBoost, Game1.UpgradeFeather)},
            {"Tooth Brooch", new Upgrade("Tooth Brooch", "Stay invincible for 40%\nlonger after being hit", ToothBrooch, UpgradeType.StatBoost, Game1.UpgradeSharkTooth)},
            {"Tail Extension", new Upgrade("Tail Extension", "Move 15% faster", TailExtension, UpgradeType.StatBoost, Game1.UpgradeSausage)},
            {"Fangs", new Upgrade("Fangs", "Damage enemies that hit you", Fangs, UpgradeType.SpecialEffect, Game1.UpgradeFang)},
            {"Time Shift", new Upgrade("Time Shift", "Move and shoot 8% faster", TimeShift, UpgradeType.StatBoost, Game1.UpgradePocketWatch)},
            {"Arrow Wrangling", new Upgrade("Arrow Wrangling", "The arrow returns from 40% farther away", ArrowWrangling, UpgradeType.StatBoost, Game1.UpgradeRope)},
            {"Overclock", new Upgrade("Overclock", "+4% arrow speed every kill\nResets each room", Overclock, UpgradeType.SpecialEffect, Game1.UpgradeGadget)},
            {"Hearty Meat", new Upgrade("Hearty Meat", "+1 Max Health", HeartyMeat, UpgradeType.StatBoost, Game1.UpgradeHeartMeat)},
            {"Slim Pickings", new Upgrade("Slim Pickings", "Choose a new upgrade after the next level", SlimPickings, UpgradeType.SpecialEffect, Game1.UpgradeBone)},
        };

        private static Dictionary<string, Upgrade> lockedUpgrades = new Dictionary<string, Upgrade>(allUpgrades);

        // Fields required to store behavior for upgrades
        public static PlayerArrow[] multishotArrows = null;
        private static int enemiesUntilVampirismProc = 4;
        public static float ArrowSpeedWithoutOverclock;

        /// <summary>
        /// Chooses 3 random upgrades to display to the player
        /// </summary>
        public static Upgrade[] GenerateUpgradeChoices()
        {
            Upgrade[] output;

            // Check if there are less than 3 upgrades left
            if (lockedUpgrades.Count < 3)
            {
                output = new Upgrade[lockedUpgrades.Count];

                int i = 0;
                foreach (Upgrade lockedUpgrade in lockedUpgrades.Values)
                {
                    output[i] = lockedUpgrade;
                    i++;
                }

                return output;
            }

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
            output = new[]
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

                // Special effect upgrades only modify a boolean
                case UpgradeType.SpecialEffect:
                    upgrade.Effect();
                    break;

                // Stat boost upgrades only run once
                case UpgradeType.StatBoost:
                    upgrade.Effect();
                    break;
            }

            // Remove this upgrade from the locked upgrades list
            lockedUpgrades.Remove(upgradeName);

            // Updates the overclock arrow speed
            if (LevelManager.HasOverclockUpgrade)
            {
                ArrowSpeedWithoutOverclock = StatsManager.ArrowSpeed;
            }
        }

        /// <summary>
        /// Resets all the upgrades
        /// </summary>
        public static void ResetUpgrades()
        {
            // Reset locked upgrades
            lockedUpgrades = new Dictionary<string, Upgrade>(allUpgrades);

            // Clear any events that might contain an upgrade
            foreach (Upgrade upgrade in allUpgrades.Values)
            {
                Game1.Crossbow.OnShot -= upgrade.Effect;
                Enemy.OnKill -= upgrade.Effect;
            }

            // Reset upgrade-specific fields
            Game1.Player.HasFangsUpgrade = false;
            LevelManager.HasOverclockUpgrade = false;

            enemiesUntilVampirismProc = 4;
            
            Enemy.OnKill -= OverclockOnKill;
        }

        #region Upgrade Behavior Methods
        /// <summary>
        /// Shoot 3 arrows every shot
        /// </summary>
        private static void Multishot()
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
        /// 8% Chance to heal when killing an enemy
        /// </summary>
        private static void Vampirism()
        {
            // Decrease vampirism tracking variable until it hits 0
            if (--enemiesUntilVampirismProc <= 0)
            {
                // Heal the player
                Game1.Player.CurrentHealth++;

                // Choose a random number of enemies from 1 to 25
                // to give the player health back after killing
                enemiesUntilVampirismProc = Game1.RNG.Next(1, 26);
            }
        }

        /// <summary>
        /// Arrows travel 15% faster
        /// </summary>
        private static void BetterFletching()
        {
            StatsManager.ArrowSpeed *= 1.15f;
        }

        /// <summary>
        /// Stay invincible for 40% longer after being hit
        /// </summary>
        public static void ToothBrooch()
        {
            StatsManager.PlayerInvulnerabilityTime += StatsManager.BasePlayerInvulnerabilityTime * 0.4f;
        }

        /// <summary>
        /// Move 15% faster
        /// </summary>
        private static void TailExtension()
        {
            StatsManager.PlayerMovementForce *= 1.075f;
            StatsManager.PlayerMaxSpeed *= 1.15f;
        }

        /// <summary>
        /// Damage enemies that hit you
        /// </summary>
        private static void Fangs()
        {
            Game1.Player.HasFangsUpgrade = true;
        }

        /// <summary>
        /// Move and shoot 8% faster
        /// </summary>
        private static void TimeShift()
        {
            StatsManager.PlayerMovementForce *= 1.04f;
            StatsManager.PlayerMaxSpeed *= 1.08f;

            StatsManager.ArrowSpeed *= 1.08f;
        }

        /// <summary>
        /// The arrow returns from 40% farther away.
        /// </summary>
        private static void ArrowWrangling()
        {
            StatsManager.ArrowReturnRadius *= 1.4f;
        }

        /// <summary>
        /// +4% arrow speed every kill. Resets each room.
        /// </summary>
        private static void Overclock()
        {
            // Subscribe the helper method to the OnKill event
            Enemy.OnKill += OverclockOnKill;

            ArrowSpeedWithoutOverclock = StatsManager.ArrowSpeed;

            // Flag CollisionManager boolean
            LevelManager.HasOverclockUpgrade = true;
        }

        /// <summary>
        /// +1 Max Health
        /// </summary>
        private static void HeartyMeat()
        {
            StatsManager.PlayerMaxHealth += 1;
            Game1.Player.CurrentHealth += 1;
        }

        /// <summary>
        /// Choose a new upgrade after the next level
        /// </summary>
        private static void SlimPickings()
        {
            Game1.QueueUpgrade();
        }

        #endregion




        // Specific upgrade helper methods
        /// <summary>
        /// Adds arrow speed each kill automatically
        /// </summary>
        public static void OverclockOnKill()
        {
            StatsManager.ArrowSpeed += ArrowSpeedWithoutOverclock * 0.04f;
        }
    }
}
