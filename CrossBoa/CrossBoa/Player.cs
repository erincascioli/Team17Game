using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using CrossBoa.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    /// <summary>
    /// Author:  TacNayn
    /// Purpose: Allows the user to interact with the game world 
    /// </summary>
    public class Player : PhysicsObject, ICollidable
    {
        // Player stats
        private int maxHealth;
        private float invulnerabilityTime;
        private float dodgeCooldown;
        private float dodgeLength;
        private float movementForce;
        private float dodgeInvulnerabilityTime;
        private float dodgeSpeedBoost;

        // Player status tracking
        private int currentHealth;
        private float timeUntilDodge;
        private float timeLeftInvincible;
        private bool canMove;
        private bool flashFrames;
        private bool inDodge;

        private bool isFacingRight;
        private Vector2 dodgeVector;

        /// <summary>
        /// Checks if the player is dead
        /// </summary>
        public bool IsDead
        {
            get { return currentHealth <= 0; }
        }

        /// <summary>
        /// Checks if the player is currently invincible
        /// </summary>
        public bool IsInvincible
        {
            get { return timeLeftInvincible > 0 || dodgeInvulnerabilityTime > 0; }
        }

        /// <summary>
        /// The current health of the player
        /// </summary>
        public int CurrentHealth
        {
            get { return currentHealth; }
            set { currentHealth = value; }
        }

        /// <summary>
        /// The maximum health the player can have
        /// </summary>
        public int MaxHealth
        {
            get { return maxHealth; }
            set { maxHealth = value; }
        }

        /// <summary>
        /// The time the player has to wait after dodging
        /// </summary>
        public float DodgeCooldown
        {
            get { return dodgeCooldown; }
            set { dodgeCooldown = value; }
        }

        public bool CanMove
        {
            get { return canMove; }
            set { canMove = value; }
        }

        /// <summary>
        /// The hitbox for this object
        /// </summary>
        public Rectangle Hitbox
        {
            get
            {
                return new Rectangle(Rectangle.X, // x position 
                    Rectangle.Y + 16, // y position
                    Rectangle.Width, // width
                    Rectangle.Height - 16); // height
            }
        }

        /// <summary>
        /// Constructs a Player object
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="rectangle">A Rectangle containing this GameObject's position and size</param>
        /// <param name="friction">How fast this object will stop moving.</param>
        /// <param name="maxSpeed">The maximum speed this object can reach</param>
        /// <param name="movementForce">The force applied to the object when pressing the arrow keys</param>
        /// <param name="maxHealth">The maximum health of this player</param>
        /// <param name="invulnerabilityTime">How long the player should be invincible after being hit</param>
        /// <param name="dodgeCooldown">How long the player must wait before being able to dodge again</param>
        /// <param name="dodgeLength">How long the player will dodge for</param>
        /// <param name="dodgeSpeed">How quickly the player will move while dodging</param>
        public Player(Texture2D sprite, Rectangle rectangle, float movementForce, float maxSpeed, float friction,
            int maxHealth, float invulnerabilityTime, float dodgeCooldown, float dodgeLength, float dodgeSpeed) :
            base(sprite, rectangle, maxSpeed, friction)
        {
            this.movementForce = movementForce;
            this.maxHealth = maxHealth;
            currentHealth = maxHealth;
            this.invulnerabilityTime = invulnerabilityTime;
            this.dodgeCooldown = dodgeCooldown;
            this.dodgeLength = dodgeLength;
            dodgeSpeedBoost = dodgeSpeed;
            this.canMove = true;
            color = Color.White;
        }

        /// <summary>
        /// Updates this Object's fields 
        /// </summary>
        /// <param name="gameTime">A reference to the GameTime</param>
        public override void Update(GameTime gameTime)
        {
            KeyboardState kbState = Keyboard.GetState();

            // Update timers
            float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeUntilDodge -= totalSeconds;
            timeLeftInvincible -= totalSeconds;

            // Flash player if damaged
            if (IsInvincible && flashFrames)
            {
                color = new Color(Color.Black, 60);
            }
                
            else
            {
                color = Color.White;
            }

            flashFrames = !flashFrames;

            if (canMove)
            {
                // Check the player's input
                Vector2 movementVector = CheckMovementInput(kbState);

                // Apply the movement
                ApplyForce(movementVector * movementForce);

                if (kbState.IsKeyDown(Keys.A))
                    isFacingRight = false;
                if (kbState.IsKeyDown(Keys.D))
                    isFacingRight = true;
            }
            else if (inDodge)
            {
                dodgeInvulnerabilityTime -= totalSeconds;
                ApplyForce(dodgeVector * movementForce * 2);
            }

            // ~ Dodging code ~
            // Activate a dodge on space press if the player is past the dodge cooldown.
            Dodge(kbState);

            // If the player is in a dodge and their invincibility
            // frames run out, give control back to the player.
            if (inDodge && dodgeInvulnerabilityTime < 0)
            {
                canMove = true;
                maxSpeed /= dodgeSpeedBoost;
                inDodge = false;
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            if(isFacingRight)
                sb.Draw(sprite, Rectangle, null, color, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
            else
                sb.Draw(sprite, Rectangle, color);
        }

        /// <summary>
        /// Checks if the player is invincible and deals damage
        /// </summary>
        /// <param name="amount"></param>
        public void TakeDamage(int amount)
        {
            if (!IsInvincible)
            {
                currentHealth -= amount;
                timeLeftInvincible = invulnerabilityTime;
            }
        }

        /// <summary>
        /// Helper method to check for player input
        /// </summary>
        public Vector2 CheckMovementInput(KeyboardState kbState)
        {
            Vector2 movementVector = new Vector2(0, 0);

            if (kbState.IsKeyDown(Keys.W))
                movementVector.Y = -1;
            if (kbState.IsKeyDown(Keys.A))
                movementVector.X = -1;
            if (kbState.IsKeyDown(Keys.S))
                movementVector.Y = 1;
            if (kbState.IsKeyDown(Keys.D))
                movementVector.X = 1;

            // Normalize the vector so the player doesn't move faster diagonally
            if (movementVector != Vector2.Zero)
                movementVector.Normalize();

            return movementVector;
        }

        public void ForceMove(int x, int y, GameTime gameTime)
        {
            Vector2 movementVector = new Vector2(x, y);

            // Normalize the vector so the player doesn't move faster diagonally
            if (movementVector != Vector2.Zero)
                movementVector.Normalize();

            // Apply the movement
            ApplyForce(movementVector * movementForce);
            //ApplyFriction(gameTime);

            UpdatePhysics(gameTime);
        }

        /*
        // UNFINISHED FEATURE - STRETCH GOAL

        /// <summary>
        /// Makes the player dodge
        /// </summary>
        public void Dodge(KeyboardState kbState)
        {
            // If the player presses space and can dodge
            if ((kbState.IsKeyDown(Keys.Space) && timeUntilDodge < 0) &&
                (kbState.IsKeyDown(Keys.A) || kbState.IsKeyDown(Keys.S) ||
                kbState.IsKeyDown(Keys.W) || kbState.IsKeyDown(Keys.D)))
            {
                timeUntilDodge = dodgeCooldown;
                dodgeVector = CheckMovementInput(kbState);
                canMove = false;
                dodgeInvulnerabilityTime = dodgeLength;
                inDodge = true;
                maxSpeed *= dodgeSpeedBoost;
            }
        }
        

    }
}
