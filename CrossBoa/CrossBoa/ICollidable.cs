using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace CrossBoa
{
    public interface ICollidable
    {
        /// <summary>
        /// The hitbox for this object
        /// </summary>
        public Rectangle Hitbox { get; }
    }
}
