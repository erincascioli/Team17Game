using System;
using System.Collections.Generic;
using System.Text;
using CrossBoa.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    /// <summary>
    /// Author: Donovan Scullion
    /// Purpose: Helps keep track of individual tiles
    ///          New class made to keep a required bool value
    /// Restrictions: none
    /// </summary>
    public class Tile : GameObject, ICollidable
    {
        protected bool isInteractable;

        public bool IsInteractable
        {
            get { return isInteractable; }
        }

        public Tile(Texture2D asset, Rectangle rectangle, bool canInteract)
            : base(asset, rectangle)
        {
            isInteractable = canInteract;
        }

        /// <summary>
        /// The hitBox for this object
        /// </summary>
        public Rectangle Hitbox
        {
            get { return Rectangle; }
        }
    }
}
