using Cascade.Core.Systems.SkyEntitySystem;
using System.Collections.Generic;

namespace Cascade.Content.Skies.SkyEntities
{
    public class StarConstellation : SkyEntity
    {
        private int ConstellationSegments;

        private float ConstellationSway;

        private float ConstellationJagednessNumerator;

        private Vector2 Destination;

        public StarConstellation(Vector2 position, Vector2 destination, Color color, float scale, float depth, int lifespan, int constellationSegments, float constellationSway, float constellationJagednessNumerator)
        {
            Position = position;
            Destination = destination;
            Color = color;
            Scale = scale;
            Depth = depth;
            Lifespan = lifespan;
            ConstellationSegments = constellationSegments;
            ConstellationSway = constellationSway;
            ConstellationJagednessNumerator = constellationJagednessNumerator;
            Opacity = 0f;
            Rotation = Main.rand.NextFloat(TwoPi);
        }

        public override void Update()
        {
            if (Time >= Lifespan - 45)
                Opacity = Clamp(Opacity - 0.1f, 0f, 1f);
            else
                Opacity = Clamp(Opacity + 0.1f, 0f, 1f);
        }

        public override void Draw(SpriteBatch spriteBatch, float intensity)
        {
            DrawEndingPointStars(spriteBatch, intensity, Position);
            DrawEndingPointStars(spriteBatch, intensity, Destination);
        }

        public float GetConstellationWidth(float completionValue) => 5f * Scale * Opacity;

        public Color GetConstellationColor(float completionValue) => Color * Opacity;

        public void DrawEndingPointStars(SpriteBatch spriteBatch, float intensity, Vector2 drawPos)
        {
            Texture2D mainTexture = CascadeTextureRegistry.GreyscaleStar.Value;
            Texture2D bloomTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;

            Vector2 screenBounds = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
            Vector2 depthRelation = new Vector2(1f / Depth, 1.1f / Depth);
            Vector2 drawPosition = (drawPos - screenBounds) * depthRelation + screenBounds - Main.screenPosition;

            Vector2 mainOrigin = mainTexture.Size() / 2f;
            Vector2 bloomOrigin = bloomTexture.Size() / 2f;

            Color mainColor = Color * Opacity * intensity;
            Color bloomColor = Color * (1f * Scale) * Opacity * intensity;

            spriteBatch.SetBlendState(BlendState.Additive);
            spriteBatch.Draw(bloomTexture, drawPosition, null, bloomColor, Rotation, bloomOrigin, Scale * 1.25f, SpriteEffects.None, 0f);
            spriteBatch.Draw(mainTexture, drawPosition, null, mainColor, Rotation, mainOrigin, Scale / 16f, SpriteEffects.None, 0f);
            spriteBatch.SetBlendState(BlendState.AlphaBlend);
        }
    }
}
