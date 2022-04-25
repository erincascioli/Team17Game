using Microsoft.Xna.Framework;

namespace CrossBoa.Interfaces
{
    /// <summary>
    /// Interface that gives classes a hitBox property, allowing for easier collision checking
    /// </summary>
    public interface ICollidable 
    {
        /// <summary>
        /// The hitBox for this object
        /// </summary>
        public Rectangle Hitbox { get; }

        public Vector2 Position { get; set; }
    }
}
