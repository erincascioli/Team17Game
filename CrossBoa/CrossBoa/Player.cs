﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    class Player : PhysicsObject
    {
        /// <summary>
        /// Constructs a PhysicsObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="rectangle">A Rectangle containing this GameObject's position and size</param>
        /// <param name="friction">How fast this object will stop moving.</param>
        /// <param name="maxSpeed">The maximum speed this object can reach</param>
        /// <param name="movementForce">The force applied to the object when pressing the arrow keys</param>
        public Player(Texture2D sprite, Rectangle rectangle, float movementForce, float maxSpeed, float friction) : base(sprite, rectangle, movementForce, maxSpeed, friction)
        {
        }

        /// <summary>
        /// Constructs a PhysicsObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="position">The GameObject's position</param>
        /// <param name="size">The GameObject's size in pixels</param>
        /// <param name="friction">How fast this object will stop moving.</param>
        /// <param name="maxSpeed">The maximum speed this object can reach</param>
        /// <param name="movementForce">The force applied to the object when pressing the arrow keys</param>
        public Player(Texture2D sprite, Vector2 position, Point size, float movementForce, float maxSpeed, float friction) : base(sprite, position, size, movementForce, maxSpeed, friction)
        {
        }

        /// <summary>
        /// Constructs a PhysicsObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="X">The X position of this GameObject</param>
        /// <param name="Y">The X position of this GameObject</param>
        /// <param name="size">The GameObject's size in pixels</param>
        /// <param name="friction">How fast this object will stop moving.</param>
        /// <param name="maxSpeed">The maximum speed this object can reach</param>
        /// <param name="movementForce">The force applied to the object when pressing the arrow keys</param>
        public Player(Texture2D sprite, float X, float Y, Point size, float movementForce, float maxSpeed, float friction) : base(sprite, X, Y, size, movementForce, maxSpeed, friction)
        {
        }

        /// <summary>
        /// Constructs a PhysicsObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="position">The GameObject's position</param>
        /// <param name="width">The width of this GameObject in pixels</param>
        /// <param name="height">The height of this GameObject in pixels</param>
        /// <param name="friction">How fast this object will stop moving.</param>
        /// <param name="maxSpeed">The maximum speed this object can reach</param>
        /// <param name="movementForce">The force applied to the object when pressing the arrow keys</param>
        public Player(Texture2D sprite, Vector2 position, int width, int height, float movementForce, float maxSpeed, float friction) : base(sprite, position, width, height, movementForce, maxSpeed, friction)
        {
        }

        /// <summary>
        /// Constructs a PhysicsObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="X">The X position of this GameObject</param>
        /// <param name="Y">The X position of this GameObject</param>
        /// <param name="width">The width of this GameObject in pixels</param>
        /// <param name="height">The height of this GameObject in pixels</param>
        /// <param name="friction">How fast this object will stop moving.</param>
        /// <param name="maxSpeed">The maximum speed this object can reach</param>
        /// <param name="movementForce">The force applied to the object when pressing the arrow keys</param>
        public Player(Texture2D sprite, float X, float Y, int width, int height, float movementForce, float maxSpeed, float friction) : base(sprite, X, Y, width, height, movementForce, maxSpeed, friction)
        {
        }

        /// <summary>
        /// Constructs a PhysicsObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="X">The X position of this GameObject</param>
        /// <param name="Y">The X position of this GameObject</param>
        /// <param name="size">The GameObject's size in pixels</param>
        /// <param name="friction">How fast this object will stop moving.</param>
        /// <param name="maxSpeed">The maximum speed this object can reach</param>
        /// <param name="movementForce">The force applied to the object when pressing the arrow keys</param>
        public Player(Texture2D sprite, int X, int Y, Point size, float movementForce, float maxSpeed, float friction) : base(sprite, X, Y, size, movementForce, maxSpeed, friction)
        {
        }

        /// <summary>
        /// Constructs a PhysicsObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="X">The X position of this GameObject</param>
        /// <param name="Y">The X position of this GameObject</param>
        /// <param name="width">The width of this GameObject in pixels</param>
        /// <param name="height">The height of this GameObject in pixels</param>
        /// <param name="friction">How fast this object will stop moving.</param>
        /// <param name="maxSpeed">The maximum speed this object can reach</param>
        /// <param name="movementForce">The force applied to the object when pressing the arrow keys</param>
        public Player(Texture2D sprite, int X, int Y, int width, int height, float movementForce, float maxSpeed, float friction) : base(sprite, X, Y, width, height, movementForce, maxSpeed, friction)
        {
        }

        /// <summary>
        /// Updates this Object's fields 
        /// </summary>
        /// <param name="gameTime">A reference to the GameTime</param>
        public override void Update(GameTime gameTime)
        {
            KeyboardState kbState = Keyboard.GetState();

            // Calculate the movement direction based on key presses
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

            // Apply the movement
            ApplyMovementForce(movementVector);
            ApplyFriction();

            UpdatePhysics(gameTime);
        }
    }
}
