using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CrossBoa.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace CrossBoa.Managers
{
    /// <summary>
    /// Author: Donovan Scullion
    /// Purpose: Gets and holds sound files
    /// Restrictions: none
    /// </summary>
    static class SoundManager
    {
        // Sound Effects
        public static SoundEffect hitWall;
        public static SoundEffect slimeHop;

        static SoundManager()
        {
            // Gives the ability to read in files
            Microsoft.Xna.Framework.Content.ContentManager Content = LevelManager.LContent;
            StreamReader fileReader = new StreamReader("../../../SoundEffects.txt");

            // Sound Effects will be gotten one by one
            hitWall = Content.Load<SoundEffect>(fileReader.ReadLine());
            hitWall.CreateInstance();

            slimeHop = Content.Load<SoundEffect>(fileReader.ReadLine());
            slimeHop.CreateInstance();

            fileReader.Close();
        }
    }
}
