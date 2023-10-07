using Cascade.Content.Buffs.Debuffs;
using Cascade.Content.Buffs.Pets;
using Cascade.Content.Particles;
using Cascade.Core.Systems.CameraSystem;
using Terraria;

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

        public new string LocalizationCategory => "Projectiles.Pets";

        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
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
            if (Owner.active && Owner.HasBuff(ModContent.BuffType<BellbirdBuff>()))
                Projectile.timeLeft = 2;

            if (AIState == 0f)
            {
                GameTime gameTime = Main.gameTimeCache;
                int screamChance = Main.zenithWorld ? ScreamChanceGFB : ScreamChanceRegular;
                // Try to run the bellbird scream at the respective random chance every 3 seconds.
                if (gameTime.TotalGameTime.Ticks % 180 == 0f && Main.rand.NextBool(10))
                {
                    AIState = 1f;
                    Timer = 0f;
                    Projectile.netUpdate = true;
                }
            }

            if (AIState == 1f)
            {
                if (Timer <= ScreamChargeTime)
                {

                }

                // Cry of God.
                if (Timer is >= ScreamChargeTime and <= ScreamChargeTime + ScreamTime)
                {
                    // Make the player's ears bleed.
                    if (Timer is ScreamChargeTime)
                        SoundEngine.PlaySound(CascadeSoundRegistry.BellbirdStunningScream with { Volume = 10f }, Projectile.Center);

                    // Visual effects.
                    CascadeCameraSystem.Screenshake(8, 30, Projectile.Center);
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
            }

            Projectile.Center = Owner.MountedCenter - Vector2.UnitY * 30f + Vector2.UnitY * Owner.gfxOffY;
            Projectile.spriteDirection = -Owner.direction;
        }

        public void StunPlayersAndNPCs()
        {
            float maxDistance = 2500f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.Distance(Projectile.Center) < maxDistance)
                {
                    npc.AddBuff(ModContent.BuffType<BellbirdCry>(), 180);
                }
            }

            for (int i = 0; i < Main.CurrentFrameFlags.ActivePlayersCount; i++)
            {
                Player player = Main.player[i];
                if (player.active && player.Distance(Projectile.Center) < maxDistance)
                {
                    player.AddBuff(ModContent.BuffType<BellbirdCry>(), 180);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Projectile.DrawTextureOnProjectile(lightColor, Projectile.rotation, Projectile.scale, Projectile.DirectionBasedSpriteEffects());
            return true;
        }
    }
}
