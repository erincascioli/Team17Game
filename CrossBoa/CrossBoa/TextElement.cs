using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrossBoa
{
    /// <summary>
    /// A special UIElement that renders text to the screen
    /// </summary>
    class TextElement : UIElement
    {
        private string text;
        private SpriteFont font;
        private float scale;

        private int lineHeight;
        private string[] lines;
        private Rectangle[] lineRects;

        /// <summary>
        /// The text that this TextElement displays
        /// <para>DO NOT CHANGE EVERY FRAME</para>
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                // The value of the text
                text = value;

                // Measure the entire string
                size = font.MeasureString(text).ToPoint();

                // Update the rectangle
                rectangle = Helper.MakeRectangleFromCenter(position.ToPoint(), size * new Point(Game1.UIScale));

                OnResize();
            }
        }

        /// <summary>
        /// The scale of this TextElement
        /// </summary>
        public float Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                size = font.MeasureString(text).ToPoint();
            }
        }

        /// <summary>
        /// The font that this text element will be displayed in
        /// </summary>
        public SpriteFont Font
        {
            get { return font; }
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

                // If the text has multiple lines, loop through it and edit the positions of each line
                if (lines != null)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        lineRects[i] = Helper.MakeRectangleFromCenter(
                            new Vector2(rectangle.Center.X,
                                rectangle.Center.Y - (rectangle.Height / 2f) + (lineRects[i].Size.Y * i) + (lineRects[i].Size.Y / 2f)).ToPoint(),
                            lineRects[i].Size);
                    }
                }
            }
        }

        /// <summary>
        /// A point representing this GameObject's size
        /// </summary>
        public override Point Size
        {
            // This set is here because it has to override GameObject's setter.
            set { throw new InvalidOperationException("You can't edit the size of a text element! Try editing the scale instead."); }
        }

        /// <summary>
        /// Constructs a Text Element. The position is dependent on the anchor.
        /// </summary>
        /// <param name="text">The text for this text element to display</param>
        /// <param name="anchor">The position to center this UI Element at</param>
        /// <param name="offset">How much to offset this UI Element from the anchor point</param>
        /// <param name="scale">The size of this object is dependent on the UIScale in Game1 <para>Default: 1</para></param>
        /// <param name="font">The font to display this text element in. <para>Default: Press Start font</para></param>
        public TextElement(string text, ScreenAnchor anchor, Point offset, float scale = 1, SpriteFont font = null) :
            base(null, anchor, offset, Point.Zero)
        {
            // Sets default font
            font ??= Game1.PressStart;

            this.scale = scale;
            this.font = font;
            this.text = text;
        }

        /// <summary>
        /// Draws this GameObject to the screen
        /// </summary>
        /// <param name="spriteBatch">A reference to the SpriteBatch</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                spriteBatch.DrawString(font,
                    lines[i],
                    lineRects[i].Location.ToVector2(),
                    color,
                    0f,
                    Vector2.Zero,
                    scale * Game1.UIScale,
                    SpriteEffects.None,
                    1f);
            }

            // Draw call for uncentered text
            //
            // spriteBatch.DrawString(font,
            //     text,
            //     rectangle.Location.ToVector2(),
            //     color,
            //     0f,
            //     Vector2.Zero,
            //     scale * Game1.UIScale,
            //     SpriteEffects.None,
            //     1f);
        }

        /// <summary>
        /// Splits the text up into lines
        /// </summary>
        private void SplitText()
        {
            // Split the text up into multiple lines
            lines = text.Split('\n');

            // Set a centered rectangle for each line
            lineRects = new Rectangle[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                Vector2 lineSize = font.MeasureString(lines[i]) * Game1.UIScale * scale;

                lineRects[i] = Helper.MakeRectangleFromCenter(
                    new Vector2(rectangle.Center.X,
                        rectangle.Center.Y - (rectangle.Height / 2f) + (lineSize.Y * i) + (lineSize.Y / 2)).ToPoint(),
                    lineSize.ToPoint());
            }
        }

        /// <summary>
        /// Moves and resizes this UI Element when the game window size changes based on the UIScale and Anchor
        /// </summary>
        public override void OnResize()
        {
            base.OnResize();
            SplitText();
        }

        /// <summary>Returns the text that this element contains.</summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return text;
        }
    }
}
