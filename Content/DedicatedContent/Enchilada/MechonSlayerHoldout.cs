﻿using Cascade.Content.Cooldowns;
using Cascade.Core.Systems.CameraSystem;
using Steamworks;

namespace Cascade.Content.DedicatedContent.Enchilada
{
    public class MechonSlayerHoldout : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];

        private ref float Timer => ref Projectile.ai[0];

        private ref float WeaponState => ref Projectile.ai[1];

        private CurveSegment Anticipation = new(EasingType.SineOut, 0f, 0f, -0.125f);
        private CurveSegment Thrust = new(EasingType.ExpOut, 0.5f, 0.125f, 0.875f);

        private const int SwingTime = 45;
        private const int MaxTime = 60;

        public float SwordOverheadThrust() => PiecewiseAnimation(Timer / SwingTime, Anticipation, Thrust);

        private bool Initialized { get; set; } = false;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (Owner.ShouldDespawnHeldProj(ModContent.ItemType<MechonSlayer>()))
            {
                Projectile.Kill();
                return;
            }

            InitializeFields(MaxTime, -PiOver2);
            Vector2 holdPosition = Owner.Top + new Vector2(0f, -20f * SwordOverheadThrust() + 35f);
            Projectile.Center = Owner.RotatedRelativePoint(holdPosition, true);

            Timer++;
            if (Timer == (SwingTime / 2))
            {
                // Apply the art specific buffs.
                Owner.Cascade_Buffs().ApplyMechonSlayerArt((int)WeaponState);

                // Visuals.
                CascadeCameraSystem.Screenshake(3, 10, Owner.Center);
                SoundEngine.PlaySound(CommonCalamitySounds.LaserCannonSound, Projectile.Center);
                for (int i = 0; i < 15; i++)
                {
                    Vector2 spawnPosition = Owner.MountedCenter;
                    Vector2 velocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(3f, 8f);
                    GenericSparkle sparkle = new(spawnPosition, velocity, GetArtColor(Color.Cyan), GetArtColor(Color.Cyan) * 0.75f, Main.rand.NextFloat(0.65f, 1.25f), Main.rand.Next(30, 45), 0.03f);
                    GeneralParticleHandler.SpawnParticle(sparkle);
                }
            }

            if (Timer >= (SwingTime / 2))
            {
                // Shake a bit after the visual effects.
                Projectile.position += Main.rand.NextVector2Circular(5f, 5f);
                // Fade out.
                if (Timer is >= 30 and <= MaxTime)
                    Projectile.Opacity = Utils.GetLerpValue(1f, 0f, (Timer - 30f) / 30f, true);
            }

            Projectile.UpdatePlayerVariablesForHeldProjectile(Owner);
        }

        public void InitializeFields(int timeLeft, float rotation, SoundStyle? soundToPlay = null)
        {
            if (!Initialized)
            {
                Initialized = true;
                SoundEngine.PlaySound(soundToPlay ?? null, Projectile.Center);
                Projectile.timeLeft = timeLeft;
                Projectile.rotation = rotation;
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawBlade();
            DrawArtSpecificVisualEffects();
            return false;
        }

        public Color GetArtColor(Color? baseColor = null)
        {
            Color glowColor = baseColor ?? Color.Transparent;
            switch (WeaponState)
            {
                // Armor.
                case 0:
                    glowColor = Color.Orange;
                    break;

                // Eater.
                case 1:
                    glowColor = Color.LightSlateGray;
                    break;

                // Enchant.
                case 2:
                    glowColor = Color.Magenta;
                    break;

                // Purge.
                case 3:
                    glowColor = Color.LimeGreen;
                    break;

                // Speed.
                case 4:
                    glowColor = Color.LightSkyBlue;
                    break;
            }

            return glowColor;
        }

        public void DrawBlade()
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            float baseDrawAngle = Projectile.rotation;
            float drawRotation = baseDrawAngle + PiOver4;

            Vector2 origin = new(0f, texture.Height);
            Vector2 drawPosition = Projectile.Center + baseDrawAngle.ToRotationVector2() - Main.screenPosition;

            // Draw the main sprite.
            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(Color.White), drawRotation, origin, Projectile.scale, 0);
        }

        public void DrawArtSpecificVisualEffects()
        {
            Texture2D glowMaskTexture = ModContent.Request<Texture2D>(GlowTexture).Value;            
            if (WeaponState > -1)
            {
                float baseDrawAngle = Projectile.rotation;
                float drawRotation = baseDrawAngle + PiOver4;

                Vector2 origin = new(0f, glowMaskTexture.Height);
                Vector2 drawPosition = Projectile.Center + baseDrawAngle.ToRotationVector2() - Main.screenPosition;

                Main.EntitySpriteDraw(glowMaskTexture, drawPosition, null, Projectile.GetAlpha(GetArtColor()), drawRotation, origin, Projectile.scale, 0);
            }
        }
    }
}