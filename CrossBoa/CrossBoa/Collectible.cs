﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrossBoa
{
    /// <summary>
    /// Represents a collectible objecct
    /// </summary>
    public class Collectible : GameObject, ICollidable
    {
        private bool isCollected;
        private Rectangle hitbox;

        public bool IsCollected
        {
            get { return isCollected; }
            set { isCollected = value; }
        }

        public Rectangle Hitbox
        {   
            get { return hitbox; }
        }

        public Collectible(Texture2D asset, Rectangle hitbox, bool isCollected) : base(asset, hitbox)
        {
            this.hitbox = hitbox;
            this.isCollected = isCollected;
        }

        //Needs to be fixed so that the collectible is not drawn if it has been collected
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsCollected)
            {
                base.Draw(spriteBatch);
            }
            else
            {
                spriteBatch.Draw(Sprite, hitbox, Color.White);
            }
        }
    }
}