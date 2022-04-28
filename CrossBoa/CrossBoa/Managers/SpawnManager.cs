using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using CrossBoa.Enemies;
using CrossBoa.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa.Managers
{
    /// <summary>
    /// Author: Donovan Scullion
    /// Purpose: Manages enemy spawns and contains methods for spawning them
    /// Restrictions: None so far
    /// </summary>
    public static class SpawnManager
    {
        private static List<GameObject> gameObjectList;
        private static List<Tile> openSpots;

        public static List<GameObject> GameObjectList
        {
            set { gameObjectList = value; }
        }


        /// <summary>
        /// Spawns a slime enemy
        /// </summary>
        /// <param name="position">The position to spawn the slime at</param>
        public static void SpawnSlime(Point position)
        {
            int health = 2;
            int expMin = 4;
            int expMax = 7;
            if (Game1.isHardModeActive)
                health = 3;
            if (Game1.isHellModeActive)
            {
                expMin = 2;
                expMax = 4;
            }
            Slime newSlime = new Slime(
                Game1.slimeSpritesheet,
                Game1.slimeDeathSpritesheet,
                health,
                (expMin, expMax),
                new Rectangle(position, new Point(64, 64)));

            CollisionManager.AddEnemy(newSlime);
            gameObjectList.Add(newSlime);
        }

        /// <summary>
        /// Spawns a totem enemy
        /// </summary>
        /// <param name="position">The position to spawn the totem at</param>
        public static void SpawnSkull(Point position)
        {
            int health = 2;
            int expMin = 6;
            int expMax = 10;
            if (Game1.isHardModeActive)
                health = 3;
            if (Game1.isHellModeActive)
            {
                expMin = 3;
                expMax = 5;
            }
            Skull testSkull = new Skull(Game1.skullSpriteSheet,
                new Rectangle(position, new Point(64, 64)),
                health,
                (expMin, expMax));

            CollisionManager.AddEnemy(testSkull);
            gameObjectList.Add(testSkull);
        }

        public static void SpawnBeast(Point position)
        {
            int health = 3;
            int expMin = 10;
            int expMax = 20;
            if (Game1.isHardModeActive)
                health = 4;
            if (Game1.isHellModeActive)
            {
                expMin = 5;
                expMax = 10;
            }
            Beast newBeast = new Beast(
                Game1.beastSprite,
                Game1.slimeDeathSpritesheet,
                health,
                (expMin, expMax),
                new Rectangle(position, new Point(58, 58)));
            CollisionManager.AddEnemy(newBeast);
            gameObjectList.Add(newBeast);
        }

        public static void SpawnTarget(Point position)
        {
            Target newTarget = new Target(
                Game1.targetSprite,
                new Rectangle(position, new Point(64, 64)));
            CollisionManager.AddEnemy(newTarget);
            gameObjectList.Add(newTarget);
        }

        /// Currently useless due to how we moved forward on enemy placement
        public static void FillLevel()
        {
            int maxEnemies;
            int minEnemies;
            int maxSlimes;
            int maxTotems;
            int maxSkeletons;

            // Determines how many enemies may spawn.
            if (Game1.isHellModeActive)
            {
                maxSlimes = 6;
                maxSkeletons = 5;
                maxTotems = 6;
                minEnemies = 3 + LevelManager.Stage / 2;
                if (minEnemies > 10)
                    minEnemies = 10;
                maxEnemies = 4 + LevelManager.Stage;
                if (maxEnemies > 20)
                    maxEnemies = 20;
            }
            else
            {
                switch (LevelManager.Stage)
                {
                    case 1:
                    case 2:
                    case 3:
                        maxSlimes = 3;
                        maxSkeletons = 1;
                        maxTotems = 1;
                        maxEnemies = 3;
                        minEnemies = 2;
                        break;

                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        maxSlimes = 4;
                        maxSkeletons = 2;
                        maxTotems = 1;
                        maxEnemies = 4;
                        minEnemies = 2;
                        break;

                    case 8:
                    case 9:
                    case 10:
                        maxSlimes = 4;
                        maxSkeletons = 3;
                        maxTotems = 2;
                        maxEnemies = 4;
                        minEnemies = 3;
                        break;

                    case 11:
                    case 12:
                    case 13:
                    case 14:
                        maxSlimes = 5;
                        maxSkeletons = 3;
                        maxTotems = 2;
                        maxEnemies = 6;
                        minEnemies = 3;
                        break;

                    // Any subsequent levels
                    default:
                        maxSlimes = 5;
                        maxSkeletons = 3;
                        maxTotems = 3;
                        maxEnemies = 6;
                        minEnemies = 4;
                        break;
                }
                if (Game1.isHardModeActive)
                {
                    minEnemies++;
                    maxEnemies++;
                    maxSkeletons++;
                    maxSlimes++;
                    maxTotems++;
                }
            }
            

            // Actual values for the level
            int slimeAmount = 0;
            int totemAmount = 0;
            int skeletonAmount = 0;
            int enemyAmount = Game1.RNG.Next(minEnemies, maxEnemies + 1);
            int currentEnemyAmount = 0;

            while (currentEnemyAmount != enemyAmount)
            {
                switch (Game1.RNG.Next(0, 3))
                {
                    case 0:
                        if (slimeAmount < maxSlimes)
                        {
                            int index = Game1.RNG.Next(0, openSpots.Count);
                            Vector2 position = openSpots[index].Position;
                            openSpots.RemoveAt(index);

                            SpawnSlime(new Point((int)position.X, (int)position.Y));
                            currentEnemyAmount++;
                            slimeAmount++;
                        }
                        break;

                    case 1:
                        if (totemAmount < maxTotems)
                        {
                            int index = Game1.RNG.Next(0, openSpots.Count);
                            Vector2 position = openSpots[index].Position;
                            openSpots.RemoveAt(index);

                            SpawnSkull(new Point((int)position.X, (int)position.Y));
                            currentEnemyAmount++;
                            totemAmount++;
                        }
                        break;

                    case 2:
                        if (skeletonAmount < maxSkeletons)
                        {
                            int index = Game1.RNG.Next(0, openSpots.Count);
                            Vector2 position = openSpots[index].Position;
                            openSpots.RemoveAt(index);

                            SpawnBeast(new Point((int)position.X, (int)position.Y));
                            currentEnemyAmount++;
                            skeletonAmount++;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Purpose: Retrieves tiles that enemies are able to spawn on
        /// Restrictions: none
        /// </summary>
        public static void UpdateLevel()
        {
            openSpots = LevelManager.GetSafe();
        }
    }
}