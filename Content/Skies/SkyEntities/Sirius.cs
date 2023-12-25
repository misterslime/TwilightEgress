using Cascade.Core.Graphics.GraphicalObjects.SkyEntities;
using Cascade.Core.Graphics.GraphicalObjects.SkyEntitySystem;

namespace Cascade.Content.Skies.SkyEntities
{
    public class Sirius : SkyEntity
    {
        public float MaxScale;

        public float MinScale;

        public float RotationSpeed;

        public float RotationDirection;

        public const int BaseLifespan = 2400;

        public Sirius(Vector2 position, Color color, float maxScale, int lifespan)
        {
            Position = position;
            Color = color;
            MaxScale = maxScale;
            MinScale = maxScale * 0.75f;
            Lifespan = lifespan + BaseLifespan;
            Depth = 150f;

            Opacity = 0f;
            Rotation = Main.rand.NextFloat(TwoPi);
            RotationSpeed = Main.rand.NextFloat(0.0025f, 0.01f);
            RotationDirection = Main.rand.NextBool().ToDirectionInt();
        }

        public override string TexturePath => "CalamityMod/Projectiles/Summon/SiriusMinion";

        public override SkyEntityDrawContext DrawContext => SkyEntityDrawContext.AfterCustomSkies;

        public override BlendState BlendState => BlendState.Additive;

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
            Texture2D bloomTexture = ModContent.Request<Texture2D>("CalamityMod/UI/ModeIndicator/BloomFlare").Value;

            Vector2 mainOrigin = StoredTexture.Size() / 2f;
            Vector2 bloomOrigin = bloomTexture.Size() / 2f;

            Color color = Color * Opacity;

            spriteBatch.Draw(bloomTexture, GetDrawPositionBasedOnDepth(), null, color * 0.5f, Rotation, bloomOrigin, Scale * 0.8f, 0, 0f);
            spriteBatch.Draw(bloomTexture, GetDrawPositionBasedOnDepth(), null, color * 0.7f, -Rotation, bloomOrigin, Scale * 0.6f, 0, 0f);

            spriteBatch.Draw(StoredTexture, GetDrawPositionBasedOnDepth(), null, Color.White * Opacity, 0f, mainOrigin, Scale, 0, 0f);
        }
    }
}
