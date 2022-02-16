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
        private List<GameObject> backgroundTiles;
        private List<GameObject> obstacleTiles;
        private StreamReader reader;

        public LevelManager()
        {
            backgroundTiles = new List<GameObject>();
            obstacleTiles = new List<GameObject>();
        }

        /// <summary>
        /// Purpose: fills different lists with tiles
        /// Restrictions: Must have a list of positions and list of assets set up to this specific pattern
        /// </summary>
        /// <param name="i"></param>
        public void LoadLevel(string fileName, List<Texture2D> assets, List<Rectangle> position)
        {
            try
            {
                // file is accessed by the reader
                reader = new StreamReader("../../" + fileName + ".txt");

                string[] symbolInfo = reader.ReadLine().Split(',');
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

                // Program currently allows for a level to use 10 different at a time
                // Extend the loop if more are needed (very unlikely)
                // Alternates between background and object tiles
                // Useless placeholders can be added if skipping is required
                for (int i = 0; i < parsingString.Length; i++)
                {
                    if (parsingString[i] == char.Parse(symbolInfo[0]))
                    {
                        backgroundTiles.Add(new GameObject(assets[0], position[i]));
                    }
                    else if (parsingString[i] == char.Parse(symbolInfo[1])) 
                    {
                        obstacleTiles.Add(new GameObject(assets[1], position[i]));
                    }
                    else if (parsingString[i] == char.Parse(symbolInfo[2]))
                    {
                        backgroundTiles.Add(new GameObject(assets[2], position[i]));
                    }
                    else if (symbolInfo.Length >= 3 && parsingString[i] == char.Parse(symbolInfo[3]))
                    {
                        obstacleTiles.Add(new GameObject(assets[3], position[i]));
                    }
                    else if (symbolInfo.Length >= 4 && parsingString[i] == char.Parse(symbolInfo[4]))
                    {
                        backgroundTiles.Add(new GameObject(assets[4], position[i]));
                    }
                    else if (symbolInfo.Length >= 5 && parsingString[i] == char.Parse(symbolInfo[5]))
                    {
                        obstacleTiles.Add(new GameObject(assets[5], position[i]));
                    }
                    else if (symbolInfo.Length >= 6 && parsingString[i] == char.Parse(symbolInfo[6]))
                    {
                        backgroundTiles.Add(new GameObject(assets[6], position[i]));
                    }
                    else if (symbolInfo.Length >= 7 && parsingString[i] == char.Parse(symbolInfo[7]))
                    {
                        obstacleTiles.Add(new GameObject(assets[7], position[i]));
                    }
                    else if (symbolInfo.Length >= 8 && parsingString[i] == char.Parse(symbolInfo[8]))
                    {
                        backgroundTiles.Add(new GameObject(assets[8], position[i]));
                    }
                    else if (symbolInfo.Length >= 9 && parsingString[i] == char.Parse(symbolInfo[9]))
                    {
                        obstacleTiles.Add(new GameObject(assets[9], position[i]));
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
    }
}
