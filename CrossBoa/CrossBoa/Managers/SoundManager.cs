using System;
using System.Collections.Generic;
using System.Globalization;
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
        public static SoundEffect slimeDeath;
        public static SoundEffect beastDamaged;
        public static SoundEffect beastCharge;
        public static SoundEffect collectXP;
        public static SoundEffect shootBow;
        public static SoundEffect totemDeath;
        public static SoundEffect beastDeath;
        public static SoundEffect hurtPlayer;
        public static SoundEffect playerDodge;
        public static SoundEffect slimeDamage;
        public static SoundEffect skullDamage;

        // Will allow menu to control master volume
        // All individual sound effects also have their own volume when 
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

            StreamReader fileReader = new StreamReader("Content/SoundEffects.txt");

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

            slimeDeath = Content.Load<SoundEffect>(fileReader.ReadLine());
            slimeDeath.CreateInstance();

            beastDamaged = Content.Load<SoundEffect>(fileReader.ReadLine());
            beastDamaged.CreateInstance();

            beastCharge = Content.Load<SoundEffect>(fileReader.ReadLine());
            beastCharge.CreateInstance();

            collectXP = Content.Load<SoundEffect>(fileReader.ReadLine());
            collectXP.CreateInstance();

            shootBow = Content.Load<SoundEffect>(fileReader.ReadLine());
            shootBow.CreateInstance();

            totemDeath = Content.Load<SoundEffect>(fileReader.ReadLine());
            totemDeath.CreateInstance();

            beastDeath = Content.Load<SoundEffect>(fileReader.ReadLine());
            beastDeath.CreateInstance();

            hurtPlayer = Content.Load<SoundEffect>(fileReader.ReadLine());
            hurtPlayer.CreateInstance();

            playerDodge = Content.Load<SoundEffect>(fileReader.ReadLine());
            playerDodge.CreateInstance();

            slimeDamage = Content.Load<SoundEffect>(fileReader.ReadLine());
            slimeDamage.CreateInstance();

            skullDamage = Content.Load<SoundEffect>(fileReader.ReadLine());
            skullDamage.CreateInstance();

            fileReader.Close();
        }
    }
}
