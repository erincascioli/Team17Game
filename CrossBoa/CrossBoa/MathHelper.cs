using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    /// <summary>
    /// A helper class with methods for vector math
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Creates a normal vector based on a direction
        /// </summary>
        /// <param name="direction">The direction to turn into a vector</param>
        /// <returns>A normal vector</returns>
        public static Vector2 GetNormalVector(float direction) =>
            new Vector2(MathF.Cos(direction), MathF.Sin(direction));
    }
}
