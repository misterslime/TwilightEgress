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

        public override bool DieWithLifespan => true;

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

            float scaleWithDepth = Scale / Depth;
            Color color = Color * Opacity;

            spriteBatch.Draw(bloomTexture, GetDrawPositionBasedOnDepth(), null, color, Rotation, bloomOrigin, scaleWithDepth / 3f, 0, 0f);
            spriteBatch.Draw(StoredTexture, GetDrawPositionBasedOnDepth(), null, color, 0f, mainOrigin, scaleWithDepth * StretchFactor, 0, 0f);
        }
    }
}
