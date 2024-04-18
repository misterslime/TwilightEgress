namespace Cascade.Content.Particles
{
    public class MechonSlayerArtParticle : Particle
    {
        private readonly float BaseScale;

        private readonly float NewScale;

        private readonly int ArtType;

        public override string AtlasTextureName => "Cascade.EmptyPixel";

        public MechonSlayerArtParticle(Vector2 position, float baseScale, float newScale, int artType, int lifespan)
        {
            Position = position;
            BaseScale = baseScale;
            NewScale = newScale;
            ArtType = artType;
            Lifetime = lifespan;
        }

        public override void Update()
        {
            Opacity = Lerp(1f, 0f, LifetimeRatio);
            Scale = new(Lerp(BaseScale, NewScale, CascadeUtilities.SineEaseOut(LifetimeRatio)));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (ArtType == -1)
                return;

            List<string> ArtTexturePaths = new()
            {
                "Cascade/Content/DedicatedContent/Enchilada/ArmorArt",
                "Cascade/Content/DedicatedContent/Enchilada/EaterArt",
                "Cascade/Content/DedicatedContent/Enchilada/EnchantArt",
                "Cascade/Content/DedicatedContent/Enchilada/PurgeArt",
                "Cascade/Content/DedicatedContent/Enchilada/SpeedArt",
            };

            Texture2D artTexture = ModContent.Request<Texture2D>(ArtTexturePaths[ArtType]).Value;
            Vector2 drawPosition = Position - Main.screenPosition;
            spriteBatch.Draw(artTexture, drawPosition, null, Color.White * Opacity, 0f, artTexture.Size() / 2f, Scale, 0, 0f);
        }
    }
}
