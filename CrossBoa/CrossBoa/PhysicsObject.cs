using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    /// <summary>
    /// A specialized GameObject parent class that contains physics-related fields
    /// </summary>
    class PhysicsObject : GameObject
    {
        protected Vector2 velocity;
        protected Vector2 netAcceleration;
        protected float movementForce;
        protected float frictionForce;
        protected float maxSpeed;
        
        private Vector2 frictionVector;

        // ------------
        //  Properties
        // ------------
        /// <summary>
        /// The velocity vector of the object
        /// </summary>
        public Vector2 Velocity
        {
            get { return velocity; }
        }

        /// <summary>
        /// The acceleration vector of the object
        /// </summary>
        public Vector2 NetAcceleration
        {
            get { return netAcceleration; }
            set { netAcceleration = value; }
        }

        /// <summary>
        /// The rate at which the object's velocity returns to 0
        /// </summary>
        public float FrictionForce
        {
            get { return frictionForce; }
        }

        /// <summary>
        /// The angle the object is currently moving towards.
        /// <para>Represented between 0 and 2π radians going clockwise from pointing up.</para>
        /// </summary>
        public float Angle
        {
            // CREDIT TO https://stackoverflow.com/questions/2276855/xna-2d-vector-angles-whats-the-correct-way-to-calculate
            get { return MathF.Atan2(velocity.Y, velocity.X); }

            // Creates a new normalized vector based on the angle, then multiplies it by the old vector's speed.
            set { velocity = (new Vector2(MathF.Cos(value), MathF.Sin(value))) * Speed; }
        }

        /// <summary>
        /// The speed the object is currently moving, regardless of direction.
        /// </summary>
        public float Speed
        {
            get { return velocity.Length(); }
            set { velocity *= value / velocity.Length(); }
        }

        /// <summary>
        /// The net acceleration of the object, disregarding direction.
        /// </summary>
        public float AccelerationMagnitude
        {
            get { return netAcceleration.Length(); }
            set { netAcceleration *= value / netAcceleration.Length(); }
        }

        // --------------
        //  Constructors
        // --------------
        /// <summary>
        /// Constructs a GameObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="rectangle">A Rectangle containing this GameObject's position and size</param>
        /// <param name="friction">How fast this object will stop moving.</param>
        /// <param name="maxSpeed">The maximum speed this object can reach</param>
        /// <param name="movementForce">The force applied to the object when pressing the arrow keys</param>
        public PhysicsObject(Texture2D sprite, Rectangle rectangle, float friction, float maxSpeed, float movementForce) : 
            base(sprite, rectangle)
        {
            this.frictionForce = friction;
            this.movementForce = movementForce;
            velocity = Vector2.Zero;
            netAcceleration = Vector2.Zero;
        }

        /// <summary>
        /// Constructs a GameObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="position">The GameObject's position</param>
        /// <param name="size">The GameObject's size in pixels</param>
        /// <param name="friction">How fast this object will stop moving.</param>
        /// <param name="maxSpeed">The maximum speed this object can reach</param>
        /// <param name="movementForce">The force applied to the object when pressing the arrow keys</param>
        public PhysicsObject(Texture2D sprite, Vector2 position, Point size, float friction, float maxSpeed, float movementForce) :
            base(sprite, position, size)
        {
            this.frictionForce = friction;
            this.movementForce = movementForce;
            velocity = Vector2.Zero;
            netAcceleration = Vector2.Zero;
        }

        /// <summary>
        /// Constructs a GameObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="X">The X position of this GameObject</param>
        /// <param name="Y">The X position of this GameObject</param>
        /// <param name="size">The GameObject's size in pixels</param>
        /// <param name="friction">How fast this object will stop moving.</param>
        /// <param name="maxSpeed">The maximum speed this object can reach</param>
        /// <param name="movementForce">The force applied to the object when pressing the arrow keys</param>
        public PhysicsObject(Texture2D sprite, float X, float Y, Point size, float friction, float maxSpeed, float movementForce) : 
            base(sprite, X, Y, size)
        {
            this.frictionForce = friction;
            this.movementForce = movementForce;
            velocity = Vector2.Zero;
            netAcceleration = Vector2.Zero;
        }

        /// <summary>
        /// Constructs a GameObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="position">The GameObject's position</param>
        /// <param name="width">The width of this GameObject in pixels</param>
        /// <param name="height">The height of this GameObject in pixels</param>
        /// <param name="friction">How fast this object will stop moving.</param>
        /// <param name="maxSpeed">The maximum speed this object can reach</param>
        /// <param name="movementForce">The force applied to the object when pressing the arrow keys</param>
        public PhysicsObject(Texture2D sprite, Vector2 position, int width, int height, float friction, float maxSpeed, float movementForce) : 
            base(sprite, position, width, height)
        {
            this.frictionForce = friction;
            this.movementForce = movementForce;
            velocity = Vector2.Zero;
            netAcceleration = Vector2.Zero;
        }

        /// <summary>
        /// Constructs a GameObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="X">The X position of this GameObject</param>
        /// <param name="Y">The X position of this GameObject</param>
        /// <param name="width">The width of this GameObject in pixels</param>
        /// <param name="height">The height of this GameObject in pixels</param>
        /// <param name="friction">How fast this object will stop moving.</param>
        /// <param name="maxSpeed">The maximum speed this object can reach</param>
        /// <param name="movementForce">The force applied to the object when pressing the arrow keys</param>
        public PhysicsObject(Texture2D sprite, float X, float Y, int width, int height, float friction, float maxSpeed, float movementForce) :
            base(sprite, X, Y, width, height)
        {
            this.frictionForce = friction;
            this.movementForce = movementForce;
            velocity = Vector2.Zero;
            netAcceleration = Vector2.Zero;
        }

        /// <summary>
        /// Constructs a GameObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="X">The X position of this GameObject</param>
        /// <param name="Y">The X position of this GameObject</param>
        /// <param name="size">The GameObject's size in pixels</param>
        /// <param name="friction">How fast this object will stop moving.</param>
        /// <param name="maxSpeed">The maximum speed this object can reach</param>
        /// <param name="movementForce">The force applied to the object when pressing the arrow keys</param>
        public PhysicsObject(Texture2D sprite, int X, int Y, Point size, float friction, float maxSpeed, float movementForce) :
            base(sprite, X, Y, size)
        {
            this.frictionForce = friction;
            this.movementForce = movementForce;
            velocity = Vector2.Zero;
            netAcceleration = Vector2.Zero;
        }

        /// <summary>
        /// Constructs a GameObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="X">The X position of this GameObject</param>
        /// <param name="Y">The X position of this GameObject</param>
        /// <param name="width">The width of this GameObject in pixels</param>
        /// <param name="height">The height of this GameObject in pixels</param>
        /// <param name="friction">How fast this object will stop moving.</param>
        /// <param name="maxSpeed">The maximum speed this object can reach</param>
        /// <param name="movementForce">The force applied to the object when pressing the arrow keys</param>
        public PhysicsObject(Texture2D sprite, int X, int Y, int width, int height, float friction, float maxSpeed, float movementForce) :
            base(sprite, X, Y, width, height)
        {
            this.frictionForce = friction;
            this.movementForce = movementForce;
            velocity = Vector2.Zero;
            netAcceleration = Vector2.Zero;
        }

        /// <summary>
        /// Applies a friction force to the object in the opposite direction of its motion
        /// </summary>
        /// <returns>A force Vector that should be applied to the netAcceleration</returns>
        public Vector2 ApplyFriction()
        {
            // Get a friction vector that is facing the opposite direction of the object's movement
            frictionVector = velocity * -1;
            frictionVector.Normalize();
            frictionVector *= frictionForce;

            // Apply the vector to the acceleration
            return frictionVector;
        }

        /// <summary>
        /// Applies a force to the object based on its movementForce field
        /// </summary>
        /// <param name="direction">The direction this object should move toward in radians</param>
        /// <returns>A force Vector that should be applied to the netAcceleration</returns>
        public Vector2 Move(float direction)
        {
            // TODO - this code is for the player, not for PhysicsObject
            /*// Calculate the movement vector based on key presses
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
            movementVector.Normalize();*/





            // Create a new vector based on the direction
            Vector2 movementVector = new Vector2(MathF.Cos(direction), MathF.Sin(direction)) * movementForce;

            // Apply the vector to the net acceleration
            return movementVector;
        }
    }
}
