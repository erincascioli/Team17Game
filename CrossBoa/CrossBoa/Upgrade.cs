using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace CrossBoa
{
    public enum UpgradeQuality
    {
        Basic,
        Strong
    }

    /// <summary>
    /// Parent class for all upgrade types
    /// </summary>
    abstract class Upgrade
    {
        protected Texture2D sprite;
        protected UpgradeQuality quality;
        protected string description;

        /// <summary>
        /// The sprite for this upgrade
        /// </summary>
        public Texture2D Sprite { get; }

        /// <summary>
        /// How powerful this upgrade is
        /// </summary>
        public UpgradeQuality Quality { get; }

        /// <summary>
        /// The description of this upgrade that will be displayed in game
        /// </summary>
        public string Description { get; }
    }
}
