using Cascade.Core.Systems.SkyEntitySystem;

namespace Cascade.Content.Skies.SkyEntities
{
    public class FadingGlowBall : SkyEntity
    {
        public FadingGlowBall(Vector2 position, Vector2 velocity, Color color, float scale, float depth, int lifespan)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Scale = scale;
            Depth = depth;
            Lifespan = lifespan;
            Opacity = 0f;
        }

        public override void Update()
        {
            if (Time >= Lifespan - 30)
                Opacity = Clamp(Opacity - 0.1f, 0f, 1f);
            else
                Opacity = Clamp(Opacity + 0.075f, 0f, 1f);
        }

        public override void Draw(SpriteBatch spriteBatch, float intensity)
        {
            Texture2D mainTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/Light").Value;
            Texture2D bloomTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;

            // Change the position depending on the depth.
            Vector2 screenBounds = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
            Vector2 depthRelation = new Vector2(1f / Depth, 1f / Depth);
            Vector2 drawPosition = (Position - screenBounds) * depthRelation + screenBounds - Main.screenPosition;

            Vector2 mainOrigin = mainTexture.Size() / 2f;
            Vector2 bloomOrigin = bloomTexture.Size() / 2f;

            Color mainColor = Color * Opacity * intensity;
            Color bloomColor = Color * (1.75f * Scale) * Opacity * intensity;

            spriteBatch.SetBlendState(BlendState.Additive);
            spriteBatch.Draw(bloomTexture, drawPosition, null, bloomColor, Rotation, bloomOrigin, Scale * 2f, SpriteEffects.None, 0f);
            spriteBatch.Draw(mainTexture, drawPosition, null, mainColor, Rotation, mainOrigin, Scale, SpriteEffects.None, 0f);
            spriteBatch.SetBlendState(BlendState.AlphaBlend);
        }
    }
}
