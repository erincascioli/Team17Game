﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    /// <summary>
    /// Author: Ian Knecht
    /// <para>A class that represents the game's camera system</para>
    /// </summary>
    public static class Camera
    {
        private static Matrix matrix = Matrix.CreateTranslation(0, 4, 0);
        private static int cameraX;
        private static int cameraY;
        private static int prevCameraX;
        private static int prevCameraY;
        private static int shakeX;
        private static int shakeY;
        private static int screenShakeFramesLeft;
        private static float screenShakeMagnitude = 0.5f;

        /// <summary>
        /// A Matrix representation of the camera location
        /// </summary>
        public static Matrix Matrix
        {
            get { return matrix; }
        }

        /// <summary>
        /// How violently the screen should shake
        /// </summary>
        public static float ScreenShakeMagnitude
        {
            get { return screenShakeMagnitude; }
            set { screenShakeMagnitude = value; }
        }


        public static void Update(KeyboardState kbState)
        {
            UpdateScreenShake();

            // Only update the camera if it's different from the previous frame
            if (cameraX != prevCameraX || cameraY != prevCameraY || screenShakeFramesLeft > 0)
            {
                matrix = Matrix.CreateTranslation(cameraX + shakeX, cameraY + shakeY, 0);
            }
        }

        /// <summary>
        /// Shakes the screen
        /// </summary>
        /// <param name="frames">How many frames to shake the screen for</param>
        public static void ShakeScreen(int frames)
        {
            screenShakeFramesLeft = frames;
        }

        /// <summary>
        /// Automatically calculates and moves the camera based on remaining screen shake frames
        /// </summary>
        private static void UpdateScreenShake()
        {
            // Shakes screen if there are frames to shake for. Shakes less as time passes
            if (screenShakeFramesLeft > 0)
            {
                screenShakeFramesLeft--;

                shakeX = (int)Math.Round((2 * Program.RNG.NextDouble() - 1) * ScreenShakeMagnitude * screenShakeFramesLeft);
                shakeY = (int)Math.Round((2 * Program.RNG.NextDouble() - 1) * ScreenShakeMagnitude * screenShakeFramesLeft);
            }

            // Reset screen position after shaking is finished
            else if (screenShakeFramesLeft == 0)
            {
                shakeX = 0;
                shakeY = 0;
                screenShakeFramesLeft--;
            }
        }


        public static void FollowPlayer(Player player)
        {
            cameraX = -(int)player.Position.X + Game1.screenWidth / 2;
            cameraY = -(int)player.Position.Y + Game1.screenHeight / 2;
        }
    }
}
