namespace Cascade.Content.Particles
{
    public class AmbientLightBallParticle : CasParticle
    {
        public float InitialOpacity;

        public float MaxOpacity;

        public override string AtlasTextureName => "Cascade.LightBall.png";

        public override BlendState BlendState => BlendState.Additive;

        public AmbientLightBallParticle(Vector2 position, Vector2 velocity, float scale, float initialOpacity, float maxOpacity, float parallaxStrength, int lifetime, Color? drawColor = null)
        {
            Position = position;
            Velocity = velocity;
            Scale = new(scale);
            InitialOpacity = initialOpacity;
            MaxOpacity = maxOpacity;
            ParallaxStrength = parallaxStrength;
            Lifetime = lifetime;
            DrawColor = drawColor ?? Color.White;
        }

        public override void Update()
        {
            int fadeInThreshold = 30;
            int fadeOutThreshold = Lifetime - 30;

            if (Time <= fadeInThreshold)
                Opacity = Clamp(Opacity + 0.1f, InitialOpacity, MaxOpacity);
            if (Time >= fadeOutThreshold && Time <= Lifetime)
                Opacity = Clamp(Opacity - 0.1f, InitialOpacity, MaxOpacity);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            AtlasTexture bloomTexture = AtlasManager.GetTexture("Cascade.BloomCircle.png");
            spriteBatch.Draw(bloomTexture, GetDrawPositionWithParallax(), Frame, DrawColor * Opacity, Rotation, scale: Scale * (ParallaxStrength / 2f) / 3f);
            spriteBatch.Draw(Texture, GetDrawPositionWithParallax(), Frame, DrawColor * Opacity, Rotation, scale: Scale * (ParallaxStrength / 2f));
        }
    }
}
