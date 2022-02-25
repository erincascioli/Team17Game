using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace CrossBoa
{
    public interface ICollidable
    {
        public Rectangle Hitbox { get; }
    }
}
