using System;
using System.Collections.Generic;
using System.Text;
using CrossBoa.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CrossBoa
{
    /// <summary>
    /// Author: Donovan Scullion
    /// Purpose: Keeps track of door states
    /// Restrictions: none
    /// </summary>
    public class Door : Tile
    {
        private bool isOpen;
        private Texture2D openSprite;
        private Texture2D closedSprite;

        public bool IsOpen
        {
            get { return isOpen; }
        }

        /// <summary>
        /// The closed sprite for this door
        /// </summary>
        public Texture2D OpenSprite
        {
            get { return openSprite; }
            set { openSprite = value; }
        }

        /// <summary>
        /// The closed sprite for this door
        /// </summary>
        public Texture2D ClosedSprite
        {
            get { return closedSprite; }
            set { closedSprite = value; }
        }

        public Door(Texture2D openSprite, Texture2D closedSprite, Rectangle rectangle, bool canInteract) 
            : base(closedSprite, rectangle, canInteract)
        {
            // Values are passed in
            // The Door will always default to closed
            // as doors will only open during a transition
            this.openSprite = openSprite;
            this.closedSprite = closedSprite;
            isOpen = false;
        }

        /// <summary>
        /// Purpose: Visually shows the door opening and closing
        /// Restrictions: none
        /// </summary>
        public void ChangeDoorState()
        {
            if (isOpen == false)
            {
                // The door opens
                sprite = openSprite;
                isOpen = true;
                isInteractable = false;

                if (LevelManager.Stage != -1 && Game1.Player.CanMove)
                    SoundManager.beastWallBump.Play();

                return; // ends method
            }

            // The door closes
            sprite = closedSprite;
            isOpen = false;
            isInteractable = true;

            if (LevelManager.Stage != -1  && Game1.Player.CanMove)
                SoundManager.beastWallBump.Play();
        }
    }
}
