using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrossBoa
{
    /// <summary>
    /// A special arrow used for upgrades that provide the player with
    /// <para>an additional arrow that does not need to be collected</para>
    /// </summary>
    class NonPickupPlayerArrow : PlayerArrow
    {
        /// <summary>
        /// Constructs a PlayerArrow that doesn't have to be recollected
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="rectangle">a Rectangle containing this GameObject's position and size</param>
        /// <param name="velocity">The direction that the playerArrow will move in</param>
        /// <param name="isPlayerArrow">Set to true if this is the player's arrow</param>
        public NonPickupPlayerArrow(Texture2D sprite, Rectangle rectangle, Vector2 velocity) : base(sprite, rectangle, velocity)
        {
        }

        /// <summary>
        /// Constructs a PlayerArrow that doesn't have to be recollected
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="rectangle">a Rectangle containing this GameObject's position and size</param>
        /// <param name="direction">The direction that the playerArrow will move in</param>
        /// <param name="magnitude">How quickly the playerArrow will move in that direction</param>
        /// <param name="isPlayerArrow">Set to true if this is the player's arrow</param>
        public NonPickupPlayerArrow(Texture2D sprite, Rectangle rectangle, float direction, float magnitude) : base(sprite, rectangle, direction, magnitude)
        {
        }
    }
}
