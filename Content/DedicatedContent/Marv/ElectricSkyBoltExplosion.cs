namespace Cascade.Content.DedicatedContent.Marv
{
    public class ElectricSkyBoltExplosion : ModProjectile, ILocalizedModType
    {
        public ref float Timer => ref Projectile.ai[0];

        public new string LocalizationCategory => "Projectiles.Magic";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Thunderous Explosion");
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = (Projectile.height = (int)(100f / Projectile.scale));
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.scale = 0.1f;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
        }

        public override void AI()
        {
            if (Timer >= 45)
            {
                Projectile.Kill();
                return;
            }

            // Sounds and other initial effects.
            if (Timer == 1)
            {
                SoundEngine.PlaySound(CommonCalamitySounds.ExoPlasmaExplosionSound);
                // Particle creation.
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int sparkLifespan = Main.rand.Next(20, 36);
                    float sparkScale = Main.rand.NextFloat(0.75f, 1.25f);
                    Color sparkColor = Color.Lerp(Color.LightYellow, Color.Goldenrod, Main.rand.NextFloat());
                    for (int i = 0; i < 50; i++)
                    {
                        Vector2 sparkVelocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(9f, 16f);
                        GeneralParticleHandler.SpawnParticle(new SparkParticle(Projectile.Center, sparkVelocity, false, sparkLifespan, sparkScale, sparkColor));
                    }
                }
            }

            float sine = SineInOutEasing(Timer / 22.5f, 1);
            if (Timer <= 45)
            {
                Projectile.scale = Lerp(0.1f, 2f, sine);
                Projectile.Opacity = Lerp(0f, 1f, sine);
            }

            Timer++;
            Main.LocalPlayer.Calamity().GeneralScreenShakePower = 4f;
            Projectile.rotation += Pi / 30f;
            Lighting.AddLight(Projectile.Center, Color.Goldenrod.ToVector3() * 0.65f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Vortex = CascadeTextureRegistry.GreyscaleVortex.Value;
            Color electroColor = Color.Lerp(Color.Goldenrod, Color.LightYellow, 0.35f);
            // Vortex 1.
            Projectile.DrawTextureOnProjectile(Projectile.GetAlpha(electroColor), -Projectile.rotation * 1.5f, Projectile.scale * 4f, texture: Vortex);
            //Vortex 2.
            Projectile.DrawTextureOnProjectile(Projectile.GetAlpha(electroColor), Projectile.rotation, Projectile.scale * 4.25f, texture: Vortex);
            // Electric Ball.
            Projectile.DrawTextureOnProjectile(Projectile.GetAlpha(electroColor), 0f, Projectile.scale, animated: true);

            return false;
        }
    }
}
