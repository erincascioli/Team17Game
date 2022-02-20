using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    /// <summary>
    /// Author: Donovan Scullion
    /// Purpose: Tracks collisions between objects
    /// Restrictions: All fields must be instantiated before
    ///               this class is first used, I think.
    /// </summary>
    public static class CollisionManager
    {
        private static Player player;
        private static CrossBow crossbow;
        private static Projectile arrow;
        private static List<Enemy> enemies;
        private static List<Projectile> enemyProjectiles;

        static Player Player
        {
            get { return player; }
        }

        static CrossBow Crossbow
        {
            get { return crossbow; }
        }

        static Projectile Arrow
        {
            get { return arrow; }
            set { arrow = value; }
        }

        static CollisionManager()
        {

        }
    }
}
