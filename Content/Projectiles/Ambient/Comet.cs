namespace Cascade.Content.Projectiles.Ambient
{
    public class Comet : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ambient";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Comet");
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.alpha = 255;
            Projectile.timeLeft = 1800;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            // We do a little bit of trolling :)))))
            Projectile.hostile = Main.zenithWorld;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
                Projectile.alpha -= 20;

            Projectile.rotation += Projectile.velocity.X * 0.02f;

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.frame = Main.rand.Next(0, 3);
                Projectile.netUpdate = true;
            }

            Lighting.AddLight(Projectile.Center, Color.SkyBlue.ToVector3());
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 speed = Utils.RandomVector2(Main.rand, -1f, 1f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.BlueFairy, speed * 5f);
                d.noGravity = true;

                Dust d2 = Dust.NewDustPerfect(Projectile.Center, DustID.TintableDust, speed * 5f);
                d2.color = Color.Lerp(Color.Brown, Color.SaddleBrown, Main.rand.NextFloat());
            }

        }

        public float SetTrailWidth(float completionRatio)
        {
            return 32f * Utils.GetLerpValue(0.75f, 0f, completionRatio, true) * Projectile.scale * Projectile.Opacity;
        }

        public Color SetTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.SkyBlue, Color.DeepSkyBlue, completionRatio) * Projectile.Opacity;
        }

        public void DrawPrims()
        {
            /*TrailDrawer ??= new PrimitiveDrawer(SetTrailWidth, SetTrailColor, true, GameShaders.Misc["CalamityMod:ArtemisLaser"]);

            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:ArtemisLaser"].UseImage1("Images/Extra_189");
            GameShaders.Misc["CalamityMod:ArtemisLaser"].UseImage2("Images/Misc/Perlin");
            TrailDrawer.DrawPrimitives(Projectile.oldPos.ToList(), Projectile.Size * 0.5f - Main.screenPosition, 85);
            Main.spriteBatch.ExitShaderRegion();*/

            Vector2 positionToCenterOffset = Projectile.Size * 0.5f;
            ManagedShader shader = ShaderManager.GetShader("Luminance.StandardPrimitiveShader");
            PrimitiveSettings primSettings = new(SetTrailWidth, SetTrailColor, _ => positionToCenterOffset, Shader: shader);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos.ToList(), primSettings, 85);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrims();

            Projectile.DrawBackglow(Projectile.GetAlpha(Color.Cyan * 0.45f), 2f);
            Projectile.DrawTextureOnProjectile(Projectile.GetAlpha(Color.White), Projectile.rotation, Projectile.scale, animated: true);
            return false;
        }
    }
}
