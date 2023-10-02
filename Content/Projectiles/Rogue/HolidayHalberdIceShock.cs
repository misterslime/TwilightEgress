﻿using CalamityMod.Buffs.StatDebuffs;

namespace Cascade.Content.Projectiles.Rogue
{
    public class HolidayHalberdIceShock : ModProjectile, ILocalizedModType
    {
        private ref float Timer => ref Projectile.ai[0];

        private ref float RandomNewScale => ref Projectile.ai[1];

        private const int MaxLifetime = 75;

        private const int ProjectileTextureOpacityIndex = 0;

        public new string LocalizationCategory => "Projectiles.Rogue";

        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CultistBossIceMist;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 92;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.Opacity = 0f;
            Projectile.scale = 0f;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void OnSpawn(IEntitySource source)
        {
            RandomNewScale = Main.rand.NextFloat(1f, 1.75f);
            Projectile.rotation = Main.rand.NextFloat(TwoPi);
            SoundEngine.PlaySound(CascadeSoundRegistry.IceShock, Projectile.Center);         
        }

        public override void AI()
        {
            ref float projectileTextureOpacity = ref Projectile.Cascade().ExtraAI[ProjectileTextureOpacityIndex];

            if (Timer >= MaxLifetime)
            {
                Projectile.Kill();
                return;
            }

            // Scale up.
            if (Timer <= 20f)
            {
                Projectile.scale = Lerp(Projectile.scale, RandomNewScale, SineInOutEasing(Timer / 20f, 0));
                Projectile.Opacity = Lerp(Projectile.Opacity, 1f, SineInOutEasing(Timer / 20f, 0));
            }

            if (Timer >= 30f)
                projectileTextureOpacity = Clamp(projectileTextureOpacity + 0.065f, 0f, 1f);

            // Particles.
            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.width);
                    Color color = Color.Lerp(Color.LightSkyBlue, Color.Cyan, Main.rand.NextFloat());
                    float scale = Main.rand.NextFloat(0.2f, 0.8f) * Projectile.scale;
                    int lifespan = Main.rand.Next(15, 30);
                    GenericSparkle sparkleParticle = new(spawnPosition, Vector2.Zero, color, color * 0.35f, scale, lifespan, 0.25f, 1.25f);
                    GeneralParticleHandler.SpawnParticle(sparkleParticle);
                }
            }
            
            if (Main.rand.NextBool(5))
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 velocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(3f, 10f);
                    Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.width);
                    Color initialColor = Color.Lerp(Color.LightSkyBlue, Color.Cyan, Main.rand.NextFloat()) * Main.rand.NextFloat(0.45f, 0.75f);
                    float scale = Main.rand.NextFloat(0.75f, 2f) * Projectile.scale;
                    float opacity = Main.rand.NextFloat(180f, 240f);
                    MediumMistParticle deathSmoke = new MediumMistParticle(spawnPosition, velocity, initialColor, initialColor * 0.45f, scale, opacity, 0.03f);
                    GeneralParticleHandler.SpawnParticle(deathSmoke);
                }
            }        

            Timer++;
            Projectile.AdjustProjectileHitboxByScale(92f, 92f);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(CascadeSoundRegistry.CryogenShieldBreak, Projectile.Center);
            // Spawn a ring of arcing snowflakes, similar to the original Iceshock.
            float snowflakeAngularVelocity = ToRadians(3f);
            for (int i = 0; i < 6; i++)
            {
                Vector2 snowflakeVelocity = Vector2.UnitX.RotatedBy(TwoPi * i / 6) * 16f;
                int damage = Projectile.damage.GetPercentageOfInteger(0.65f);
                Projectile.SpawnProjectile(Projectile.Center, snowflakeVelocity, ModContent.ProjectileType<HolidayHalberdIceShockSnowflake>(), damage, 
                    Projectile.knockBack, owner: Projectile.owner, ai0: snowflakeAngularVelocity);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn2, 180);
            if (Main.rand.NextBool(10) && !target.HasBuff(ModContent.BuffType<GlacialState>()))
            {
                target.AddBuff(ModContent.BuffType<GlacialState>(), 180);
                SoundEngine.PlaySound(CascadeSoundRegistry.IceShockPetrify, Projectile.Center);
            }
            
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ref float projectileTextureOpacity = ref Projectile.Cascade().ExtraAI[ProjectileTextureOpacityIndex];
            Texture2D glowTexture = ModContent.Request<Texture2D>("Cascade/Content/Projectiles/Rogue/HolidayHalberdIceShock_Glow").Value;

            // Backglow.
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Projectile.DrawBackglow(Projectile.GetAlpha(Color.LightSkyBlue * 0.45f), 3f);
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);

            // White overlay.
            Projectile.DrawTextureOnProjectile(Projectile.GetAlpha(Color.White), Projectile.rotation, Projectile.scale, texture: glowTexture);
            // Actual projectile texture.
            Projectile.DrawTextureOnProjectile(Color.White * projectileTextureOpacity, Projectile.rotation, Projectile.scale);

            return false;
        }
    }
}