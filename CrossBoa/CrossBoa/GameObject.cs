using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    /// <summary>
    /// Author: Ian Knecht
    /// <para>Base class for any objects that will be rendered to the screen</para>
    /// </summary>
    public class GameObject
    {
        // ------------
        //    Fields
        // ------------
        protected Texture2D sprite;
        protected Vector2 position;
        protected Point size;
        protected Color color;

        // ------------
        //  Properties
        // ------------
        /// <summary>
        /// The Texture2D that this GameObject will use
        /// </summary>
        public Texture2D Sprite
        {
            get { return sprite; }
            set { sprite = value; }
        }

        /// <summary>
        /// A rectangle containing this GameObject's position and size
        /// </summary>
        public virtual Rectangle Rectangle
        {
            get
            {
                return new Rectangle(position.ToPoint(), size);
            }
        }

        /// <summary>
        /// A Vector2 representing this GameObject's position
        /// </summary>
        public virtual Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// A point representing this GameObject's size
        /// </summary>
        public virtual Point Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>
        /// The width of this GameObject's hitbox
        /// </summary>
        public int Width
        {
            get { return size.X; }
        }

        /// <summary>
        /// The height of this GameObject's hitbox
        /// </summary>
        public int Height
        {
            get { return size.Y; }
        }

        /// <summary>
        /// The color this GameObject's sprite should be tinted
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        // ------------
        // Constructors
        // ------------
        /// <summary>
        /// Constructs a GameObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="rectangle">a Rectangle containing this GameObject's position and size</param>
        public GameObject(Texture2D sprite, Rectangle rectangle)
        {
            this.sprite = sprite;
            this.position = rectangle.Location.ToVector2();
            this.size = rectangle.Size;
            this.color = Color.White;
        }

        // -----------
        //   Methods
        // -----------
        /// <summary>
        /// Put this in the Game1 Update() method
        /// </summary>
        /// <param name="gameTime">A reference to the GameTime</param>
        public virtual void Update(GameTime gameTime)
        {

        }

        /// <summary>
        /// Draws this GameObject to the screen
        /// </summary>
        /// <param name="spriteBatch">A reference to the SpriteBatch</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, Rectangle, color);
        }
    }
}
