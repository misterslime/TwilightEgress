using Cascade.Content.Events.CosmostoneShowers;

namespace Cascade.Content.Skies
{
    public class CosmostoneShowersSceneEffect : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override float GetWeight(Player player) => 0.8f;

        public override bool IsSceneEffectActive(Player player) => player.ZoneCosmostoneShowers();

        public override void SpecialVisuals(Player player, bool isActive) => player.ManageSpecialBiomeVisuals("Cascade:CosmostoneShowers", isActive);
    }

    public class CosmostoneShowersSky : CustomSky
    {
        private bool isActive;

        private float FadeOpacity;

        public override float GetCloudAlpha() => (1f - FadeOpacity) * 0.3f + 0.7f;

        public override void Activate(Vector2 position, params object[] args) => isActive = true;

        public override void Deactivate(params object[] args) => isActive = false;

        public override void Reset() => isActive = false;

        public override bool IsActive() => isActive || FadeOpacity > 0f;

        public override void Update(GameTime gameTime)
        {
            if (isActive && FadeOpacity < 1f)
                FadeOpacity += 0.01f;
            else if (!isActive && FadeOpacity > 0f)
                FadeOpacity -= 0.01f;

            // Disable vanilla Terraria's stars in turn of making ours more abundant and noticeable.
            for (int i = 0; i < Main.maxStars; i++)
                Main.star[i] = new();
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            float gradientHeightInterpolant = Lerp(-0.1f, -0.35f, Main.LocalPlayer.Center.Y / (float)Main.worldSurface * 0.35f);
            if (maxDepth >= float.MaxValue && minDepth < float.MaxValue)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.BackgroundViewMatrix.EffectMatrix);

                // Bakcground nebula.
                Texture2D skyTexture = CascadeTextureRegistry.PurpleBlueNebulaGalaxyBlurred.Value;

                ShaderManager.TryGetShader("Cascade.CosmostoneShowersSkyShader", out ManagedShader cosmoSkyShader);
                cosmoSkyShader.TrySetParameter("galaxyOpacity", FadeOpacity * 0.15f);
                cosmoSkyShader.TrySetParameter("fadeOutMargin", 0.85f);
                cosmoSkyShader.TrySetParameter("textureSize", new Vector2(skyTexture.Width, skyTexture.Height));
                cosmoSkyShader.SetTexture(CascadeTextureRegistry.RealisticClouds, 1, SamplerState.LinearWrap);
                cosmoSkyShader.SetTexture(CascadeTextureRegistry.RealisticClouds, 2, SamplerState.LinearWrap);
                cosmoSkyShader.SetTexture(CascadeTextureRegistry.PerlinNoise2, 3, SamplerState.LinearWrap);
                cosmoSkyShader.Apply();

                spriteBatch.Draw(skyTexture, new Rectangle(0, (int)(Main.worldSurface * gradientHeightInterpolant + 25f), Main.screenWidth, Main.screenHeight), Color.White * FadeOpacity);
                spriteBatch.ExitShaderRegion();

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.BackgroundViewMatrix.EffectMatrix);

                // Clouds below the nebula.
                Texture2D cloudTexture = CascadeTextureRegistry.NeuronNebulaGalaxyBlurred.Value;

                ShaderManager.TryGetShader("Cascade.CosmostoneShowersCloudsShader", out ManagedShader cosmoCloudsShader);
                cosmoCloudsShader.TrySetParameter("cloudOpacity", FadeOpacity * 0.6f);
                cosmoCloudsShader.TrySetParameter("fadeOutMargin", 0.55f);
                cosmoCloudsShader.TrySetParameter("erosionStrength", 0.85f);
                cosmoCloudsShader.TrySetParameter("textureSize", cloudTexture.Size());
                cosmoCloudsShader.SetTexture(CascadeTextureRegistry.RealisticClouds, 1, SamplerState.LinearWrap);
                cosmoCloudsShader.SetTexture(CascadeTextureRegistry.PerlinNoise3, 2, SamplerState.LinearWrap);
                cosmoCloudsShader.SetTexture(MiscTexturesRegistry.TurbulentNoise.Value, 3, SamplerState.LinearWrap);
                cosmoCloudsShader.Apply();

                spriteBatch.Draw(cloudTexture, new Rectangle(0, (int)(Main.worldSurface * gradientHeightInterpolant + 450f), Main.screenWidth, Main.screenHeight), Color.White * FadeOpacity);
                spriteBatch.ExitShaderRegion();
            }
        }
    }
}
