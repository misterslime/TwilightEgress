using System.Collections.Generic;
using Terraria.Graphics.Shaders;

namespace Cascade.Content.DedicatedContent.Marv
{
    public class ElectricSkyBolt : ModProjectile, ILocalizedModType
    {
        public Vector2 StrikePosition { get; set; }

        public List<Vector2> StrikePositions = new List<Vector2>();

        public PrimitiveDrawingSystem TrailDrawer { get; set; }

        public new string LocalizationCategory => "Projectiles.Magic";

        public override string Texture => "Cascade/Assets/ExtraTextures/GreyscaleObjects/StarNonPixelated";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Thunderbolt");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.Opacity = 1f;
            Projectile.timeLeft = 45;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            // Initialization.
            if (Projectile.timeLeft == 45)
            {
                StrikePosition = Main.MouseWorld;
                Projectile.rotation = Main.rand.NextFloat(TwoPi);
            }

            if (Projectile.timeLeft == 44)
            {
                StrikePositions = Utilities.CreateLightningBoltPoints(Projectile.Center, StrikePosition);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), StrikePosition, Vector2.Zero, ModContent.ProjectileType<ElectricSkyBoltExplosion>(), Projectile.damage, Projectile.knockBack, Owner: Projectile.owner);
                int numOfMist = Main.rand.Next(5, 10);
                for (int i = 0; i < numOfMist; i++)
                {
                    Vector2 mistVelocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(1f, 3f);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), StrikePosition, mistVelocity, ModContent.ProjectileType<ElectricSkyBoltMist>(), Projectile.damage, Projectile.knockBack,  Owner: Projectile.owner);
                }

                bool correctPlayerName = owner.name == "Marv" || owner.name == "EmolgaLover";
                SoundStyle lightning = correctPlayerName ? CascadeSoundRegistry.PokemonThunderbolt : CommonCalamitySounds.LightningSound;
                SoundEngine.PlaySound(lightning, StrikePosition);
                Projectile.netUpdate = true;
            }

            if (Projectile.timeLeft <= 30)
            {
                Projectile.Opacity = Lerp(1f, 0f, Utils.GetLerpValue(30f, 0f, Projectile.timeLeft, true));
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 speed = Utils.RandomVector2(Main.rand, -1f, 1f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.YellowStarDust, speed * 5f);
                d.noGravity = true;
            }
        }

        public float SetTrailWidth(float completionRatio)
        {
            return 16f * Utils.GetLerpValue(1f, 0.01f, completionRatio, true) * Lerp(1f, 0f, Utils.GetLerpValue(30f, 10f, Projectile.timeLeft, true));
        }

        public Color SetTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.Yellow, Color.Goldenrod, completionRatio) * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawer ??= new PrimitiveDrawingSystem(SetTrailWidth, SetTrailColor, true, GameShaders.Misc["CalamityMod:HeavenlyGaleLightningArc"]);

            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:HeavenlyGaleLightningArc"].UseImage1("Images/Misc/Perlin");
            GameShaders.Misc["CalamityMod:HeavenlyGaleLightningArc"].Apply();
            TrailDrawer.DrawPrimitives(StrikePositions, -Main.screenPosition, 22);
            Main.spriteBatch.ExitShaderRegion();

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            DrawBloomFlare(texture);
            DrawBloomFlare(texture, true);

            return false;
        }

        public void DrawBloomFlare(Texture2D texture, bool strikePosition = false)
        {
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Vector2 drawPosition = strikePosition ? StrikePosition : Projectile.Center;
            Main.EntitySpriteDraw(texture, drawPosition - Main.screenPosition, texture.Frame(), Color.LightYellow * Projectile.Opacity, Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(texture, drawPosition - Main.screenPosition, texture.Frame(), Color.LightYellow * Projectile.Opacity, Projectile.rotation * 2f, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(texture, drawPosition - Main.screenPosition, texture.Frame(), Color.LightYellow * Projectile.Opacity, Projectile.rotation * 3f, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(texture, drawPosition - Main.screenPosition, texture.Frame(), Color.LightYellow * Projectile.Opacity, Projectile.rotation * 4f, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
        }
    }
}
