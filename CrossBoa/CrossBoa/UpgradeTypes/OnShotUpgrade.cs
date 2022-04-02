using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace CrossBoa.UpgradeTypes
{
    class OnShotUpgrade : Upgrade
    {
        /// <summary>
        /// Initializes a new upgrade type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="effect"></param>
        /// <param name="sprite"></param>
        public OnShotUpgrade(string name, string description, UpgradeBehavior effect, Texture2D sprite) :
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
