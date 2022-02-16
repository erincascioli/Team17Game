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
    /// Purpose: Manages all interactions with buttons
    /// Restrictions: none
    /// </summary>
    public class Button : GameObject
    {
        private MouseState mouse;
        private MouseState previousState;
        private bool isInteractable;
        private Rectangle button;

        public bool IsInteractable
        {
            get { return isInteractable; }
        }

        // Already established location
        public Button(MouseState cursor, bool unLocked, Rectangle rectangle) : base()
        {
            mouse = cursor;
            isInteractable = unLocked;
            button = rectangle;
        }

        // Creates own rectangle
        public Button(MouseState cursor, bool unLocked, int x, int y, int width, int height) : base()
        {
            mouse = cursor;
            isInteractable = unLocked;
            button = new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// Purpose: Determines whether or not the cursor is currently over a button
        /// Restrictions: none
        /// </summary>
        /// <returns></returns>
        public bool isMouseOver()
        {
            if (mouse.X > button.X && mouse.X < button.X + button.Width && 
                mouse.Y > button.Y && mouse.Y < button.Y + button.Height)
            {
                // Mouse is over the button
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Purpose: Determines if a button was clicked
        /// </summary>
        /// <returns></returns>
        bool hasBeenPressed()
        {
            if (isMouseOver() && previousState.LeftButton == ButtonState.Pressed && mouse.LeftButton == ButtonState.Released)
            {
                previousState = mouse;

                // Button was clicked
                return true;
            }
            else
            {
                previousState = mouse;
                return false;
            }
        }
    }
}
