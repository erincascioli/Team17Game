using System;
using Microsoft.Xna.Framework.Graphics;

namespace CrossBoa.UpgradeTypes
{
    class StatBoostUpgrade : Upgrade
    {
        /// <summary>Initializes a new upgrade type.</summary>
        public StatBoostUpgrade(string name, string description, UpgradeBehavior effect, Texture2D sprite) : 
            base(name, description, effect, sprite)
        {


        }

        /// <summary>
        /// Runs whenever this upgrade should provide an effect
        /// </summary>
        public override void Run()
        {
            throw new NotImplementedException();
        }
    }
}
