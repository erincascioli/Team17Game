﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrossBoa
{
    public enum ScreenAnchor
    {
        TopLeft,
        TopCenter,
        TopRight,
        LeftCenter,
        Center,
        RightCenter,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    /// <summary>
    /// A class for a UI Element
    /// </summary>
    public class UIElement : GameObject
    {
        protected ScreenAnchor anchor;
        protected Rectangle rectangle;
        private Point windowCenter;
        private bool doesPositionScale = true;
        private bool doesSizeScale = true;

        // Rectangle is the actual UI element rectangle in Screen Space.
        //     The position and size are used to calculate it based on the UI Scale and the Anchor

        /// <summary>
        /// Which section of the screen this UI Element is anchored to
        /// </summary>
        public ScreenAnchor Anchor
        {
            get { return anchor; }
            set
            {
                anchor = value;
                OnResize();
            }
        }

        /// <summary>
        /// A Vector2 representing the center of this UI Element
        /// </summary>
        public override Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                OnResize();
            }
        }

        /// <summary>
        /// The actual position and size of this rectangle relative to the window
        /// </summary>
        public override Rectangle Rectangle
        {
            get { return rectangle; }
        }

        /// <summary>
        /// Whether or not the position of this UIElement will scale with the screen
        /// </summary>
        public bool DoesPositionScale
        {
            get { return doesPositionScale; }
            set { doesPositionScale = value; }
        }

        /// <summary>
        /// Whether or not the size of this UIElement will scale with the screen
        /// </summary>
        public bool DoesSizeScale
        {
            get { return doesSizeScale; }
            set { doesSizeScale = value; }
        }

        /// <summary>
        /// Constructs a UI Element. The position is dependent on the anchor.
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="anchor">The position to center this UI Element at</param>
        /// <param name="offset">How much to offset this UI Element from the anchor point</param>
        /// <param name="size">The size of this object is dependent on the UIScale in Game1</param>
        public UIElement(Texture2D sprite, ScreenAnchor anchor, Point offset, Point size) : base(sprite, new Rectangle(offset, size))
        {
            this.position = offset.ToVector2();
            this.size = size;
            this.rectangle = Helper.MakeRectangleFromCenter(offset, size * new Point(Game1.UIScale));
            this.anchor = anchor;

            windowCenter = Game1.windowRect.Center;

            Game1.UIElementsList.Add(this);
        }

        /// <summary>
        /// Moves and resizes this UI Element when the game window size changes based on the UIScale and Anchor
        /// </summary>
        public virtual void OnResize()
        {
            windowCenter = Game1.windowRect.Center;

            Vector2 offset = doesPositionScale ? position * Game1.UIScale : position;
            Point newSize = doesSizeScale ? size * new Point(Game1.UIScale) : size;
    
            // Makes a new rectangle based on the position and the anchor, multiplying them by the UIScale
            rectangle = anchor switch
            {
                ScreenAnchor.TopLeft => Helper.MakeRectangleFromCenter(
                    offset.ToPoint(), newSize),
                
                ScreenAnchor.TopCenter => Helper.MakeRectangleFromCenter(new Point(
                    (int) (windowCenter.X + offset.X),
                    (int) offset.Y), newSize),
               
                ScreenAnchor.TopRight => Helper.MakeRectangleFromCenter(new Point(
                    (int) (Game1.windowWidth + offset.X),
                    (int) offset.Y), newSize),
                
                ScreenAnchor.LeftCenter => Helper.MakeRectangleFromCenter(new Point(
                    (int) offset.X,
                    (int) (windowCenter.Y + offset.Y)), newSize),
              
                ScreenAnchor.Center => Helper.MakeRectangleFromCenter(
                    windowCenter + offset.ToPoint(), newSize),
             
                ScreenAnchor.RightCenter => Helper.MakeRectangleFromCenter(new Point(
                    (int) (Game1.windowWidth + offset.X),
                    (int) (windowCenter.Y + offset.Y)), newSize),
             
                ScreenAnchor.BottomLeft => Helper.MakeRectangleFromCenter(new Point(
                    (int) offset.X, 
                    (int) (Game1.windowHeight + offset.Y)), newSize),
             
                ScreenAnchor.BottomCenter => Helper.MakeRectangleFromCenter(new Point(
                    (int) (windowCenter.X + offset.X),
                    (int) (Game1.windowHeight + offset.Y)), newSize),
             
                ScreenAnchor.BottomRight => Helper.MakeRectangleFromCenter(
                    Game1.windowRect.Size + offset.ToPoint(), newSize),

                _ => rectangle
            };
        }
    }
}
