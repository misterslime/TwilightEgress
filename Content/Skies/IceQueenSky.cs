using Cascade.Assets.Effects;
using Cascade.Assets.ExtraTextures;
using System.Collections.Generic;
using Terraria.Graphics.Effects;

namespace Cascade.Content.Skies
{
    public class IceQueenSceneEffect : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player) => !Main.gameMenu && NPC.AnyNPCs(NPCID.IceQueen);

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalamityReworks:IceQueen", isActive); 
        }
    }

    public class IceQueenSky : CustomSky
    {
        public class GlowStar
        {
            public int Time;

            public int Lifespan;

            public int IdenityIndex;

            public float Scale;

            public float Opacity;

            public float Depth;

            public Vector2 Position;

            public Vector2 Velocity;

            public Color Color;

            public GlowStar(int lifeSpan, int indenity, float scale, float depth, Vector2 position, Vector2 velocity, Color color)
            {
                Lifespan = lifeSpan;
                IdenityIndex = indenity;
                Scale = scale;
                Opacity = 1f;
                Depth = depth;
                Position = position;
                Velocity = velocity;
                Color = color;
            }
        }

        private Texture2D GlowStarTexture;

        private bool isActive;

        private float FadeOpacity;

        private List<GlowStar> GlowStars = new List<GlowStar>();

        private int GlowStarSpawnChance
        {
            get
            {
                if (!isActive)
                {
                    return int.MaxValue;
                }

                return 12;
            }
        }

        private float GlowStarSpeed
        {
            get
            {
                if (!isActive)
                {
                    return 0f;
                }

                return Main.rand.NextFloat(0.95f, 5f);
            }
        }

        private Color GetGlowStarColor()
        {
            Color firstColor = Utils.SelectRandom(Main.rand, Color.SkyBlue, Color.LightSkyBlue);
            Color secondColor = Utils.SelectRandom(Main.rand, Color.Cyan, Color.CornflowerBlue);
            return Color.Lerp(firstColor, secondColor, Main.rand.NextFloat(0.1f, 1f));
        }

        public override void OnLoad()
        {
            GlowStarTexture = CascadeTextureRegistry.GreyscaleStar.Value;
        }

        public override void Update(GameTime gameTime)
        {
            if (isActive && FadeOpacity < 1f)
            {
                FadeOpacity += 0.01f;
            }
            else if (!isActive && FadeOpacity > 0f)
            {
                FadeOpacity -= 0.01f;
            }

            if (isActive)
            {
                if (Main.rand.NextBool(GlowStarSpawnChance))
                {
                    int lifeSpan = Main.rand.Next(420, 480);
                    float depth = Main.rand.NextFloat(0f, 1f);
                    float startingScale = Main.rand.NextFloat(0.35f, 0.85f);
                    Vector2 startingPosition = Main.screenPosition + new Vector2(Main.screenWidth * Main.rand.NextFloat(-0.1f, 1.1f), Main.screenHeight * 1.05f);
                    Vector2 initialVelocity = -Vector2.UnitY.SafeNormalize(Vector2.Zero) * GlowStarSpeed;
                    GlowStars.Add(new GlowStar(lifeSpan, GlowStars.Count(), startingScale, depth, startingPosition, initialVelocity, GetGlowStarColor()));
                }
            }

            for (int i = 0; i < GlowStars.Count; i++)
            {
                GlowStars[i].Time++;
                GlowStar star = GlowStars[i];
                star.Position += star.Velocity;
                if (star.Time >= star.Lifespan)
                    star.Opacity -= 0.025f;
                star.Color *= star.Opacity;
            }
            GlowStars.RemoveAll((GlowStar star) => star.Opacity <= 0f);
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 1f && minDepth < 1f)
            {
                CalamityUtils.EnterShaderRegion(Main.spriteBatch);
                Vector2 scale = new Vector2(Main.screenWidth, Main.screenWidth) / TextureAssets.MagicPixel.Value.Size() * Main.GameViewMatrix.Zoom;

                CascadeEffectRegistry.IceQueenScrollingBackgroundShader.SetShaderTexture(CascadeTextureRegistry.GreyscaleSeemlessNoise, 1);
                CascadeEffectRegistry.IceQueenScrollingBackgroundShader.UseColor(Color.DarkGray);
                CascadeEffectRegistry.IceQueenScrollingBackgroundShader.Apply();

                spriteBatch.Draw(TextureAssets.MagicPixel.Value, Vector2.Zero, null, Color.Lerp(Color.White, Color.Transparent, 0.35f) * FadeOpacity, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                CalamityUtils.ExitShaderRegion(Main.spriteBatch);
            }

            for (int i = 0; i < GlowStars.Count; i++)
            {
                CalamityUtils.SetBlendState(spriteBatch, BlendState.Additive);
                Vector2 drawPosition = GlowStars[i].Position - Main.screenPosition;
                spriteBatch.Draw(TextureAssets.Extra[49].Value, drawPosition, null, GlowStars[i].Color * FadeOpacity, 0f, GlowStarTexture.Size() / 2f, GlowStars[i].Scale / 2f, SpriteEffects.None, 0f);
                spriteBatch.Draw(CascadeTextureRegistry.GreyscaleStar.Value, drawPosition, null, GlowStars[i].Color * FadeOpacity, 0f, GlowStarTexture.Size() / 2f, GlowStars[i].Scale / 12f, SpriteEffects.None, GlowStars[i].Depth);
                CalamityUtils.SetBlendState(spriteBatch, BlendState.AlphaBlend);
            }
        }

        public override float GetCloudAlpha()
        {
            return (1f - FadeOpacity) * 0.3f + 0.7f;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override bool IsActive() => isActive || FadeOpacity > 0f;
    }
}
