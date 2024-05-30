using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Cascade.Content.Items.Weapons.Ranged.SailorsSingularity
{
    public class SailorsSingularityHoldout : ModProjectile
    {
        public override string Texture => "Cascade/Content/Items/Weapons/Ranged/SailorsSingularity/SailorsSingularity";
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 20;
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
        private int despawnCounter;
        Vector2 toMouse = Vector2.Zero;
        float fireRotation = 0f;
        private static bool swingUp = true;
        public override void AI()
        {           
            Player owner = Main.player[Projectile.owner];
            if (fireRate == 0)
                fireRate = 40;
            if (!owner.active || owner.dead || (!owner.channel && despawnCounter >= 30) || ((!owner.Calamity().mouseRight || despawnCounter >= 30) && owner.altFunctionUse == 2))
            {
                fireRate = 40;
                attackCounter = 0;
                counter = 0;
                Projectile.active = false;
            }
            else
            {
                if (owner.altFunctionUse != 2)
                {
                    if (!owner.channel)
                        despawnCounter++;
                    else
                        despawnCounter = 0;

                    if (despawnCounter != 0)
                    {
                        if (despawnCounter == 1)
                        {
                            counter = 0;
                            swingUp = swingUp ? false : true;
                        }
                        float range = Pi / 2 - (float)(attackCounter / 8f);
                        Vector2 position = new Vector2(owner.Center.X, owner.Center.Y - 24) + toMouse * 20;
                        int rotationDirection = swingUp ? -1 : 1;

                        if (attackCounter == 14)
                            fireRate = (int)Math.Ceiling(40 / 1.5);

                        if (counter == 0)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_BookStaffCast);
                            toMouse = (Main.MouseWorld - owner.Center).SafeNormalize(Vector2.Zero);
                        }
                        if (attackCounter != 14) //Uncharged Attack
                        {
                            if (counter < 10 && Main.myPlayer == owner.whoAmI)
                            {
                                fireRotation = toMouse.RotatedBy(range / 2 * rotationDirection + range / 10 * (counter) * rotationDirection - range * rotationDirection).ToRotation();
                                if (counter % 2 == 0 && Main.myPlayer == Projectile.owner)
                                    Projectile.NewProjectile(Terraria.Entity.GetSource_NaturalSpawn(), position, toMouse.RotatedBy(range / 2 * rotationDirection + range / 4 * (counter / 2) * rotationDirection - range * rotationDirection) * 10f, ModContent.ProjectileType<SailorStar>(), Projectile.damage, Projectile.knockBack, owner.whoAmI);
                            }
                        }
                        else //Max Charge Attack
                        {                            
                            if (counter == 0 && Main.myPlayer == owner.whoAmI)
                            {
                                fireRotation = toMouse.ToRotation();
                                if (Main.myPlayer == Projectile.owner)
                                {
                                    Projectile.NewProjectile(Terraria.Entity.GetSource_NaturalSpawn(), position, toMouse * 12f, ModContent.ProjectileType<SailorBlast>(), Projectile.damage * 3, Projectile.knockBack, owner.whoAmI);
                                    Main.player[Projectile.owner].velocity += toMouse.RotatedBy(Pi) * 5f;
                                }
                            }                           
                            if(despawnCounter < 15)
                                despawnCounter = 15;
                            fireRotation = fireRotation.ToRotationVector2().RotatedBy(0.41f/(despawnCounter -14) * -owner.direction).ToRotation();
                        }
                        counter++;
                    }
                    else
                    {
                        toMouse = (Main.MouseWorld - owner.Center).SafeNormalize(Vector2.Zero);
                        fireRotation = toMouse.ToRotation();
                        if (attackCounter == 14)
                            fireRotation = fireRotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)).ToRotation();
                        if (counter % 4 == 0 && attackCounter < 14)
                            attackCounter++;
                        counter++;
                    }
                }
                else
                {
                    if (!owner.Calamity().mouseRight || Main.projectile.Any(p => p.active && p.type == ModContent.ProjectileType<SailorVortex>()))
                        despawnCounter++;
                    else
                        despawnCounter = 0;
                    toMouse = (Main.MouseWorld - owner.Center).SafeNormalize(Vector2.Zero);
                    if(counter < 45)
                        fireRotation = toMouse.ToRotation();                   
                    if (counter >= 45 && Main.myPlayer == owner.whoAmI)
                    {
                        if (counter == 45)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_BookStaffCast);
                            Projectile.NewProjectile(Terraria.Entity.GetSource_NaturalSpawn(), new Vector2(Projectile.Center.X, Projectile.Center.Y), toMouse * 5f, ModContent.ProjectileType<SailorVortex>(), Projectile.damage, Projectile.knockBack, owner.whoAmI);
                        }
                        if (despawnCounter < 15)
                            despawnCounter = 15;
                        fireRotation = fireRotation.ToRotationVector2().RotatedBy(0.41f / (despawnCounter - 14) * -owner.direction).ToRotation();
                    }
                    counter++;
                }
                UpdateProjectileHeldVariables(owner.RotatedRelativePoint(owner.MountedCenter, true));
                ManipulatePlayerVariables();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 halfSizeTexture = new(TextureAssets.Projectile[Projectile.type].Value.Width / 2, TextureAssets.Projectile[Projectile.type].Value.Height / 2);
            Vector2 origin = new(halfSizeTexture.X - 24, halfSizeTexture.Y + 4 * (int)(Projectile.rotation.ToRotationVector2().X / Math.Abs(Projectile.rotation.ToRotationVector2().X)));
            Vector2 drawPosition = new Vector2(Projectile.Center.X, Projectile.Center.Y - 8) - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if ((int)(Projectile.rotation.ToRotationVector2().X / Math.Abs(Projectile.rotation.ToRotationVector2().X)) == -1)
                spriteEffects = SpriteEffects.FlipVertically;
            Main.EntitySpriteDraw(texture, drawPosition, texture.Bounds, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0f);
            return false;
        }
        private void UpdateProjectileHeldVariables(Vector2 armPosition)
        {
            Projectile.velocity = Vector2.Zero;
            Player owner = Main.player[Projectile.owner];
            Vector2 toMouse = (Main.MouseWorld - owner.Center).SafeNormalize(Vector2.Zero);
            Projectile.Center = new Vector2(owner.Center.X, owner.Center.Y);
            Projectile.rotation = fireRotation;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
        }

        private void ManipulatePlayerVariables()
        {
            Player owner = Main.player[Projectile.owner];
            owner.ChangeDir((int)(Projectile.rotation.ToRotationVector2().X / Math.Abs(Projectile.rotation.ToRotationVector2().X)));
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;
                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, fireRotation - Pi / 2);
        }
    }
}
