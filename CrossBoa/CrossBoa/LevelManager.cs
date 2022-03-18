using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    /// <summary>
    /// Author: Donovan Scullion
    /// Purpose: handles the generation of levels
    /// Restrictions: levels must be made manually, 
    ///               and this class will need to be updated each
    ///               time to contain them
    /// </summary>
    public static class LevelManager
    {
        private static List<string[]> tileList;
        private static List<Tile> levelTiles;
        private static StreamReader reader;
        private const int blockWidth = 64;
        private const int blockHeight = 64;
        private static Microsoft.Xna.Framework.Content.ContentManager Content;
        private static Door entrance;
        private static Door exit;
        private static int stage;
        private static int levelWidth;
        private static int levelHeight;
        private static ExitLocation exitLocation;

        // Requires a reference
        public static Microsoft.Xna.Framework.Content.ContentManager LContent
        {
            set { Content = value; }
        }

        public static Door Exit
        {
            get { return exit; }
        }

        static LevelManager()
        {
            levelTiles = new List<Tile>();
            tileList = new List<string[]>();
            stage = 0; // No levels yet

            // tileList is immediately filled as it's data is needed
            // before any other method can be made
            try
            {
                reader = new StreamReader("../../../LevelObjectList.txt");

                // Gets the default instructions out of the file
                string[] fileData = reader.ReadLine().Split(',');
                
                while (!reader.EndOfStream)
                {
                    fileData = reader.ReadLine().Split(',');
                    // Data for each basic block type is added
                    tileList.Add(fileData);
                }
            }
            catch (Exception e)
            {
                // Specific error can be read in the debug menu
                Console.WriteLine("Error: {0}", e);

                try
                {
                    // Just in case it's open
                    reader.Close();
                }
                catch
                {
                    // Needed in case the file failed to open or already closed
                }
            }
        }

        /// <summary>
        /// Purpose: Fills level with tiles
        /// Restrictions: Given file must exist and be accurate
        ///               16 x 16 is the only accepted file size at the moment
        /// </summary>
        /// <param name="fileName"></param>
        public static void LoadLevel(string fileName)
        {
            if (entrance == null)
            {
                // This is done here because the load method can't be passed in before 
                // this class's constructor is established
                // Doors are created and will be constantly used
                entrance = new Door(Content.Load<Texture2D>("FloorShadow"), // Open Sprite
                    Content.Load<Texture2D>("GrassTest"), // Closed Sprite
                    new Rectangle(-100, -100, 64, 64), // Location and size
                    true);

                exit = new Door(Content.Load<Texture2D>("FloorShadow"), // Open Sprite
                    Content.Load<Texture2D>("GrassTest"), // Closed Sprite
                    new Rectangle(-100, -100, 64, 64), // Location and size
                    true); // Has hitbox
            }

            // Stage number is updated
            stage++;

            // Level is cleared so that the next may be loaded
            levelTiles.Clear();

            try
            {
                // file is accessed by the reader
                reader = new StreamReader("../../../" + fileName + ".txt");

                // Data for table size is stored
                string[] tableInfo = reader.ReadLine().Split(',');
                int.TryParse(tableInfo[0], out levelWidth);
                int.TryParse(tableInfo[1], out levelHeight);
                

                List<string> tableState = new List<string>();

                // Rest of the file is input into the list
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    tableState.Add(line);
                }

                // Reader is closed to prevent program self destruction
                reader.Close();

                // List is turned into a long string to make parsing easier
                string parsingString = "";
                for (int l = 0; l < tableState.Count; l++)
                {
                    parsingString = string.Format(parsingString + tableState[l]);
                }

                string[] allTiles = parsingString.Split(',');

                int stringIndex = 0;

                // All levelObjects are put into a single list
                for(int yIterator = 0; yIterator < levelHeight; yIterator++)
                {
                    for (int xIterator = 0; xIterator < levelWidth; xIterator++)
                    {
                        foreach (string[] i in tileList)
                        {
                            if (int.Parse(i[2]) == int.Parse(allTiles[stringIndex]))
                            {
                                levelTiles.Add(new Tile(             
                                    Content.Load<Texture2D>(i[0]),      // Asset
                                    new Rectangle(xIterator * blockWidth,  // X position
                                    yIterator * blockHeight,               // Y position
                                    blockWidth, blockHeight),                // Constant dimensions
                                    bool.Parse(i[1])));           // IsInteractable   
                            }
                        }

                        // Moves on to next tile
                        stringIndex++;
                    }
                }

                // Places the Exit door in the first level
                if (stage == 1)
                {
                    switch (Program.RNG.Next(0, 4))
                    {
                        // Top
                        case 0:
                            // Door
                            exit.Position = new Vector2(levelTiles[(int)Math.Round(levelWidth + levelWidth / 2.0) - 1].Rectangle.X, 
                                levelTiles[(int)Math.Round(levelWidth + levelWidth / 2.0) - 1].Rectangle.Y);
                            levelTiles[(int)Math.Round(levelWidth + levelWidth / 2.0) - 1] = exit; // Replacement

                            // Intro to Hallway; Tile is replaced to be something without interactions
                            levelTiles[(int)Math.Round(levelWidth / 2.0)] = new Tile(Content.Load<Texture2D>("Shadow"), // Asset
                                levelTiles[(int)Math.Round(levelWidth / 2.0)].Rectangle,                                              // Location
                                false);                                                                                     // Hitbox

                            exitLocation = ExitLocation.Top;
                            break;

                        // Right
                        case 1:
                            exit.Position = new Vector2(levelTiles[(int)Math.Round(levelWidth * (levelHeight / 2.0)) - 1].Rectangle.X, 
                                levelTiles[(int)Math.Round(levelWidth * (levelHeight / 2.0)) - 1].Rectangle.Y);
                            levelTiles[(int)Math.Round(levelWidth * (levelHeight / 2.0)) - 1] = exit; // Replacement

                            exitLocation = ExitLocation.Right;
                            break;

                        // Bottom
                        case 2:
                            // Door
                            exit.Position = new Vector2(levelTiles[(int)Math.Round(levelWidth * (levelHeight - 2) + levelWidth / 2.0)].Rectangle.X,
                                levelTiles[(int)Math.Round(levelWidth * (levelHeight - 2) + levelWidth / 2.0)].Rectangle.Y);
                            levelTiles[(int)Math.Round(levelWidth * (levelHeight - 2) + levelWidth / 2.0)] = exit; // Replacement

                            // Intro to Hallway; Tile is replaced to be something without interactions
                            levelTiles[(int)Math.Round(levelWidth * (levelHeight - 1) + levelWidth / 2.0) - 1] = new Tile(Content.Load<Texture2D>("Shadow"), // Asset
                                levelTiles[(int)Math.Round(levelWidth * (levelHeight - 1) + levelWidth / 2.0) - 1].Rectangle,                                              // Location
                                false);                                                                                                                          // Hitbox

                            exitLocation = ExitLocation.Bottom;
                            break;

                        // Left
                        case 3:
                            exit.Position = new Vector2(levelTiles[(int)Math.Round(levelWidth * (levelHeight / 2.0)) - levelWidth].Rectangle.X,
                                levelTiles[(int)Math.Round(levelWidth * (levelHeight / 2.0)) - levelWidth].Rectangle.Y);
                            levelTiles[(int)Math.Round(levelWidth * (levelHeight / 2.0)) - levelWidth] = exit; // Replacement

                            exitLocation = ExitLocation.Left;
                            break;
                    }
                }

                // passes all active tiles to the collision manager
                CollisionManager.UpdateLevel();
            }
            catch (Exception e)
            {
                // Breakpoint on error message in debugging mode
                // should tell what happened. The game simply
                // wouldn't load otherwise
                Console.WriteLine("Error: {0}", e);

                try
                {
                    // Just in case
                    reader.Close();
                }
                catch
                {
                    // This block is required for the try block
                }
            }
        }

        /// <summary>
        /// Purpose: Draws the level to the screen each frame
        /// Restrictions: Level must have been loaded in already
        /// </summary>
        /// <param name="sb"></param>
        public static void Draw(SpriteBatch sb)
        {
            foreach (GameObject i in levelTiles)
            {
                sb.Draw(i.Sprite,   // Asset
                    i.Rectangle,     // Location
                    Color.White);   // Background color
            }
        }

        /// <summary>
        /// Purpose: Compiles list of tiles that can be interacted with
        /// Restrictions: Level must already have been loaded
        /// </summary>
        /// <returns></returns>
        public static List<Tile> GetCollidables()
        {
            List<Tile> collidables = new List<Tile>();

            foreach (Tile i in levelTiles)
            {
                if (i.IsInteractable)
                {
                    collidables.Add(i);
                }
            }

            return collidables;
        }

        public enum ExitLocation
        {
            Top, 
            Right,
            Bottom,
            Left
        }
    }
}
