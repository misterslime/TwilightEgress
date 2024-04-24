namespace Cascade.Content.Particles
{
    public class ManaInkParticle : Particle
    {
        private float Opacity;

        private readonly float BaseOpacity;

        private readonly Texture2D SmokeTexture;

        public ManaInkParticle(Vector2 position, Color color, float scale, float baseOpacity, int lifespan)
        {
            Position = position;
            Color = color;
            Scale = scale;
            BaseOpacity = baseOpacity;
            Lifetime = lifespan;

            Rotation = Main.rand.NextFloat(TwoPi);
            SmokeTexture = ModContent.Request<Texture2D>(CascadeTextureRegistry.Smokes[Main.rand.Next(CascadeTextureRegistry.Smokes.Count)]).Value;
        }

        public override string Texture => "Cascade/Assets/ExtraTextures/EmptyPixel";

        public override bool UseAdditiveBlend => true;

        public override bool UseCustomDraw => true;

        public override bool SetLifetime => true;

        public override void Update()
        {
            Rotation += Velocity.X * 0.004f;
            Velocity *= 0.98f;

            int fadeOutThreshold = Lifetime - 10;
            Opacity = Lerp(BaseOpacity, 0f, (Time - fadeOutThreshold) / 10f);
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Vector2 position = Position - Main.screenPosition;
            Vector2 origin = SmokeTexture.Size() / 2f;
            spriteBatch.Draw(SmokeTexture, position, SmokeTexture.Frame(), Color * Opacity, Rotation, origin, Scale / 12f, 0, 0f);
        }
    }
}
