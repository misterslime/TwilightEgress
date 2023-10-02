﻿namespace Cascade.Content.Projectiles.Summoner.GeminiGenies
{
    public class SpiritFlame : ModProjectile, ILocalizedModType
    {
        private ref float Timer => ref Projectile.ai[0];

        public new string LocalizationCategory => "Projectiles.Summon";

        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.SpiritFlame;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.timeLeft = 405;
            Projectile.scale = 0f;
            Projectile.Opacity = 0f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
        }

        public override void AI()
        {
            NPC closestTarget = Projectile.FindClosestNPCToProjectile(1500f);
            if (closestTarget is null || GeminiGeniePsychic.Myself is null)
            {
                Projectile.Kill();
                return;
            }

            int fadeinTime = 45;
            if (Timer <= fadeinTime)
            {
                Projectile.velocity *= 0.9f;
                Projectile.Opacity = Lerp(Projectile.Opacity, 1f, SineInOutEasing(Timer / fadeinTime, 0));
                Projectile.scale = Lerp(Projectile.scale, 1f, SineInOutEasing(Timer / fadeinTime, 0));
            }

            // Move towards nearby targets.
            if (Timer >= fadeinTime)
            {
                Projectile.SimpleMove(closestTarget.Center, 20f, 60f);

                Vector2 dustPosition = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height);
                Utilities.CreateDustLoop(2, dustPosition, Vector2.Zero, DustID.Shadowflame);
            }

            Timer++;
            Projectile.AdjustProjectileHitboxByScale(14f, 20f);
            Projectile.UpdateProjectileAnimationFrames(0, 4, 3);
        }

        public override void OnKill(int timeLeft)
        {
            Utilities.CreateRandomizedDustExplosion(20, Projectile.Center, DustID.Shadowflame);
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Projectile.DrawBackglow(Projectile.GetAlpha(Color.White * 0.45f), 2f);
            DrawAfterimagesCentered(Projectile, 0, Projectile.GetAlpha(Color.Magenta));
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);

            Projectile.DrawTextureOnProjectile(Projectile.GetAlpha(Color.White), Projectile.rotation, Projectile.scale, animated: true);
            return false;
        }
    }
}