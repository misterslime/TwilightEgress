﻿using TwilightEgress.Core.Graphics.GraphicalObjects.SkyEntities;

namespace TwilightEgress.Content.Skies.SkyEntities
{
    public class ShiningStar : SkyEntity
    {
        public float MaxScale;

        public float MinScale;

        public Vector2 StretchFactor;

        private readonly int TextureIndex;

        public const int BaseLifespan = 480;

        public ShiningStar(Vector2 position, Color color, float maxScale, float depth, Vector2 stretchFactor, int lifespan)
        {
            Position = position;
            Color = color;
            MaxScale = maxScale;
            MinScale = maxScale * 0.5f;
            StretchFactor = stretchFactor;
            Lifetime = lifespan + BaseLifespan;
            Depth = depth;

            Opacity = 0f;
            Rotation = Main.rand.NextFloat(TwoPi);
            RotationSpeed = Main.rand.NextFloat(0.0025f, 0.01f);
            RotationDirection = Main.rand.NextBool().ToDirectionInt();

            // Pick a different texture depending on the max scale of the star.
            if (MaxScale <= 1.5f)
                TextureIndex = Main.rand.Next(2);
            if (MaxScale is > 1.5f and <= 2f)
                TextureIndex = Main.rand.Next(2, 4);
            if (MaxScale is > 2f and <= 3f)
                TextureIndex = Main.rand.Next(4, 6);
        }

        public override string AtlasTextureName => "TwilightEgress.EmptyPixel.png";

        public override BlendState BlendState => BlendState.Additive;

        public override SkyEntityDrawContext DrawContext => SkyEntityDrawContext.AfterCustomSkies;

        public override void Update()
        {
            int timeToDisappear = Lifetime - 120;
            int timeToAppear = 120;
            float appearInterpolant = Time / (float)timeToAppear;
            float twinkleInterpolant = TwilightEgressUtilities.SineEaseInOut(Time / 60f);
            float disappearInterpolant = (Time - timeToDisappear) / 120f;

            Scale = new Vector2(Lerp(MinScale, MaxScale, twinkleInterpolant));

            if (Time <= timeToAppear)
                Opacity = Lerp(0f, 1f, appearInterpolant);
            if (Time >= timeToDisappear && Time <= Lifetime)
                Opacity = Lerp(Opacity, 0f, disappearInterpolant);

            Rotation += RotationSpeed * RotationDirection;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            AtlasTexture starTextures = AtlasManager.GetTexture(TwilightEgressTextureRegistry.FourPointedStars_Atlas[TextureIndex]);
            AtlasTexture bloomTexture = AtlasManager.GetTexture("TwilightEgress.BloomFlare.png");

            Vector2 mainOrigin = starTextures.Size / 2f;
            Vector2 bloomOrigin = bloomTexture.Size / 2f;

            Vector2 scaleWithDepth = Scale / Depth;
            Color color = Color * Opacity;

            spriteBatch.Draw(bloomTexture, GetDrawPositionBasedOnDepth(), null, color, Rotation, bloomOrigin, scaleWithDepth / 8f);
            spriteBatch.Draw(starTextures, GetDrawPositionBasedOnDepth(), null, Color.White * Opacity, 0f, mainOrigin, scaleWithDepth * StretchFactor * 0.6f);
            spriteBatch.Draw(starTextures, GetDrawPositionBasedOnDepth(), null, color, 0f, mainOrigin, scaleWithDepth * StretchFactor);
        }
    }
}
