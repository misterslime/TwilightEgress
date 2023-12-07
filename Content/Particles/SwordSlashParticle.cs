namespace Cascade.Content.Particles
{
    public class SwordSlashParticle : Particle
    {
        private float BaseScale;

        private float BloomOpacity;

        private Vector2 StretchFactor;

        private Color BloomColor;

        public override bool SetLifetime => true;

        public override bool UseCustomDraw => true;

        public override string Texture => Utilities.EmptyPixelPath;

        public SwordSlashParticle(Vector2 position, Color slashColor, Color bloomColor, float rotation, Vector2 stretchFactor, float scale, int lifespan)
        {
            Position = position;
            Color = slashColor;
            BloomColor = bloomColor;
            Rotation = rotation;
            StretchFactor = stretchFactor;
            BaseScale = scale;
            Lifetime = lifespan;
        }

        public override void Update()
        {
            Scale = Lerp(BaseScale, 0f, LifetimeCompletion);
            BloomOpacity = Lerp(1f, 0f, LifetimeCompletion);
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D slashTexture = TextureAssets.Extra[98].Value;
            Texture2D bloomTexture = ModContent.Request<Texture2D>("CalamityMod/UI/ModeIndicator/BloomFlare").Value;

            Vector2 drawPosition = Position - Main.screenPosition;

            spriteBatch.SetBlendState(BlendState.Additive);
            spriteBatch.Draw(bloomTexture, drawPosition, null, BloomColor * BloomOpacity, Rotation, bloomTexture.Size() / 2f, Scale, SpriteEffects.None, 0f);  
            spriteBatch.Draw(slashTexture, drawPosition, null, Color, Rotation, slashTexture.Size() / 2f, Scale * StretchFactor, SpriteEffects.None, 0f);
            spriteBatch.SetBlendState(BlendState.AlphaBlend);
        }
    }
}
