using System.Threading;

namespace Cascade.Content.Particles
{
    public class RoaringShockwaveParticle : Particle
    {
        private float Opacity;

        public override string Texture => "Cascade/Assets/ExtraTextures/GreyscaleObjects/DistortedShockwave2";

        public override bool SetLifetime => false;

        public override bool UseAdditiveBlend => true;

        public override bool UseCustomDraw => true;

        public RoaringShockwaveParticle(int lifespan, Vector2 position, Vector2 velocity, Color color, float scale, float rotation = 1f)
        {
            Lifetime = lifespan;
            Position = position;
            Velocity = velocity;
            Color = color;
            Scale = scale;
            Rotation = rotation;
            Opacity = 1f;
        }

        public override void Update()
        {
            Scale += 1.2f;
            Opacity = Lerp(1f, 0f, LifetimeCompletion);

            if (Opacity <= 0f) { Kill(); }
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * Opacity, Rotation, texture.Size() * 0.5f, Scale * 0.3f, 0, 0f);
        }
    }
}
