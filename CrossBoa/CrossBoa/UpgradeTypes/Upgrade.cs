using Microsoft.Xna.Framework.Graphics;

namespace CrossBoa.UpgradeTypes
{
    /// <summary>
    /// The quality of this upgrade
    /// </summary>
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
    public abstract class Upgrade
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
        /// <param name="name">The name of the upgrade that will be shown in game</param>
        /// <param name="description">The description of the upgrade that will be shown in game.</param>
        /// <param name="effect">A delegate from UpgradeManager that this upgrade will run when it is obtained</param>
        /// <param name="sprite">The sprite that this upgrade will appear as in game</param>
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
        public virtual void Run()
        {
            effect();
        }
    }
}
