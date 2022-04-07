using Microsoft.Xna.Framework.Graphics;

namespace CrossBoa.Upgrades
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
    /// The type of upgrade
    /// </summary>
    public enum UpgradeType
    {
        OnShot,
        StatBoost
    }

    /// <summary>
    /// The method that this upgrade should call when used
    /// </summary>
    public delegate void UpgradeBehavior();

    /// <summary>
    /// Parent class for all upgrade types
    /// </summary>
    public class Upgrade
    {
        protected string name;
        protected string description;
        protected Texture2D sprite;
        protected UpgradeBehavior effect;
        protected UpgradeType type;

        //protected UpgradeQuality quality;
        
        /// <summary>
        /// The name of this upgrade
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// The description of this upgrade that will be displayed in game
        /// </summary>
        public string Description
        {
            get { return description; }
        }
        
        /// <summary>
        /// The sprite for this upgrade
        /// </summary>
        public Texture2D Sprite
        {
            get { return sprite; }
        }

        /// <summary>
        /// The effect that this upgrade will have
        /// </summary>
        public UpgradeBehavior Effect
        {
            get { return effect; }
        }

        /// <summary>
        /// The type of upgrade that this is
        /// </summary>
        public UpgradeType Type
        {
            get { return type; }
        }

        /// <summary>
        /// How powerful this upgrade is
        /// </summary>
        //public UpgradeQuality Quality { get; }

        /// <summary>
        /// Initializes a new upgrade type.
        /// </summary>
        /// <param name="name">The name of the upgrade that will be shown in game</param>
        /// <param name="description">The description of the upgrade that will be shown in game.</param>
        /// <param name="effect">A delegate from UpgradeManager that this upgrade will run when it is obtained</param>
        /// <param name="type">The type of upgrade that this will be</param>
        /// <param name="sprite">The sprite that this upgrade will appear as in game</param>
        public Upgrade(string name, string description, UpgradeBehavior effect, UpgradeType type, Texture2D sprite)
        {
            this.effect = effect;
            this.name = name;
            this.description = description;
            this.type = type;
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
