namespace Cascade.Content.DedicatedContent.MPG
{
    public class UndeadSpirit : ModProjectile, ILocalizedModType
    {
        private Player Owner => Main.player[Projectile.owner];

        private ref float Timer => ref Projectile.ai[0];

        private ref float AIState => ref Projectile.ai[1];

        private ref float HitCounter => ref Projectile.ai[2];

        private const int TimeBeforeCharging = 30;

        private const int MaxChargingTime = 300;

        private const int HitCounterIndex = 0;

        public new string LocalizationCategory => "Projectiles.Summon";

        public override string Texture => "Terraria/Images/NPC_" + NPCID.PirateGhost;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 50;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.Opacity = 0f;
            Projectile.scale = 1.75f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 7;
        }

        public override void AI()
        {
            float maxDetectionRadius = 3500f;
            NPC nearestTarget = Projectile.FindClosestNPCToProjectile(maxDetectionRadius);
            if (nearestTarget == null)
            {
                AIState = 2f;
                Projectile.netUpdate = true;
            }

            if (AIState == 0f)
            {
                if (Timer <= TimeBeforeCharging)
                {
                    Projectile.velocity *= 0.9f;
                    Projectile.Opacity = Lerp(Projectile.Opacity, 1f, SineInOutEasing(Timer / 30f, 0));
                    if (Timer >= TimeBeforeCharging)
                    {
                        Projectile.velocity = Projectile.SafeDirectionTo(nearestTarget.Center) * 40f;
                        Projectile.damage = Projectile.originalDamage;
                        AIState = 1f;
                        Timer = 0f;
                        Projectile.netUpdate = true;
                    }
                }
            }

            if (AIState == 1f)
            {
                if (Timer >= TimeBeforeCharging + MaxChargingTime || HitCounter >= 1f)
                {
                    AIState = 2f;
                    Timer = 0f;
                    Projectile.netUpdate = true;
                }
            }

            if (AIState == 2f)
            {
                // Fade out and die.
                Projectile.Opacity = Lerp(Projectile.Opacity, 0f, SineInOutEasing(Timer / 45f, 0));
                Projectile.velocity *= 0.9f;
                Projectile.damage = 0;
                
                if (Timer >= 45f)
                {
                    Projectile.Kill();
                    return;
                }
            }

            Timer++;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.UpdateProjectileAnimationFrames(0, 4, 5);
            Projectile.AdjustProjectileHitboxByScale(42f, 50f);
            Projectile.rotation = Projectile.velocity.X * 0.03f;
        }

        public override bool? CanDamage() => AIState > 0;

        public override void OnHitNPC(NPC target, NPC.HitInfo info, int damageDone)
        {
            if (AIState > 0)
                HitCounter++;
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
                SpriteEffects effects = Projectile.oldSpriteDirection[i] > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
                Color trailColor = Color.Cyan * 0.75f * ((float)(Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                Main.EntitySpriteDraw(baseTexture, drawPosition, projRec, Projectile.GetAlpha(trailColor), Projectile.oldRot[i], projRec.Size() / 2f, Projectile.scale, effects, 0);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);

            return false;
        }
    }
}
