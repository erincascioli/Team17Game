﻿using System.Collections.Generic;
using System.Net.Mime;
using System.Security.Cryptography;
using CrossBoa.Enemies;
using CrossBoa.Interfaces;
using CrossBoa.Upgrades;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace CrossBoa.Managers
{
    /// <summary>
    /// Author: Donovan Scullion
    /// Purpose: Tracks collisions between objects
    /// Restrictions: This class must only be created once
    /// </summary>
    public static class CollisionManager
    {
        private static CrossBow crossbow;
        private static List<Enemy> enemies;
        private static List<Projectile> enemyProjectiles;
        private static List<Tile> levelObstacles;
        private static int alternate;

        private static int stagesUntilHealthPickup = 2;

        // Every field requires a reference

        public static CrossBow Crossbow
        {
            get { return crossbow;}
            set { crossbow = value; }
        }

        static CollisionManager()
        {
            // Lists are created
            enemies = new List<Enemy>();
            enemyProjectiles = new List<Projectile>();

            alternate = 0;
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
                if(!i.IsActive)
                    continue;

                // First checks for player projectile collisions
                if (i.Hitbox.Intersects(Game1.Player.Hitbox) && !Game1.Player.IsInvincible)
                {
                    if (isInvincibilityActive) continue;

                    i.HitSomething();
                    Game1.Player.TakeDamage(null, 1, i.Direction);
                }
                else
                {
                    // Next checks if any projectiles hit a wall/Obstacle
                    foreach (Tile j in levelObstacles)
                    {
                        if (!i.Hitbox.Intersects(j.Rectangle)) continue;
                        
                        i.HitSomething();
                        SoundManager.fireDissipate.Play(.6f, 0, 0);
                    }
                }
            }

            // Temp data tracking
            List<Enemy> survivors = new List<Enemy>();

            // Enemies
            foreach (Enemy enemy in enemies)
            {
                // with player
                if (!isInvincibilityActive && Game1.Player.Hitbox.Intersects(enemy.Rectangle))
                {
                    enemy.DealContactDamage(Game1.Player);
                    // If the enemy is a Beast, have it get knocked back
                    if (enemy is Beast {InCharge: true} beast && !Game1.Player.IsInvincible)
                    {
                        enemy.GetKnockedBack(Game1.Player, 500);
                        beast.HasCollided = true;
                        Camera.ShakeScreen(10);
                    }
                }

                // with player arrow
                foreach (PlayerArrow playerArrow in Game1.playerArrowList)
                {
                    if (!playerArrow.IsInAir || !playerArrow.Hitbox.Intersects(enemy.Hitbox) ||
                        enemy.Health <= 0) continue;

                    // Health value not decided on yet
                    enemy.TakeDamage(1);


                    playerArrow.HitSomething();

                    // Knock the enemy back
                    enemy.GetKnockedBack(playerArrow, 35000);
                }

                // Tracks Living Enemies
                if (enemy.Health > 0)
                {
                    survivors.Add(enemy);
                }
            }

            // Enemy List is updated to only use living enemies
            enemies = survivors;

            // Collidable tiles
            foreach (Tile tile in levelObstacles)
            {
                if (tile == LevelManager.Exit && enemies.Count == 0 && !LevelManager.Exit.IsOpen && Game1.Player.CanMove)
                {
                    OpenExit();
                }

                foreach (PlayerArrow playerArrow in Game1.playerArrowList)
                {
                    if (!playerArrow.IsActive || !playerArrow.IsInAir ||
                        !playerArrow.Hitbox.Intersects(tile.Rectangle)) continue;

                    // Sound of an arrow hitting a wall
                    SoundManager.hitWall.Play(.3f, 0, 0);
                    playerArrow.HitSomething();
                }

                if (Game1.Player.Hitbox.Intersects(tile.Rectangle))
                {
                    EntityEnvironmentCollide(Game1.Player, tile);
                }

                // Enemies with tile
                foreach (Enemy enemy in enemies)
                {
                    if (!enemy.Hitbox.Intersects(tile.Rectangle)) continue;

                    EntityEnvironmentCollide(enemy, tile);

                    // If the enemy is a Beast, have it get knocked back
                    if (!(enemy is Beast {InCharge: true} beast)) continue;

                    // I am so good at coding
                    // -Leo
                    // I simplified the code
                    // -Donovan
                    // Thanks
                    // -Leo
                    // I have strike again
                    // -Donovan
                    enemy.GetKnockedBack(tile, 500000);
                    beast.HasCollided = true;
                    SoundManager.beastWallBump.Play();
                    Camera.ShakeScreen(10);
                }

                // Collectibles with tiles
                foreach (Collectible collectible in Game1.Collectibles)
                {
                    // Don't check if collectible is not moving
                    if (collectible.Velocity != Vector2.Zero && 
                        collectible.Hitbox.Intersects(tile.Rectangle))
                    {
                        EntityEnvironmentCollide(collectible, tile);
                    }
                }
            }

            // Player against an inactive player arrow
            foreach (PlayerArrow playerArrow in Game1.playerArrowList)
            {
                // If the arrow is on the ground and intersects with the player, give the arrow back
                if (playerArrow.IsMainArrow && !playerArrow.IsInAir &&
                    Game1.Player.Hitbox.Intersects(playerArrow.Hitbox))
                { 
                    playerArrow.GetPickedUp();
                }
            }

            // Removes open doors from collision
            if (LevelManager.Exit.IsOpen)
            {
                levelObstacles.Remove(LevelManager.Exit);
            }

            foreach (Collectible c in Game1.Collectibles)
            {
                if (Game1.Player.Hitbox.Intersects(c.Hitbox))
                {
                    c.GetCollected();
                }
            }

            // Removes inactive arrows from collision
            for (int i = 0; i < enemyProjectiles.Count; i++)
            {
                if (enemyProjectiles[i].IsActive) continue;

                enemyProjectiles.RemoveAt(i);
                i--;

            }
        }

        /// <summary>
        /// Purpose: draws all hitboxes to screen for debugging purposes
        /// Restrictions: none
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="hitBox"></param>
        /// <param name="arrowPoint"></param>
        public static void Draw(SpriteBatch sb, Texture2D hitBox, Texture2D arrowPoint)
        {
            sb.Draw(hitBox, Game1.Player.Hitbox, Color.White);

            foreach (Collectible c in Game1.Collectibles)
            {
                c.Draw(sb);
            }

            foreach (PlayerArrow playerArrow in Game1.playerArrowList)
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
                if (i is PlayerArrow)
                {
                    sb.Draw(arrowPoint, new Rectangle(i.Hitbox.X - 2, i.Hitbox.Y - 2, 5, 5), Color.Red);
                }
                else
                {
                    sb.Draw(hitBox, i.Hitbox, Color.Red);
                }
            }

            foreach (Enemy enemy in enemies)
            {

                if (enemy.Health > 0)
                    sb.Draw(hitBox, enemy.Hitbox, Color.White);
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
        public static void AddEnemy(Enemy enemy)
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
        /// Restrictions: only game objects with a hitbox may use it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="tile"></param>
        public static void EntityEnvironmentCollide<T>(T entity, Tile tile) where T : ICollidable
        {
            // Rectangle that holds the intersection area
            Rectangle overlap = Rectangle.Intersect(entity.Hitbox, tile.Rectangle);

            // Is the overlapping rectangle taller than it is wide
            if (overlap.Width > overlap.Height || (overlap.Width == overlap.Height && alternate == 0))
            {
                // Y value must be changed

                // Is the entity below the tile
                if (entity.Hitbox.Y >= overlap.Y)
                {
                    entity.Position =
                        new Vector2(entity.Position.X,
                            entity.Position.Y + overlap.Height); 
                }
                else
                {
                    entity.Position =
                        new Vector2(entity.Position.X,
                            entity.Position.Y - overlap.Height);
                }

                if (overlap.Width == overlap.Height)
                {
                    // Prevents stuttering glitch caused by
                    // width >= height
                    alternate = 1;
                }
            }
            else
            {
                // Is to the right of the tile ?
                entity.Position = entity.Hitbox.X >= overlap.X ? new Vector2(entity.Position.X + overlap.Width, entity.Position.Y) 
                    : new Vector2(entity.Position.X - overlap.Width, entity.Position.Y);

                if (overlap.Width == overlap.Height)
                {
                    // Prevents stuttering glitch caused by
                    // width >= height
                    alternate = 0;
                }
            }
        }

        /// <summary>
        /// Purpose: Takes all enemies out of the list
        /// Restrictions: none
        /// </summary>
        public static void ClearEnemiesList()
        {
            enemyProjectiles.Clear();
            enemies.Clear();
        }

        /// <summary>
        /// Runs when the exit opens
        /// </summary>
        private static void OpenExit()
        {
            LevelManager.Exit.ChangeDoorState();

            if (--stagesUntilHealthPickup <= 0 && LevelManager.Stage != 0)
            {
                // Spawn a health collectible
                HealthCollectible hc =
                    new HealthCollectible(Game1.healthRecoverySprite, new Point(48));
                Game1.Collectibles.Add(hc);
                hc.Spawn(Game1.gameRenderTarget.Bounds.Center);

                // Randomize the amount of time until the next health pickup between 3 and 5
                stagesUntilHealthPickup = Game1.RNG.Next(3, 6);
            }
        }
    }
}
