namespace Cascade.Content.DedicatedContent.MPG
{
    public class MassiveUnderworldLantern : ModProjectile, ILocalizedModType
    {
        private Player Owner => Main.player[Projectile.owner];

        private ref float Timer => ref Projectile.ai[0];

        private ref float AIState => ref Projectile.ai[1];

        private ref float HitCounter => ref Projectile.ai[2];   

        private const int TimeBeforeCharging = 30;

        private const int MaxChargingTime = 300;

        private const int FadeoutTime = 60;

        public new string LocalizationCategory => "Projectiles.Summon";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 13;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 56;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.Opacity = 0f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.scale = 3f;
        }

        public override void AI()
        {

            NPC closestTarget = Projectile.FindClosestNPCToProjectile(3500f);
            if (closestTarget == null)
            {
                // Just rapidly speed up in the direction its facing if there are no enemies.
                // Fade out after the usual charging time.
                Projectile.velocity *= 1.023f;
                Projectile.rotation = Projectile.velocity.ToRotation();

                if (Timer >= MaxChargingTime)
                {
                    Projectile.Opacity = Lerp(Projectile.Opacity, 0f, 0.03f);
                    Projectile.damage = 0;
                    Projectile.velocity *= 0.9f;
                    if (Timer >= FadeoutTime)
                        Projectile.Kill();
                }

                Timer++;
                return;
            }

            if (AIState == 0f)
            {
                Projectile.Opacity = Lerp(Projectile.Opacity, 1f, SineInOutEasing(Timer / TimeBeforeCharging, 0));
                Projectile.rotation = Projectile.AngleTo(closestTarget.Center);
                Projectile.velocity *= 0.9f;

                // Handle destroying a lantern and applying the debuff.
                // If there are no nearby targets then everything is left the same.
                if (Timer == 1)
                {
                    
                }

                if (Timer == TimeBeforeCharging)
                {
                    Projectile.velocity = Projectile.SafeDirectionTo(closestTarget.Center) * 75f;
                    AIState = 1f;
                    Timer = 0f;
                    Projectile.netUpdate = true;
                }
            }

            if (AIState == 1f)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();

                if (Timer >= MaxChargingTime || HitCounter >= 1f)
                {
                    Main.LocalPlayer.Calamity().GeneralScreenShakePower = 8f;
                    AIState = 2f;
                    Timer = 0f;

                    // Some particles to mimic an explosion like effect.
                    for (int i = 0; i < 35; i++)
                    {
                        Vector2 velocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(15f, 25f);
                        Color color = Color.Lerp(Color.Cyan, Color.CornflowerBlue, Main.rand.NextFloat());
                        float scale = Main.rand.NextFloat(3f, 6f);
                        HeavySmokeParticle heavySmoke = new(Projectile.Center, velocity, color, Main.rand.Next(75, 140), scale, Main.rand.NextFloat(0.35f, 1f), 0.06f, true, 0);
                        GeneralParticleHandler.SpawnParticle(heavySmoke);
                    }

                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f) * 25f;
                        Color normalColor = Color.Lerp(Color.Cyan, Color.CornflowerBlue, Main.rand.NextFloat());
                        Color bloomColor = Color.Lerp(Color.PowderBlue, Color.LightSkyBlue, Main.rand.NextFloat());
                        float scale = Main.rand.NextFloat(0.45f, 4f);
                        int lifespan = Main.rand.Next(15, 45);
                        Particle sparkle = new GenericSparkle(Projectile.Center, velocity, normalColor, bloomColor, scale, lifespan, 0.25f, bloomScale: scale);
                        GeneralParticleHandler.SpawnParticle(sparkle);
                    }
                }
            }

            if (AIState == 2f)
            {
                Projectile.Opacity = Lerp(Projectile.Opacity, 0f, 0.03f);
                Projectile.damage = 0;
                Projectile.velocity *= 0.9f;
                if (Timer >= FadeoutTime)
                {
                    Projectile.Kill();
                    return;
                }
            }

            Timer++;
            Projectile.AdjustProjectileHitboxByScale(48f, 56f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HitCounter++;
            // Release some particles in the direction of the hit.
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = Projectile.DirectionTo(target.Center).RotatedByRandom(ToRadians(75f)) * Main.rand.NextFloat(10f, 25f);
                Color color = Color.Lerp(Color.Cyan, Color.CornflowerBlue, Main.rand.NextFloat());
                float scale = Main.rand.NextFloat(0.45f, 2f);
                int lifespan = Main.rand.Next(45, 90);
                Particle sparkle = new SquishyLightParticle(Projectile.Center, velocity, scale, color, lifespan);
                GeneralParticleHandler.SpawnParticle(sparkle);
            }
        }

        public override void OnKill(int timeLeft)
        {
           
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D baseTexture = TextureAssets.Projectile[Type].Value;

            int frameHeight = baseTexture.Height / Main.projFrames[Type];
            int frameY = frameHeight * Projectile.frame;
            Rectangle projRec = new Rectangle(0, frameY, baseTexture.Width, frameHeight);

            Main.spriteBatch.SetBlendState(BlendState.Additive);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                SpriteEffects effects = Projectile.direction < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
                Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
                Color trailColor = Color.Cyan * 0.75f * ((float)(Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                Main.EntitySpriteDraw(baseTexture, drawPosition, projRec, Projectile.GetAlpha(trailColor), Projectile.rotation, projRec.Size() / 2f, Projectile.scale, effects, 0);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);

            return false;
        }
    }
}
