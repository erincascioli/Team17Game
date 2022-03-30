using System;
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
        private bool doesScaling = true;

        // Rectangle is the actual UI element rectangle in Screen Space.
        //     The position and size are used to calculate it based on the UI Scale and the Anchor

        /// <summary>
        /// Which section of the screen this UI Element is anchored to
        /// </summary>
        public ScreenAnchor Anchor
        {
            get { return anchor; }
            set { anchor = value; }
        }

        /// <summary>
        /// Whether this UI Element will scale with the screen size
        /// </summary>
        public bool DoesScaling
        {
            get { return doesScaling; }
            set { doesScaling = value; }
        }

        /// <summary>
        /// Constructs a UI Element. The position is dependent on the anchor.
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="position">The position of this object is dependent on the anchor and the UIScale in Game1</param>
        /// <param name="size">The size of this object is dependent on the UIScale in Game1</param>
        /// <param name="anchor">The center of this object will be placed at the anchor. The position will offset it from that</param>
        public UIElement(Texture2D sprite, Point position, Point size, ScreenAnchor anchor) : base(sprite, new Rectangle(position, size))
        {
            this.position = position.ToVector2();
            this.size = size;
            this.rectangle = MathHelper.MakeRectangleFromCenter(position, size * new Point(Game1.UIScale));
            this.anchor = anchor;

            windowCenter = Game1.windowRect.Center;

            OnResize();
            Game1.UIElementsList.Add(this);
        }

        /// <summary>
        /// Moves and resizes this UI Element when the game window size changes based on the UIScale and Anchor
        /// </summary>
        public void OnResize()
        {
            windowCenter = Game1.windowRect.Center;
            Vector2 offset = position * Game1.UIScale;
            Point newSize = size * new Point(Game1.UIScale);

            // Makes a new rectangle based on the position and the anchor, multiplying them by the UIScale
            rectangle = anchor switch
            {
                ScreenAnchor.TopLeft => 
                    new Rectangle(offset.ToPoint(), newSize),
                
                ScreenAnchor.TopCenter => MathHelper.MakeRectangleFromCenter(new Point(
                    (int) (windowCenter.X + offset.X),
                    (int) offset.Y), newSize),
               
                ScreenAnchor.TopRight => MathHelper.MakeRectangleFromCenter(new Point(
                    (int) (Game1.windowWidth + offset.X),
                    (int) offset.Y), newSize),
                
                ScreenAnchor.LeftCenter => MathHelper.MakeRectangleFromCenter(new Point(
                    (int) offset.X,
                    (int) (windowCenter.Y + offset.Y)), newSize),
              
                ScreenAnchor.Center => MathHelper.MakeRectangleFromCenter(
                    windowCenter + offset.ToPoint(), newSize),
             
                ScreenAnchor.RightCenter => MathHelper.MakeRectangleFromCenter(new Point(
                    (int) (Game1.windowWidth + offset.X),
                    (int) (windowCenter.Y + offset.Y)), newSize),
             
                ScreenAnchor.BottomLeft => MathHelper.MakeRectangleFromCenter(new Point(
                    (int) offset.X, 
                    (int) (Game1.windowHeight + offset.Y)), newSize),
             
                ScreenAnchor.BottomCenter => MathHelper.MakeRectangleFromCenter(new Point(
                    (int) (windowCenter.X + offset.X),
                    (int) (Game1.windowHeight + offset.Y)), newSize),
             
                ScreenAnchor.BottomRight => MathHelper.MakeRectangleFromCenter(
                    Game1.windowRect.Size + offset.ToPoint(), newSize),

                _ => rectangle
            };
        }
    }
}
