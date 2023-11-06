using Cascade.Content.Buffs.Debuffs;
using Cascade.Content.Buffs.Pets;
using Cascade.Content.Particles;
using Cascade.Core.Systems.CameraSystem;

namespace Cascade.Content.DedicatedContent.Lynel
{
    public class EarPiercingBellbird : ModProjectile, ILocalizedModType
    {
        private Player Owner => Main.player[Projectile.owner];

        private ref float Timer => ref Projectile.ai[0];

        private ref float AIState => ref Projectile.ai[1];

        private const int ScreamChanceRegular = 1000000000;

        private const int ScreamChanceGFB = 100000;

        private const int ScreamChargeTime = 120;

        private const int ScreamTime = 360;

        private const int ScreamChargeVisualScaleIndex = 0;

        private const int ScreamChargeVisualOpacityIndex = 1;

        public new string LocalizationCategory => "Projectiles.Pets";

        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            Main.projFrames[Type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            ref float screamChargeVisualScale = ref Projectile.Cascade().ExtraAI[ScreamChargeVisualScaleIndex];
            ref float screamChargeVisualOpacity = ref Projectile.Cascade().ExtraAI[ScreamChargeVisualOpacityIndex];

            if (Owner.active && Owner.HasBuff(ModContent.BuffType<BellbirdBuff>()))
                Projectile.timeLeft = 2;

            // Teleport if too far away.
            float distanceFromOwner = Vector2.Distance(Owner.Center, Projectile.Center);
            if (distanceFromOwner > 2000f)
            {
                Projectile.Center = Owner.Center;
                Projectile.netUpdate = true;
            }

            if (AIState == 0f)
            {
                GameTime gameTime = Main.gameTimeCache;
                int screamChance = Main.zenithWorld ? ScreamChanceGFB : ScreamChanceRegular;
                
                // Float around the player.
                Projectile.FloatingPetAI(true, 0.03f);
                Projectile.UpdateProjectileAnimationFrames(0, 4, 4);

                // Try to run the bellbird scream at the respective random chance every 12 seconds.
                if (gameTime.TotalGameTime.Ticks % 720 == 0f && Main.rand.NextBool(2))
                {
                    AIState = 1f;
                    Timer = 0f;
                    Projectile.netUpdate = true;
                }
            }

            if (AIState == 1f)
            {
                // Slow down and play a small visual effect.
                if (Timer <= ScreamChargeTime)
                {
                    Projectile.velocity *= 0.9f;
                    Projectile.UpdateProjectileAnimationFrames(3, 3, 1);

                    float screamChargeInterpolant = Utils.GetLerpValue(0f, ScreamChargeTime - 39, Timer, true);

                    // Fade in then quickly fade out.
                    if (Timer <= ScreamChargeTime - 30)
                    {
                        screamChargeVisualScale = Lerp(5f, 1f, screamChargeInterpolant);
                        screamChargeVisualOpacity = Lerp(0f, 1f, screamChargeInterpolant);
                    }
                    
                    if (Timer >= ScreamChargeTime - 15 && Timer <= ScreamChargeTime)
                        screamChargeVisualOpacity = Clamp(screamChargeVisualOpacity - 0.1f, 0f, 1f);
                }

                // Cry of God.
                if (Timer is >= ScreamChargeTime and <= ScreamChargeTime + ScreamTime)
                {
                    // Make the player's ears bleed.
                    if (Timer is ScreamChargeTime)
                        SoundEngine.PlaySound(CascadeSoundRegistry.BellbirdStunningScream with { Volume = 30f }, Projectile.Center);

                    // Visual effects.
                    CascadeCameraSystem.Screenshake(8, 30, Projectile.Center);
                    Projectile.UpdateProjectileAnimationFrames(0, 0, 1);
                    if (Timer % 10 == 0)
                    {
                        RoaringShockwaveParticle shockwave = new(45, Projectile.Center, Vector2.Zero, Color.White, 0.1f, Main.rand.NextFloat(TwoPi));
                        GeneralParticleHandler.SpawnParticle(shockwave);
                    }

                    // Stun any nearby NPCs or Players.
                    StunPlayersAndNPCs();
                }

                if (Timer is >= ScreamChargeTime + ScreamTime)
                {
                    AIState = 0f;
                    Timer = 0f;
                    Projectile.netUpdate = true;
                }

                Timer++;
                Projectile.rotation = Projectile.velocity.X * 0.03f;
            }
        }

        public void StunPlayersAndNPCs()
        {
            float maxDistance = 2500f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.Distance(Projectile.Center) < maxDistance)
                    npc.AddBuff(ModContent.BuffType<BellbirdCry>(), 180);
            }

            for (int i = 0; i < Main.CurrentFrameFlags.ActivePlayersCount; i++)
            {
                Player player = Main.player[i];
                if (player.active && player.Distance(Projectile.Center) < maxDistance)
                    player.AddBuff(ModContent.BuffType<BellbirdCry>(), 180);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D chargeVisualTexture = ModContent.Request<Texture2D>(GlowTexture).Value;

            ref float screamChargeVisualScale = ref Projectile.Cascade().ExtraAI[ScreamChargeVisualScaleIndex];
            ref float screamChargeVisualOpacity = ref Projectile.Cascade().ExtraAI[ScreamChargeVisualOpacityIndex];

            int individualFrameHeight = texture.Height / Main.projFrames[Type];
            int currentYFrame = individualFrameHeight * Projectile.frame;
            Rectangle rectangle = new(0, currentYFrame, texture.Width, individualFrameHeight);
            Vector2 origin = rectangle.Size() / 2f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);

            Main.EntitySpriteDraw(texture, drawPosition, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, Projectile.DirectionBasedSpriteEffects());
            Main.EntitySpriteDraw(chargeVisualTexture, drawPosition, rectangle, Color.White * screamChargeVisualOpacity, Projectile.rotation, origin, screamChargeVisualScale, Projectile.DirectionBasedSpriteEffects());
            return false;
        }
    }
}
