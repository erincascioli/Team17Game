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


        static SpawnManager()
        {
        }

        
        /// <summary>
        /// Spawns a slime enemy
        /// </summary>
        /// <param name="position">The position to spawn the slime at</param>
        public static void SpawnSlime(Point position)
        {
            Slime newSlime = new Slime(
                Game1.slimeSpritesheet,
                Game1.slimeDeathSpritesheet,
                3,
                new Rectangle(position, new Point(64, 64)));

            CollisionManager.AddEnemy(newSlime);
            gameObjectList.Add(newSlime);
        }

        /// <summary>
        /// Spawns a totem enemy
        /// </summary>
        /// <param name="position">The position to spawn the totem at</param>
        public static void SpawnTotem(Point position)
        {
            Totem testTotem = new Totem(Game1.totemSprite,
                new Rectangle(new Point(1300, 300), position),
                3);

            CollisionManager.AddEnemy(testTotem);
            gameObjectList.Add(testTotem);
        }

        public static void SpawnSkeleton(Point position)
        {
            Skeleton newSkeleton = new Skeleton(
                Game1.skeletonSprite,
                Game1.slimeDeathSpritesheet,
                3,
                new Rectangle(position, new Point(64, 64)));
            CollisionManager.AddEnemy(newSkeleton);
            gameObjectList.Add(newSkeleton);
        }

        public static void SpawnTarget(Point position)
        {
            Target newTarget = new Target(
                Game1.whiteSquareSprite,
                new Rectangle(position, new Point(64, 64)));
            CollisionManager.AddEnemy(newTarget);
            gameObjectList.Add(newTarget);
        }

        /// Currently useless due to how we moved forward on enemy placement
        /*public static void FillLevel()
        {
            int maxEnemies;
            int minEnemies;
            int maxSlimes;
            int maxTotems;

            // Determines how many enemies may spawn
            switch (LevelManager.Stage)
            {
                case 1:
                case 2:
                    maxSlimes = 3;
                    maxTotems = 1;
                    maxEnemies = 3;
                    minEnemies = 2;
                    break;

                case 3:
                case 4:
                case 5:
                    maxSlimes = 4;
                    maxTotems = 1;
                    maxEnemies = 4;
                    minEnemies = 2;
                    break;

                case 6:
                case 7:
                    maxSlimes = 4;
                    maxTotems = 2;
                    maxEnemies = 4;
                    minEnemies = 3;
                    break;

                case 8:
                case 9:
                case 10:
                    maxSlimes = 5;
                    maxTotems = 2;
                    maxEnemies = 6;
                    minEnemies = 3;
                    break;

                // Any subsequent levels
                default:
                    maxSlimes = 5;
                    maxTotems = 3;
                    maxEnemies = 6;
                    minEnemies = 4;
                    break;
            }

            // Actual values for the level
            int slimeAmount = 0;
            int totemAmount = 0;
            int enemyAmount = Game1.RNG.Next(minEnemies, maxEnemies + 1);
            int currentEnemyAmount = 0;

            while (currentEnemyAmount != enemyAmount)
            {
                switch (Game1.RNG.Next(0, 2))
                {
                    case 0:
                        if (slimeAmount < maxSlimes)
                        {
                            int index = Game1.RNG.Next(0, openSpots.Count);
                            Vector2 position = openSpots[index].Position;

                            SpawnSlime(position.X, position.Y);
                        }
                            break;
                }
            }
        }*/

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