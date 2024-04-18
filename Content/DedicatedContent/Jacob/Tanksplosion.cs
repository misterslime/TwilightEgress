using Cascade.Content.DedicatedContent.Fluffy;

namespace Cascade.Content.DedicatedContent.Jacob
{
    public class Tanksplosion : Bastsplosion, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";

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
