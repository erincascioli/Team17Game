using System;
using System.Collections.Generic;
using System.Text;
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
    public class Tile : GameObject
    {
        private bool isInteractable;

        public bool IsInteractable
        {
            get { return isInteractable; }
        }

        public Tile(Texture2D asset, Rectangle rectangle, bool canInteract)
            : base(asset, rectangle)
        {
            isInteractable = canInteract;
        }
    }
}
