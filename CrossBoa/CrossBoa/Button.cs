﻿using System;
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
        private bool hovering;
        private Rectangle button;
        private Texture2D onButtonTexture;
        private Texture2D offButtonTexture;

        public bool IsInteractable
        {
            get { return isInteractable; }
        }

        /// <summary>
        /// Used for an already established location
        /// </summary>
        /// <param name="onImage"></param>
        /// <param name="offImage"></param>
        /// <param name="cursor"></param>
        /// <param name="unLocked"></param>
        /// <param name="rectangle"></param>
        public Button(Texture2D onImage, Texture2D offImage, MouseState cursor, bool unLocked, Rectangle rectangle) 
            : base(offImage,  rectangle)
        {
            mouse = cursor;
            isInteractable = unLocked;
            button = rectangle;
            onButtonTexture = onImage;
            offButtonTexture = offImage;
        }

        /// <summary>
        /// Let's gameObject create it's own rectangle
        /// </summary>
        /// <param name="onImage"></param>
        /// <param name="offImage"></param>
        /// <param name="cursor"></param>
        /// <param name="unLocked"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Button(Texture2D onImage, Texture2D offImage, MouseState cursor, bool unLocked, int x, int y, int width, int height) 
            : base(offImage, x, y, width, height)
        {
            mouse = cursor;
            isInteractable = unLocked;
            button = new Rectangle(x, y, width, height);
            onButtonTexture = onImage;
            offButtonTexture = offImage;
        }

        /// <summary>
        /// Purpose: Determines whether or not the cursor is currently over a button
        /// Restrictions: none
        /// </summary>
        /// <returns></returns>
        public bool IsMouseOver()
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
        /// Purpose: Determines if a button was clicked during this frame
        /// </summary>
        /// <returns>true if the mouse button was released on this frame; otherwise returns false</returns>
        public bool HasBeenPressed()
        {
            if (hovering && previousState.LeftButton == ButtonState.Pressed && mouse.LeftButton == ButtonState.Released)
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

        /// <summary>
        /// Purpose: Draws button to the screen
        /// Restrictions: spritebatch.Draw must have been called already
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (hovering == true)
            {
                spriteBatch.Draw(onButtonTexture, button, Color.White);
            }
            else
            {
                spriteBatch.Draw(offButtonTexture, button, Color.White);
            }
        }

        /// <summary>
        /// Purpose: Updates changing variables of object every frame
        /// Restrictions: should likley be called before any other class method
        /// </summary>
        /// <param name="cursor"></param>
        public override void Update(GameTime gameTime)
        {
            MouseState cursor = Mouse.GetState();
            mouse = cursor;
            hovering = IsMouseOver();
        }
    }
}