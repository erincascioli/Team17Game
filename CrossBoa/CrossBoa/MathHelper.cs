﻿using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    /// <summary>
    /// A helper class with methods for vector math
    /// </summary>
    public static class MathHelper
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
        /// Finds the squared distance between two points. Is computationally cheaper than MathHelper.Distance()
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
    }
}
