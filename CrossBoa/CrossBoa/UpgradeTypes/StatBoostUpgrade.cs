using System;
using Microsoft.Xna.Framework.Graphics;

namespace CrossBoa.UpgradeTypes
{
    public class StatBoostUpgrade : Upgrade
    {

        /// <summary>Initializes a new upgrade type.</summary>
        public StatBoostUpgrade(string name, string description, UpgradeBehavior effect, Texture2D sprite) : 
            base(name, description, effect, UpgradeType.StatBoost, sprite)
        {


        }
    }
}
