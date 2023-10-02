using Cascade.Core.Systems.SkyEntitySystem;

namespace Cascade.Content.Skies.SkyEntities
{
    public class FadingGlowStar : SkyEntity
    {
        private float RotationSpeed;

        private float RotationDirection;

        public FadingGlowStar(Vector2 position, Color color, float scale, float depth, int lifespan)
        {
            Position = position;
            Color = color;
            Scale = scale;
            Depth = depth;
            Lifespan = lifespan;
            Opacity = 0f;
            Rotation = Main.rand.NextFloat(TwoPi);
            RotationSpeed = Main.rand.NextFloat(0.0025f, 0.01f);
            RotationDirection = Main.rand.NextBool().ToDirectionInt();
        }

        public override void Update()
        {
            Rotation += RotationSpeed * RotationDirection;
            if (Time >= Lifespan - 45)
                Opacity = Clamp(Opacity - 0.1f, 0f, 1f);
            else
                Opacity = Clamp(Opacity + 0.1f, 0f, 1f);
        }

        public override void Draw(SpriteBatch spriteBatch, float intensity)
        {
            Texture2D mainTexture = CascadeTextureRegistry.GreyscaleStar.Value;
            Texture2D bloomTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;

            Vector2 screenBounds = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
            Vector2 depthRelation = new Vector2(1f / Depth, 1.1f / Depth);
            Vector2 drawPosition = (Position - screenBounds) * depthRelation + screenBounds - Main.screenPosition;

            Vector2 mainOrigin = mainTexture.Size() / 2f;
            Vector2 bloomOrigin = bloomTexture.Size() / 2f;

            Color mainColor = Color * Opacity * intensity;
            Color bloomColor = Color * (0.95f * Scale) * Opacity * intensity;

            spriteBatch.SetBlendState(BlendState.Additive);
            spriteBatch.Draw(bloomTexture, drawPosition, null, bloomColor, Rotation, bloomOrigin, Scale * 2f, SpriteEffects.None, 0f);
            spriteBatch.Draw(mainTexture, drawPosition, null, mainColor, Rotation, mainOrigin, Scale / 4f, SpriteEffects.None, 0f);
            spriteBatch.SetBlendState(BlendState.AlphaBlend);
        }
    }
}
