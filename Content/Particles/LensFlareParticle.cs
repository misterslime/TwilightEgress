using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Cascade.Content.Particles
{
    public class LensFlareParticle : Particle
    {

        private float Opacity;

        public override string Texture => "Cascade/Assets/ExtraTextures/GreyscaleObjects/StarNonPixelated";

        public override bool SetLifetime => true;

        public override bool UseAdditiveBlend => true;

        public override bool UseCustomDraw => true;

        public LensFlareParticle(int lifespan, Vector2 position, Vector2 velocity, float scale, float rotation = 0f, float opacity = 1f, Color? color = null)
        {
            Lifetime = lifespan;
            Position = position;
            Velocity = velocity;
            Scale = scale;
            Rotation = rotation;
            Opacity = opacity;
            Color = color ?? Color.White;
        }

        public override void Update()
        {
            Opacity -= 0.03f;
            Scale -= 0.02f;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D texture = GeneralParticleHandler.GetTexture(Type);
            Texture2D bloom = ModContent.Request<Texture2D>("CalamityMod/Skies/XerocLight").Value;

            Vector2 drawPosition = Position - Main.screenPosition;
            Rectangle flareRec = texture.Frame();
            Rectangle bloomRec = bloom.Frame();

            Vector2 flareOrigin = texture.Size() / 2f;
            Vector2 bloomOrigin = bloom.Size() / 2f;

            spriteBatch.Draw(bloom, drawPosition, bloomRec, Color * Opacity, Rotation, bloomOrigin, Scale * 0.75f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition, flareRec, Color * Opacity, Rotation, flareOrigin, Scale, SpriteEffects.None, 0f);
        }
    }
}
