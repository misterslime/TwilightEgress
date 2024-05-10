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
            if (maxDepth >= float.MaxValue && minDepth < float.MaxValue)
            {
                float gradientHeightInterpolant = Lerp(-0.001f, -0.25f, Main.LocalPlayer.Center.Y / (float)Main.worldSurface * 0.35f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.BackgroundViewMatrix.EffectMatrix);

                ShaderManager.TryGetShader("Cascade.CosmostoneShowersSkyShader", out ManagedShader cosmoSkyShader);
                cosmoSkyShader.TrySetParameter("galaxyOpacity", FadeOpacity * 0.4f);
                cosmoSkyShader.TrySetParameter("fadeOutMargin", 0.75f);
                cosmoSkyShader.TrySetParameter("pixelationFactor", new Vector2(500f, 500f));
                cosmoSkyShader.SetTexture(CascadeTextureRegistry.PerlinNoise, 1, SamplerState.LinearWrap);
                cosmoSkyShader.SetTexture(CascadeTextureRegistry.PerlinNoise2, 2, SamplerState.LinearWrap);
                cosmoSkyShader.Apply();

                spriteBatch.Draw(CascadeTextureRegistry.PurpleBlueNebulaGalaxyBlurred.Value, new Rectangle(0, (int)(Main.worldSurface * gradientHeightInterpolant), Main.screenWidth, Main.screenHeight), new Color(85, 113, 255) * FadeOpacity);
                spriteBatch.ResetToDefault();
            }
        }
    }
}
