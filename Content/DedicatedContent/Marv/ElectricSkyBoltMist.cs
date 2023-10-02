﻿namespace Cascade.Content.DedicatedContent.Marv
{
    public class ElectricSkyBoltMist : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";

        public override string Texture => "CalamityMod/Projectiles/Summon/SmallAresArms/MinionPlasmaGas";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Thunderous Mist");
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 255;
            Projectile.timeLeft = 480;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 480)
            {
                Projectile.rotation = Main.rand.NextFloat(TwoPi);
                Projectile.scale = Main.rand.NextFloat(0.65f, 1.25f);
            }

            Projectile.rotation += Projectile.velocity.X * 0.003f;
            Projectile.velocity *= 0.98f;

            if (Projectile.timeLeft >= 60)
            {
                Projectile.alpha = (int)Clamp(Projectile.alpha - 17, 0, 255);
            }
            else
            {
                Projectile.alpha = (int)Clamp(Projectile.alpha + 17, 0, 255);
            }

            Lighting.AddLight(Projectile.Center, Color.Goldenrod.ToVector3() * 0.35f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Projectile.DrawTextureOnProjectile(Projectile.GetAlpha(Color.Goldenrod), Projectile.rotation, Projectile.scale, animated: true);
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}