using CalamityMod.Particles;
using Cascade.Content.Particles;

namespace Cascade.Content.Items.Dedicated.Marv
{
    public class BoltStrike : ModProjectile, ILocalizedModType
    {
        public Player Owner => Main.player[Projectile.owner];

        public ref float Timer => ref Projectile.ai[0];

        public ref float ScaleControl => ref Projectile.ai[1];

        public ref float AIPhase => ref Projectile.localAI[0];

        public ref float ColorTimer => ref Projectile.localAI[1];

        public AresCannonChargeParticleSet ChargingParticles = new AresCannonChargeParticleSet(-1, 15, 100f, Color.LightSkyBlue);

        public new string LocalizationCategory => "Projectiles.Magic";

        public override string Texture => "Cascade/Content/DedicatedContent/Marv/ElectricSkyBoltExplosion";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bolt Strike");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.scale = 0.1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
            Projectile.Opacity = 0f;
        }

        public override void AI()
        {
            bool isChanneling = (Owner.channel || Owner.Calamity().mouseRight) && Owner.active && Owner.HeldItem.type == ModContent.ItemType<ThunderousFury>();
            if (!isChanneling && ScaleControl < 0.25f)
            {
                // Disentegrate into a worthless little puff of dust if under a certain scale.
                Projectile.Kill();
                return;
            }

            const int MaxChargeTime = 480;
            if (AIPhase == 0f)
            {
                if (isChanneling)
                {
                    Projectile.Center = Owner.MountedCenter + Owner.MountedCenter.AngleTo(Owner.Calamity().mouseWorld).ToRotationVector2() * 130f;
                    if (Timer < MaxChargeTime + 1)
                    {
                        // Slowly increase scale and opacity over time.
                        // Color is also adjusted accordingly in the Drawcode below.
                        HandleParticleDrawers(ChargingParticles, Timer, MaxChargeTime, true);
                        ScaleControl = 2f * Utils.GetLerpValue(0.1f, 1f, Timer / MaxChargeTime, true);
                        Projectile.Opacity = Lerp(0f, 1f, Timer / 60);
                        Timer++;
                        ColorTimer++;
                    }
                    else
                    {
                        // Stop particles from spawning.
                        ChargingParticles.ParticleSpawnRate = int.MaxValue;
                    }

                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile projectile = Main.projectile[i];
                        if (projectile.active && projectile.type == ModContent.ProjectileType<ThunderousFuryHoldout>() && projectile.timeLeft <= 2)
                        {
                            // Fire and switch the next AI Phase if the weapon is channeled for too long.
                            AIPhase = 1f;
                            Timer = 0f;
                            Projectile.velocity = Projectile.DirectionTo(Owner.Calamity().mouseWorld) * 35f;
                            SoundEngine.PlaySound(CommonCalamitySounds.PlasmaBlastSound, Projectile.Center);
                            //Main.LocalPlayer.Calamity().GeneralScreenShakePower = 7f * Projectile.scale;
                            ScreenShakeSystem.StartShake(7f * Projectile.scale, shakeStrengthDissipationIncrement: 0.185f);

                            Color shockwaveColor = Color.Lerp(Color.Yellow, Color.Cyan, CascadeUtilities.SineEaseInOut(ColorTimer / 480));
                            int lifespan = (int)Lerp(10, 45, Timer / 480);
                            new RoaringShockwaveParticle(lifespan, Projectile.Center, Vector2.Zero, shockwaveColor, 0.1f, Main.rand.NextFloat(TwoPi)).Spawn();
                            Projectile.netUpdate = true;
                        }
                    }
                }
                else
                {
                    // Fire off towards the cursor and play a sound with some visuals.
                    AIPhase = 1f;
                    Timer = 0f;
                    Projectile.velocity = Projectile.DirectionTo(Owner.Calamity().mouseWorld) * 35f;
                    SoundEngine.PlaySound(CommonCalamitySounds.PlasmaBlastSound, Projectile.Center);
                    //Main.LocalPlayer.Calamity().GeneralScreenShakePower = 7f * Projectile.scale;
                    ScreenShakeSystem.StartShake(7f * Projectile.scale, shakeStrengthDissipationIncrement: 0.185f);

                    Color shockwaveColor = Color.Lerp(Color.Yellow, Color.Cyan, CascadeUtilities.SineEaseInOut(ColorTimer / 480));
                    new RoaringShockwaveParticle(45, Projectile.Center, Vector2.Zero, shockwaveColor, 0.1f, Main.rand.NextFloat(TwoPi)).Spawn();
                    Projectile.netUpdate = true;
                }
            }

            if (AIPhase == 1f)
            {
                // Speed up a bit.
                if (Projectile.velocity.Length() < 45f)
                {
                    Projectile.velocity *= 1.02f;
                }

                // Die if it comes in contact with nothing.
                if (Timer >= 240f)
                {
                    Projectile.Kill();
                    return;
                }
                Timer++;
            }

            if (AIPhase == 2f)
            {
                // BIG BOOM
                ScaleControl += 0.07f;
                Projectile.Opacity = Lerp(1f, 0f, Timer / 180);

                if (Timer >= 180f)
                {
                    Projectile.Kill();
                }

                // Particles.
                Color particleColor = Color.Lerp(Color.Yellow, Color.Cyan, CascadeUtilities.SineEaseInOut(ColorTimer / 480));
                if (Timer % 10 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int lifespan = (int)Lerp(45, 100, Timer / 480);
                    new RoaringShockwaveParticle(lifespan, Projectile.Center, Vector2.Zero, particleColor, 0.1f, Main.rand.NextFloat(TwoPi)).Spawn();
                }

                if (Timer % 3 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int sparkLifespan = Main.rand.Next(20, 36);
                    float sparkScale = Main.rand.NextFloat(0.75f, 1.25f);
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 sparkVelocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(6f, 55f);
                        GeneralParticleHandler.SpawnParticle(new SparkParticle(Projectile.Center, sparkVelocity, false, sparkLifespan, sparkScale, particleColor));
                    }
                }

                Timer++;
                Projectile.velocity *= 0.3f;
                //Main.LocalPlayer.Calamity().GeneralScreenShakePower = 5f;
                ScreenShakeSystem.StartShake(5f, shakeStrengthDissipationIncrement: 0.185f);
            }

            Projectile.scale = ScaleControl;
            // Resize the hitbox based on scale.
            int oldWidth = Projectile.width;
            int idealWidth = (int)(Projectile.scale * 100f);
            int idealHeight = (int)(Projectile.scale * 100f);
            if (idealWidth != oldWidth)
            {
                Projectile.position.X += Projectile.width / 2;
                Projectile.position.Y += Projectile.height / 2;
                Projectile.width = idealWidth;
                Projectile.height = idealHeight;
                Projectile.position.X -= Projectile.width / 2;
                Projectile.position.Y -= Projectile.height / 2;
            }

            // Passive dust visuals.
            for (int i = 0; i < 3; i++)
            {
                Vector2 speed = Utils.RandomVector2(Main.rand, -1f, 1f);
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.TintableDustLighted, speed * Main.rand.NextFloat(2f, 6f));
                d.noGravity = true;
                d.color = Color.Lerp(Color.Yellow, Color.Cyan, CascadeUtilities.SineEaseInOut(ColorTimer / 480));
            }

            Projectile.rotation += Pi / 30f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            bool correctPlayerName = Owner.name == "Marv" || Owner.name == "EmolgaLover";
            if (AIPhase == 1f)
            {
                // Super Effective! VS. BIG SHOT
                SoundStyle boom = correctPlayerName ? CascadeSoundRegistry.SuperEffective : new SoundStyle("CalamityMod/Sounds/Item/TeslaCannonFire");
                SoundEngine.PlaySound(boom, Projectile.Center);
                if (correctPlayerName)
                {
                    Rectangle rectangle = new Rectangle((int)target.Center.X, (int)target.Center.Y, target.width, target.height);
                    CombatText.NewText(rectangle, Color.Yellow, "Super Effective!", true);
                }

                AIPhase = 2f;
                Timer = 0f;
                Projectile.netUpdate = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (ScaleControl < 0.25f)
            {
                for (int i = 0; i < 30; i++)
                {
                    Vector2 speed = Utils.RandomVector2(Main.rand, -1f, 1f);
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.YellowStarDust, speed * 5f);
                    d.noGravity = true;
                }
            }
        }

        public void HandleParticleDrawers(AresCannonChargeParticleSet chargingParticles, float timer, float chargeDelay, bool charging = false)
        {
            chargingParticles.ParticleSpawnRate = int.MaxValue;
            if (charging)
            {
                float chargeCompletion = Clamp(timer / chargeDelay, 0f, 1f);
                float spawnRateCompletion = Lerp(5f, 1f, timer / chargeDelay);
                chargingParticles.ParticleSpawnRate = (int)spawnRateCompletion;
                chargingParticles.SpawnAreaCompactness = 200f;
                chargingParticles.chargeProgress = chargeCompletion;
                chargingParticles.ParticleColor = Color.Lerp(Color.Yellow, Color.Cyan, CascadeUtilities.SineEaseInOut(ColorTimer / 480));

                if (timer % 15 == 0f)
                    chargingParticles.AddPulse(chargeCompletion * 12f);
            }
        }

        public float SetTrailWidth(float completionRatio)
        {
            return 40f * Utils.GetLerpValue(0.75f, 0f, completionRatio, true) * Projectile.scale * Projectile.Opacity;
        }

        public Color SetTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.Yellow, Color.Cyan, CascadeUtilities.SineEaseInOut(ColorTimer / 480)) * Projectile.Opacity;
        }

        public void DrawPrims()
        {
            /*TrailDrawer ??= new PrimitiveDrawer(SetTrailWidth, SetTrailColor, true, GameShaders.Misc["CalamityMod:ArtemisLaser"]);

            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:ArtemisLaser"].UseImage1("Images/Extra_189");
            GameShaders.Misc["CalamityMod:ArtemisLaser"].UseImage2("Images/Misc/Perlin");
            TrailDrawer.DrawPrimitives(Projectile.oldPos.ToList(), Projectile.Size * 0.5f - Main.screenPosition, 85);
            Main.spriteBatch.ExitShaderRegion();*/

            Vector2 positionToCenterOffset = Projectile.Size * 0.5f;
            ManagedShader shader = ShaderManager.GetShader("Luminance.StandardPrimitiveShader");
            PrimitiveSettings primSettings = new(SetTrailWidth, SetTrailColor, _ => positionToCenterOffset, Shader: shader);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos.ToList(), primSettings, 85);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D Vortex = CascadeTextureRegistry.GreyscaleVortex.Value;

            SpriteEffects effects = Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = Projectile.rotation;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            int individualFrame = texture.Height / Main.projFrames[Type];
            int currentYFrame = individualFrame * Projectile.frame;
            Rectangle rec = new Rectangle(0, currentYFrame, texture.Width, individualFrame);

            Color color = Color.Lerp(Color.Yellow, Color.Cyan, CascadeUtilities.SineEaseInOut(ColorTimer / 480)) * Projectile.Opacity;

            // Vortex 1.
            Main.EntitySpriteDraw(Vortex, drawPosition, Vortex.Frame(), Projectile.GetAlpha(color), rotation, Vortex.Size() / 2f, Projectile.scale * 3f, SpriteEffects.None, 0);
            //Vortex 2.
            Main.EntitySpriteDraw(Vortex, drawPosition, Vortex.Frame(), Projectile.GetAlpha(color), -rotation, Vortex.Size() / 2f, Projectile.scale * 4f, SpriteEffects.None, 0);

            // Draw pulsing backglow effects.
            for (int i = 0; i < 4; i++)
            {
                float backglowRadius = Lerp(2f, 5f, CascadeUtilities.SineEaseInOut((float)(Main.timeForVisualEffects / 30f)));
                Vector2 backglowDrawPositon = drawPosition + Vector2.UnitY.RotatedBy(i * TwoPi / 4) * backglowRadius;

                Main.spriteBatch.UseBlendState(BlendState.Additive);
                Main.EntitySpriteDraw(texture, backglowDrawPositon, rec, Projectile.GetAlpha(color), rotation, rec.Size() / 2f, Projectile.scale, effects, 0);
                Main.spriteBatch.ResetToDefault();
            }

            // Draw the prim trail.
            DrawPrims();

            // Draw the main sprite.
            Main.EntitySpriteDraw(texture, drawPosition, rec, Projectile.GetAlpha(color), rotation, rec.Size() / 2f, Projectile.scale, effects, 0);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            // Draw the particle set.
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            ChargingParticles.DrawBloom(Projectile.Center);
            ChargingParticles.DrawPulses(Projectile.Center);
            ChargingParticles.DrawSet(Projectile.Center);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
