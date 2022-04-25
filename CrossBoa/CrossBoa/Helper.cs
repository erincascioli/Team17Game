using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    /// <summary>
    /// A helper class with methods for vector math
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Returns the direction between two points in radians
        /// </summary>
        /// <param name="basePoint">The point to start from</param>
        /// <param name="pointToFace">The point to end at</param>
        /// <returns>A direction in radians</returns>
        public static float DirectionBetween(Point basePoint, Point pointToFace)
        {
            // Formula used for calculations: 
            // Cos(A) = (b^2 + c^2 - a^2) / 2bc
            // A = arccos the formula above
            float horizDist = pointToFace.X - basePoint.X;
            float vertDist = pointToFace.Y - basePoint.Y;
            float totalDist = (float)Math.Sqrt(Math.Pow(vertDist, 2) +
                                               Math.Pow(horizDist, 2));
            double cosA = (Math.Pow(totalDist, 2) + Math.Pow(horizDist, 2) - Math.Pow(vertDist, 2))
                          / (2 * horizDist * totalDist);
            if (double.IsNaN(cosA))
                cosA = 0;
            if (pointToFace.Y < basePoint.Y)
                return (float)Math.Acos(cosA) * -1;
            return (float)Math.Acos(cosA);
        }

        /// <summary>
        /// Returns the direction between two vectors in radians
        /// </summary>
        /// <param name="baseVector">The vector to start from</param>
        /// <param name="vectorToFace">The vector to end at</param>
        /// <returns>A direction in radians</returns>
        public static float DirectionBetween(Vector2 baseVector, Vector2 vectorToFace)
        {
            // Formula used for calculations: 
            // Cos(A) = (b^2 + c^2 - a^2) / 2bc
            // A = arccos the formula above
            float horizDist = vectorToFace.X - baseVector.X;
            float vertDist = vectorToFace.Y - baseVector.Y;
            float totalDist = (float)Math.Sqrt(Math.Pow(vertDist, 2) +
                                               Math.Pow(horizDist, 2));
            // Code to prevent horizDistance from crashing when any strictly horizontal
            // angle would be created
            if (horizDist < 0 && horizDist > -1)
                horizDist = -1;
            else if (horizDist >= 0 && horizDist < 1)
                horizDist = 1;
            if (vertDist < 0 && vertDist > -1)
                vertDist = -1;
            else if (vertDist >= 0 && vertDist < 1)
                vertDist = 1;

            double cosA = (Math.Pow(totalDist, 2) + Math.Pow(horizDist, 2) - Math.Pow(vertDist, 2))
                          / (2 * horizDist * totalDist);
            if (double.IsNaN(cosA))
                cosA = 0;
            if (vectorToFace.Y < baseVector.Y)
                return (float)Math.Acos(cosA) * -1;
            return (float)Math.Acos(cosA);
        }

        /// <summary>
        /// Creates a normal vector based on a direction
        /// </summary>
        /// <param name="direction">The direction to turn into a vector</param>
        /// <returns>A normal vector</returns>
        public static Vector2 GetNormalVector(float direction) =>
            new Vector2(MathF.Cos(direction), MathF.Sin(direction));

        /// <summary>
        /// Finds the distance between two points.
        /// </summary>
        /// <param name="a">The first point</param>
        /// <param name="b">The second point</param>
        /// <returns>The distance between the points as a float</returns>
        public static float Distance(Point a, Point b) => MathF.Sqrt(MathF.Pow(a.X - b.X, 2) + MathF.Pow(a.Y - b.Y, 2));

        /// <summary>
        /// Finds the squared distance between two points. Is computationally cheaper than Helper.Distance()
        /// </summary>
        /// <param name="a">The first point</param>
        /// <param name="b">The second point</param>
        /// <returns>The squared distance between the points as a float</returns>
        public static float DistanceSquared(Point a, Point b) => MathF.Pow(a.X - b.X, 2) + MathF.Pow(a.Y - b.Y, 2);


        /// <summary>
        /// Moves this rectangle so the center is at a specific point
        /// </summary>
        public static void SetCenter(this Rectangle rectangle, Point destination)
        {
            rectangle.Location = destination - new Point(rectangle.Size.X / 2, rectangle.Size.Y / 2);
        }

        /// <summary>
        /// Makes a new rectangle using the location as the center
        /// </summary>
        public static Rectangle MakeRectangleFromCenter(Point center, Point size)
        {
            return new Rectangle(center - size / new Point(2), size);
        }

        /// <summary>
        /// Makes a new rectangle using the location as the center
        /// </summary>
        public static Rectangle MakeRectangleFromCenter(int x, int y, int width, int height)
        {
            return new Rectangle(x - width / 2, y - height / 2, width, height);
        }

        /// <summary>
        /// Takes a string and returns a new string with line breaks that fit within the desired width.
        /// <para>DO NOT RUN EVERY FRAME</para>
        /// </summary>
        /// <param name="text">The string to wrap</param>
        /// <param name="spriteFont">The SpriteFont to draw the text in</param>
        /// <param name="maxLineWidth">The maximum length that a single line may be</param>
        /// <returns>A string with newlines inserted where the text would run past the desired length</returns>
        public static string WrapText(this string text, SpriteFont spriteFont, float maxLineWidth)
        {
            string[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder(text.Length);
            float lineWidth = 0f;
            float spaceWidth = spriteFont.MeasureString(" ").X;

            sb.Append(words[0]);
            for (int i = 1; i < words.Length; i++)
            {
                Vector2 size = spriteFont.MeasureString(words[i]);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(" " + words[i]);
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + words[i]);
                    lineWidth = size.X;
                }
            }

            return sb.ToString();
        }
    }
}
