using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Ranged;

namespace Cascade.Content.Projectiles.Ranged
{
    public class ChickenCannonHoldout : ModProjectile, ILocalizedModType
    {
        private Player Owner => Main.player[Projectile.owner];

        private ref float Timer => ref Projectile.ai[0];

        private bool ShouldDespawn => Owner.dead || Owner.CCed || !Owner.active || Owner.HeldItem.type != ModContent.ItemType<ChickenCannon>();

        private const int ChargeUpTime = 180;

        private const int TimeSpentReloading = 60;

        public const int OldRotationIndex = 0;

        public const int BackglowRotationIndex = 1;

        public new string LocalizationCategory => "Projectiles.Ranged";

        public override string Texture => "CalamityMod/Projectiles/Ranged/ChickenCannonHeld";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 126;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            bool isChanneling = Owner.active && Owner.channel && Owner.HeldItem.type == ModContent.ItemType<ChickenCannon>();
            if (!isChanneling)
            {
                Projectile.Kill();
                return;
            }

            Timer++;
            DoBehavior();
            UpdatePlayerVariables(Owner);
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += Pi;
        }

        public void DoBehavior()
        {
            ref float oldRotation = ref Projectile.Cascade().ExtraAI[OldRotationIndex];

            Projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter, true);

            if (Timer <= ChargeUpTime)
            {
                Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.AngleTo(Main.MouseWorld), 0.2f);
                if (Timer % 60f == 0)
                {
                    float maxScale = Timer == ChargeUpTime ? 0.01f : 1.25f;
                    float newScale = Timer == ChargeUpTime ? 5f : 0.01f;
                    GeneralParticleHandler.SpawnParticle(new DirectionalPulseRing(Projectile.Center + Projectile.rotation.ToRotationVector2() * 60f, Vector2.Zero, Color.Orange, new Vector2(0.5f, 1f), Projectile.rotation, maxScale + Main.rand.NextFloat(0.3f), newScale, 30));

                    // Play a different yharon sound at every interval.
                    SoundStyle sound = CascadeSoundRegistry.YharonHurt;
                    if (Timer >= 180f)
                        sound = CascadeSoundRegistry.YharonRoarShort;
                    SoundEngine.PlaySound(sound, Projectile.Center);
                }

                if (Timer == ChargeUpTime)
                {
                    oldRotation = Projectile.AngleTo(Main.MouseWorld);
                    Vector2 spawnPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * 60f;
                    Vector2 velocity = Projectile.SafeDirectionTo(Main.MouseWorld) * 15f;

                    Projectile.SpawnProjectile(spawnPosition, velocity, ModContent.ProjectileType<ChickenRocket>(), Projectile.damage, Projectile.knockBack, true, CommonCalamitySounds.LargeWeaponFireSound, Projectile.owner);
                }
            }

            // Rotate backwards from recoil and animate the projectile.
            if (Timer >= ChargeUpTime && Timer <= ChargeUpTime + TimeSpentReloading)
            {
                Projectile.rotation = Lerp(Projectile.rotation, oldRotation + ToRadians(-65f) * Owner.direction, 0.3f);
                Projectile.UpdateProjectileAnimationFrames(0, 4, 4);
            }

            // Reset.
            if (Timer >= ChargeUpTime + 30f)
                Projectile.Kill();
        }

        public void UpdatePlayerVariables(Player owner)
        {
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;
            if (Timer <= ChargeUpTime)
                owner.ChangeDir(Math.Sign(Projectile.rotation.ToRotationVector2().X));
            owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - PiOver2);
            owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - PiOver2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ref float backglowRotation = ref Projectile.Cascade().ExtraAI[BackglowRotationIndex];

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteEffects effects = Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = Projectile.rotation + (Owner.direction < 0 ? Pi : 0f);
            Vector2 drawPosition = Owner.MountedCenter + new Vector2(0f, 0f) + Projectile.rotation.ToRotationVector2() - Main.screenPosition;

            int individualFrameHeight = texture.Height / Main.projFrames[Type];
            int currentYFrame = individualFrameHeight * Projectile.frame;
            Rectangle projRec = new Rectangle(0, currentYFrame, texture.Width, individualFrameHeight);

            // Draw pulsing backglow effects.
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            for (int i = 0; i < 4; i++)
            {
                backglowRotation += TwoPi / 300f;
                float backglowRadius = Lerp(2f, 5f, SineInOutEasing((float)(Main.timeForVisualEffects / 30f), 1));
                Vector2 backglowDrawPositon = drawPosition + Vector2.UnitY.RotatedBy(backglowRotation + TwoPi * i / 4) * backglowRadius;

                Main.EntitySpriteDraw(texture, backglowDrawPositon, projRec, Projectile.GetAlpha(Color.Orange), rotation, projRec.Size() / 2f, Projectile.scale, effects, 0);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);

            // Draw the main sprite.
            Main.EntitySpriteDraw(texture, drawPosition, projRec, Projectile.GetAlpha(lightColor), rotation, projRec.Size() / 2f, Projectile.scale, effects, 0);
            return false;
        }
    }
}
