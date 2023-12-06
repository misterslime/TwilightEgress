using static Cascade.Core.Graphics.PrimitiveDrawer;

namespace Cascade.Content.Particles
{
    public class LightningArcParticle : Particle
    {
        private float LightningLengthFactor;

        private bool Initialized;

        private Vector2 ActualEndPosition;

        private List<Vector2> LightningPoints;

        public float PointDisplacementVariance { get; set; }

        public float JaggednessNumerator { get; set; }

        public bool UseSmoothening { get; set; }

        public bool AdditiveBlending { get; set; }

        public Vector2 EndPosition { get; set; }

        public PrimitiveDrawer LightningDrawer { get; set; } = null;

        public LightningArcParticle(Vector2 basePosition, Vector2 endPosition, float pointDisplacementVariance, float jaggednessNumerator, float scale, Color color, int lifespan, bool useSmoothening = false, bool additiveBlending = true)
        {
            Position = basePosition;
            EndPosition = endPosition;
            PointDisplacementVariance = pointDisplacementVariance;
            JaggednessNumerator = jaggednessNumerator;
            Scale = scale;
            Color = color;
            Lifetime = lifespan;
            UseSmoothening = useSmoothening;
            AdditiveBlending = additiveBlending;
        }

        public override string Texture => Utilities.EmptyPixelPath;

        public override bool SetLifetime => true;

        public override bool UseCustomDraw => true;

        public override void Update()
        {
            if (!Initialized)
            {
                Initialized = true;
                LightningPoints = Utilities.CreateLightningBoltPoints(Position, EndPosition, PointDisplacementVariance, JaggednessNumerator);
            }

            LightningLengthFactor = Clamp(LightningLengthFactor + 0.15f, 0f, 1f);
            ActualEndPosition += (LightningLengthFactor * EndPosition.Length()).ToRotationVector2();
        }

        public float GetLightningWidth(float completionRatio) => Scale * Utils.GetLerpValue(1f, 0f, completionRatio, true) * Lerp(1f, 0f, LifetimeCompletion);

        public Color GetLightningColor(float completionRatio) => Color;

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            LightningDrawer ??= new(GetLightningWidth, GetLightningColor, UseSmoothening, GameShaders.Misc["CalamityMod:HeavenlyGaleLightningArc"]);

            spriteBatch.EnterShaderRegion(AdditiveBlending ? BlendState.Additive : BlendState.AlphaBlend);
            GameShaders.Misc["CalamityMod:HeavenlyGaleLightningArc"].UseImage1("Images/Misc/Perlin");
            GameShaders.Misc["CalamityMod:HeavenlyGaleLightningArc"].Apply();
            LightningDrawer.DrawPrimitives(LightningPoints, -Main.screenPosition, LightningPoints.Count * 2);
            spriteBatch.ExitShaderRegion();
        }
    }
}
