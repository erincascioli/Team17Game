namespace CrossBoa.Interfaces
{
    /// <summary>
    /// Written by Leo Schindler-Gerendasi
    /// </summary>
    interface IShoot
    {
        /// <summary>
        /// Shoots the projectile.
        /// </summary>
        /// <param name="projectile">The projectile to shoot.</param>
        void Shoot(Projectile projectile);
    }
}
