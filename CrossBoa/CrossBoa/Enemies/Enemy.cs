using System.Collections.Generic;
using CrossBoa.Interfaces;
using CrossBoa.Managers;
using CrossBoa.Upgrades;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrossBoa.Enemies
{
    public class Enemy : PhysicsObject, ICollidable
    {
        /// <summary>
        /// Invokes upgrades that affect when the player kills an enemy
        /// </summary>
        public static event UpgradeBehavior OnKill;

        protected List<Collectible> expReward;

        protected int health;
        protected bool isAlive;

        private double hurtFlashTime;

        /// <summary>
        /// The hitbox for this object
        /// </summary>
        public virtual Rectangle Hitbox
        {
            get { return Rectangle; }
        }

        /// <summary>
        /// Property to get and set the current health of the enemy.
        /// </summary>
        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        /// <summary>
        /// Whether this enemy is alive or not
        /// </summary>
        public bool IsAlive
        {
            get { return isAlive; }
        }

        /// <summary>
        /// Constructs an Enemy
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="rectangle">A Rectangle containing this GameObject's position and size</param>
        /// <param name="health">The health this enemy will have when it spawns</param>
        /// <param name="friction">How fast this object will stop moving.</param>
        /// <param name="maxSpeed">The maximum speed this object can reach</param>
        public Enemy(Texture2D sprite, Rectangle rectangle, int health, float? maxSpeed, float friction) : base(sprite, rectangle, maxSpeed, friction)
        {
            this.health = health;
            this.isAlive = true;

            expReward = new List<Collectible>(Game1.RNG.Next(4, 10));

            // Check the collectible list for inactive orbs that are not yet assigned to an enemy
            foreach (Collectible collectible in Game1.Collectibles)
            {
                if (!collectible.IsAssigned && !collectible.IsActive)
                {
                    expReward.Add(collectible);
                    collectible.IsAssigned = true;
                }

                if (expReward.Count >= expReward.Capacity)
                    break;
            }

            // If there are not enough collectibles instantiated, generate more until the list is full
            while (expReward.Count < expReward.Capacity)
            {
                Collectible newCollectible;
                if (Game1.RNG.Next(1,91) == 10)
                    newCollectible = new HealthCollectible(Game1.healthRecoverySprite, new Point(32));
                else
                    newCollectible = new Collectible(Game1.xpSprite, new Point(32));

                Game1.Collectibles.Add(newCollectible);
                expReward.Add(newCollectible);
            }
        }

        /// <summary>
        /// Updates this object's physics and friction, and turns this enemy red when hit
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (hurtFlashTime > 0)
            {
                hurtFlashTime -= gameTime.ElapsedGameTime.TotalSeconds;
                color = Color.Red;
            }
            else
            {
                color = Color.White;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws this GameObject to the screen
        /// </summary>
        /// <param name="spriteBatch">A reference to the SpriteBatch</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if(isAlive)
                base.Draw(spriteBatch);
        }

        /// <summary>
        /// Determines how the enemy moves.
        /// </summary>
        public virtual void Move()
        {

        }

        /// <summary>
        /// Deals damage on contact with the player.
        /// </summary>
        public virtual void DealContactDamage(Player player)
        {
            float dir = Helper.DirectionBetween(
                            new Point(Rectangle.X, Rectangle.Y),
                            new Point(Game1.Player.Rectangle.X, Game1.Player.Rectangle.Y));
            player.TakeDamage(1, dir);
        }

        /// <summary>
        /// Method to have the enemy take damage, and if their health reaches 0,
        /// kill them.
        /// </summary>
        public virtual void TakeDamage(int damage)
        {
            health -= damage;
            hurtFlashTime = 0.1f;
            if (health <= 0)
            {
                Die();
                if (this is Skull)
                {
                    SoundManager.totemDeath.Play(.3f, 0, 0);
                }
                else if (this is Beast)
                {
                    SoundManager.beastDeath.Play(.2f, 0, 0);
                }
                else
                {
                    SoundManager.targetDamage.Play(.4f, -.2f, 0);
                }
            }
        }

        /// <summary>
        /// Pools this enemy
        /// </summary>
        public virtual void Die()
        {
            // Invoke OnKill upgrades
            OnKill?.Invoke();

            // Spawn collectibles
            foreach (Collectible collectible in expReward)
            {
                collectible.Spawn(this);
                collectible.IsAssigned = false;
                collectible.IsActive = true;
            }

            // Disable this enemy
            isAlive = false;
            position = new Vector2(-1000, -1000);
        }

        /// <summary>
        /// Handles knockback when this enemy gets hit
        /// </summary>
        /// <param name="other">The object causing this enemy to be knocked back</param>
        /// <param name="force">How much force to knock this enemy back by</param>
        public virtual void GetKnockedBack(ICollidable other, float force)
        {
            // Knock this enemy backwards
            velocity = Vector2.Zero;
            ApplyForce(Helper.DirectionBetween(other.Hitbox.Center, this.Rectangle.Center), force);
        }
    }
}
