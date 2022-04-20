using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CrossBoa.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

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
        public static Song titleTheme;
        public static SoundEffect beastWallBump;
        public static SoundEffect fireShoot;
        public static SoundEffect fireDissipate;

        // Will allow menu to control master volume
        // All individual soundeffects also have their own volume when 
        // played, which master volume should scale too
        public static float SFXVolume
        {
            get { return SoundEffect.MasterVolume; }
            set { SoundEffect.MasterVolume = value; }
        }

        // Will allow menu to control music volume
        public static float MusicVolume
        {
            get { return MediaPlayer.Volume; }
            set { MediaPlayer.Volume = value; }
        }

        static SoundManager()
        {
            // Gives the ability to read in files
            Microsoft.Xna.Framework.Content.ContentManager Content = LevelManager.LContent;

            titleTheme = Content.Load<Song>("Canopy Cacophony");

            StreamReader fileReader = new StreamReader("../../../SoundEffects.txt");

            // Sound Effects will be gotten one by one
            hitWall = Content.Load<SoundEffect>(fileReader.ReadLine());
            hitWall.CreateInstance();

            slimeHop = Content.Load<SoundEffect>(fileReader.ReadLine());
            slimeHop.CreateInstance();

            beastWallBump = Content.Load<SoundEffect>(fileReader.ReadLine());
            beastWallBump.CreateInstance();

            fireShoot = Content.Load<SoundEffect>(fileReader.ReadLine());
            fireShoot.CreateInstance();

            fireDissipate = Content.Load<SoundEffect>(fileReader.ReadLine());
            fireDissipate.CreateInstance();

            fileReader.Close();
        }
    }
}
