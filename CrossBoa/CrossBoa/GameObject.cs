using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    /// <summary>
    /// Base class for any objects that will be rendered to the screen
    /// </summary>
    public class GameObject
    {
        // ------------
        //    Fields
        // ------------
        protected Texture2D sprite;
        protected Vector2 position;
        protected Point size;

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
        public Rectangle Rectangle
        {
            get { return new Rectangle(position.ToPoint(), size); }
        }

        /// <summary>
        /// A Vector2 representing this GameObject's position
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// A point representing this GameObject's size
        /// </summary>
        public Point Size
        {
            get { return size; }
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
            this.position = new Vector2(rectangle.X, rectangle.Y);
            this.size = rectangle.Size;
        }

        /// <summary>
        /// Constructs a GameObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="position">The GameObject's position</param>
        /// <param name="size">The GameObject's size in pixels</param>
        public GameObject(Texture2D sprite, Vector2 position, Point size)
        {
            this.sprite = sprite;
            this.position = position;
            this.size = size;
        }

        /// <summary>
        /// Constructs a GameObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="X">The X position of this GameObject</param>
        /// <param name="Y">The X position of this GameObject</param>
        /// <param name="size">The GameObject's size in pixels</param>
        public GameObject(Texture2D sprite, float X, float Y, Point size)
        {
            this.sprite = sprite;
            this.position = new Vector2(X, Y);
            this.size = size;
        }

        /// <summary>
        /// Constructs a GameObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="position">The GameObject's position</param>
        /// <param name="width">The width of this GameObject in pixels</param>
        /// <param name="height">The height of this GameObject in pixels</param>
        public GameObject(Texture2D sprite, Vector2 position, int width, int height)
        {
            this.sprite = sprite;
            this.position = position;
            this.size = new Point(width, height);
        }

        /// <summary>
        /// Constructs a GameObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="X">The X position of this GameObject</param>
        /// <param name="Y">The X position of this GameObject</param>
        /// <param name="width">The width of this GameObject in pixels</param>
        /// <param name="height">The height of this GameObject in pixels</param>
        public GameObject(Texture2D sprite, float X, float Y, int width, int height)
        {
            this.sprite = sprite;
            this.position = new Vector2(X, Y);
            this.size = new Point(width, height);
        }

        /// <summary>
        /// Constructs a GameObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="X">The X position of this GameObject</param>
        /// <param name="Y">The X position of this GameObject</param>
        /// <param name="size">The GameObject's size in pixels</param>
        public GameObject(Texture2D sprite, int X, int Y, Point size)
        {
            this.sprite = sprite;
            this.position = new Vector2(X, Y);
            this.size = size;
        }

        /// <summary>
        /// Constructs a GameObject
        /// </summary>
        /// <param name="sprite">The sprite for this GameObject</param>
        /// <param name="X">The X position of this GameObject</param>
        /// <param name="Y">The X position of this GameObject</param>
        /// <param name="width">The width of this GameObject in pixels</param>
        /// <param name="height">The height of this GameObject in pixels</param>
        public GameObject(Texture2D sprite, int X, int Y, int width, int height)
        {
            this.sprite = sprite;
            this.position = new Vector2(X, Y);
            this.size = new Point(width, height);
        }

        // -----------
        //   Methods
        // -----------
        /// <summary>
        /// Draws this GameObject to the screen
        /// </summary>
        /// <param name="spriteBatch">A reference to the SpriteBatch</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, Rectangle, Color.White);
        }
    }
}
