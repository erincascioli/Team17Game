using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    /// <summary>
    /// Author:  TacNayn
    /// <para>A class that represents the game's camera system</para>
    /// </summary>
    public static class Camera
    {
        private static Matrix matrix = Matrix.CreateTranslation(0, 4, 0);
        private static int cameraX;
        private static int cameraY;
        private static int prevCameraX;
        private static int prevCameraY;
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
            // Shake the screen if the player presses space
            if (kbState.IsKeyDown(Keys.Space))
            {
                ShakeScreen(20);
            }

            // Only update the camera if it's different from the previous frame
            UpdateScreenShake();
            
            if (cameraX != prevCameraX || cameraY != prevCameraY)
            {
                matrix = Matrix.CreateTranslation(cameraX, cameraY, 0);
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

                cameraX = (int)Math.Round((2 * Program.RNG.NextDouble() - 1) * ScreenShakeMagnitude * screenShakeFramesLeft);
                cameraY = (int)Math.Round((2 * Program.RNG.NextDouble() - 1) * ScreenShakeMagnitude * screenShakeFramesLeft);
            }

            // Reset screen position after shaking is finished
            else if (screenShakeFramesLeft == 0)
            {
                matrix = Matrix.CreateTranslation(0, 4, 0);
                screenShakeFramesLeft--;
            }
        }
    }
}
