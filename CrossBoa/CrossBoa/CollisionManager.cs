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
            // enemy Projectiles
            foreach (Projectile i in enemyProjectiles)
            {
                // First checks for player projectile collisions
                if (i.Hitbox.Intersects(player.Hitbox))
                {
                    i.HitSomething();
                }
                else
                {
                    // Next checks if any projectiles hit a wall/Obstacle
                    foreach(Tile j in levelObstacles)
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
                if (player.Hitbox.Intersects(i.Rectangle))
                {
                    i.DealContactDamage(player);
                }
                else
                {
                    i.CurrentColor = Color.Green;
                }

                // with player arrow
                if (playerArrow != null && playerArrow.Hitbox.Intersects(i.Rectangle) && 
                    playerArrow.IsActive && i.Health > 0)
                {
                    // Health value not decided on yet
                    i.TakeDamage(1);

                    // Change enemy color
                    i.CurrentColor = Color.Red;
                }

            }

            // Collidable tiles
            foreach (Tile i in levelObstacles)
            {
                if (playerArrow != null && playerArrow.Hitbox.Intersects(i.Rectangle) && playerArrow.IsActive)
                {
                    playerArrow.HitSomething();
                }

                if (player.Hitbox.Intersects(i.Rectangle)) 
                {
                    // All collisions check by creating a smaller rectangle within the player character
                    // to check collisions with the wall against
                    // This prevents any part of the rectangle triggering a right side collision for example

                    // Against left wall
                    if (new Rectangle(player.Hitbox.X, player.Hitbox.Y + player.Hitbox.Height / 4, 
                        player.Hitbox.Width / 2, player.Hitbox.Height / 2).Intersects(i.Rectangle) && player.Hitbox.Left < i.Rectangle.Right)
                    {
                        player.Position = new Vector2(i.Rectangle.Right, player.Position.Y);
                    }
                    // Against right wall
                    if (new Rectangle(player.Hitbox.X + player.Hitbox.Width / 2, player.Hitbox.Y + player.Hitbox.Height / 4, 
                        player.Hitbox.Width / 2, player.Hitbox.Height / 2).Intersects(i.Rectangle) && player.Hitbox.Right > i.Rectangle.Left)
                    {
                        player.Position = new Vector2(i.Rectangle.Left - player.Width, player.Position.Y);
                    }
                    

                    // Player top of tile
                    if (new Rectangle(player.Hitbox.X + player.Hitbox.Width / 4, player.Hitbox.Y + player.Height / 2, 
                        player.Hitbox.Width / 2, player.Hitbox.Height / 2).Intersects(i.Rectangle) && player.Hitbox.Top < i.Rectangle.Bottom) 
                    {
                        player.Position = new Vector2(player.Position.X, i.Rectangle.Top - player.Height);
                    }

                    // Against bottom of tile
                    if (new Rectangle(player.Hitbox.X + player.Hitbox.Width / 4, player.Hitbox.Y, 
                        player.Hitbox.Width / 2, player.Hitbox.Height / 2).Intersects(i.Rectangle) && player.Hitbox.Bottom > i.Rectangle.Top)
                    {
                        player.Position = new Vector2(player.Position.X, i.Rectangle.Top + player.Height);
                    }
                }
            }

            // Player against an inactive player's arrow
            if (!playerArrow.IsInAir && !crossbow.IsOnCooldown && player.Hitbox.Intersects(playerArrow.Hitbox))
            {
                crossbow.PickUpArrow();
                playerArrow.IsActive = false;
            }
                
        }

        /// <summary>
        /// Purpose: draws all hitboxes to screen
        ///          best for debugging
        /// Restrictions: none
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb, Texture2D hitBox, Texture2D arrowPoint)
        {
            sb.Draw(hitBox, player.Hitbox, Color.White);

            if(playerArrow != null)
            {
                sb.Draw(arrowPoint, new Rectangle(playerArrow.Hitbox.X - 2, PlayerArrow.Hitbox.Y - 2, 5, 5), Color.Red);
            }

            foreach(Projectile i in enemyProjectiles)
            {
                sb.Draw(arrowPoint, new Rectangle(i.Hitbox.X - 2, i.Hitbox.Y - 2, 5, 5), Color.Red);
            }

            foreach (IEnemy i in enemies)
            {
                // Doesn't use ICollidable yet
                sb.Draw(hitBox, i.Rectangle, Color.White);
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
