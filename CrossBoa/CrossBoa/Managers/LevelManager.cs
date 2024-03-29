﻿using System;
using System.Collections.Generic;
using System.IO;
using CrossBoa.Upgrades;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CrossBoa.Managers
{
    /// <summary>
    /// Delegate for matching methods that need to run when the level changes
    /// </summary>
    public delegate void LevelChangeDelegate();

    /// <summary>
    /// Author: Donovan Scullion
    /// Purpose: handles the generation of levels
    /// Restrictions: levels must be made manually, 
    ///               and this class will need to be updated each
    ///               time to contain them
    /// </summary>
    public static class LevelManager
    {
        /// <summary>
        /// Is invoked when the player touches the exit
        /// </summary>
        public static event LevelChangeDelegate PlayerTouchedExit;

        /// <summary>
        /// Is invoked when the level is about to change, while the screen is fully black
        /// </summary>
        public static event LevelChangeDelegate LevelChanged;

        /// <summary>
        /// Is invoked when the camera reaches the center of the screen during a level transition
        /// </summary>
        public static event LevelChangeDelegate PlayerRegainedControl;

        private static List<string[]> tileList;
        private static List<Tile> levelTiles;
        private static StreamReader reader;
        private const int blockWidth = 64;
        private const int blockHeight = 64;
        private static ContentManager Content;
        private static Door entrance;
        private static Door exit;
        private static int stage;
        private static int levelWidth;
        private static int levelHeight;
        private static ExitLocation exitLocation;
        private static ExitLocation previousExit;
        private static int forcedX;
        private static int forcedY;
        private static string currentLevel;

        private static Tile entranceOtherHalf;
        private static Tile exitOtherHalf;

        // Requires a reference
        public static ContentManager LContent
        {
            get { return Content; }
            set { Content = value; }
        }

        public static Door Exit
        {
            get { return exit; }
        }

        public static Door Entrance
        {
            get { return entrance; }
        }

        public static ExitLocation Exitlocation
        {
            get { return exitLocation; }
        }

        public static int Stage
        {
            get { return stage; }
        }

        public static string CurrentLevel
        {
            get { return currentLevel; }
            set { currentLevel = value; }
        }

        public static StreamReader Reader
        {
            get { return reader; }
            set { reader = value; }
        }

        /// <summary>
        /// Whether or not the player has the overclock upgrade unlocked
        /// </summary>
        public static bool HasOverclockUpgrade { get; set; }

        static LevelManager()
        {
            levelTiles = new List<Tile>();
            tileList = new List<string[]>();
            stage = -1; // No levels yet

            // tileList is immediately filled as it's data is needed
            // before any other method can be made
            try
            {
                reader = new StreamReader("Content/Levels/LevelObjectList.txt");

                // Gets the default instructions out of the file
                string[] fileData = reader.ReadLine()?.Split(',');

                while (!reader.EndOfStream)
                {
                    fileData = reader.ReadLine()?.Split(',');
                    // Data for each basic block type is added
                    tileList.Add(fileData);
                }
            }
            catch (Exception e)
            {
                // Specific error can be read in the debug menu
                Console.WriteLine("Error: {0}", e);

                // Just in case it's open
                reader?.Close();
            }
        }

        /// <summary>
        /// Purpose: Fills level with tiles
        /// Restrictions: Given file must exist and be accurate
        ///               16 x 16 is the only accepted file size at the moment
        /// </summary>
        public static void LoadLevel()
        {
            // Invoke level change event
            LevelChanged?.Invoke();

            // Level is cleared so that the next may be loaded
            levelTiles.Clear();

            if (entrance == null)
            {
                // This is done here because the load method can't be passed in before 
                // this class's constructor is established
                // Doors are created and will be constantly used
                entrance = new Door(Game1.floorSprite, // Open Sprite
                    Game1.wallSprite, // Closed Sprite
                    new Rectangle(-100, -100, blockWidth, blockHeight), // Location and size
                    true);
                
                entranceOtherHalf = new Tile(Game1.topDoorBottomHalfSprite,
                    new Rectangle(-100, -100, blockWidth, blockHeight),
                    true);

                exit = new Door(Game1.floorSprite, // Open Sprite
                    Game1.leftRightDoorSprite, // Closed Sprite
                    new Rectangle(-100, -100, blockWidth, blockHeight), // Location and size
                    true); // Has hitbox

                exitOtherHalf = new Tile(Game1.topExitBottomHalfSprite,
                    new Rectangle(-100, -100, blockWidth, blockHeight),
                    true);
            }

            // Exit must default to closed
            if (exit.IsOpen)
            {
                exit.ChangeDoorState();
            }

            // Entrance must default to open
            if (!entrance.IsOpen)
            {
                entrance.ChangeDoorState();
            }

            // Stage number is updated
            stage++;

            try
            {
                // file is accessed by the reader
                reader = new StreamReader("Content/Levels/" + currentLevel + ".txt");

                // Data for table size is stored
                string[] tableInfo = reader.ReadLine()?.Split(',');
                int.TryParse(tableInfo?[0], out levelWidth);
                int.TryParse(tableInfo?[1], out levelHeight);


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
                foreach (var t in tableState)
                {
                    parsingString = string.Format(parsingString + t);
                }

                string[] allTiles = parsingString.Split(',');

                int stringIndex = 0;

                // All levelObjects are put into a single list
                for (int yIterator = 0; yIterator < levelHeight; yIterator++)
                {
                    for (int xIterator = 0; xIterator < levelWidth; xIterator++)
                    {
                        foreach (string[] i in tileList)
                        {
                            if (int.Parse(i[2]) == int.Parse(allTiles[stringIndex]))
                            {
                                levelTiles.Add(new Tile(
                                    Content.Load<Texture2D>(i[0]), // Asset
                                    new Rectangle(xIterator * blockWidth, // X position
                                        yIterator * blockHeight, // Y position
                                        blockWidth, blockHeight), // Constant dimensions
                                    bool.Parse(i[1]))); // IsInteractable   
                            }
                        }

                        // Moves on to next tile
                        stringIndex++;
                    }
                }

                // Done before doors so enemies don't spawn in doorways
                if (stage >= 1)
                    SpawnManager.UpdateLevel();

                // Doors are inserted into the level
                PlaceDoors();

                // passes all active tiles to the collision manager
                CollisionManager.UpdateLevel();
                Game1.Collectibles.Clear();
                if (stage >= 1)
                    SpawnManager.FillLevel();
            }
            catch (Exception e)
            {
                // Breakpoint on error message in debugging mode
                // should tell what happened. The game simply
                // wouldn't load otherwise
                Console.WriteLine("Error: {0}", e);

                // Just in case
                reader?.Close();
            }

            // Clear any arrows that are left over from the previous level
            ClearArrows();
        }

        /// <summary>
        /// Purpose: Draws the level to the screen each frame
        /// Restrictions: Level must have been loaded in already
        /// </summary>
        /// <param name="sb"></param>
        public static void Draw(SpriteBatch sb)
        {
            foreach (Tile i in levelTiles)
            {
                sb.Draw(i.Sprite,   // Asset
                    i.Rectangle,     // Location
                    Color.White);   // Background color
            }

            if (!entrance.IsOpen && (previousExit == ExitLocation.Top || previousExit == ExitLocation.Bottom))
            {
                entranceOtherHalf.Draw(sb);
            }

            if (!exit.IsOpen && (exitLocation == ExitLocation.Top || exitLocation == ExitLocation.Bottom))
            {
                exitOtherHalf.Draw(sb);
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

        /// <summary>
        /// Purpose: Passes a list of tiles that aren't collidable
        /// Restrictions: none
        /// </summary>
        /// <returns></returns>
        public static List<Tile> GetSafe()
        {
            List<Tile> safe = new List<Tile>();

            foreach (Tile i in levelTiles)
            {
                if (!i.IsInteractable)
                {
                    safe.Add(i);
                }
            }

            Stack<int> toRemove = new Stack<int>();
            int index = 0;

            switch (exitLocation)
            {
                case ExitLocation.Left:
                {
                    foreach (Tile i in safe)
                    {
                        if (i.Rectangle.X > 21 * blockWidth &&
                            i.Rectangle.Y <= (exit.Rectangle.Y + blockHeight) &&
                            i.Rectangle.Y >= exit.Rectangle.Y - blockHeight)
                        {
                            toRemove.Push(index);
                        }
                        index++;
                    }

                    break;
                }
                case ExitLocation.Right:
                {
                    foreach (Tile i in safe)
                    {
                        if (i.Rectangle.X < 3 * blockWidth &&
                            i.Rectangle.Y <= (exit.Rectangle.Y + blockHeight) &&
                            i.Rectangle.Y >= exit.Rectangle.Y - blockHeight)
                        {
                            toRemove.Push(index);
                        }
                        index++;
                    }

                    break;
                }
                case ExitLocation.Top:
                {
                    foreach (Tile i in safe)
                    {
                        if (i.Rectangle.Y > 9 * blockHeight &&
                            i.Rectangle.X <= (exit.Rectangle.X + blockHeight) &&
                            i.Rectangle.X >= exit.Rectangle.X - blockHeight)
                        {
                            toRemove.Push(index);
                        }
                        index++;
                    }

                    break;
                }
                case ExitLocation.Bottom:
                {
                    foreach (Tile i in safe)
                    {
                        if (i.Rectangle.Y < 4 * blockHeight &&
                            i.Rectangle.X <= (exit.Rectangle.X + blockHeight) &&
                            i.Rectangle.X >= exit.Rectangle.X - blockHeight)
                        {
                            toRemove.Push(index);
                        }
                        index++;
                    }

                    break;
                }
            }

            while (toRemove.Count > 0)
            {
                safe.RemoveAt(toRemove.Pop());
            }

            return safe;
        }

        /// <summary>
        /// Purpose: Helper method for letting the player resume the game after a transition
        /// Restrictions: none
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static void Update(Player player)
        {
            // Will close off the entrance after a player fully enters a stage
            if (!entrance.IsOpen) return;
            player.CanMove = true;
            entrance.ChangeDoorState();
            forcedX = 0;
            forcedY = 0;


            // Adds door to collisionManager
            CollisionManager.UpdateLevel();
        }

        /// <summary>
        /// Purpose: Determines how doors will be placed once a new level is loaded in
        /// Restrictions: none
        /// </summary>
        public static void PlaceDoors()
        {
            // Entrance location placement; always opposite of the previous exit
            if (stage >= 1)
            {
                switch (exitLocation)
                {
                    case ExitLocation.Top:
                        // Door
                        entrance.ClosedSprite = Game1.wallSprite;
                        entrance.Position = new Vector2(
                            levelTiles[(int)Math.Round(levelWidth * (levelHeight - 2) + levelWidth / 2.0)].Rectangle.X,
                            levelTiles[(int)Math.Round(levelWidth * (levelHeight - 2) + levelWidth / 2.0)].Rectangle.Y);
                        levelTiles[(int)Math.Round(levelWidth * (levelHeight - 2) + levelWidth / 2.0)] =
                            entrance; // Replacement

                        // Intro to Hallway; Tile is replaced to be something without interactions
                        levelTiles[(int)Math.Round(levelWidth * (levelHeight - 1) + levelWidth / 2.0) - 1] = new Tile(
                            Game1.blackSquareSprite, // Asset
                            levelTiles[(int)Math.Round(levelWidth * (levelHeight - 1) + levelWidth / 2.0) - 1]
                                .Rectangle, // Location
                            false); // Hitbox

                        // Spawn a fake tile on the bottom of the hallway to fill in the door texture
                        entranceOtherHalf.Sprite = Game1.topDoorBottomHalfSprite;
                        entranceOtherHalf.Position = new Vector2(
                            levelTiles[(int)Math.Round(levelWidth * (levelHeight - 2) + levelWidth / 2.0)].Rectangle.X,
                            levelTiles[(int)Math.Round(levelWidth * (levelHeight - 1) + levelWidth / 2.0)].Rectangle.Y);

                        break;

                    case ExitLocation.Right:
                        entrance.Sprite = Game1.blackSquareSprite;
                        entrance.ClosedSprite = Game1.sideWallSprite;

                        entrance.Position = new Vector2(
                            levelTiles[(int)Math.Round(levelWidth * (levelHeight / 2.0)) - levelWidth].Rectangle.X,
                            levelTiles[(int)Math.Round(levelWidth * (levelHeight / 2.0)) - levelWidth].Rectangle.Y);
                        levelTiles[(int)Math.Round(levelWidth * (levelHeight / 2.0)) - levelWidth] =
                            entrance; // Replacement
                        break;

                    case ExitLocation.Bottom:
                        // Door
                        entrance.ClosedSprite = Game1.topDoorBottomHalfSprite;
                        entrance.Position = new Vector2(
                            levelTiles[(int)Math.Round(levelWidth + levelWidth / 2.0) - 1].Rectangle.X,
                            levelTiles[(int)Math.Round(levelWidth + levelWidth / 2.0) - 1].Rectangle.Y);
                        levelTiles[(int)Math.Round(levelWidth + levelWidth / 2.0) - 1] = entrance; // Replacement

                        // Intro to Hallway; Tile is replaced to be something without interactions
                        levelTiles[(int)Math.Round(levelWidth / 2.0)] = new Tile(
                            Game1.blackSquareSprite, // Asset
                            levelTiles[(int)Math.Round(levelWidth / 2.0)].Rectangle, // Location
                            false); // Hitbox

                        // Spawn a fake tile on top of the hallway to fill in the door texture
                        entranceOtherHalf.Sprite = Game1.wallSprite;
                        entranceOtherHalf.Position = new Vector2(
                            levelTiles[(int)Math.Round(levelWidth * (levelHeight - 2) + levelWidth / 2.0)].Rectangle.X,
                            0);

                        break;

                    case ExitLocation.Left:
                        entrance.ClosedSprite = Game1.sideWallSprite;
                        entrance.Position = new Vector2(
                            levelTiles[(int)Math.Round(levelWidth * (levelHeight / 2.0)) - 1].Rectangle.X,
                            levelTiles[(int)Math.Round(levelWidth * (levelHeight / 2.0)) - 1].Rectangle.Y);
                        levelTiles[(int)Math.Round(levelWidth * (levelHeight / 2.0)) - 1] =
                            entrance; // Replacement
                        break;
                }
            }

            // Saves enum before it changes
            previousExit = exitLocation;


            // Exit location placement
            switch (Game1.RNG.Next(0, 4))
            {
                // Top
                case 0:
                    if (stage >= 1 && exitLocation == ExitLocation.Bottom)
                    {
                        // Invalid location for the exit. New attempt is made
                        PlaceDoors();
                        break;
                    }

                    // Door
                    exit.Sprite = Game1.topExitBottomHalfSprite;
                    exit.OpenSprite = Game1.floorSprite;

                    exit.Position = new Vector2(
                        levelTiles[(int)Math.Round(levelWidth + levelWidth / 2.0) - 1].Rectangle.X,
                        levelTiles[(int)Math.Round(levelWidth + levelWidth / 2.0) - 1].Rectangle.Y);
                    levelTiles[(int)Math.Round(levelWidth + levelWidth / 2.0) - 1] = exit; // Replacement

                    // Intro to Hallway; Tile is replaced to be something without interactions
                    levelTiles[(int)Math.Round(levelWidth / 2.0)] = new Tile(
                        Game1.blackSquareSprite, // Asset
                        levelTiles[(int)Math.Round(levelWidth / 2.0)].Rectangle, // Location
                        false); // Hitbox

                    // Spawn a fake tile on top of the hallway to fill in the door texture
                    exitOtherHalf.Sprite = Game1.bottomTopDoorSprite;
                    exitOtherHalf.Position = new Vector2(
                        levelTiles[(int)Math.Round(levelWidth * (levelHeight - 2) + levelWidth / 2.0)].Rectangle.X,
                        0);

                    exitLocation = ExitLocation.Top;
                    break;

                // Right
                case 1:
                    if (stage >= 1 && exitLocation == ExitLocation.Left)
                    {
                        // Invalid location for the exit. New attempt is made
                        PlaceDoors();
                        break;
                    }

                    exit.Sprite = Game1.leftRightDoorSprite;
                    exit.OpenSprite = Game1.floorSprite;

                    exit.Position = new Vector2(
                    levelTiles[(int)Math.Round(levelWidth * (levelHeight / 2.0)) - 1].Rectangle.X,
                    levelTiles[(int)Math.Round(levelWidth * (levelHeight / 2.0)) - 1].Rectangle.Y);
                    levelTiles[(int)Math.Round(levelWidth * (levelHeight / 2.0)) - 1] =
                        exit; // Replacement

                    exitLocation = ExitLocation.Right;
                    break;

                // Bottom
                case 2:
                    if (stage >= 1 && exitLocation == ExitLocation.Top)
                    {
                        // Invalid location for the exit. New attempt is made
                        PlaceDoors();
                        break;
                    }

                    // Door
                    exit.Sprite = Game1.bottomTopDoorSprite;
                    exit.OpenSprite = Game1.floorSprite;

                    exit.Position = new Vector2(
                    levelTiles[(int)Math.Round(levelWidth * (levelHeight - 2) + levelWidth / 2.0)].Rectangle.X,
                    levelTiles[(int)Math.Round(levelWidth * (levelHeight - 2) + levelWidth / 2.0)].Rectangle
                            .Y);
                    levelTiles[(int)Math.Round(levelWidth * (levelHeight - 2) + levelWidth / 2.0)] =
                        exit; // Replacement

                    // Intro to Hallway; Tile is replaced to be something without interactions
                    levelTiles[(int)Math.Round(levelWidth * (levelHeight - 1) + levelWidth / 2.0) - 1] = new Tile(
                        Game1.blackSquareSprite, // Asset
                        levelTiles[(int)Math.Round(levelWidth * (levelHeight - 1) + levelWidth / 2.0) - 1]
                            .Rectangle, // Location
                        false); // Hitbox

                    // Spawn a fake tile on top of the hallway to fill in the door texture
                    exitOtherHalf.Sprite = Game1.topExitBottomHalfSprite;
                    exitOtherHalf.Position = new Vector2(
                        levelTiles[(int)Math.Round(levelWidth * (levelHeight - 2) + levelWidth / 2.0)].Rectangle.X,
                        levelTiles[(int)Math.Round(levelWidth * (levelHeight - 1) + levelWidth / 2.0)].Rectangle.Y);

                    exitLocation = ExitLocation.Bottom;
                    break;

                // Left
                case 3:
                    if (stage >= 1 && exitLocation == ExitLocation.Right)
                    {
                        // Invalid location for the exit. New attempt is made
                        PlaceDoors();
                        break;
                    }
                    exit.Sprite = Game1.leftRightDoorSprite;
                    exit.OpenSprite = Game1.blackSquareSprite;

                    exit.Position = new Vector2(
                        levelTiles[(int)Math.Round(levelWidth * (levelHeight / 2.0)) - levelWidth].Rectangle.X,
                        levelTiles[(int)Math.Round(levelWidth * (levelHeight / 2.0)) - levelWidth].Rectangle.Y);
                    levelTiles[(int)Math.Round(levelWidth * (levelHeight / 2.0)) - levelWidth] =
                        exit; // Replacement

                    exitLocation = ExitLocation.Left;
                    break;
            }
        }

        /// <summary>
        /// Purpose: Helps facilitate the transition between levels
        ///          First part will execute before a new level is loaded
        ///          Second level executes after a level is loaded
        /// Restrictions: God I sure hope there isn't any
        /// </summary>
        public static void LevelTransition(Player player, CrossBow crossbow, GameTime gameTime)
        {
            // Part 1
            if (exitLocation == ExitLocation.Top)
            {
                // Prevents player from getting trapped on the wall
                if (player.Rectangle.Intersects(new Rectangle((int)exit.Position.X, (int)exit.Position.Y, exit.Width, exit.Height / 2)))
                {
                    PlayerCollidedWithExit(player);

                    // Vectors for player movement
                    forcedX = 0;
                    forcedY = -1;
                }

                if (Camera.CameraY >= 1100)
                {
                    // Player and Camera go to the bottom of the level
                    Camera.MoveCamera(0, -Game1.gameRenderTarget.Height - 1700);
                    Game1.Player.Position = new Vector2(Game1.Player.Position.X,
                                Game1.Player.Position.Y + 1300);

                    // Prompts the next level to load in
                    RandomizeLevel();
                    LoadLevel();
                }

                if (player.Position.Y < -100)
                {
                    Camera.MoveCamera(0, 90);

                    // player is made invisible
                    player.Color = Color.Black;
                    crossbow.Color = Color.Black;
                }
            }
            if (exitLocation == ExitLocation.Bottom)
            {
                // Prevents player from getting trapped on the wall
                if (player.Rectangle.Intersects(new Rectangle((int)exit.Position.X, (int)exit.Position.Y + exit.Height / 2, exit.Width, exit.Height / 2)))
                {
                    PlayerCollidedWithExit(player);

                    // Vectors for player movement
                    forcedX = 0;
                    forcedY = 1;
                }

                if (Camera.CameraY <= -Game1.gameRenderTarget.Height - 1100)
                {
                    // Player and Camera go to the bottom of the level
                    Camera.MoveCamera(0, Game1.gameRenderTarget.Height + 1700);
                    Game1.Player.Position = new Vector2(Game1.Player.Position.X,
                        Game1.Player.Position.Y - 1300);

                    // Prompts the next level to load in
                    RandomizeLevel();
                    LoadLevel();
                }

                if (player.Position.Y > Game1.gameRenderTarget.Height + 100)
                {
                    Camera.MoveCamera(0, -90);

                    // player is made invisible
                    player.Color = Color.Black;
                    crossbow.Color = Color.Black;
                }
            }
            if (exitLocation == ExitLocation.Right)
            {
                // Prevents player from getting trapped on the wall
                if (player.Rectangle.Intersects(new Rectangle((int)exit.Position.X + exit.Width / 2, (int)exit.Position.Y, exit.Width / 2, exit.Height)))
                {
                    PlayerCollidedWithExit(player);

                    // Vectors for player movement
                    forcedX = 1;
                    forcedY = 0;
                }

                if (Camera.CameraX <= -Game1.gameRenderTarget.Width - 1100)
                {
                    // Player and Camera go to the bottom of the level
                    Camera.MoveCamera(Game1.gameRenderTarget.Width + 1700, 0);
                    Game1.Player.Position = new Vector2(Game1.Player.Position.X - 2000,
                        Game1.Player.Position.Y);

                    // Prompts the next level to load in
                    RandomizeLevel();
                    LoadLevel();
                }

                if (player.Position.X > Game1.gameRenderTarget.Width + 100)
                {
                    Camera.MoveCamera(-90, 0);

                    // player is made invisible
                    player.Color = Color.Black;
                    crossbow.Color = Color.Black;
                }
            }
            if (exitLocation == ExitLocation.Left)
            {
                // Prevents player from getting trapped on the wall
                if (player.Rectangle.Intersects(new Rectangle((int)exit.Position.X, (int)exit.Position.Y, exit.Width / 2, exit.Height)))
                {
                    PlayerCollidedWithExit(player);

                    // Vectors for player movement
                    forcedX = -1;
                    forcedY = 0;
                }

                if (Camera.CameraX >= Game1.gameRenderTarget.Width + 1100)
                {
                    // Player and Camera go to the bottom of the level
                    Camera.MoveCamera(-Game1.gameRenderTarget.Width - 1700, 0);
                    Game1.Player.Position = new Vector2(Game1.Player.Position.X + 2000,
                        Game1.Player.Position.Y);

                    // Prompts the next level to load in
                    RandomizeLevel();
                    LoadLevel();
                }

                if (player.Position.X < -100)
                {
                    Camera.MoveCamera(90, 0);

                    // player is made invisible
                    player.Color = Color.Black;
                    crossbow.Color = Color.Black;
                }
            }

            // Part 2
            // Overrides player movement
            if (!player.CanMove)
            {
                player.ForceMove(forcedX, forcedY, gameTime);

                // Makes sure consecutive blocks of code can't happen
                if (previousExit == ExitLocation.Top && player.Position.Y > 0 && !(player.Position.X > Game1.gameRenderTarget.Width) && !(player.Position.X < 0))
                {
                    Camera.MoveCamera(0, 90);

                    if (!(Camera.CameraY < 0))
                    {
                        EnterLevel(player, crossbow);

                        // Conditions to give player control again
                        if (player.Position.Y + player.Height + 10 < entrance.Position.Y)
                        {
                            Update(player);
                        }
                    }
                }
                if (previousExit == ExitLocation.Bottom && player.Position.Y < Game1.gameRenderTarget.Height && !(player.Position.X > Game1.gameRenderTarget.Width) && !(player.Position.X < 0))
                {
                    Camera.MoveCamera(0, -90);


                    if (!(Camera.CameraY > 0))
                    {
                        EnterLevel(player, crossbow);

                        // Conditions to give player control again
                        if (player.Position.Y - player.Height - 10 > entrance.Position.Y)
                        {
                            Update(player);
                        }
                    }
                }
                if (previousExit == ExitLocation.Right && player.Position.X < Game1.gameRenderTarget.Width && !(player.Position.Y > Game1.gameRenderTarget.Height) && !(player.Position.Y < 0))
                {
                    Camera.MoveCamera(-90, 0);


                    if (!(Camera.CameraX > 0))
                    {
                        EnterLevel(player, crossbow);

                        // Conditions to give player control again
                        if (player.Position.X > entrance.Position.X + player.Width + 10)
                        {
                            Update(player);
                        }
                    }
                }
                if (previousExit == ExitLocation.Left && player.Position.X > 0 && !(player.Position.Y > Game1.gameRenderTarget.Height) && !(player.Position.Y < 0))
                {
                    Camera.MoveCamera(90, 0);


                    if (!(Camera.CameraX < 0))
                    {
                        EnterLevel(player, crossbow);

                        // Conditions to give player control again
                        if (player.Position.X < entrance.Position.X - player.Width - 10)
                        {
                            Update(player);
                        }
                    }
                }

                // Arrow will return to the player if still on screen
                PlayerArrow playerArrow = Game1.playerArrowList[0];
                if (!playerArrow.IsActive) return;

                playerArrow.GetSuckedIntoPlayer((int)Helper.DistanceSquared(
                    new Point((int)player.Position.X, (int)player.Position.Y), playerArrow.Size), 9000);

                // Doesn't let the player arrow zoom onto screen if it is too far out of bounds
                if (!(playerArrow.Position.X < -50) && !(playerArrow.Position.X > Game1.gameRenderTarget.Width + 50) &&
                    !(playerArrow.Position.Y < -50) && !(playerArrow.Position.Y >
                                                         Game1.gameRenderTarget.Height + 50)) return;
                playerArrow.HitSomething();
                playerArrow.GetPickedUp();
            }
        }

        /// <summary>
        /// Sets the stage number to 0. Currently broken, don't use yet
        /// </summary>
        public static void GameOver()
        {
            stage = -1;
            exitLocation = ExitLocation.Null;
            previousExit = ExitLocation.Null;
            // Might do more if we want the level manager to do other stuff
            // upon game over
        }

        /// <summary>
        /// Purpose: Randomizes what levels may load in
        /// Restrictions: Currently requires hard coding in levels
        ///               Can use stage number to add further restrictions
        /// </summary>
        public static void RandomizeLevel()
        {
            int level = Game1.RNG.Next(1, 6);
            currentLevel = level switch
            {
                1 => "Level1",
                2 => "Level2",
                3 => "Level3",
                4 => "Level4",
                5 => "Level5",
                _ => currentLevel
            };
        }

        /// <summary>
        /// Helper method for when the camera reaches the center of the level
        /// </summary>
        private static void EnterLevel(Player player, CrossBow crossbow)
        {
            // Invoke respective event
            PlayerRegainedControl?.Invoke();

            // Camera is locked to the center of the screen
            Camera.Center();

            // The player is made visible again
            player.Color = Color.White;
            crossbow.Color = Color.White;
        }

        /// <summary>
        /// Helper method for when the player touches the exit
        /// </summary>
        private static void PlayerCollidedWithExit(Player player)
        {
            // Invoke respective event
            PlayerTouchedExit?.Invoke();

            // Player Movement is locked for the transition
            player.CanMove = false;

            // Resets the arrow speed for the next level
            if (HasOverclockUpgrade)
            {
                StatsManager.ArrowSpeed = UpgradeManager.ArrowSpeedWithoutOverclock;
            }
        }

        /// <summary>
        /// Clears all the arrows from the screen
        /// </summary>
        public static void ClearArrows()
        {
            PlayerArrow mainArrow = Game1.playerArrowList[0];

            // Return the main arrow
            foreach (PlayerArrow arrow in Game1.playerArrowList)
            {
                arrow.HitSomething();
                arrow.GetPickedUp();
            }
        }

        public enum ExitLocation
        {
            Null, // So doors don't mess up
            Top,
            Right,
            Bottom,
            Left
        }
    }
}
