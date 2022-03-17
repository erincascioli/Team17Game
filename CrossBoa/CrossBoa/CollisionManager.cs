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
    public static class CollisionManager
    {
        private static Player player;
        private static CrossBow crossbow;
        private static Projectile playerArrow;
        private static List<IEnemy> enemies;
        private static List<Projectile> enemyProjectiles;
        private static List<Tile> levelObstacles;

        // Every field requires a reference
        public static Player Player
        {
            set { player = value; }
        }

        public static CrossBow Crossbow
        {
            set { crossbow = value; }
        }

        public static Projectile PlayerArrow
        {
            set { playerArrow = value; }
        }

        static CollisionManager()
        {
            // Lists are created
            enemies = new List<IEnemy>();
            enemyProjectiles = new List<Projectile>();
        }

        /// <summary>
        /// Purpose: Checks for collisions between all things in the level
        /// Restrictions: Level collidables must be parsed in beforehand
        /// </summary>
        public static void CheckCollision(bool isInvincibilityActive)
        {
            // enemy Projectiles
            foreach (Projectile i in enemyProjectiles)
            {
                // First checks for player projectile collisions
                if (i.Hitbox.Intersects(player.Hitbox))
                {
                    if (!isInvincibilityActive)
                    {
                        i.HitSomething();
                    }
                }
                else
                {
                    // Next checks if any projectiles hit a wall/Obstacle
                    foreach (Tile j in levelObstacles)
                    {
                        if (i.Hitbox.Intersects(j.Rectangle))
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
                if (!isInvincibilityActive && player.Hitbox.Intersects(i.Rectangle))
                {
                    i.DealContactDamage(player);
                }

                // with player arrow
                if (playerArrow != null && playerArrow.IsInAir &&
                    playerArrow.Hitbox.Intersects(i.Hitbox) && i.Health > 0)
                {
                    // Health value not decided on yet
                    i.TakeDamage(1);
                    playerArrow.HitSomething();
                }

            }

            // Collidable tiles
            foreach (Tile i in levelObstacles)
            {
                if (playerArrow != null && playerArrow.IsActive
                    && playerArrow.IsInAir && playerArrow.Hitbox.Intersects(i.Rectangle))
                {
                    playerArrow.HitSomething();
                }

                if (player.Hitbox.Intersects(i.Rectangle))
                {
                    EntityEnvironmentCollide<Player>(player, i);
                }

                // Enemies with tile
                foreach (IEnemy j in enemies)
                {
                    if (j.Hitbox.Intersects(i.Rectangle))
                    {
                        EntityEnvironmentCollide<IEnemy>(j, i);
                    }
                }
            }

            // Player against an inactive player's arrow
            if (!playerArrow.IsInAir && !crossbow.IsOnCooldown && player.Hitbox.Intersects(playerArrow.Hitbox))
            {
                crossbow.PickUpArrow();
                playerArrow.Disable();
            }
        }

        /// <summary>
        /// Purpose: draws all hitboxes to screen
        ///          best for debugging
        /// Restrictions: none
        /// </summary>
        /// <param name="sb"></param>
        public static void Draw(SpriteBatch sb, Texture2D hitBox, Texture2D arrowPoint)
        {
            sb.Draw(hitBox, player.Hitbox, Color.White);

            
            if (playerArrow != null)
            {
                // Make drawn hitbox size larger if hitbox is a point
                if (playerArrow.Hitbox.Size == Point.Zero)
                    sb.Draw(arrowPoint,
                    new Rectangle(playerArrow.Hitbox.X - (playerArrow.Hitbox.Width / 2) - 2,
                        playerArrow.Hitbox.Y - (playerArrow.Hitbox.Height / 2) - 2, playerArrow.Hitbox.Width + 4,
                        playerArrow.Hitbox.Height + 4), Color.Red);
                // Else draw hitbox normally
                else
                    sb.Draw(arrowPoint, playerArrow.Hitbox, Color.Red);
            }

            foreach (Projectile i in enemyProjectiles)
            {
                sb.Draw(arrowPoint, new Rectangle(i.Hitbox.X - 2, i.Hitbox.Y - 2, 5, 5), Color.Red);
            }

            foreach (IEnemy i in enemies)
            {

                if (i.Health > 0)
                    sb.Draw(hitBox, i.Hitbox, Color.White);
            }

            foreach (Tile i in levelObstacles)
            {
                sb.Draw(hitBox, i.Rectangle, Color.White);
            }
        }

        /// <summary>
        /// Purpose: Let's the manager add an enemy to check it's hitboxes
        /// Restrictions: none
        /// </summary>
        /// <param name="enemy"></param>
        public static void AddEnemy(IEnemy enemy)
        {
            enemies.Add(enemy);
        }

        /// <summary>
        /// Purpose: stores new projectiles every time an enemy shoots
        /// Restrictions: none
        /// </summary>
        /// <param name="projectile"></param>
        public static void AddProjectile(Projectile projectile)
        {
            enemyProjectiles.Add(projectile);
        }

        /// <summary>
        /// Purpose: Stores a levels innate collisions when it is first loaded
        /// Restrictions: should be called as soon as possible after loading a level
        /// </summary>
        public static void UpdateLevel()
        {
            levelObstacles = LevelManager.GetCollidables();
        }

        /// <summary>
        /// Purpose: Handles all colisions with tiles
        /// Restrictions: only objects with a hitbox may use it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="tile"></param>
        public static void EntityEnvironmentCollide<T>(T entity, Tile tile) where T : ICollidable 
        {
            // Rectangle that holds the intersection area
            Rectangle overlap = Rectangle.Intersect(entity.Hitbox, tile.Rectangle);


            // Is the overlapping rectangle taller than it is wide
            if (overlap.Width >= overlap.Height)
            {
                // Y value must be changed

                // Is the entity below the tile
                if (entity.Hitbox.Y >= overlap.Y)
                {
                    entity.Position = new Vector2(entity.Position.X, entity.Position.Y + overlap.Height);
                }
                else
                {
                    entity.Position = new Vector2(entity.Position.X, entity.Position.Y - overlap.Height);

                }
            }
            else
            {
                // X value must be changed

                // Is the entity below the rectangle
                if (entity.Hitbox.X >= overlap.X)
                {
                    entity.Position = new Vector2(entity.Position.X + overlap.Width, entity.Position.Y);
                }
                else
                {
                    entity.Position = new Vector2(entity.Position.X - overlap.Width, entity.Position.Y);
                }
            }
        }
    }
}
