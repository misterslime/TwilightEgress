using Cascade.Content.Particles;

namespace Cascade.Content.DedicatedContent.Marv
{
    public class ThunderousFuryHoldout : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];

        private ref float ChargeTimer => ref Projectile.ai[0];

        private ref float DelayTimer => ref Projectile.ai[1];

        private const int DelayBeforeFiring = 75;

        private const int FireRate = 60;

        public override string Texture => "Cascade/Content/DedicatedContent/Marv/ThunderousFury";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Thunderous Fury");
        }

        public override void SetDefaults()
        {
            Projectile.width = 124;
            Projectile.height = 52;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 1200;
        }

        public override void AI()
        {
            bool isChanneling = (Owner.channel || Owner.Calamity().mouseRight) && Owner.active && Owner.HeldItem.type == ModContent.ItemType<ThunderousFury>();
            if (!isChanneling)
            {
                Projectile.Kill();
                return;
            }

            UpdateProjectileSpecificVariables(Owner);
            UpdatePlayerSpecificVariables(Owner);
        }

        public void UpdateProjectileSpecificVariables(Player owner)
        {
            Projectile.Center = owner.RotatedRelativePoint(owner.MountedCenter, true);
            Projectile.rotation = owner.MountedCenter.AngleTo(owner.Calamity().mouseWorld);
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += Pi;
            }

            bool correctPlayerName = owner.name == "Marv" || owner.name == "ThatOneEmolgaLiker";
            // Play a special sound while charging. Also spawn some particles.
            if (DelayTimer == 1)
            {
                if (Owner.Calamity().mouseRight)
                {
                    SoundStyle boltStrikeStartSound = correctPlayerName ? CascadeSoundRegistry.ZekromCry : CommonCalamitySounds.ExoPlasmaShootSound;
                    SoundEngine.PlaySound(boltStrikeStartSound, Projectile.Center);
                }
                else
                {
                    SoundStyle thunderboltStartSound = correctPlayerName ? CascadeSoundRegistry.PikachuCry : CommonCalamitySounds.LightningSound;
                    SoundEngine.PlaySound(thunderboltStartSound, Projectile.Center);
                }

                Color particleColor = Owner.Calamity().mouseRight ? Color.Lerp(Color.Cyan, Color.SkyBlue, Main.rand.NextFloat()) : Color.Lerp(Color.Yellow, Color.Goldenrod, Main.rand.NextFloat());
                int sparkLifespan = Main.rand.Next(20, 36);
                float sparkScale = Main.rand.NextFloat(0.75f, 1.25f);
                for (int i = 0; i < 50; i++)
                {
                    Vector2 sparkVelocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(9f, 16f);
                    Utilities.SpawnParticleBetter(new SparkParticle(Projectile.Center, sparkVelocity, false, sparkLifespan, sparkScale, particleColor));
                }
                Utilities.SpawnParticleBetter(new RoaringShockwaveParticle(60, Projectile.Center, Vector2.Zero, particleColor, 0.1f, Main.rand.NextFloat(TwoPi)));
            }

            if (DelayTimer >= DelayBeforeFiring)
            {
                FireProjectiles(owner);
                ChargeTimer++;
            }
            DelayTimer++;
        }

        public void UpdatePlayerSpecificVariables(Player owner)
        {
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;
            owner.ChangeDir(Math.Sign(Projectile.rotation.ToRotationVector2().X));
            owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - PiOver2);
        }

        public void FireProjectiles(Player owner)
        {
            if (owner.Calamity().mouseRight)
            {
                if (DelayTimer == DelayBeforeFiring + 1 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 spawnPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * 120f;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), spawnPosition, Vector2.Zero, ModContent.ProjectileType<BoltStrike>(), Projectile.damage, Projectile.knockBack, Owner: Projectile.owner);
                }
            }
            else
            {
                if (ChargeTimer % FireRate == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 spawnPosition = Main.MouseWorld + new Vector2(Main.rand.NextFloat(-300f, 300f), -900f);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), spawnPosition, Vector2.Zero, ModContent.ProjectileType<ElectricSkyBolt>(), Projectile.damage, Projectile.knockBack, Owner: Projectile.owner);
                }
            }
        }

        public void UpdateVisuals(Player owner)
        {

        }

        public override bool PreKill(int timeLeft)
        {
            // Ensures that timers aren't gone along for too long.
            if (Owner.channel)
            {
                ResetToStart();
                return false;
            }
            return true;
        }

        public void ResetToStart()
        {
            Projectile.timeLeft = 1200;
            ChargeTimer = 0f;
            DelayTimer = 0f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteEffects effects = Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = Projectile.rotation + (Owner.direction < 0 ? Pi : 0f);
            Vector2 drawPosition = Owner.MountedCenter + new Vector2(10f, -15f) + Projectile.rotation.ToRotationVector2() - Main.screenPosition;

            // Draw pulsing backglow effects.
            for (int i = 0; i < 4; i++)
            {
                float backglowRadius = Lerp(2f, 5f, SineInOutEasing((float)(Main.timeForVisualEffects / 30f), 1));
                Vector2 backglowDrawPositon = drawPosition + Vector2.UnitY.RotatedBy(i * TwoPi / 4) * backglowRadius;

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
