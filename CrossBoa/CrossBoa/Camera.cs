using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    public static class Camera
    {
        private static Matrix matrix = Matrix.CreateTranslation(0, 0, 0);
        private static int cameraX;
        private static int cameraY;
        private static int prevCameraX;
        private static int prevCameraY;
        private static int screenShakeFramesLeft;
        private static float screenShakeMagnitude = 0.5f;

        public static Matrix Matrix
        {
            get { return matrix; }
        }

        public static void Update(KeyboardState kbState)
        {
            // Shake the screen if the player presses space
            if (kbState.IsKeyDown(Keys.Space))
            {
                screenShakeFramesLeft = 20;
            }

            // Shakes screen. Shakes less as time passes
            if (screenShakeFramesLeft > 0)
            {
                screenShakeFramesLeft--;

                cameraX = (int)Math.Round((2 * Program.RNG.NextDouble() - 1) * screenShakeMagnitude * screenShakeFramesLeft);
                cameraY = (int)Math.Round((2 * Program.RNG.NextDouble() - 1) * screenShakeMagnitude * screenShakeFramesLeft);
            }
            // Reset screen position after shaking is finished
            else if (screenShakeFramesLeft == 0)
            {
                matrix = Matrix.CreateTranslation(0, 0, 0);
                screenShakeFramesLeft--;
            }

            // Only update the camera if it's different from the previous frame
            if (cameraX != prevCameraX || cameraY != prevCameraY)
            {
                matrix = Matrix.CreateTranslation(cameraX, cameraY, 0);
            }
        }
    }
}
