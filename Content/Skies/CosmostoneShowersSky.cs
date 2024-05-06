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
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= float.MaxValue && minDepth < float.MaxValue)
            {
                Texture2D skyTexture = ModContent.Request<Texture2D>("Cascade/Content/Skies/CosmostoneShowersSky").Value;

                float gradientHeightInterpolant = Lerp(-0.002f, -0.02f, Main.LocalPlayer.Center.Y / (float)Main.worldSurface * 0.35f);

                spriteBatch.UseBlendState(BlendState.Additive);
                spriteBatch.Draw(skyTexture, new Rectangle(0, (int)(Main.worldSurface * 16f * gradientHeightInterpolant), Main.screenWidth * 2, Main.screenHeight), new Color(85, 113, 255) * FadeOpacity);
                spriteBatch.ResetToDefault();
            }
        }
    }
}
