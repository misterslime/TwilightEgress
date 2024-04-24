namespace Cascade.Content.Particles
{
    public class LightningArcParticle : CasParticle
    {
        private float LightningLengthFactor;

        private bool Initialized;

        private List<Vector2> LightningPoints;

        private Vector2 ActualEndPosition;

        public float PointDisplacementVariance { get; set; }

        public float JaggednessNumerator { get; set; }

        public bool UseSmoothening { get; set; }

        public bool AdditiveBlending { get; set; }

        public Vector2 EndPosition { get; set; }

        public override string AtlasTextureName => "Cascade.EmptyPixel.png";

        public LightningArcParticle(Vector2 basePosition, Vector2 endPosition, float pointDisplacementVariance, float jaggednessNumerator, float scale, Color color, int lifespan, bool useSmoothening = false, bool additiveBlending = true)
        {
            Position = basePosition;
            EndPosition = endPosition;
            PointDisplacementVariance = pointDisplacementVariance;
            JaggednessNumerator = jaggednessNumerator;
            Lifetime = lifespan;
            Scale = new(scale, scale);
            DrawColor = color;
            UseSmoothening = useSmoothening;
            AdditiveBlending = additiveBlending;
        }

        public override void Update()
        {
            if (!Initialized)
            {
                Initialized = true;
                LightningPoints = CascadeUtilities.CreateLightningBoltPoints(Position, EndPosition, PointDisplacementVariance, JaggednessNumerator);
            }

            LightningLengthFactor = Clamp(LightningLengthFactor + 0.15f, 0f, 1f);
            ActualEndPosition += (LightningLengthFactor * EndPosition.Length()).ToRotationVector2();
        }

        public float GetLightningWidth(float completionRatio) => Scale.X * Utils.GetLerpValue(1f, 0f, completionRatio, true) * Lerp(1f, 0f, LifetimeRatio);

        public Color GetLightningColor(float completionRatio) => DrawColor;

        public override void Draw(SpriteBatch spriteBatch)
        {
            /*spriteBatch.EnterShaderRegion(AdditiveBlending ? BlendState.Additive : BlendState.AlphaBlend);
            GameShaders.Misc["CalamityMod:HeavenlyGaleLightningArc"].UseImage1("Images/Misc/Perlin");
            GameShaders.Misc["CalamityMod:HeavenlyGaleLightningArc"].Apply();

            PrimitiveRenderer.RenderTrail(LightningPoints, new(GetLightningWidth, GetLightningColor), LightningPoints.Count * 2);
            spriteBatch.ExitShaderRegion();*/

            ManagedShader shader = ShaderManager.GetShader("Luminance.StandardPrimitiveShader");
            PrimitiveSettings primSettings = new(GetLightningWidth, GetLightningColor, Shader: shader);
            PrimitiveRenderer.RenderTrail(LightningPoints, primSettings, LightningPoints.Count * 2);
        }
    }
}
