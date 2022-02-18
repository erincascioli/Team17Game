using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// Author: Donovan Scullion
/// Purpose: handles the generation of levels
/// Restrictions: levels must be made manually, 
///               and this class will need to be updated each
///               time to contain them
/// </summary>
namespace CrossBoa
{
    public class LevelManager
    {
        private List<string[]> tileList;
        private List<GameObject> levelTiles;
        private StreamReader reader;
        private const int blockWidth = 16;
        private const int blockHeight = 16;
        private bool isInteractable;
        Microsoft.Xna.Framework.Content.ContentManager Content;

        public bool IsInteractable
        {
            get { return isInteractable; }
        }

        public LevelManager(Microsoft.Xna.Framework.Content.ContentManager loading)
        {
            levelTiles = new List<GameObject>();
            tileList = new List<string[]>();
            Content = loading;

            // tileList is immediatly filled as it's data is needed
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
        /// </summary>
        /// <param name="i"></param>
        public void LoadLevel(string fileName)
        {
            // Level is cleared so that the next may be loaded
            levelTiles.Clear();

            try
            {
                // file is accessed by the reader
                reader = new StreamReader("../../" + fileName + ".txt");

                // Data for table size is stored
                int numColumns;
                int numRows;

                string[] tableInfo = reader.ReadLine().Split(',');
                int.TryParse(tableInfo[0], out numColumns);
                int.TryParse(tableInfo[1], out numRows);
                

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

                int stringIndex = 0;

                // All levelObjects are put into a single list
                for(int yIterator = 0; yIterator < numRows; yIterator++)
                {
                    for (int xIterator = 0; xIterator < numColumns; xIterator++)
                    {
                        foreach (string[] i in tileList)
                        {
                            if (char.Parse(i[2]) == parsingString[stringIndex])
                            {
                                levelTiles.Add(new GameObject(             
                                    Content.Load<Texture2D>(i[0]),        // Asset
                                    new Rectangle(xIterator * blockWidth, // X position
                                    yIterator * blockHeight,              // Y position
                                    blockWidth, blockHeight)));           // Constant dimensions

                                // levelTiles[levelTiles.Count - 1].
                            }
                        }

                        // Moves on to next tile
                        stringIndex++;
                    }
                }
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
        public void Draw(SpriteBatch sb)
        {
            foreach (GameObject i in levelTiles)
            {
                sb.Draw(i.Sprite,   // Asset
                    i.Position,     // Location
                    Color.White);   // Background color
            }
        }
    }
}
