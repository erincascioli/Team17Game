using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    class Enemy : GameObject
    {
        // This may not be the best constructor; feel free to change
        public Enemy(Texture2D asset, Rectangle position) 
            : base(asset, position)
        {

        }
    }
}
