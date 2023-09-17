using CalamityMod.Items.Placeables.Walls;
using Cascade.Content.Buffs.Minions;
using System.Collections.Generic;

namespace Cascade.Content.DedicatedContent.MPG
{
    public class UnderworldLantern : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];

        private ref float Timer => ref Projectile.ai[0];

        private const int IdleAngleIndex = 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 13;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults() 
        {
            Projectile.width = 24;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.timeLeft = 9999;
            Projectile.minion = true;
            Projectile.minionSlots = 1;
        }

        public override bool? CanCutTiles() => false;

        public override bool MinionContactDamage() => false;

        public override void AI()
        {
            if (!CheckActive(Owner))
                return;

            ref float idleAngle = ref Projectile.Cascade().ExtraAI[IdleAngleIndex];

            // Idle movement.
            Vector2 idlePosition = Owner.Top;
            List<Projectile> brotherMinions = new List<Projectile>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile brotherProj = Main.projectile[i];
                if (brotherProj.active && brotherProj.type == Projectile.type && brotherProj.owner == Projectile.owner)
                {
                    brotherMinions.Add(brotherProj);
                }
            }
            brotherMinions.Sort((x, y) => x.minionPos - y.minionPos);
            int minionCount = brotherMinions.Count;
            if (minionCount > 0)
            {
                int order = brotherMinions.IndexOf(Projectile);
                idleAngle = (TwoPi * order) / minionCount;
                idleAngle += TwoPi * Main.GlobalTimeWrappedHourly / 5f;
                idlePosition.X += 140f * Cos(idleAngle);
                idlePosition.Y += -125f - 50f * Sin(idleAngle) + Owner.gfxOffY;
            }

            Projectile.Center = Vector2.Lerp(Projectile.Center, idlePosition, 0.225f);

            // Search for any nearby targets.
            Projectile.SearchForViableTargetsForMinion(Owner, 2500f, 300f, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            if (foundTarget)
            {
                Vector2 spiritSpawnPosition = targetCenter + Main.rand.NextVector2CircularEdge(200f, 200f);
                Vector2 initialSpiritVelocity = Vector2.UnitY * -6f;
                if (Timer % 150 == 0)
                    Projectile.SpawnProjectile(spiritSpawnPosition, initialSpiritVelocity, ModContent.ProjectileType<UndeadSpirit>(), Projectile.damage, 0f, owner: Projectile.owner);
            }

            Timer++;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(2f, 8f);
                Color color = Color.Lerp(Color.Cyan, Color.CornflowerBlue, Main.rand.NextFloat());
                float scale = Main.rand.NextFloat(0.25f, 1.25f);
                HeavySmokeParticle heavySmoke = new(Projectile.Center, velocity, color, Main.rand.Next(75, 140), scale, Main.rand.NextFloat(0.35f, 1f), 0.06f, true, 0);
                Utilities.SpawnParticleBetter(heavySmoke);
            }
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
                Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
                Color trailColor = Color.Cyan * 0.75f * ((float)(Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                Main.EntitySpriteDraw(baseTexture, drawPosition, projRec, Projectile.GetAlpha(trailColor), Projectile.rotation, projRec.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);

            return false;
        }

        private bool CheckActive(Player owner)
        {
            if (owner.HasBuff(ModContent.BuffType<MoonSpiritLanternBuff>()))
            {
                Projectile.timeLeft = 2;
                return true;
            }
            else
            {
                Projectile.Kill();
                return false;
            }
        }
    }
}
