﻿using System;
using System.Collections.Generic;
using System.Text;
using CrossBoa.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    /// <summary>
    /// A delegate that handles when a button gets clicked
    /// </summary>
    public delegate void OnClickHandler();

    /// <summary>
    /// Author: Donovan Scullion
    /// Purpose: Manages all interactions with buttons
    /// Restrictions: none
    /// </summary>
    public class Button : UIElement
    {
        /// <summary>
        /// Event that runs when this button is clicked
        /// </summary>
        public event OnClickHandler OnClick;

        private MouseState mouseState;
        private MouseState previousState;
        private bool isInteractable;
        private bool hovering;
        private Texture2D hoverButtonTexture;
        private Texture2D offButtonTexture;

        public bool IsInteractable
        {
            get { return isInteractable; }
        }

        /// <summary>
        /// Used for an already established location
        /// </summary>
        /// <param name="hoverImage"></param>
        /// <param name="offImage"></param>
        /// <param name="isInteractable"></param>
        /// <param name="anchor">Which edge of the screen to anchor this object to</param>
        /// <param name="offset"></param>
        /// <param name="size">The size of this object is dependent on the UIScale in Game1</param>
        public Button(Texture2D hoverImage, Texture2D offImage, bool isInteractable, ScreenAnchor anchor, Point offset,
            Point size) 
            : base(offImage, anchor, offset, size)
        {
            this.isInteractable = isInteractable;
            hoverButtonTexture = hoverImage;
            offButtonTexture = offImage;
        }

        /// <summary>
        /// Purpose: Determines whether or not the cursor is currently over a button
        /// Restrictions: none
        /// </summary>
        /// <returns></returns>
        public bool IsMouseOver()
        {
            if (mouseState.X > Rectangle.Left && mouseState.X < Rectangle.Right &&
                mouseState.Y > Rectangle.Top && mouseState.Y < Rectangle.Bottom)
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
            if (hovering && previousState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
            {
                previousState = mouseState;

                // Button was clicked
                if (OnClick != null) 
                    OnClick();

                return true;
            }
            else
            {
                previousState = mouseState;
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
            if (hovering)
            {
                spriteBatch.Draw(hoverButtonTexture, Rectangle, Color.White);
            }
            else
            {
                spriteBatch.Draw(offButtonTexture, Rectangle, Color.White);
            }
        }

        /// <summary>
        /// Purpose: Updates changing variables of object every frame
        /// Restrictions: should likely be called before any other class method
        /// </summary>
        /// <param name="cursor"></param>
        public override void Update(GameTime gameTime)
        {
            mouseState = Mouse.GetState();
            hovering = IsMouseOver();
        }
    }
}