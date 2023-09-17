using Cascade.Content.DedicatedContent.Fluffy;

namespace Cascade.Content.DedicatedContent.Jacob
{
    public class Tanksplosion : Bastsplosion, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";

        // Nothin to see here, just inheriting this class so I don't have to copy the same code over.
        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }
    }
}
