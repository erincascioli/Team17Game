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

        /// <summary>
        /// The text that this TextElement displays
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                size = (font.MeasureString(text) * scale).ToPoint();
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
                size = (font.MeasureString(text) * scale).ToPoint();
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

            this.text = text;
            this.font = font;
            this.scale = scale;

            size = (font.MeasureString(text) * scale).ToPoint();
        }

        /// <summary>
        /// Draws this GameObject to the screen
        /// </summary>
        /// <param name="spriteBatch">A reference to the SpriteBatch</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font,
                    text,
                    rectangle.Location.ToVector2(),
                    color,
                    0f,
                    Vector2.Zero,
                    scale * Game1.UIScale,
                    SpriteEffects.None,
                    1f);
        }

        /// <summary>Returns the text that this element contains.</summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return text;
        }
    }
}
