using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Projectiles.Rogue;
using Cascade.Content.Items.Weapons.Ranged;
using Cascade.Core.Systems.CameraSystem;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Cascade.Content.Projectiles.Ranged
{
    public class TriploonHarpoon : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];

        private Projectile Triploon => Main.projectile[(int)Projectile.ai[0]];

        private ref float Timer => ref Projectile.localAI[0];

        private ref float AIState => ref Projectile.localAI[1];

        private ref float PiercedEnemyIndex => ref Projectile.ai[1];

        private ref float HitCounter => ref Projectile.ai[2];

        private const int MaxTravelTime = 180;

        private const int MaxHits = 50;

        private SoundStyle[] StabSounds = new SoundStyle[]
        {
            CascadeSoundRegistry.FleshySwordStab,
            CascadeSoundRegistry.FleshySwordStab2,
            CascadeSoundRegistry.FleshySwordStab3
        };

        private SoundStyle[] RipSounds = new SoundStyle[]
        {
            CascadeSoundRegistry.FleshySwordRip,
            CascadeSoundRegistry.FleshySwordRip2
        };

        private bool IsChanneling => Owner.active && Owner.channel && Owner.HeldItem.type == ModContent.ItemType<Triploon>();

        private bool ShouldDespawn => Owner.dead || Owner.CCed || !Owner.active || Owner.HeldItem.type != ModContent.ItemType<Triploon>();

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 100000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.Opacity = 0f;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float lineLength = 46f * Projectile.scale;
            Vector2 startPoint = Projectile.Center + Projectile.rotation.ToRotationVector2();
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), startPoint, startPoint + Projectile.rotation.ToRotationVector2() * lineLength, 12, ref collisionPoint);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            for (int i = 0; i < Projectile.localAI.Length; i++)
                writer.Write(Projectile.localAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            for (int i = 0; i < Projectile.localAI.Length; i++)
                Projectile.localAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (ShouldDespawn || !Triploon.active)
            {
                Projectile.Kill();
                return;
            }

            // Initially launched.
            if (AIState == 0f)
            {
                if (Timer >= MaxTravelTime)
                {
                    Projectile.velocity.Y += 0.23f;
                    Projectile.velocity.X *= 0.98f;
                }

                if (!IsChanneling)
                {
                    AIState = 1f;
                    Timer = 0f;
                    Projectile.netUpdate = true;
                }
                

                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            }

            // Reelback called, go back to the player and die.
            if (AIState == 1f)
            {
                Projectile.tileCollide = false;
                Projectile.damage = 0;
                Projectile.velocity = Projectile.SafeDirectionTo(Owner.Center) * 45f;
                if (Projectile.Hitbox.Intersects(Owner.Hitbox))
                {
                    Projectile.Kill();
                    return;
                }

                Projectile.rotation = Projectile.velocity.ToRotation() - 1.57f;
            }

            // Hit an enemy, stay stuck in them.
            if (AIState == 2f)
            {
                NPC piercedEnemy = Main.npc[(int)PiercedEnemyIndex];
                if (piercedEnemy.active && !piercedEnemy.dontTakeDamage)
                {
                    Projectile.Center = piercedEnemy.Center - Projectile.velocity * 2f;
                    Projectile.gfxOffY = piercedEnemy.gfxOffY;
                    Projectile.rotation = Projectile.AngleTo(piercedEnemy.Center);
                    piercedEnemy.HitEffect();
                }

                if (HitCounter >= MaxHits)
                {
                    Projectile.Kill();
                    return;
                }

                if (!IsChanneling || piercedEnemy.dontTakeDamage || !piercedEnemy.active)
                {
                    AIState = 1f;
                    Timer = 0f;
                    CascadeCameraSystem.Screenshake(12, 10, Projectile.Center);
                    if (piercedEnemy.Organic())
                        SoundEngine.PlaySound(Utilities.GetRandomSoundFromList(RipSounds.ToList()) with { MaxInstances = 1 }, Projectile.Center); Projectile.netUpdate = true;
                }

                Projectile.tileCollide = false;
            }

            Timer++;
            Projectile.Opacity = Clamp(Projectile.Opacity + 0.2f, 0f, 1f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Pierce the enemy and remain stuck in them.
            if (AIState != 2f)
            {
                AIState = 2f;
                Timer = 0f;
                PiercedEnemyIndex = target.whoAmI;
                Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
                CascadeCameraSystem.Screenshake(8, 15, Projectile.Center);
                SoundEngine.PlaySound(Utilities.GetRandomSoundFromList(StabSounds.ToList()) with { MaxInstances = 0 }, Projectile.Center);
                Projectile.netUpdate = true;
            }

            // Increase the hit counter.
            // Double it if it lands a critical hit.
            HitCounter++;
            if (hit.Crit)
                HitCounter += 2f;
        }

        public override void Kill(int timeLeft)
        {
            // Kill the Triploon Holdout here as well since it seems to not want to die whenever the harpoons retract.
            List<Projectile> activeHarpoons = Main.projectile.Take(Main.maxProjectiles).Where(p => p.active && p.type == ModContent.ProjectileType<TriploonHarpoon>()).ToList();
            if (activeHarpoons.Count <= 1)
            {
                Triploon.Kill();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            AIState = 1f;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawChains(lightColor);
            DrawHarpoon(lightColor);
            return false;
        }

        public void DrawChains(Color lightColor)
        {
            Texture2D chainTexture = TextureAssets.Chain22.Value;

            Vector2 mountedCenter = Triploon.Center;
            Vector2 projCenter = Projectile.Center;
            Vector2 origin = new(chainTexture.Width * 0.5f, chainTexture.Height * 0.5f);
            Vector2 distanceFromOwner = mountedCenter - projCenter;

            float chainHeight = chainTexture.Height;
            float rotation = Atan2(distanceFromOwner.Y, distanceFromOwner.X) - 1.57f;

            bool canDrawChains = true;
            if (float.IsNaN(projCenter.X) && float.IsNaN(projCenter.Y))
                canDrawChains = false;
            if (float.IsNaN(distanceFromOwner.X) && float.IsNaN(distanceFromOwner.Y))
                canDrawChains = false;

            while (canDrawChains)
            {
                if (distanceFromOwner.Length() < chainHeight + 1f)
                {
                    canDrawChains = false;
                    continue;
                }

                distanceFromOwner.Normalize();
                projCenter += distanceFromOwner * chainHeight;
                distanceFromOwner = mountedCenter - projCenter;
                Main.spriteBatch.Draw(chainTexture, projCenter - Main.screenPosition, null, lightColor, rotation, origin, 1f, SpriteEffects.None, 0f);
            }
        }

        public void DrawHarpoon(Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            // Draw the main sprite.
            Main.EntitySpriteDraw(texture, drawPosition, texture.Frame(), Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
        }

        public void ParticleEffects(NPC npc, int type = 0)
        {
            // First type, initially stabbed.
            if (type == 0)
            {
                
            }

            // Second type, tear out.
            if (type == 1)
            {

            }
        }
    }
}
