using CalamityMod.Projectiles.Typeless;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cascade.Content.Items.Weapons.Ranged.SailorsSingularity
{
    public class SailorVortex : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/NuclearFuryProjectile";
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.scale = 1f;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;

        }
        private int aiCounter
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        private int explosionCounter
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        private const float acceleration = 1f;
        private const int maxSpeed = 8;
        private NPC Target
        {
            get
            {
                NPC target = null;
                Player owner = Main.player[Projectile.owner];
                if (owner.HasMinionAttackTargetNPC)
                    target = CheckNPCTargetValidity(Main.npc[owner.MinionAttackTargetNPC]);

                if (target != null)
                    return target;

                else
                {
                    for (int npcIndex = 0; npcIndex < Main.npc.Length; npcIndex++)
                    {
                        target = CheckNPCTargetValidity(Main.npc[npcIndex]);
                        if (target != null)
                            return target;
                    }
                }

                return null;
            }
        }
        public static float AggroRange = 1000f;
        public NPC CheckNPCTargetValidity(NPC potentialTarget)
        {
            if (potentialTarget.CanBeChasedBy(this, false))
            {
                float targetDist = Vector2.Distance(potentialTarget.Center, Projectile.Center);

                if ((targetDist < AggroRange) && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, potentialTarget.position, potentialTarget.width, potentialTarget.height))
                {
                    return potentialTarget;
                }
            }

            return null;
        }

        public override void AI()
        {
            Projectile.scale = 2f;
            #region Movement
            Player owner = Main.player[Projectile.owner];
            if((owner.Center - Projectile.Center).Length() >= 1200f)
                Projectile.active = false;
            NPC target = Target;
            if (target != null)
            {
                Projectile.velocity += (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 0.5f;
                if (Projectile.velocity.Length() > maxSpeed)
                    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * maxSpeed;
            }
            #endregion
            
            if (IsProjectileTouchingProjectile(Projectile, ModContent.ProjectileType<SailorStar>()))
                explosionCounter++;
            if (IsProjectileTouchingProjectile(Projectile, ModContent.ProjectileType<SailorBlast>()))
                explosionCounter = 6;

            if (explosionCounter >= 6 && Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CosmicDashExplosion>(), 200, 1f, Projectile.owner);
                Projectile.active = false;
            }
            aiCounter++;
        }
        private static bool IsProjectileTouchingProjectile(Projectile myProjectile, int projType)
        {
            foreach (Projectile projectile in Main.projectile.Where(n => n.type == projType && n.active))
            {
                if (myProjectile.Hitbox.Intersects(projectile.Hitbox))
                {
                    projectile.active = false;
                    return true;
                }
            }
            return false;
        }
        /*
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (CalamityUtils.CircularHitboxCollision(Projectile.Center, projHitbox.Width / 2, targetHitbox))
                return true;
            return false;
        }
        */
    }
}
