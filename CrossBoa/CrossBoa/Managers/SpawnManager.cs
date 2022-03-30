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
    /// Manages enemy spawns and contains methods for spawning them
    /// </summary>
    public static class SpawnManager
    {
        private static List<GameObject> gameObjectList;

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

            // Tracks wether enemy position should be allowed
            bool safe = true;

            //foreach
            CollisionManager.AddEnemy(newSlime);
            gameObjectList.Add(newSlime);
        }

        /// <summary>
        /// Spawns a totem enemy
        /// </summary>
        /// <param name="position">The position to spawn the totem at</param>
        public static void SpawnTotem(Point position)
        {
            Totem testTotem = new Totem(Game1.whiteSquareSprite,
                new Rectangle(new Point(1300, 300), position),
                3);

            CollisionManager.AddEnemy(testTotem);
            gameObjectList.Add(testTotem);
        }

        //public static void FillLevel
        //{

        //}
    }
}