using Cascade.Core.Graphics.GraphicalObjects.SkyEntitySystem;

namespace Cascade.Content.Skies.SkyEntities.StationaryAsteroids
{
    public class StationaryCosmostoneAsteroidLarge : SkyEntity
    {
        public float RotationSpeed;

        public float RotationDirection;

        public StationaryCosmostoneAsteroidLarge(Vector2 position, float scale, float depth, float rotationSpeed, int lifespan)
        {
            Position = position;
            Scale = scale;
            Depth = depth;
            RotationSpeed = rotationSpeed;
            Lifespan = lifespan;

            Opacity = 0f;
            Frame = 0;
            Rotation = Main.rand.NextFloat(PiOver2);
            RotationDirection = Main.rand.NextBool().ToDirectionInt();
        }

        public override string TexturePath => "Cascade/Content/NPCs/CosmostoneShowers/Asteroids/CosmostoneAsteroidLarge";

        public override int MaxFrames => 1;

        public override bool DieWithLifespan => true;

        public override BlendState BlendState => BlendState.AlphaBlend;

        public override void Update()
        {
            int timeToDisappear = Lifespan - 60;

            // Fade in and out.
            if (Time < timeToDisappear)
                Opacity = Clamp(Opacity + 0.1f, 0f, 1f);
            if (Time >= timeToDisappear && Time <= Lifespan)
                Opacity = Clamp(Opacity - 0.1f, 0f, 1f);

            Rotation += RotationSpeed * RotationDirection;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle frameRectangle = StoredTexture.Frame(1, MaxFrames, 0, Frame % MaxFrames);
            Vector2 mainOrigin = frameRectangle.Size() / 2f;

            Color color = Color.Lerp(Color.White, Color.Lerp(Main.ColorOfTheSkies, Color.Black, 0.3f + Depth / 15f), 0.15f + Depth / 15f) * Opacity;
            DrawCosmostone(spriteBatch, GetDrawPositionBasedOnDepth(), frameRectangle, color, Rotation, mainOrigin, Scale / Depth, 0, 0f);
        }

        public void DrawCosmostone(SpriteBatch spriteBatch, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float worthless = 0f)
        {
            Texture2D glowmask = ModContent.Request<Texture2D>("Cascade/Content/NPCs/CosmostoneShowers/Asteroids/CosmostoneAsteroidLarge_Glowmask").Value;

            Main.EntitySpriteDraw(StoredTexture, position, sourceRectangle, color, rotation, origin, scale, effects, worthless);

            Main.spriteBatch.PrepareForShaders();

            ManagedShader shader = ShaderManager.GetShader("Cascade.ManaPaletteShader");
            shader.TrySetParameter("flowCompactness", 3.0f);
            shader.TrySetParameter("gradientPrecision", 10f);
            shader.TrySetParameter("palette", CascadeUtilities.CosmostonePalette);
            shader.Apply();
            Main.spriteBatch.Draw(glowmask, position, sourceRectangle, color, rotation, origin, scale, effects, worthless);
            Main.spriteBatch.ResetToDefault();
        }
    }
}
