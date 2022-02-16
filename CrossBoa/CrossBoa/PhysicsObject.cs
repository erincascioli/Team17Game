using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrossBoa
{
    /// <summary>
    /// A specialized GameObject parent class that contains physics-related fields
    /// </summary>
    class PhysicsObject : GameObject
    {
        private Vector2 velocity;
        private Vector2 acceleration;
        private float friction;

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
        public Vector2 Acceleration
        {
            get { return acceleration; }
            set { acceleration = value; }
        }

        /// <summary>
        /// The rate at which the object's velocity returns to 0
        /// </summary>
        public float Friction
        {
            get { return friction; }
            set { friction = value; }
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
            get { return acceleration.Length(); }
            set { acceleration *= value / acceleration.Length(); }
        }

        // --------------
        //  Constructors
        // --------------
        /// <summary>
        /// Constructs a GameObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="rectangle">A Rectangle containing this GameObject's position and size</param>
        /// <param name="friction">How quickly this object will stop moving</param>
        public PhysicsObject(Texture2D sprite, Rectangle rectangle, float friction) : 
            base(sprite, rectangle)
        {
            this.friction = friction;
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
        }

        /// <summary>
        /// Constructs a GameObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="position">The GameObject's position</param>
        /// <param name="size">The GameObject's size in pixels</param>
        /// <param name="friction">How quickly this object will stop moving</param>
        public PhysicsObject(Texture2D sprite, Vector2 position, Point size, float friction) :
            base(sprite, position, size)
        {
            this.friction = friction;
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
        }

        /// <summary>
        /// Constructs a GameObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="X">The X position of this GameObject</param>
        /// <param name="Y">The X position of this GameObject</param>
        /// <param name="size">The GameObject's size in pixels</param>
        /// <param name="friction">How quickly this object will stop moving</param>
        public PhysicsObject(Texture2D sprite, float X, float Y, Point size, float friction) : 
            base(sprite, X, Y, size)
        {
            this.friction = friction;
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
        }

        /// <summary>
        /// Constructs a GameObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="position">The GameObject's position</param>
        /// <param name="width">The width of this GameObject in pixels</param>
        /// <param name="height">The height of this GameObject in pixels</param>
        /// <param name="friction">How quickly this object will stop moving</param>
        public PhysicsObject(Texture2D sprite, Vector2 position, int width, int height, float friction) : 
            base(sprite, position, width, height)
        {
            this.friction = friction;
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
        }

        /// <summary>
        /// Constructs a GameObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="X">The X position of this GameObject</param>
        /// <param name="Y">The X position of this GameObject</param>
        /// <param name="width">The width of this GameObject in pixels</param>
        /// <param name="height">The height of this GameObject in pixels</param>
        /// <param name="friction">How quickly this object will stop moving</param>
        public PhysicsObject(Texture2D sprite, float X, float Y, int width, int height, float friction) :
            base(sprite, X, Y, width, height)
        {
            this.friction = friction;
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
        }

        /// <summary>
        /// Constructs a GameObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="X">The X position of this GameObject</param>
        /// <param name="Y">The X position of this GameObject</param>
        /// <param name="size">The GameObject's size in pixels</param>
        /// <param name="friction">How quickly this object will stop moving</param>
        public PhysicsObject(Texture2D sprite, int X, int Y, Point size, float friction) :
            base(sprite, X, Y, size)
        {
            this.friction = friction;
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
        }

        /// <summary>
        /// Constructs a GameObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="X">The X position of this GameObject</param>
        /// <param name="Y">The X position of this GameObject</param>
        /// <param name="width">The width of this GameObject in pixels</param>
        /// <param name="height">The height of this GameObject in pixels</param>
        /// <param name="friction">How quickly this object will stop moving</param>
        public PhysicsObject(Texture2D sprite, int X, int Y, int width, int height, float friction) :
            base(sprite, X, Y, width, height)
        {
            this.friction = friction;
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
        }

        void ApplyFriction()
        {

        }
    }
}
