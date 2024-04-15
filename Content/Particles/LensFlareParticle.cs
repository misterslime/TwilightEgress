namespace Cascade.Content.Particles
{
    public class LensFlareParticle : Luminance.Core.Graphics.Particle
    {
        public override string AtlasTextureName => "Cascade.SoftStar";

        public LensFlareParticle(int lifespan, Vector2 position, Vector2 velocity, float scale, float rotation = 0f, float opacity = 1f, Color? color = null)
        {
            Lifetime = lifespan;
            Position = position;
            Velocity = velocity;
            Scale = new(scale);
            Rotation = rotation;
            Opacity = opacity;
            DrawColor = color ?? Color.White;
        }

        public override void Update()
        {
            Opacity -= 0.03f;
            Scale -= new Vector2(0.02f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D bloom = ModContent.Request<Texture2D>("CalamityMod/Skies/XerocLight").Value;

            Vector2 drawPosition = Position - Main.screenPosition;
            Rectangle flareRec = Texture.Frame;
            Rectangle bloomRec = bloom.Frame();

            Vector2 flareOrigin = Texture.Size / 2f;
            Vector2 bloomOrigin = bloom.Size() / 2f;

            spriteBatch.Draw(bloom, drawPosition, bloomRec, DrawColor * Opacity, Rotation, bloomOrigin, Scale * 0.75f, SpriteEffects.None, 0f);
            spriteBatch.Draw(Texture, drawPosition, flareRec, DrawColor * Opacity, Rotation, flareOrigin, Scale, Direction.ToSpriteDirection());
        }
    }
}
