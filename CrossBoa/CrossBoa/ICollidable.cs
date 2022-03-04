using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace CrossBoa
{
    /// <summary>
    /// Interface that gives classes a hitbox property, allowing for easier collision checking
    /// </summary>
    public interface ICollidable
    {
        /// <summary>
        /// The hitbox for this object
        /// </summary>
        public Rectangle Hitbox { get; }

        public Vector2 Position { get; set; }
    }
}
