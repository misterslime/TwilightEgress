using Cascade.Content.Items.Weapons.Melee;
using CalamityMod.Buffs.DamageOverTime;
using Cascade.Core.Systems.CameraSystem;
using Terraria.GameContent.Events;

namespace Cascade.Content.Projectiles.Melee
{
    public class ResplendentRoarHoldout : ModProjectile
    {
        private enum AttackTypes
        {
            RebirthSlashes,
            FinalDyingRoar
        }

        private Player Owner => Main.player[Projectile.owner];

        private ref float Timer => ref Projectile.ai[0];

        private ref float AttackCounter => ref Projectile.ai[1];

        private ref float AttackType => ref Projectile.localAI[0];

        private ref float AIState => ref Projectile.localAI[1];

        private bool IsChannelingRMB => Owner.Calamity().mouseRight && Owner.active && Owner.HeldItem.type == ModContent.ItemType<ResplendentRoar>();

        private bool ShouldDespawn => Owner.dead || !Owner.active || Owner.CCed || Owner.HeldItem.type != ModContent.ItemType<ResplendentRoar>();

        private Vector2 Direction = Vector2.Zero;

        private Vector2 DistanceFromPlayer => Direction * -2f;

        private float SwingDirection => Projectile.ai[2] * Sign(Direction.X);

        private float BaseRotation { get; set; }

        private bool Initialized { get; set; }

        private CurveSegment Anticipation = new(EasingType.CircIn, 0f, 0f, 0.125f);

        private CurveSegment Thrust = new(EasingType.ExpOut, 0.5f, 0.125f, 0.875f);

        private const float StartingAngle = -(PiOver2 + PiOver4);

        private const float LargeSwingAngle = -(Pi + PiOver4);

        private const int SmallSwingMaxTime = 35;   

        private const int LargeSwingMaxTime = 45;

        private const int YharonFrameIndex = 0;

        private const int YharonFrameCounterIndex = 1;

        private const int YharonOpacityIndex = 2;

        private const int YharonScaleIndex = 3;

        private const int YharonDrawImageRotationIndex = 4;

        private const int YharonDrawImageRadiusIndex = 5;

        private PrimitiveTrail TrailDrawer = null;

        public float SmallSwingRatio() => PiecewiseAnimation(Timer / SmallSwingMaxTime, Anticipation, Thrust);

        public float LargeSwingRatio() => PiecewiseAnimation(Timer / LargeSwingMaxTime, Anticipation, Thrust);

        public override string Texture => "CalamityMod/Items/Weapons/Melee/TheBurningSky";

        public override void Load()
        {
            On_Main.DrawInfernoRings += DrawYharon;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 24;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.scale = 1.25f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float lineLength = 146f * Projectile.scale;
            Vector2 startPoint = Projectile.Center + Projectile.rotation.ToRotationVector2() + DistanceFromPlayer;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), startPoint, startPoint + Projectile.rotation.ToRotationVector2() * lineLength, 60, ref collisionPoint);
        }

        public override void AI()
        {
            if (ShouldDespawn)
            {
                Projectile.Kill();
                return;
            }

            // Left click and right click.
            switch ((AttackTypes)AttackType)
            {
                case AttackTypes.RebirthSlashes:
                    RebirthSlashes();
                    break;

                case AttackTypes.FinalDyingRoar:
                    FinalDyingRoar(); 
                    break;
            }

            Timer++;
            Projectile.Center = Owner.MountedCenter + DistanceFromPlayer;
            ParticleVisuals();
            UpdatePlayerVariables(AttackType == 1f);
        }

        public void RebirthSlashes()
        {
            // The attack pattern.
            switch (AttackCounter)
            {
                case < 2:
                    DoBehavior_SmallSwing();
                    break;

                case >= 2 and < 4:
                    DoBehavior_LargeSwing();
                    break;
            }
        }

        public void DoBehavior_SmallSwing()
        {
            // Some initialization.
            if (!Initialized)
            {
                Initialized = true;
                Projectile.timeLeft = SmallSwingMaxTime;
                Direction = Projectile.velocity;
                Direction.Normalize();
                BaseRotation = Projectile.velocity.ToRotation();
                Projectile.rotation = StartingAngle * SwingDirection + Projectile.velocity.ToRotation();
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }

            float startingAngle = StartingAngle * SwingDirection + BaseRotation;
            float endingAngle = -StartingAngle * SwingDirection + BaseRotation;
            Projectile.rotation = Lerp(startingAngle, endingAngle, SmallSwingRatio());

            float distanceToMiddle = Distance(SmallSwingRatio(), 0.5f);
            if (Timer == (SmallSwingMaxTime / 2))
            {
                SoundEngine.PlaySound(CascadeSoundRegistry.YharonHurt with { PitchVariance = 1f }, Projectile.Center);
                SoundEngine.PlaySound(CommonCalamitySounds.LouderSwingWoosh, Projectile.Center);
            }

            Projectile.scale = 1.25f + (float)Math.Sin((double)(SmallSwingRatio() * (float)Math.PI)) * 0.45f;
        }

        public void DoBehavior_LargeSwing()
        {
            if (!Initialized)
            {
                Initialized = true;
                Projectile.timeLeft = LargeSwingMaxTime;
                Direction = Projectile.velocity;
                Direction.Normalize();
                BaseRotation = Projectile.velocity.ToRotation();
                Projectile.rotation = LargeSwingAngle * SwingDirection + Projectile.velocity.ToRotation();
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }

            float startingAngle = LargeSwingAngle * SwingDirection + BaseRotation;
            float endingAngle = -StartingAngle * SwingDirection + BaseRotation;
            Projectile.rotation = Lerp(startingAngle, endingAngle, LargeSwingRatio());

            float distanceToMiddle = Distance(LargeSwingRatio(), 0.5f);
            if (Timer == (LargeSwingMaxTime / 2))
            {
                SoundEngine.PlaySound(CascadeSoundRegistry.YharonRoarShort with { PitchVariance = 0.15f }, Projectile.Center);
                SoundEngine.PlaySound(CommonCalamitySounds.LouderPhantomPhoenix, Projectile.Center);
            }

            Projectile.scale = 1.25f + (float)Math.Sin((double)(LargeSwingRatio() * (float)Math.PI)) * 0.75f;
        }

        public void FinalDyingRoar()
        {
            ref float yharonFrame = ref Projectile.Cascade().ExtraAI[YharonFrameIndex];
            ref float yharonFrameCounter = ref Projectile.Cascade().ExtraAI[YharonFrameCounterIndex];
            ref float yharonOpacity = ref Projectile.Cascade().ExtraAI[YharonOpacityIndex];
            ref float yharonScale = ref Projectile.Cascade().ExtraAI[YharonScaleIndex];

            if (!Initialized)
            {
                Initialized = true;
                Direction = Projectile.velocity;
                Direction.Normalize();
                BaseRotation = Projectile.velocity.ToRotation();
                Projectile.rotation = BaseRotation;
                SoundEngine.PlaySound(CascadeSoundRegistry.YharonRoarShort with { PitchVariance = 1f }, Projectile.Center);
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }

            if (AIState == 0f)
            {
                Projectile.rotation = Projectile.AngleTo(Main.MouseWorld);

                if (++yharonFrameCounter >= 5)
                {
                    if (++yharonFrame >= 5)
                        yharonFrame = 0f;
                    yharonFrameCounter = 0f;
                }

                float maximumOpacity = 1f * Utils.GetLerpValue(0f, 1f, (Owner.CascadePlayer_ResplendantRoar().ResplendentRazeCharge / 100f), true);
                float maximumScale = 1.35f * Utils.GetLerpValue(0f, 1f, (Owner.CascadePlayer_ResplendantRoar().ResplendentRazeCharge / 100f), true);
                yharonOpacity = Lerp(yharonOpacity, maximumOpacity, Timer / 90f);
                yharonScale = Lerp(yharonScale, maximumScale, Timer / 90f);

                if ((Timer >= 90f && !IsChannelingRMB) || Timer >= 600f)
                {
                    AIState = 1f;
                    Timer = 0f;
                    SoundEngine.PlaySound(CascadeSoundRegistry.YharonRoar with { Volume = 4f }, Projectile.Center);

                    for (int i = 0; i < 3; i++)
                    {
                        Color colorGroup = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.IndianRed, Color.Yellow, Color.Red);
                        Color secondColorGroup = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.OrangeRed, Color.Sienna, Color.PaleVioletRed);
                        Color fireColor = Color.Lerp(colorGroup, secondColorGroup, Main.rand.NextFloat(0.2f, 0.8f));

                        float scale = i == 2 ? 9f : i == 1f ? 6f : 3f;
                        DirectionalPulseRing pulseRing = new(Owner.Center, Vector2.Zero, fireColor, new(1f, 1f), 0f, 0.01f, scale, 60);
                        Utilities.SpawnParticleBetter(pulseRing);
                    }

                    Projectile.netUpdate = true;
                }
            }
           
            if (AIState == 1f)
            {
                yharonFrame = 6;
                yharonFrameCounter = 0f;

                Owner.immuneTime = 30;
                Owner.velocity = Projectile.rotation.ToRotationVector2() * 55f;

                if (Timer >= 30f)
                {
                    Timer = 0f;
                    AIState = 2f;
                    Projectile.netUpdate = true;
                }
            }

            if (AIState == 2f)
            {
                yharonOpacity = Clamp(yharonOpacity - 0.025f, 0f, 1f);
                yharonScale = Clamp(yharonScale - 0.025f, 0f, 1.35f);
                Projectile.scale = Clamp(Projectile.scale - 0.025f, 0f, 1f);
                Owner.velocity *= 0.9f;

                if (Timer >= 30f)
                {
                    Projectile.Kill();
                    Owner.CascadePlayer_ResplendantRoar().ResplendentRazeCharge = 0f;
                }
            }
            Owner.CascadePlayer_ResplendantRoar().ResplendentRazeUpdateTimer = 0;
        }

        public void ParticleVisuals()
        {
            if (Owner.CascadePlayer_ResplendantRoar().ResplendentRazeCharge >= 25f)
            {
                for (int i = 0; i < 3; i++)
                {
                    float lineLength = 146f * Projectile.scale;
                    Vector2 spawnPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * lineLength + DistanceFromPlayer + Main.rand.NextVector2Circular(80f, 80f);

                    Color colorGroup = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.IndianRed, Color.Yellow, Color.Red);
                    Color secondColorGroup = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.OrangeRed, Color.Sienna, Color.PaleVioletRed);
                    Color fireColor = Color.Lerp(colorGroup, secondColorGroup, Main.rand.NextFloat(0.2f, 0.8f));

                    int lifespan = Main.rand.Next(15, 30);
                    float scale = Main.rand.NextFloat(0.25f, 0.85f) * (Owner.CascadePlayer_ResplendantRoar().ResplendentRazeCharge / 100f) - 2.5f;
                    float opacity = Main.rand.NextFloat(0.9f, 4f) * (Owner.CascadePlayer_ResplendantRoar().ResplendentRazeCharge / 100f) - 1f;

                    HeavySmokeParticle flames = new(spawnPosition, Vector2.Zero, fireColor, lifespan, scale, opacity, 0.03f, true);
                    Utilities.SpawnParticleBetter(flames);
                }

                for (int i = 0; i < 4; i++)
                {
                    float lineLength = 146f * Projectile.scale;
                    Vector2 spawnPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * lineLength + DistanceFromPlayer + Main.rand.NextVector2Circular(120f, 120f);

                    Color colorGroup = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.IndianRed, Color.Yellow, Color.Red);
                    Color secondColorGroup = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.OrangeRed, Color.Sienna, Color.PaleVioletRed);
                    Color sparkleColor = Color.Lerp(colorGroup, secondColorGroup, Main.rand.NextFloat(0.2f, 0.8f));

                    int lifespan = Main.rand.Next(15, 30);
                    float scale = Main.rand.NextFloat(0.45f, 1f) * (Owner.CascadePlayer_ResplendantRoar().ResplendentRazeCharge / 100f) - 3f;
                    float opacity = Main.rand.NextFloat(0.65f, 2f) * (Owner.CascadePlayer_ResplendantRoar().ResplendentRazeCharge / 100f) - 1f;

                    GenericSparkle sparkle = new(spawnPosition, Vector2.Zero, sparkleColor * opacity, sparkleColor * 1.05f * opacity, scale, lifespan);
                    Utilities.SpawnParticleBetter(sparkle);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Charge the resplendant raze.
            Owner.CascadePlayer_ResplendantRoar().ResplendentRazeCharge += 2f;
            if (hit.Crit)
                Owner.CascadePlayer_ResplendantRoar().ResplendentRazeCharge += 4f;

            // Add the Dragonfire Debuff after the charge reaches 75%
            if (Owner.CascadePlayer_ResplendantRoar().ResplendentRazeCharge >= 75f)
            {
                target.AddBuff(ModContent.BuffType<Dragonfire>(), 180);
                SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { PitchVariance = 1f, MaxInstances = 0 }, target.Center);
            }

            // Visual stuff.
            CascadeCameraSystem.Screenshake(3, 6, target.Center);
            for (int i = 0; i < Main.rand.Next(15, 25); i++)
            {
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(5f, 15f);
                float scale = Main.rand.NextFloat(0.85f, 1.25f);
                int lifespan = Main.rand.Next(25, 45);

                Color colorGroup = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.IndianRed, Color.Yellow, Color.Red);
                Color secondColorGroup = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.OrangeRed, Color.Sienna, Color.PaleVioletRed);
                Color sparkColor = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.5f, colorGroup, secondColorGroup);

                SparkParticle sparks = new(target.Center, velocity, false, lifespan, scale, sparkColor);
                Utilities.SpawnParticleBetter(sparks);
            }

            if (Owner.CascadePlayer_ResplendantRoar().ResplendentRazeCharge >= 75f)
            {
                for (int i = 0; i < Main.rand.Next(10, 12); i++)
                {
                    Vector2 velocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(3f, 7f);
                    float scale = Main.rand.NextFloat(1f, 2f);
                    int lifespan = Main.rand.Next(25, 45);

                    Color colorGroup = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.IndianRed, Color.Yellow, Color.Red);
                    Color secondColorGroup = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.OrangeRed, Color.Sienna, Color.PaleVioletRed);
                    Color flameColor = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.5f, colorGroup, secondColorGroup);

                    HeavySmokeParticle flames = new(target.Center, velocity, flameColor, lifespan, scale, 1f, 0.03f, true);
                    Utilities.SpawnParticleBetter(flames);
                }
            }

        }

        public void UpdatePlayerVariables(bool updateDirectionByRotation = false)
        {
            Owner.heldProj = Projectile.whoAmI;
            if (updateDirectionByRotation)
                Owner.direction = Sign(Projectile.rotation.ToRotationVector2().X);
            else
                Owner.direction = Math.Sign(Projectile.velocity.X);
            Owner.itemRotation = Projectile.rotation;
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= (float)Math.PI;
            }
            Owner.itemRotation = WrapAngle(Projectile.rotation);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - PiOver2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ref float yharonOpacity = ref Projectile.Cascade().ExtraAI[YharonOpacityIndex];
            ref float yharonScale = ref Projectile.Cascade().ExtraAI[YharonScaleIndex];

            // This sets the starting position of the primitive trail so that the other positions follow it accordingly
            Projectile.oldPos[0] = Projectile.position + Projectile.rotation.ToRotationVector2() * 128f * Projectile.scale;

            if (AttackType != 1f)
                DrawPrimTrail();

            DrawHalberd();
            return false;
        }

        public void DrawHalberd()
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            bool shouldFlipSprite = SwingDirection == -1;

            SpriteEffects effects = shouldFlipSprite ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            float extraAngle = (shouldFlipSprite ? PiOver2 : 0f);
            float baseDrawAngle = Projectile.rotation;
            float drawRotation = baseDrawAngle + PiOver4 + extraAngle;

            Vector2 origin = new Vector2(shouldFlipSprite ? texture.Width : 0f, texture.Height);
            Vector2 drawPosition = Projectile.Center + baseDrawAngle.ToRotationVector2() - Main.screenPosition;

            // Draw backglow effects. 
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            for (int i = 0; i < 4; i++)
            {
                Vector2 backglowDrawPositon = drawPosition + Vector2.UnitY.RotatedBy(i * TwoPi / 4) * 3f;

                Color colorGroup = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.IndianRed, Color.Yellow, Color.Red);
                Color secondColorGroup = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.OrangeRed, Color.Sienna, Color.PaleVioletRed);
                Color backglowColor = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.5f, colorGroup, secondColorGroup);

                Main.EntitySpriteDraw(texture, backglowDrawPositon, null, Projectile.GetAlpha(backglowColor), drawRotation, origin, Projectile.scale * 1.015f, effects, 0);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);

            // Draw the main sprite.
            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(Color.White), drawRotation, origin, Projectile.scale, effects, 0);
        }

        public void DrawYharon(On_Main.orig_DrawInfernoRings orig, Main self)
        {
            ref float yharonFrame = ref Projectile.Cascade().ExtraAI[YharonFrameIndex];
            ref float yharonOpacity = ref Projectile.Cascade().ExtraAI[YharonOpacityIndex];
            ref float yharonScale = ref Projectile.Cascade().ExtraAI[YharonScaleIndex];
            ref float yharonImageRotation = ref Projectile.Cascade().ExtraAI[YharonDrawImageRotationIndex];
            ref float yharonImageRadius = ref Projectile.Cascade().ExtraAI[YharonDrawImageRadiusIndex];

            Texture2D baseTexture = ModContent.Request<Texture2D>("Cascade/Content/Projectiles/Melee/ResplendentRoarYharon").Value;
            SpriteEffects effects = Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Rectangle projRec = baseTexture.Frame(1, 7, 0, (int)(yharonFrame % 7));
            Vector2 origin = new Vector2((Owner.direction < 0) ? projRec.Width * 0.4f : projRec.Width * 0.6f, projRec.Height * 0.6f);

            float extraAngle = (Owner.direction < 0 ? Pi : 0f);
            float rotation = Projectile.rotation - extraAngle; 

            Color colorGroup = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.IndianRed, Color.Yellow, Color.Red);
            Color secondColorGroup = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.OrangeRed, Color.Sienna, Color.PaleVioletRed);
            Color chickenColor = Color.Lerp(MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.5f, colorGroup, secondColorGroup), Color.White, 0.3f);

            for (int i = 0; i < 6; i++)
            {
                yharonImageRotation += TwoPi / 1200f;
                Vector2 drawPosition = Owner.Center - Main.screenPosition + Vector2.UnitY.RotatedBy(yharonImageRotation + TwoPi * i / 6f) * 30f;
                Main.spriteBatch.Draw(baseTexture, drawPosition, projRec, Projectile.GetAlpha(chickenColor * 0.9f) * yharonOpacity, rotation, origin, yharonScale, effects, 0);
            }
            orig.Invoke(self);
        }

        public float SetTrailWidth(float completionRatio)
        {
            return 60f * Utils.GetLerpValue(1f, 0f, completionRatio, true) * Projectile.scale;
        }

        public Color SetTrailColor(float completionRatio)
        {
            Color colorGroup = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.IndianRed, Color.Yellow, Color.Red);
            Color secondColorGroup = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.OrangeRed, Color.Sienna, Color.PaleVioletRed);
            Color trailColor = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.5f, colorGroup, secondColorGroup);
            return trailColor;
        }

        public void DrawPrimTrail()
        {
            TrailDrawer ??= new PrimitiveTrail(SetTrailWidth, SetTrailColor, null, GameShaders.Misc["CalamityMod:ExobladePierce"]);

            Color colorGroup = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.IndianRed, Color.Yellow, Color.Red);
            Color secondColorGroup = MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.OrangeRed, Color.Sienna, Color.PaleVioletRed);

            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseColor(colorGroup);
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseSecondaryColor(secondColorGroup);
            GameShaders.Misc["CalamityMod:ExobladePierce"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/EternityStreak"));
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseImage2("Images/Extra_189");
            GameShaders.Misc["CalamityMod:ExobladePierce"].Apply();
            Vector2 trailOffset = Projectile.Size / 2f - Main.screenPosition;

            TrailDrawer.Draw(Projectile.oldPos.Take(9), trailOffset, 48);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
