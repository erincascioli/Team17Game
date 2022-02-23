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
    /// Restrictions: This class must only be created once
    /// </summary>
    public class CollisionManager
    {
        private Player player;
        private CrossBow crossbow;
        private Projectile arrow;
        private List<Enemy> enemies;
        private List<Projectile> enemyProjectiles;

        public Player Player
        {
            get { return player; }
        }

        public CrossBow Crossbow
        {
            get { return crossbow; }
        }

        public Projectile Arrow
        {
            get { return arrow; }
            set { arrow = value; }
        }

        public CollisionManager(Player character, CrossBow weapon, Projectile bolt)
        {
            // All fields get a reference location
            player = character;
            crossbow = weapon;
            arrow = bolt;

            // Lists are created
            enemies = new List<Enemy>();
            enemyProjectiles = new List<Projectile>();
        }
    }
}
