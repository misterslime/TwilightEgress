using Cascade.Core.Graphics.GraphicalObjects.SkyEntitySystem;

namespace Cascade.Content.Skies.SkyEntities
{
    public class CosmicGas : SkyEntity
    {
        public float MaxScale;

        public float RotationSpeed;

        public float RotationDirection;

        private int TextureIndex;

        public const int BaseLifespan = 480;

        public CosmicGas(Vector2 position, Color color, float maxScale, float depth, int lifespan)
        {
            Position = position;
            Color = color;
            Scale = maxScale;
            Lifespan = lifespan + BaseLifespan;
            Depth = depth;

            Opacity = 0f;
            TextureIndex = Main.rand.Next(CascadeTextureRegistry.Smokes.Count);
            Rotation = Main.rand.NextFloat(TwoPi);
            RotationSpeed = Main.rand.NextFloat(0.001f, 0.003f);
            RotationDirection = Main.rand.NextBool().ToDirectionInt();
        }

        public override string TexturePath => Utilities.EmptyPixelPath;

        public override BlendState BlendState => BlendState.Additive;

        public override void Update()
        {
            int timeToDisappear = Lifespan - 120;
            int timeToAppear = 120;
            float appearInterpolant = Time / (float)timeToAppear;
            float disappearInterpolant = (Time - timeToDisappear) / 120f;

            if (Time <= timeToAppear)
                Opacity = Lerp(0f, 1f, appearInterpolant);
            if (Time >= timeToDisappear && Time <= Lifespan)
                Opacity = Lerp(Opacity, 0f, disappearInterpolant);

            Rotation += RotationSpeed * RotationDirection;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D smokeTexture = ModContent.Request<Texture2D>(CascadeTextureRegistry.Smokes[TextureIndex]).Value;

            Vector2 mainOrigin = smokeTexture.Size() / 2f;
            Color color = Color * Opacity * 0.3f;

            spriteBatch.Draw(smokeTexture, GetDrawPositionBasedOnDepth(), null, color, Rotation, mainOrigin, Scale / 8f, 0, 0f);
        }
    }
}
