using Cascade.Core.Systems.SkyEntitySystem;

namespace Cascade.Content.Skies.SkyEntities
{
    public class StardustPillar : SkyEntity
    {
        public StardustPillar(Vector2 position, Color color, float scale, float depth, int lifespan)
        {
            Position = position;
            Color = color;
            Scale = scale;
            Depth = depth;
            Lifespan = lifespan;
            Opacity = 0f;
        }

        public override void Update()
        {
            if (Time >= Lifespan - 75)
                Opacity = Clamp(Opacity - 0.01f, 0f, 1f);
            else
                Opacity = Clamp(Opacity + 0.005f, 0f, 1f);
        }

        public override void Draw(SpriteBatch spriteBatch, float intensity)
        {
            Texture2D mainTexture = TextureAssets.Npc[493].Value;
            Texture2D bloomTexture = ModContent.Request<Texture2D>("Cascade/Content/Skies/SkyEntities/StardustPillar_Glow").Value;

            // Change the position depending on the depth.
            Vector2 screenBounds = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
            Vector2 depthRelation = new Vector2(1f / Depth, 1f / Depth);
            Vector2 drawPosition = (Position - screenBounds) * depthRelation + screenBounds - Main.screenPosition;

            Vector2 mainOrigin = mainTexture.Size() / 2f;
            Vector2 bloomOrigin = bloomTexture.Size() / 2f;

            Color mainColor = Color.Lerp(Color.White, Color.Black, 0.75f) * Opacity * intensity;
            Color bloomColor = Color * (2f * Scale) * Opacity * intensity;

            spriteBatch.SetBlendState(BlendState.Additive);
            spriteBatch.Draw(bloomTexture, drawPosition, null, bloomColor, Rotation, bloomOrigin, Scale * 1.075f, SpriteEffects.None, 0f);
            spriteBatch.SetBlendState(BlendState.AlphaBlend);

            spriteBatch.Draw(mainTexture, drawPosition, null, mainColor, Rotation, mainOrigin, Scale, SpriteEffects.None, 0f);
        }
    }
}
