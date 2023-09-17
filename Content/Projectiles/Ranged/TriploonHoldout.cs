using Cascade.Content.Items.Weapons.Ranged;
using System.Collections.Generic;

namespace Cascade.Content.Projectiles.Ranged
{
    public class TriploonHoldout : ModProjectile, ILocalizedModType
    {
        private Player Owner => Main.player[Projectile.owner];

        private ref float Timer => ref Projectile.ai[0];

        private bool ShouldDespawn => Owner.dead || Owner.CCed || !Owner.active || Owner.HeldItem.type != ModContent.ItemType<Triploon>();

        public new string LocalizationCategory => "Projectiles.Ranged";

        public override void SetDefaults()
        {
            Projectile.width = 88;
            Projectile.height = 48;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            if (ShouldDespawn)
            {
                Projectile.Kill();
                return;
            }

            TriploonBehavior();
            UpdatePlayerVariables();

            Timer++;
            Projectile.Center = Owner.MountedCenter;
        }

        public void TriploonBehavior()
        {
            // Get a list of all the active harpoons and kill the holdout when the list becomes empty.
            List<Projectile> activeHarpoons = Main.projectile.Take(Main.maxProjectiles).Where(p => p.active && p.type == ModContent.ProjectileType<TriploonHarpoon>()).ToList();
            if (Timer >= 3f && activeHarpoons.Count <= 0f)
            {
                Projectile.Kill();
                return;
            }

            // Fire off the harpoons.
            if (Timer == 1f)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 harpoonVelocity = Projectile.SafeDirectionTo(Main.MouseWorld).RotatedByRandom(ToRadians(25f)) * 45f;
                    Projectile.SpawnProjectile(Projectile.Center, harpoonVelocity, ModContent.ProjectileType<TriploonHarpoon>(), Projectile.damage, Projectile.knockBack, true, CommonCalamitySounds.LargeWeaponFireSound, Projectile.owner, Projectile.whoAmI);
                }
            }

            if (Timer >= 3f)
            {
                Projectile harpoonToFollow = activeHarpoons.LastOrDefault();
                Projectile.rotation = Owner.AngleTo(harpoonToFollow.Center);
            }
            else
            {
                Projectile.rotation = Owner.AngleTo(Main.MouseWorld);
            }
        }

        public void UpdatePlayerVariables()
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.ChangeDir(Math.Sign(Projectile.rotation.ToRotationVector2().X));
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - PiOver2);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - PiOver2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteEffects effects = Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = Projectile.rotation + (Owner.direction < 0 ? Pi : 0f);
            Vector2 drawPosition = Owner.MountedCenter + new Vector2(0f, 0f) + Projectile.rotation.ToRotationVector2() - Main.screenPosition;

            // Draw the main sprite.
            Main.EntitySpriteDraw(texture, drawPosition, texture.Frame(), Projectile.GetAlpha(lightColor), rotation, texture.Size() / 2f, Projectile.scale, effects, 0);
            return false;
        }
    }
}
