using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using CrossBoa.Enemies;
using CrossBoa.Interfaces;
using CrossBoa.Managers;
using CrossBoa.Upgrades;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    public enum PlayerAnimState
    {
        Idle,
        Walk
    }

    /// <summary>
    /// Author:  TacNayn
    /// Purpose: Allows the user to interact with the game world 
    /// </summary>
    public class Player : PhysicsObject, ICollidable
    {
        // Special Effect Upgrades
        public bool HasFangsUpgrade { get; set; }

        // Player stats
        private int maxHealth;
        private float dodgeCooldown;
        private float dodgeLength;
        private float dodgeInvulnerabilityTime;
        private float dodgeSpeedBoost;

        // Player status tracking
        private Vector2 movementVector;
        private int currentHealth;
        private float timeUntilDodge;
        private float timeLeftInvincible;
        private float knockbackTime;
        private float kbDirection;
        private bool canMove;
        private bool flashFrames;
        private bool inDodge;

        // Animation
        private PlayerAnimState animationState;
        private int currentAnimationFrame;
        private double previousWalkFrameChange;
        private Texture2D flashAnimSheet;
        private int flashAnimFrame;
        private bool canFlashTrigger;
        private double prevFlashFrameChange;

        private SpriteEffects spriteFlipEffect;
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
            set { currentHealth = value >= maxHealth ? maxHealth : value; }
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
                // Save the rectangle to avoid creating 4 new value types every time this is accessed
                Rectangle rect = Rectangle;

                return new Rectangle(rect.X + 4, // x position 
                    rect.Y + 14, // y position
                    rect.Width - 8, // width
                    rect.Height - 14); // height
            }
        }

        /// <summary>
        /// Get: returns whether or not the player is still in a dodge.<para></para>
        /// Set: Enables inDodge to be set to false, ending the dodge early and 
        /// resetting the dodge cooldown timer. InDodge cannot be set to true.
        /// </summary>
        public bool InDodge
        {
            get
            {
                return inDodge;
            }
            set
            {
                if (!value && inDodge)
                {
                    timeUntilDodge = 0;
                    EndDodge();
                }
                else if (value)
                    throw new ArgumentException("InDodge cannot be set to true.");
            }
        }

        public bool InKnockback
        {
            get
            {
                return knockbackTime < 0.25;
            }
        }

        /// <summary>
        /// Constructs a Player object
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="rectangle">A Rectangle containing this GameObject's position and size</param>
        /// <param name="movementForce">The force applied to the object when pressing the arrow keys</param>
        /// <param name="maxSpeed">The maximum speed this object can reach</param>
        /// <param name="friction">How fast this object will stop moving.</param>
        /// <param name="maxHealth">The maximum health of this player</param>
        /// <param name="dodgeCooldown">How long the player must wait before being able to dodge again</param>
        /// <param name="dodgeLength">How long the player will dodge for</param>
        /// <param name="dodgeSpeed">How quickly the player will move while dodging</param>
        /// <param name="flashSprite"></param>
        public Player(Texture2D sprite, Rectangle rectangle, float friction,
            int maxHealth, float dodgeCooldown, float dodgeLength, float dodgeSpeed, Texture2D flashSprite) :
            base(sprite, rectangle, StatsManager.BasePlayerMaxSpeed, friction)
        {
            this.maxHealth = maxHealth;
            currentHealth = maxHealth;
            this.dodgeCooldown = dodgeCooldown;
            this.dodgeLength = dodgeLength;
            dodgeSpeedBoost = dodgeSpeed;
            this.canMove = true;
            color = Color.White;
            knockbackTime = 1;
            kbDirection = 0;
            animationState = PlayerAnimState.Idle;
            currentAnimationFrame = 0;
            flashAnimSheet = flashSprite;
            flashAnimFrame = 4;
            canFlashTrigger = false;
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
            // If the player can't dodge, tint them gray
            else if (timeUntilDodge > 0)
                color = new Color(214, 214, 214);//Color.LightGray;
            else
            {
                color = Color.White;
            }

            flashFrames = !flashFrames;

            // Check the player's movement
            CheckMovementInput(kbState);

            // Apply the player's movement
            if (canMove && !InKnockback)
            {
                ApplyForce(movementVector * StatsManager.PlayerMovementForce);

                if (movementVector.X < 0)
                    spriteFlipEffect = SpriteEffects.FlipHorizontally;
                if (movementVector.X > 0)
                    spriteFlipEffect = SpriteEffects.None;
            }
            else if (inDodge)
            {
                dodgeInvulnerabilityTime -= totalSeconds;
                ApplyForce(dodgeVector * StatsManager.BasePlayerMovementForce * 2);
            }
            else if (InKnockback)
            {
                knockbackTime += totalSeconds;
                ApplyForce(kbDirection, 50000 * (1 - 4 * knockbackTime));
            }

            // ~ Dodging code ~
            // Activate a dodge on space press if the player is past the dodge cooldown.
            Dodge(kbState);

            // If the player is in a dodge and their invincibility
            // frames run out, give control back to the player.
            if (inDodge && dodgeInvulnerabilityTime < 0)
                EndDodge();

            // When the player regains their dodge, trigger a flash to notify them.
            if (canFlashTrigger && timeUntilDodge <= 0)
            {
                canFlashTrigger = false;
                flashAnimFrame = 0;
                SoundManager.dodgeRegain.Play(.2f, -0.3f, 0);
            }

            // Update animations for this frame
            UpdateAnimations(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        { 
            sb.Draw(sprite, Rectangle, new Rectangle(currentAnimationFrame * 14, 0, 14, 14), color, 0, Vector2.Zero, spriteFlipEffect, 0);
            
            // Dodge recovery flash animation
            if (flashAnimFrame <= 3)
            {
                sb.Draw(flashAnimSheet, new Rectangle(Rectangle.X - 8, Rectangle.Y - 8, 64, 64),
                    new Rectangle(flashAnimFrame * 16, 0, 16, 16), 
                    Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            }
        }

        /// <summary>
        /// Checks if the player is invincible and deals damage
        /// </summary>
        /// <param name="dealer">The enemy that dealt the damage</param>
        /// <param name="amount"></param>
        /// <param name="direction"></param>
        public void TakeDamage(Enemy dealer, int amount, float direction)
        {
            if (IsInvincible) return;

            if (HasFangsUpgrade)
            {
                dealer?.TakeDamage(1);
            }
            
            currentHealth -= amount;
            timeLeftInvincible = StatsManager.PlayerInvulnerabilityTime;
            knockbackTime = 0;
            velocity = Vector2.Zero;
            kbDirection = direction;
            SoundManager.hurtPlayer.Play(.2f, -.9f, 0);
        }

        /// <summary>
        /// Helper method to check for player input
        /// </summary>
        public void CheckMovementInput(KeyboardState kbState)
        {
            // Reset vector if player is not moving in either direction
            if (kbState.IsKeyUp(Keys.W) && kbState.IsKeyUp(Keys.S))
                movementVector.Y = 0;
            if (kbState.IsKeyUp(Keys.A) && kbState.IsKeyUp(Keys.D))
                movementVector.X = 0;

            // If a key is pressed, override the previous key. OR
            //     If a key is released and the other is still held, set it back
            if (kbState.IsKeyUp(Keys.S) && kbState.IsKeyDown(Keys.W))
                movementVector.Y = -1;
            if (kbState.IsKeyUp(Keys.D) && kbState.IsKeyDown(Keys.A))
                movementVector.X = -1;
            if (kbState.IsKeyUp(Keys.W) && kbState.IsKeyDown(Keys.S))
                movementVector.Y = 1;
            if (kbState.IsKeyUp(Keys.A) && kbState.IsKeyDown(Keys.D))
                movementVector.X = 1;

            // Un-normalizes vector from last frame
            movementVector.X = MathF.Sign(movementVector.X);
            movementVector.Y = MathF.Sign(movementVector.Y);

            // Normalize the vector so the player doesn't move faster diagonally
            if (movementVector != Vector2.Zero)
                movementVector.Normalize();
        }

        public void ForceMove(int x, int y, GameTime gameTime)
        {
            // If the player can't dodge, changes variables to allow the player to dodge upon regaining control.
            if (timeUntilDodge > 0)
            {
                timeUntilDodge = 0;
                canFlashTrigger = false;
                flashAnimFrame = 4;
            }

            Vector2 movementVector = new Vector2(x, y);

            // Normalize the vector so the player doesn't move faster diagonally
            if (movementVector != Vector2.Zero)
                movementVector.Normalize();

            // Apply the movement
            ApplyForce(movementVector * StatsManager.PlayerMovementForce);
            //ApplyFriction(gameTime);

            UpdateAnimations(gameTime);
            UpdatePhysics(gameTime);
        }

        /// <summary>
        /// Updates the player's animations based on the current state
        /// </summary>
        private void UpdateAnimations(GameTime gameTime)
        {
            switch (animationState)
            {
                case PlayerAnimState.Idle:
                    currentAnimationFrame = 0;

                    if (movementVector != Vector2.Zero)
                        animationState = PlayerAnimState.Walk;

                    break;

                case PlayerAnimState.Walk:
                    // Change frame every 0.2 seconds
                    if (gameTime.TotalGameTime.TotalSeconds > previousWalkFrameChange + 0.2)
                    {
                        previousWalkFrameChange = gameTime.TotalGameTime.TotalSeconds;

                        currentAnimationFrame++;
                        if (currentAnimationFrame > 3)
                            currentAnimationFrame = 0;
                    }

                    if (movementVector == Vector2.Zero)
                        animationState = PlayerAnimState.Idle;

                    break;
            }

            if (flashAnimFrame > 3) return;

            prevFlashFrameChange += gameTime.ElapsedGameTime.TotalSeconds;

            if (!(prevFlashFrameChange > 0.05)) return;

            prevFlashFrameChange = 0;
            flashAnimFrame++;
        }

        /// <summary>
        /// Makes the player dodge
        /// </summary>
        public void Dodge(KeyboardState kbState)
        {
            // If the player presses space and can dodge
            if (!Game1.WasKeyPressed(Keys.Space) || !(timeUntilDodge < 0) || (!kbState.IsKeyDown(Keys.A) &&
                                                                              !kbState.IsKeyDown(Keys.S) &&
                                                                              !kbState.IsKeyDown(Keys.W) &&
                                                                              !kbState.IsKeyDown(Keys.D)) ) return;
            if (InKnockback)
                return;
            timeUntilDodge = dodgeCooldown;
            dodgeVector = movementVector;
            canMove = false;
            dodgeInvulnerabilityTime = dodgeLength;
            inDodge = true;
            maxSpeed *= dodgeSpeedBoost;
            canFlashTrigger = true;
            SoundManager.playerDodge.Play(.1f, -.4f, 0);
        }

        /// <summary>
        /// Takes the player out of the dodge. If this method was not
        /// called from Update(), the dodge timer is set to 0 in addition to the
        /// normal functions.
        /// </summary>
        public void EndDodge()
        {
            canMove = true;
            maxSpeed = StatsManager.PlayerMaxSpeed;
            inDodge = false;
            dodgeInvulnerabilityTime = 0;
        }

        /// <summary>
        /// Resets the player's stats and position to normal,
        /// used whenever a new game is started.
        /// </summary>
        /// <param name="startingPos"></param>
        public void ResetPlayer(Rectangle startingPos)
        {
            currentHealth = maxHealth;
            position = new Vector2(startingPos.X, startingPos.Y);
            timeLeftInvincible = 0;
            canMove = true;
            knockbackTime = 1;
        }
    }
}
