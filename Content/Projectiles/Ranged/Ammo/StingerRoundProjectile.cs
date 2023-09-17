namespace Cascade.Content.Projectiles.Ranged.Ammo
{
    public class StingerRoundProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.Bullet;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 600;
            Projectile.Calamity().pointBlankShotDuration = 18;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int stingerAmount = Main.rand.Next(3, 7);
            for (int i = 0; i < stingerAmount; i++)
            {
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(TwoPi) * 7f;
                Projectile.SpawnProjectile(Projectile.Center, velocity, ModContent.ProjectileType<StingerRoundStinger>(), (int)(Projectile.damage * 0.65f), Projectile.knockBack * 0.45f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }
    }
}
