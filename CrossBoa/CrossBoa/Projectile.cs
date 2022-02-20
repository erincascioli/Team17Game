using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    public class Projectile : GameObject
    {
        public Projectile(Texture2D asset, Vector2 vector2, Point size) 
            : base(asset, vector2, size)
        {

        }
    }
}
