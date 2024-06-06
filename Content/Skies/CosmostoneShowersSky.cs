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
            SamplerState samplerState = SamplerState.LinearWrap;
            // Makes the backgrounds move up or down on the screen depending on how high up near Space the player is.
            float gradientHeightInterpolant = Lerp(0.1f, -1.6f, Main.Camera.Center.Y / (float)(Main.worldSurface * 16f - Main.maxTilesY * 0.3f));

            if (maxDepth >= float.MaxValue && minDepth < float.MaxValue)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, samplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.BackgroundViewMatrix.EffectMatrix);

                // Bakcground nebula.
                Texture2D skyTexture = CascadeTextureRegistry.PurpleBlueNebulaGalaxyBlurred.Value;

                ShaderManager.TryGetShader("Cascade.CosmostoneShowersSkyShader", out ManagedShader cosmoSkyShader);
                cosmoSkyShader.TrySetParameter("galaxyOpacity", FadeOpacity);
                cosmoSkyShader.TrySetParameter("fadeOutMargin", 0.85f);
                cosmoSkyShader.TrySetParameter("textureSize", new Vector2(skyTexture.Width, skyTexture.Height));
                cosmoSkyShader.SetTexture(CascadeTextureRegistry.RealisticClouds, 1, samplerState);
                cosmoSkyShader.SetTexture(CascadeTextureRegistry.RealisticClouds, 2, samplerState);
                cosmoSkyShader.SetTexture(CascadeTextureRegistry.PerlinNoise2, 3, samplerState);
                cosmoSkyShader.Apply();

                spriteBatch.Draw(skyTexture, new Rectangle(0, (int)(Main.worldSurface * gradientHeightInterpolant + 50f), Main.screenWidth, Main.screenHeight), Color.White * FadeOpacity);
                spriteBatch.ExitShaderRegion();

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, samplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.BackgroundViewMatrix.EffectMatrix);

                // Clouds below the nebula.
                Texture2D cloudTexture = CascadeTextureRegistry.NeuronNebulaGalaxyBlurred.Value;

                ShaderManager.TryGetShader("Cascade.CosmostoneShowersCloudsShader", out ManagedShader cosmoCloudsShader);
                cosmoCloudsShader.TrySetParameter("cloudOpacity", FadeOpacity * 0.6f);
                cosmoCloudsShader.TrySetParameter("fadeOutMarginTop", 0.92f);
                cosmoCloudsShader.TrySetParameter("fadeOutMarginBottom", 0.75f);
                cosmoCloudsShader.TrySetParameter("erosionStrength", 0.8f);
                cosmoCloudsShader.TrySetParameter("textureSize", cloudTexture.Size());
                cosmoCloudsShader.SetTexture(CascadeTextureRegistry.RealisticClouds, 1, samplerState);
                cosmoCloudsShader.SetTexture(CascadeTextureRegistry.PerlinNoise3, 2, samplerState);
                cosmoCloudsShader.SetTexture(MiscTexturesRegistry.WavyBlotchNoise.Value, 3, samplerState);
                cosmoCloudsShader.Apply();

                spriteBatch.Draw(cloudTexture, new Rectangle(0, (int)(Main.worldSurface * gradientHeightInterpolant + 250f), Main.screenWidth, Main.screenHeight), Color.White * FadeOpacity);
                spriteBatch.ExitShaderRegion();
            }
        }
    }
}
