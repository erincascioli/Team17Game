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
        private Projectile playerArrow;
        private List<IEnemy> enemies;
        private List<Projectile> enemyProjectiles;
        private List<Tile> levelObstacles;

        public Player Player
        {
            get { return player; }
        }

        public CrossBow Crossbow
        {
            get { return crossbow; }
        }

        public Projectile PlayerArrow
        {
            get { return playerArrow; }
            set { playerArrow = value; }
        }


        public CollisionManager(Player character, CrossBow weapon, Projectile bolt)
        {
            // All fields get a reference location
            player = character;
            crossbow = weapon;
            playerArrow = bolt;

            // Lists are created
            enemies = new List<IEnemy>();
            enemyProjectiles = new List<Projectile>();
        }

        /// <summary>
        /// Purpose: Checks for collisions between all things in the level
        /// Restrictions: Level collidables must be parsed in beforehand
        /// </summary>
        public void CheckCollision()
        {
            // Projectiles
            foreach (Projectile i in enemyProjectiles)
            {
                // First checks for player projectile collisions
                if (i.Hitbox.Intersects(player.Rectangle))
                {
                    i.HitSomething();
                }
                else
                {
                    // Next checks if any projectiles hit a wall/Obstacle
                    foreach(Tile j in levelObstacles)
                    {
                        if (i.Rectangle.Intersects(j.Rectangle))
                        {
                            i.HitSomething();
                        }
                    }
                }
            }

            // Enemies
            foreach (IEnemy i in enemies)
            {
                // with player
                if (player.Rectangle.Intersects(i.Rectangle))
                {
                    i.DealContactDamage(player);
                }
                else
                {
                    i.CurrentColor = Color.Green;
                }

                // with player arrow
                if (playerArrow != null && playerArrow.Hitbox.Intersects(i.Rectangle))
                {
                    // Health value not decided on yet
                    i.Health -= 1;

                    // Change enemy color
                    i.CurrentColor = Color.Red;
                }
            }

            // Player arrow with wall
            foreach (Tile i in levelObstacles)
            {
                if (playerArrow != null && playerArrow.Rectangle.Intersects(i.Rectangle))
                {
                    playerArrow.HitSomething();
                }
            }
        }

        public void Update()
        {
            // parses in level collidables; will likely be put somewhere else
            // eventually because it shouldn't happen every update
            levelObstacles = LevelManager.GetCollidables();
        }

        public void AddEnemy(IEnemy enemy)
        {
            enemies.Add(enemy);
        }
    }
}
