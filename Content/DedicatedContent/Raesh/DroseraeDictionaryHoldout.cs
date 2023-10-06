﻿namespace Cascade.Content.DedicatedContent.Raesh
{
    public class DroseraeDictionaryHoldout : ModProjectile, ILocalizedModType
    {
        private Player Owner => Main.player[Projectile.owner];

        private ref float Timer => ref Projectile.ai[0];

        private const int MaxChargeTime = 60;

        private const int RitualCircleOpacityIndex = 0;

        private const int RitualCircleRotationIndex = 1;

        private const int RitualCircleScaleIndex = 2;

        private bool ShouldDespawn => Owner.dead || Owner.CCed || !Owner.active || Owner.HeldItem.type != ModContent.ItemType<DroseraeDictionary>();

        public new string LocalizationCategory => "Projectiles.Magic";

        public override string Texture => Utilities.EmptyPixelPath;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 114;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            ref float ritualCircleOpacity = ref Projectile.Cascade().ExtraAI[RitualCircleOpacityIndex];
            ref float ritualCircleRotation = ref Projectile.Cascade().ExtraAI[RitualCircleRotationIndex];
            ref float ritualCircleScale = ref Projectile.Cascade().ExtraAI[RitualCircleScaleIndex];

            bool manaIsAvailable = Owner.CheckMana(Owner.HeldItem.mana);
            bool weaponIsInUse = manaIsAvailable && Owner.PlayerIsChannelingWithItem(ModContent.ItemType<DroseraeDictionary>());

            if (ShouldDespawn || !weaponIsInUse)
            {
                Projectile.Kill();
                return;
            }

            DoBehavior_MainAttack(ref ritualCircleOpacity, ref ritualCircleScale);

            Timer++;
            Projectile.Center = Owner.MountedCenter + Projectile.rotation.ToRotationVector2() * 60f;
            Projectile.rotation = Owner.AngleTo(Main.MouseWorld);
            ritualCircleRotation += TwoPi / 150f;
            UpdatePlayerVariables();
        }

        public void DoBehavior_MainAttack(ref float ritualCircleOpacity, ref float ritualCircleScale)
        {
            // Scale up and fade in.
            if (Timer <= MaxChargeTime)
            {
                ritualCircleOpacity = Lerp(ritualCircleOpacity, 1f, Timer / MaxChargeTime);
                ritualCircleScale = Lerp(ritualCircleScale, 1f, Timer / MaxChargeTime);
                DrawInChargeParticles();
            }

            // Fire.
            if (Timer >= MaxChargeTime && Timer % 30 == 0)
            {
                Vector2 flytrapMawSpawnPos = Projectile.Center;
                Vector2 flyTrapMawVelocity = Projectile.SafeDirectionTo(Main.MouseWorld) * 35f;

                float damageScaleFactor = Lerp(1f, 5f, Utils.GetLerpValue(Owner.statLifeMax, 100f, Owner.statLife, true));
                int damage = Projectile.originalDamage.GetPercentageOfInteger(damageScaleFactor);
                Projectile.SpawnProjectile(flytrapMawSpawnPos, flyTrapMawVelocity, ModContent.ProjectileType<FlytrapMaw>(), damage, Projectile.knockBack, true, CascadeSoundRegistry.FlytrapMawSpawn, Projectile.owner);

                Owner.ConsumeManaManually(Owner.HeldItem.mana);
                ParticleBurst();
                Timer = MaxChargeTime;
            }
        }

        public void DrawInChargeParticles()
        {
            Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2CircularEdge((Projectile.width * 0.375f) + 50f, (Projectile.height * 0.485f) + 50f);
            Vector2 velocity = Vector2.Normalize(Projectile.Center - spawnPosition) * Main.rand.NextFloat(5f, 9f);

            int lifespan = Main.rand.Next(30, 45);
            float scale = Main.rand.NextFloat(0.65f, 1f);

            SparkParticle magicSparks = new(spawnPosition, velocity, false, lifespan, scale, Color.Crimson);
            GeneralParticleHandler.SpawnParticle(magicSparks);
        }

        public void ParticleBurst()
        {
            int sparkCount = Main.rand.Next(15, 25);
            for (int i = 0; i < sparkCount; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(Projectile.width * 0.375f, Projectile.height * 0.485f) * Main.rand.NextFloat(0.05f, 0.2f);

                int lifespan = Main.rand.Next(30, 45);
                float scale = Main.rand.NextFloat(0.65f, 1f);

                SparkParticle magicSparks = new(Projectile.Center, velocity, false, lifespan, scale, Color.Crimson);
                GeneralParticleHandler.SpawnParticle(magicSparks);
            }
        }

        public void IdleDustEffects()
        {
            if (Main.rand.NextBool(3))
            {
                Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width * 0.375f, Projectile.height * 0.485f);
                Color dustColor = Color.Lerp(Color.Crimson, Color.DarkRed, Main.rand.NextFloat());
                float dustScale = Main.rand.NextFloat(0.65f, 1f);
                Utilities.CreateDustLoop(3, spawnPosition, Vector2.Zero, 264, dustScale: dustScale, dustColor: dustColor);
            }
        }

        public void UpdatePlayerVariables()
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.ChangeDir(Sign(Projectile.rotation.ToRotationVector2().X));
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - PiOver2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawRitualCircle();
            CalamityUtils.ExitShaderRegion(Main.spriteBatch);

            return false;
        }

        public void DrawRitualCircle()
        {
            ref float ritualCircleOpacity = ref Projectile.Cascade().ExtraAI[RitualCircleOpacityIndex];
            ref float ritualCircleRotation = ref Projectile.Cascade().ExtraAI[RitualCircleRotationIndex];
            ref float ritualCircleScale = ref Projectile.Cascade().ExtraAI[RitualCircleScaleIndex];

            Texture2D ritualCircle = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/RancorMagicCircle").Value;
            Texture2D blurredRitualCircle = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/RancorMagicCircleGlowmask").Value;

            // Summoning Circle.
            Vector2 ritualCircleDrawPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() - Main.screenPosition;

            Utilities.ApplyRancorMagicCircleShader(blurredRitualCircle, ritualCircleOpacity, -ritualCircleRotation, Projectile.AngleTo(Main.MouseWorld), Projectile.direction, Color.Crimson, Color.Red, BlendState.Additive);
            Main.EntitySpriteDraw(blurredRitualCircle, ritualCircleDrawPosition, null, Color.Red, 0f, blurredRitualCircle.Size() / 2f, ritualCircleScale * 1.275f, SpriteEffects.None, 0);
            Utilities.ApplyRancorMagicCircleShader(ritualCircle, ritualCircleOpacity, ritualCircleRotation, Projectile.AngleTo(Main.MouseWorld), Projectile.direction, Color.DarkRed, Color.Crimson, BlendState.AlphaBlend);
            Main.EntitySpriteDraw(ritualCircle, ritualCircleDrawPosition, null, Color.Red, 0f, ritualCircle.Size() / 2f, ritualCircleScale, SpriteEffects.None, 0);
            CalamityUtils.ExitShaderRegion(Main.spriteBatch);
        }
    }
}
