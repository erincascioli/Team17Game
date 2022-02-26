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


        public CollisionManager(Player character, CrossBow weapon)
        {
            // All fields get a reference location
            player = character;
            crossbow = weapon;

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

        /// <summary>
        /// Purpose: draws all hitboxes to screen
        ///          best for debugging
        /// Restrictions: none
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb, Texture2D hitBox)
        {
            sb.Draw(hitBox, player.Hitbox, Color.Red);

            if(playerArrow != null)
            {
                sb.Draw(hitBox, playerArrow.Hitbox, Color.Red);
            }

            foreach(Projectile i in enemyProjectiles)
            {
                sb.Draw(hitBox, i.Hitbox, Color.Red);
            }

            foreach (IEnemy i in enemies)
            {
                // Doesn't use ICollidable yet
                sb.Draw(hitBox, i.Rectangle, Color.Red);
            }

            foreach (Tile i in levelObstacles)
            {
                sb.Draw(hitBox, i.Rectangle, Color.Red);
            }
        }


        /// <summary>
        /// Purpose: Let's the manager add an enemy to check it's hitboxes
        /// Restrictions: none
        /// </summary>
        /// <param name="enemy"></param>
        public void AddEnemy(IEnemy enemy)
        {
            enemies.Add(enemy);
        }

        /// <summary>
        /// Purpose: stores new projectiles every time an enemy shoots
        /// Restrictions: none
        /// </summary>
        /// <param name="projectile"></param>
        public void AddProjectile(Projectile projectile)
        {
            enemyProjectiles.Add(projectile);
        }

        /// <summary>
        /// Purpose: Stores a levels innate collisions when it is first loaded
        /// Restrictions: should be called as soon as possible after loading a level
        /// </summary>
        public void UpdateLevel()
        {
            levelObstacles = LevelManager.GetCollidables();
        }
    }
}
