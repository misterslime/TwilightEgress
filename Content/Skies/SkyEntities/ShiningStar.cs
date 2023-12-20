using Cascade.Core.Graphics.GraphicalObjects.SkyEntities;
using Cascade.Core.Graphics.GraphicalObjects.SkyEntitySystem;
using Microsoft.Xna.Framework;

namespace Cascade.Content.Skies.SkyEntities
{
    public class ShiningStar : SkyEntity
    {
        public float MaxScale;

        public float MinScale;

        public float RotationSpeed;

        public float RotationDirection;

        public Vector2 StretchFactor;

        private int TextureIndex;

        public const int BaseLifespan = 480;

        public ShiningStar(Vector2 position, Color color, float maxScale, float depth, Vector2 stretchFactor, int lifespan)
        {
            Position = position;
            Color = color;
            MaxScale = maxScale;
            MinScale = maxScale * 0.5f;
            StretchFactor = stretchFactor;
            Lifespan = lifespan + BaseLifespan;
            Depth = depth;

            Opacity = 0f;
            Rotation = Main.rand.NextFloat(TwoPi);
            RotationSpeed = Main.rand.NextFloat(0.0025f, 0.01f);
            RotationDirection = Main.rand.NextBool().ToDirectionInt();
        }

        public override string TexturePath => "Terraria/Images/Projectile_79";

        public override BlendState BlendState => BlendState.Additive;

        public override SkyEntityDrawContext DrawContext => SkyEntityDrawContext.AfterBackgroundFog;

        public override void OnSpawn()
        {
            // Pick a different texture depending on the max scale of the star.
            if (MaxScale <= 1.5f)
                TextureIndex = Main.rand.Next(2);
            if (MaxScale is > 1.5f and <= 2f)
                TextureIndex = Main.rand.Next(2, 4);
            if (MaxScale is > 2f and <= 3f)
                TextureIndex = Main.rand.Next(4, 6);
        }

        public override void Update()
        {
            int timeToDisappear = Lifespan - 120;
            int timeToAppear = 120;
            float appearInterpolant = Time / (float)timeToAppear;
            float twinkleInterpolant = SineInOutEasing(Time / 120f, 0);
            float disappearInterpolant = (Time - timeToDisappear) / 120f;

            Scale = Lerp(MinScale, MaxScale, twinkleInterpolant);

            if (Time <= timeToAppear)
                Opacity = Lerp(0f, 1f, appearInterpolant);
            if (Time >= timeToDisappear && Time <= Lifespan)
                Opacity = Lerp(Opacity, 0f, disappearInterpolant);

            Rotation += RotationSpeed * RotationDirection;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D starTextures = ModContent.Request<Texture2D>(CascadeTextureRegistry.FourPointedStars[TextureIndex]).Value;
            Texture2D bloomTexture = ModContent.Request<Texture2D>("CalamityMod/UI/ModeIndicator/BloomFlare").Value;

            Vector2 mainOrigin = starTextures.Size() / 2f;
            Vector2 bloomOrigin = bloomTexture.Size() / 2f;

            float scaleWithDepth = Scale / Depth;
            Color color = Color * Opacity;

            spriteBatch.Draw(bloomTexture, GetDrawPositionBasedOnDepth(), null, color, Rotation, bloomOrigin, scaleWithDepth * 0.6f, 0, 0f);
            spriteBatch.Draw(starTextures, GetDrawPositionBasedOnDepth(), null, color, 0f, mainOrigin, scaleWithDepth * StretchFactor, 0, 0f);
        }
    }
}
