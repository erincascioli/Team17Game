using Microsoft.Xna.Framework.Graphics;

namespace CrossBoa.UpgradeTypes
{
    public enum UpgradeQuality
    {
        Bronze,
        Silver,
        Gold
    }

    /// <summary>
    /// The method that this upgrade should call when used
    /// </summary>
    public delegate void UpgradeBehavior();

    /// <summary>
    /// Parent class for all upgrade types
    /// </summary>
    abstract class Upgrade
    {
        protected string name;
        protected string description;
        protected UpgradeBehavior effect;
        protected Texture2D sprite;
        //protected UpgradeQuality quality;

        /// <summary>
        /// The sprite for this upgrade
        /// </summary>
        public Texture2D Sprite { get; }

        /// <summary>
        /// How powerful this upgrade is
        /// </summary>
        //public UpgradeQuality Quality { get; }

        /// <summary>
        /// The description of this upgrade that will be displayed in game
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Initializes a new upgrade type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="effect"></param>
        /// <param name="sprite"></param>
        protected Upgrade(string name, string description, UpgradeBehavior effect, Texture2D sprite)
        {
            this.effect = effect;
            this.name = name;
            this.description = description;
            this.sprite = sprite;
        }

        /// <summary>
        /// Runs whenever this upgrade should provide an effect
        /// </summary>
        public abstract void Run();
    }
}
