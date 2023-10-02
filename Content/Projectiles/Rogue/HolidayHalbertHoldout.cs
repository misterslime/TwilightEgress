﻿using Cascade.Content.Items.Weapons.Rogue;

namespace Cascade.Content.Projectiles.Rogue
{
    public class HolidayHalbertHoldout : ModProjectile, ILocalizedModType
    {
        private Player Owner => Main.player[Projectile.owner];

        private ref float Timer => ref Projectile.ai[0];

        private bool ShouldDespawn => Owner.dead || !Owner.channel || !Owner.active || Owner.CCed || Owner.HeldItem.type != ModContent.ItemType<HolidayHalberd>();

        private const int MaxSpinTimeThreshold = 50;

        private const int RotationSpeedIndex = 0;

        private PrimitiveDrawingSystem TrailDrawer { get; set; }

        public new string LocalizationCategory => "Projectiles.Rogue";

        public override string Texture => "Cascade/Content/Items/Weapons/Rogue/HolidayHalberd";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.scale = 0f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // idk wtf collision point does personally, don't ask me
            // - fryzahh
            float collisionPoint = 0f;

            // Get the length we want for our collision line. In this casee it should match the weapon.
            float lineLength = 78f * Projectile.scale;
            // We get the position where the blade is being held.
            Vector2 startPoint = Projectile.Center + Projectile.rotation.ToRotationVector2();
            // And finally, return a line collision check.
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), startPoint, startPoint + Projectile.rotation.ToRotationVector2() * lineLength, 32, ref collisionPoint);
        }

        public override void AI()
        {
            if (ShouldDespawn)
            {
                Projectile.Kill();
                return;
            }

            Timer++;
            AttackBehavior();
            UpdatePlayerVariables();
        }

        public void AttackBehavior()
        {
            ref float rotationSpeed = ref Projectile.Cascade().ExtraAI[RotationSpeedIndex];

            // Increase the spin speed over time.
            if (Timer <= 30f)
                rotationSpeed = Lerp(45f, 8f, Timer / 30f);


            // Fire and kill.
            if (Timer >= MaxSpinTimeThreshold)
            {
                Vector2 velocity = Projectile.SafeDirectionTo(Main.MouseWorld) * 30f * Projectile.scale;
                Vector2 spawnPosition = Projectile.Center + Projectile.SafeDirectionTo(Main.MouseWorld) * 5f;
                int p = Projectile.SpawnProjectile(spawnPosition, velocity, ModContent.ProjectileType<HolidayHalberdThrown>(), Projectile.damage,
                    Projectile.knockBack, true, CommonCalamitySounds.LouderSwingWoosh, Projectile.owner);

                if (Main.projectile.IndexInRange(p))
                {
                    if (Owner.Calamity().StealthStrikeAvailable())
                    {
                        Main.projectile[p].Calamity().stealthStrike = true;
                        Main.projectile[p].damage = Projectile.damage.GetPercentageOfInteger(0.25f);
                        Owner.ConsumeStealthManually();
                    }
                }

                Projectile.Kill();
                return;
            }

            Projectile.Center = Owner.MountedCenter;
            Projectile.rotation += Pi / rotationSpeed * Owner.direction;
            Projectile.scale = Clamp(Projectile.scale + 0.05f, 0f, 1f);

            // Particles.
            if (Main.rand.NextBool(3))
            {
                Vector2 spawnPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * 78f + Main.rand.NextVector2Circular(65f, 65f) * Projectile.scale;
                float scale = Main.rand.NextFloat(0.2f, 0.8f) * Projectile.scale;
                int lifespan = Main.rand.Next(15, 30);
                GenericSparkle sparkleParticle = new(spawnPosition, Vector2.Zero, GetHalberdVisualColors(), GetHalberdVisualColors() * 0.35f, scale, lifespan, 0.25f, 1.25f);
                GeneralParticleHandler.SpawnParticle(sparkleParticle);
            }
        }

        public void UpdatePlayerVariables()
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.direction = Main.MouseWorld.X < Owner.Center.X ? -1 : 1;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - PiOver2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Get the draw points for the primitive trail.
            for (int i = 0; i < 12; i++)
            {
                float localRotation = Projectile.oldRot[i];
                if (i == 0)
                    localRotation = Projectile.rotation;
                Projectile.oldPos[i] = Projectile.position + localRotation.ToRotationVector2() * 78f * Projectile.scale;
            }

            DrawPrimTrail();
            DrawHalberd();
            return false;
        }

        public Color GetHalberdVisualColors()
        {
            Color mainColor = ColorSwap(Color.Red, Color.Lime, 2f);
            Color stealthColor = MulticolorLerp(Main.GlobalTimeWrappedHourly / 2f, Color.Cyan, Color.LightCyan, Color.LightSkyBlue, Color.LightBlue);
            if (Owner.Calamity().rogueStealth > 0f)
                mainColor = Color.Lerp(mainColor, stealthColor, Owner.Calamity().rogueStealth / Owner.Calamity().rogueStealthMax);

            return mainColor;
        }

        public void DrawHalberd()
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteEffects effects = Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // This is all explained in MSK's file if you're having trouble understanding.
            float extraAngle = (Owner.direction < 0 ? PiOver2 : 0f);
            float baseDrawAngle = Projectile.rotation;
            float drawRotation = baseDrawAngle + PiOver4 + extraAngle;

            Vector2 origin = new Vector2((Owner.direction < 0) ? texture.Width : 0f, texture.Height);
            Vector2 drawPosition = Projectile.Center + baseDrawAngle.ToRotationVector2() - Main.screenPosition;

            // Draw backglow effects. 
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            for (int i = 0; i < 4; i++)
            {
                Vector2 backglowDrawPositon = drawPosition + Vector2.UnitY.RotatedBy(i * TwoPi / 4) * 3f;
                Main.EntitySpriteDraw(texture, backglowDrawPositon, null, Projectile.GetAlpha(GetHalberdVisualColors()), drawRotation, origin, Projectile.scale, effects, 0);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);

            // Draw the main sprite.
            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(Color.White), drawRotation, origin, Projectile.scale, effects, 0);
        }

        public float SetTrailWidth(float completionRatio)
        {
            return 30f * Utils.GetLerpValue(1f, 0.4f, completionRatio, true) * Projectile.scale;
        }

        public Color SetTrailColor(float completionRatio) => GetHalberdVisualColors();      

        public void DrawPrimTrail()
        {
            TrailDrawer ??= new PrimitiveDrawingSystem(SetTrailWidth, SetTrailColor, true, GameShaders.Misc["CalamityMod:TrailStreak"]);

            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/FabstaffStreak", AssetRequestMode.AsyncLoad));
            Vector2 trailOffset = Projectile.Size / 2f - Main.screenPosition; 

            TrailDrawer.DrawPrimitives(Projectile.oldPos.ToList(), trailOffset, 36);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}