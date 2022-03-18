using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;



namespace CrossBoa
{
    /// <summary>
    /// Author:  TacNayn
    /// <para>A specialized GameObject parent class that contains physics-related fields</para>
    /// </summary>
    public class PhysicsObject : GameObject
    {
        protected Vector2 velocity;
        private Vector2 netAcceleration;
        protected float friction;
        protected float? maxSpeed;

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
        /// The rate at which the object's velocity returns to 0
        /// </summary>
        public float Friction
        {
            get { return friction; }
        }

        /// <summary>
        /// The angle the object is currently moving towards.
        /// <para>Represented between 0 and 2π radians going clockwise from pointing up.</para>
        /// </summary>
        public float VelocityAngle
        {
            // CREDIT TO https://stackoverflow.com/questions/2276855/xna-2d-vector-angles-whats-the-correct-way-to-calculate
            get { return MathF.Atan2(velocity.Y, velocity.X); }

            // Creates a new normal vector based on the angle, then multiplies it by the old vector's speed.
            set { velocity = (MathHelper.GetNormalVector(value)) * Speed; }
        }

        /// <summary>
        /// The speed the object is currently moving, regardless of direction.
        /// </summary>
        public float Speed
        {
            get { return velocity.Length(); }
            set { velocity *= value / velocity.Length(); }
        }

        // --------------
        //  Constructors
        // --------------
        /// <summary>
        /// Constructs a PhysicsObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="rectangle">A Rectangle containing this GameObject's position and size</param>
        /// <param name="friction">How fast this object will stop moving.</param>
        /// <param name="maxSpeed">The maximum speed this object can reach</param>
        public PhysicsObject(Texture2D sprite, Rectangle rectangle, float? maxSpeed, float friction) : 
            base(sprite, rectangle)
        {
            this.friction = friction;
            this.maxSpeed = maxSpeed;
            velocity = Vector2.Zero;
        }

        /// <summary>
        /// Constructs a PhysicsObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="position">The GameObject's position</param>
        /// <param name="size">The GameObject's size in pixels</param>
        /// <param name="friction">How fast this object will stop moving.</param>
        /// <param name="maxSpeed">The maximum speed this object can reach</param>
        public PhysicsObject(Texture2D sprite, Vector2 position, Point size, float? maxSpeed, float friction) :
            base(sprite, position, size)
        {
            this.friction = friction;
            this.maxSpeed = maxSpeed;
            velocity = Vector2.Zero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            UpdatePhysics(gameTime);
        }

        /// <summary>
        /// Automatically calculates and applies a friction force to the object in the opposite direction of its motion
        /// <para>Stops object from moving if it's moving too slowly</para>
        /// </summary>
        public void ApplyFriction(GameTime gameTime)
        {
            // Get a friction vector that is facing the opposite direction of the object's movement
            Vector2 frictionVector = velocity * -1;

            if (frictionVector != Vector2.Zero)
                frictionVector.Normalize();
            frictionVector *= friction;

            Vector2 frictionApplied = velocity + (frictionVector * (float)gameTime.ElapsedGameTime.TotalSeconds);

            // If the friction would reverse the object's movement, stop the object from moving in that direction instead
            int xSign = MathF.Sign(frictionApplied.X);
            int ySign = MathF.Sign(frictionApplied.Y);

            bool isFrictionXOvershooting = xSign != MathF.Sign(velocity.X) && xSign != 0;
            bool isFrictionYOvershooting = ySign != MathF.Sign(velocity.Y) && ySign != 0;

            if (isFrictionXOvershooting || isFrictionYOvershooting)
            {
                frictionVector = Vector2.Zero;         
                velocity = Vector2.Zero; 
            }

            // Apply the vector to the acceleration
            netAcceleration += frictionVector;
        }

        /// <summary>
        /// Applies a force to the object
        /// </summary>
        /// <param name="direction">The direction this object should be pushed toward in radians</param>
        /// <param name="magnitude">How hard the object should be moved</param>
        public virtual void ApplyForce(float direction, float magnitude)
        {
            // Create a new vector based on the direction
            Vector2 movementVector = new Vector2(MathF.Cos(direction), MathF.Sin(direction)) * magnitude;

            // Apply the vector to the net acceleration
            netAcceleration += movementVector;
        }

        /// <summary>
        /// Applies a force to the object
        /// </summary>
        /// <param name="force">A vector representing how hard and in what direction to push the object</param>
        public virtual void ApplyForce(Vector2 force)
        {
            netAcceleration += force;
        }

        /// <summary>
        /// Applies acceleration to velocity and velocity to position, accounting for maxSpeed
        /// </summary>
        /// <param name="gameTime">A reference to the GameTime</param>
        public void UpdatePhysics(GameTime gameTime)
        {
            // Update velocity and position
            velocity += netAcceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Cap velocity
            if (maxSpeed.HasValue && Speed > maxSpeed)
                Speed = maxSpeed.Value;

            // Update position based on velocity
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Resets the acceleration vector for the next frame
            netAcceleration = Vector2.Zero;
        }
    }
}
