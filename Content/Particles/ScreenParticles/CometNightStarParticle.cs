namespace Cascade.Content.Particles.ScreenParticles
{
    public class CometNightStarParticle : ScreenParticle
    {
        private float Opacity;

        private float BaseScale;

        private Color BaseColor;

        private bool DynamicOpacityChange;

        public override string Texture => Utilities.ExtraTexturesDirectory + "/GreyscaleObjects/FourPointedStar_Small_2";

        public override bool UseAdditiveBlend => true;

        public CometNightStarParticle(Vector2 startingPosition, Vector2 initialScreenPosition, float parallaxStrength, Color color, float scale, int maxTime, bool dynamicOpacityChange = false)
        {
            Position = startingPosition;
            InitialScreenPosition = initialScreenPosition;
            ParallaxStrength = parallaxStrength;
            BaseColor = color;
            BaseScale = scale;
            Lifetime = maxTime;
            DynamicOpacityChange = dynamicOpacityChange;
        }

        public override void Update()
        {
            if (Time >= Lifetime)
            {
                Opacity -= 0.03f;
                if (Opacity <= 0f)
                {
                    Kill();
                }
            }
            if (DynamicOpacityChange)
            {
                Opacity = (float)Math.Sin((double)(Time * (float)Math.PI) / Lifetime);
            }
            else if (Time < Lifetime)
            {
                Opacity = Clamp(Opacity + 0.05f, 0f, 1f);
            }

            Color = BaseColor * Opacity;
            Scale = BaseScale;
            Velocity *= 0.9f;
        }
    }
}
