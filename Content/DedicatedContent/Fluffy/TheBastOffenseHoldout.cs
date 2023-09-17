namespace Cascade.Content.DedicatedContent.Fluffy
{
    public class TheBastOffenseHoldout : ModProjectile, ILocalizedModType
    {
        private Player Owner => Main.player[Projectile.owner];

        private ref float ChargeTimer => ref Projectile.ai[0];

        private ref float ResetAnimationTimer => ref Projectile.ai[1];

        private ref float AIState => ref Projectile.ai[2];

        public const int BastIncreaseDelayIndex = 0;

        public const int BastCatCountIndex = 1;

        public const int BackglowRotationIndex = 2;

        public const int WeaponShakeIndex = 3;

        public const int OldRotationIndex = 4;

        public const int RecoilStrengthIndex = 5;

        public new string LocalizationCategory => "Projectiles.Ranged";

        public override string Texture => "Cascade/Content/DedicatedContent/Fluffy/TheBastOffense";

        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 2700;
        }

        public override void AI()
        {
            ref float bastIncreaseDelay = ref Projectile.Cascade().ExtraAI[BastIncreaseDelayIndex];
            ref float bastCatCount = ref Projectile.Cascade().ExtraAI[BastCatCountIndex];
            ref float weaponShake = ref Projectile.Cascade().ExtraAI[WeaponShakeIndex];
            
            bool shouldDespawn = !Owner.active || Owner.HeldItem.type != ModContent.ItemType<TheBastOffense>();
            if (shouldDespawn)
            {
                Projectile.Kill();
                return;
            }

            UpdateProjectileVariablesAndFunctions(Owner, ref bastIncreaseDelay, ref bastCatCount);
            UpdatePlayerVariables(Owner);
        }

        public void UpdateProjectileVariablesAndFunctions(Player owner, ref float bastIncreaseDelay, ref float bastCatCount)
        {
            Projectile.Center = owner.RotatedRelativePoint(owner.MountedCenter, true);
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += Pi;
            }

            PerformAttacks(owner, ref bastIncreaseDelay, ref bastCatCount);
        }

        public void PerformAttacks(Player owner, ref float bastIncreaseDelay, ref float bastCatCount)
        {
            bool isChanneling = Owner.channel && Owner.active && Owner.HeldItem.type == ModContent.ItemType<TheBastOffense>();
            bool isChannelingRMB = Owner.Calamity().mouseRight && Owner.active && Owner.HeldItem.type == ModContent.ItemType<TheBastOffense>();

            if (AIState == 0f)
            {
                int chargeTime = 120;
                if (Owner.altFunctionUse == 2 && isChannelingRMB)
                {
                    Projectile.rotation = Projectile.rotation.AngleLerp(owner.MountedCenter.AngleTo(Main.MouseWorld), 0.2f);
                    if (ChargeTimer <= chargeTime)
                    {
                        if (ChargeTimer % 10 == 0)
                        {
                            float maxScale = ChargeTimer == chargeTime ? 0.01f : 1.25f;
                            float newScale = ChargeTimer == chargeTime ? 5f : 0.01f;
                            Utilities.SpawnParticleBetter(new DirectionalPulseRing(Projectile.Center + Projectile.rotation.ToRotationVector2() * 35f, Vector2.Zero, Color.Gold, new Vector2(0.5f, 1f), Projectile.rotation, maxScale + Main.rand.NextFloat(0.3f), newScale, 30));
                            SoundEngine.PlaySound(SoundID.Item75, Projectile.Center);
                        }
                    }

                    if (ChargeTimer >= chargeTime)
                    {
                        AIState = 1f;
                        Vector2 bastStatueMegaVelocity = Projectile.SafeDirectionTo(Main.MouseWorld, Vector2.UnitY) * 10f;
                        Projectile.SpawnProjectile(Projectile.Center + Projectile.rotation.ToRotationVector2() * 50f, bastStatueMegaVelocity, ModContent.ProjectileType<GiantBastStatue>(), Projectile.damage, Projectile.knockBack, true, SoundID.Item62, Projectile.owner);
                    }

                    ChargeTimer++;
                }
                else
                {
                    Projectile.rotation = owner.MountedCenter.AngleTo(Main.MouseWorld);
                    

                    int increaseBastInterval = 5;
                    int maxBastCatCount = 200;

                    // Initialization.
                    if (ChargeTimer == 0)
                    {
                        bastIncreaseDelay = 60f;
                        Projectile.netUpdate = true;
                    }

                    // Everytime the charge timer has reached past both the interval and delay it is set back to 0
                    // and the delay is decreased by 1, making it charge faster over time.
                    if (ChargeTimer >= increaseBastInterval + bastIncreaseDelay && bastCatCount <= maxBastCatCount)
                    {
                        Utilities.SpawnParticleBetter(new DirectionalPulseRing(Projectile.Center + Projectile.rotation.ToRotationVector2() * 35f, Vector2.Zero, Color.Gold, new Vector2(0.5f, 1f), Projectile.rotation, 1.25f + Main.rand.NextFloat(0.3f), 0.01f, 30));
                        SoundEngine.PlaySound(SoundID.Item149 with { MaxInstances = 0 }, Projectile.Center);
                        // Play a different sound and a spawn a dust circle to indicate it's done charging.
                        if (bastCatCount == maxBastCatCount)
                        {
                            Utilities.CreateDustCircle(30, Projectile.Center, DustID.Firework_Yellow, 10f, shouldDefyGravity: true);
                            SoundEngine.PlaySound(SoundID.ResearchComplete with { MaxInstances = 0 }, Projectile.Center);
                        }
                        bastIncreaseDelay = (int)Clamp(bastIncreaseDelay - 2f, 5f, 60f);
                        bastCatCount++;
                        ChargeTimer = increaseBastInterval;
                    }

                    if (isChanneling)
                        ChargeTimer++;
                    else
                    {
                        AIState = 1f;
                        ChargeTimer = 0f;
                        Projectile.netUpdate = true;
                    }
                }
            }

            if (AIState == 1f)
            {
                ref float oldRotation = ref Projectile.Cascade().ExtraAI[OldRotationIndex];
                ref float recoilStrength = ref Projectile.Cascade().ExtraAI[RecoilStrengthIndex];
                recoilStrength = Lerp(1f, 3f, bastCatCount / 200f);

                if (Owner.altFunctionUse == 2)
                {
                    if (ResetAnimationTimer == 0f)
                    {
                        oldRotation = owner.MountedCenter.AngleTo(Main.MouseWorld);
                        Projectile.netUpdate = true;
                    }

                    // Recoil animation.
                    ResetAnimationTimer++;
                    if (ResetAnimationTimer <= 45f)
                        Projectile.rotation = Lerp(Projectile.rotation, oldRotation + ToRadians(-135f) * Owner.direction, ExpOutEasing(ResetAnimationTimer / 35f, 0));


                    if (ResetAnimationTimer >= 45f)
                        Projectile.Kill();
                }
                else
                {
                    // Play a fart sound if there were not cats loaded.
                    if (ResetAnimationTimer == 0f)
                    {
                        if (bastCatCount <= 0)
                            SoundEngine.PlaySound(SoundID.Item16, Projectile.Center);
                        oldRotation = owner.MountedCenter.AngleTo(Main.MouseWorld);
                        Projectile.netUpdate = true;
                    }

                    // Fire the cat armada.
                    if (bastCatCount > 0)
                    {
                        for (int i = 0; i < bastCatCount; i++)
                        {
                            Vector2 velocity = Projectile.SafeDirectionTo(Main.MouseWorld, Vector2.UnitY).RotatedByRandom(ToRadians(25f)) * Main.rand.NextFloat(13f, 19f);
                            Vector2 spawnPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * 50f;
                            Projectile.SpawnProjectile(spawnPosition, velocity, ModContent.ProjectileType<HomingBastStatue>(), Projectile.damage, Projectile.knockBack, true, SoundID.Item62, Projectile.owner);
                        }

                        ChargeTimer = 0f;
                        bastCatCount = 0f;
                        Utilities.SpawnParticleBetter(new DirectionalPulseRing(Projectile.Center + Projectile.rotation.ToRotationVector2() * 35f, Vector2.Zero, Color.Gold, new Vector2(0.5f, 1f), Projectile.rotation, 0.01f + Main.rand.NextFloat(0.3f), recoilStrength, 30));
                    }

                    // Recoil animation.
                    ResetAnimationTimer++;
                    if (ResetAnimationTimer <= 45f)
                    {
                        Projectile.rotation = Lerp(Projectile.rotation, oldRotation + ToRadians(-85f) * Owner.direction * recoilStrength, ExpOutEasing(ResetAnimationTimer / 25f, 0));
                    }

                    if (ResetAnimationTimer >= 45f)
                    {
                        Projectile.Kill();
                        return;
                    }
                }

                
            }
        }

        public void KibbyExplosion(Player owner)
        {
        }

        public void Reset()
        {
            AIState = 0f;
            ChargeTimer = 0f;
            ResetAnimationTimer = 0f;
            Projectile.timeLeft = 2700;
        }

        public void UpdatePlayerVariables(Player owner)
        {
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;
            if (AIState == 0f)
                owner.ChangeDir(Math.Sign(Projectile.rotation.ToRotationVector2().X));
            owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - PiOver2);
            owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - PiOver2);
        }

        public override bool PreKill(int timeLeft)
        {
            // Blow the player to smithereenes if they decide not too fire for the entire duration.
            if (Owner.channel)
                KibbyExplosion(Owner);
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ref float backglowRotation = ref Projectile.Cascade().ExtraAI[BackglowRotationIndex];

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteEffects effects = Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = Projectile.rotation + (Owner.direction < 0 ? Pi : 0f);
            Vector2 drawPosition = Owner.MountedCenter + new Vector2(0f, 0f) + Projectile.rotation.ToRotationVector2() - Main.screenPosition;

            // Draw pulsing backglow effects.
            for (int i = 0; i < 4; i++)
            {
                backglowRotation += TwoPi / 300f;
                float backglowRadius = Lerp(2f, 5f, SineInOutEasing((float)(Main.timeForVisualEffects / 30f), 1));
                Vector2 backglowDrawPositon = drawPosition + Vector2.UnitY.RotatedBy(backglowRotation + TwoPi * i / 4) * backglowRadius;

                Main.spriteBatch.SetBlendState(BlendState.Additive);
                Main.EntitySpriteDraw(texture, backglowDrawPositon, texture.Frame(), Projectile.GetAlpha(Color.LightYellow), rotation, texture.Size() / 2f, Projectile.scale, effects, 0);
                Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            }

            // Draw the main sprite.
            Main.EntitySpriteDraw(texture, drawPosition, texture.Frame(), Projectile.GetAlpha(lightColor), rotation, texture.Size() / 2f, Projectile.scale, effects, 0);
            return false;
        }
    }
}
