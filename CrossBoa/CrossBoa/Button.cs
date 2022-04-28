using System;
using System.Collections.Generic;
using System.Text;
using CrossBoa.Interfaces;
using CrossBoa.Managers;
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
        public event OnClickHandler OnRightClick;

        private bool isInteractable;
        private bool hovering;
        private Texture2D hoverButtonTexture;

        public bool IsInteractable
        {
            get { return isInteractable; }
        }

        public Texture2D HoverTexture
        {
            get { return hoverButtonTexture; }
            set { hoverButtonTexture = value; }
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
            sprite = offImage;
        }

        /// <summary>
        /// Purpose: Determines whether or not the cursor is currently over a button
        /// Restrictions: none
        /// </summary>
        /// <returns></returns>
        public bool IsMouseOver()
        {
            return Game1.MState.X > Rectangle.Left && Game1.MState.X < Rectangle.Right &&
                   Game1.MState.Y > Rectangle.Top && Game1.MState.Y < Rectangle.Bottom;
        }

        /// <summary>
        /// Purpose: Determines if a button was clicked during this frame
        /// </summary>
        /// <returns>true if the mouse button was released on this frame; otherwise returns false</returns>
        public bool HasBeenPressed()
        {
            return hovering && Game1.PreviousMState.LeftButton == ButtonState.Pressed && Game1.MState.LeftButton == ButtonState.Released;
        }

        public bool HasBeenRightClicked()
        {
            return hovering && Game1.PreviousMState.RightButton == ButtonState.Pressed && Game1.MState.RightButton == ButtonState.Released;
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
                // If there is no texture assigned, draw the regular sprite larger
                if (hoverButtonTexture == null)
                {
                    spriteBatch.Draw(sprite, 
                        Helper.MakeRectangleFromCenter(Rectangle.Center,
                        new Vector2(rectangle.Size.X * 1.1f, rectangle.Size.Y * 1.1f).ToPoint()),
                        Color.White);
                }
                else
                {
                    // Draw the hover sprite
                    spriteBatch.Draw(hoverButtonTexture, Rectangle, Color.White);
                }
            }
            else
            {
                // Draw the regular sprite
                spriteBatch.Draw(sprite, Rectangle, Color.White);
            }
        }

        public void DrawDisabled(SpriteBatch sb)
        {
            sb.Draw(hoverButtonTexture, Rectangle, Color.Gray);
        }

        /// <summary>
        /// Purpose: Updates changing variables of object every frame
        /// Restrictions: should likely be called before any other class method
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            hovering = IsMouseOver();

            // Run click event on this button
            if (!HasBeenPressed() && !HasBeenRightClicked()) return;

            if (HasBeenRightClicked())
            {
                OnRightClick?.Invoke();
                return;
            }

            OnClick?.Invoke();
            SoundManager.buttonClick.Play(.1f, 0, 0);
        }
    }
}