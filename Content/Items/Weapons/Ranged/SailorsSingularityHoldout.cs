using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Cascade.Content.Items.Weapons.Ranged
{
    public class SailorsSingularityHoldout : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
        }
        private int fireRate
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private int attackCounter = 0;
        private int counter = 0;
        public override void AI()
        {
            //CalamityUtils.DisplayLocalizedText($"{counter}, {attackCounter}, {fireRate}");
            Player owner = Main.player[Projectile.owner];
            if (fireRate == 0)
                fireRate = 40;
            if (!owner.active || owner.dead || !owner.channel && (counter == 0 || attackCounter == 14))
            {                 
                fireRate = 40;
                attackCounter = 0;
                counter = 0;
                Projectile.active = false;
            }
            else
            {                
                float range = Pi - (float)(attackCounter / 4f);
                Vector2 toMouse = (Main.MouseWorld - owner.Center).SafeNormalize(Vector2.Zero);
                Vector2 position = owner.Center + (toMouse * 20);
                int rotationDirection = attackCounter % 2 == 0 ? -1 : 1;

                UpdateProjectileHeldVariables(owner.RotatedRelativePoint(owner.MountedCenter, true));
                ManipulatePlayerVariables();

                if (attackCounter == 14 && counter == (int)Math.Ceiling(40 / 1.5))
                    fireRate = (int)Math.Ceiling(40 / 1.5);

                if (counter == 0)
                    SoundEngine.PlaySound(SoundID.DD2_BookStaffCast);
                
                if (attackCounter != 14)
                {
                    if (counter % 4 == 0 && counter < 20 && Main.myPlayer == owner.whoAmI)
                        Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), position, toMouse.RotatedBy((range / 2 * rotationDirection) + (range / 4 * (counter / 4) * rotationDirection) - (range * rotationDirection)) * 8.5f, ProjectileID.StarCannonStar, Projectile.damage, Projectile.knockBack, owner.whoAmI);
                }
                else
                {
                    if (counter == 0 && Main.myPlayer == owner.whoAmI)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), position + ((Pi / 2).ToRotationVector2().RotatedBy(TwoPi / 5 * (i) + TwoPi / 10) * 30), toMouse * 8.5f, ProjectileID.StarCannonStar, Projectile.damage, Projectile.knockBack, owner.whoAmI);
                        }
                        Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), position, toMouse * 8.5f, ProjectileID.StarCannonStar, Projectile.damage, Projectile.knockBack, owner.whoAmI);
                    }
                }
                if (counter == 20 && attackCounter < 14)
                    attackCounter++;
                counter++;
                if (counter >= fireRate)
                    counter = 0;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
        private void UpdateProjectileHeldVariables(Vector2 armPosition)
        {
            Projectile.velocity = Vector2.Zero;
            Player owner = Main.player[Projectile.owner];
            Vector2 toMouse = (Main.MouseWorld - owner.Center).SafeNormalize(Vector2.Zero);
            Projectile.Center = new Vector2(owner.Center.X - 26, owner.Center.Y - 4) + (toMouse * 38);
            Projectile.rotation = toMouse.ToRotation();
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
        }

        private void ManipulatePlayerVariables()
        {
            CalamityUtils.DisplayLocalizedText($"{Projectile.rotation}");
            Player owner = Main.player[Projectile.owner];
            owner.ChangeDir(Projectile.direction);
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;
            owner.itemRotation = Projectile.rotation;
            if(Math.Abs(Projectile.rotation) > PiOver2)
                owner.direction = -1;
            else
                owner.direction = 1;
        }
    }
}
