using Cascade.Core.Graphics.GraphicalObjects.SkyEntitySystem;

namespace Cascade.Content.Skies.SkyEntities
{
    public class StationaryAsteroid : SkyEntity
    {
        public float RotationSpeed;

        public float RotationDirection;

        public StationaryAsteroid(Vector2 position, float scale, float depth, float rotationSpeed, int lifespan)
        {
            Position = position;
            Scale = scale;
            Depth = depth;
            RotationSpeed = rotationSpeed;
            Lifespan = lifespan;

            Opacity = 0f;
            Frame = Main.rand.Next(3);
            Rotation = Main.rand.NextFloat(TwoPi);
            RotationDirection = Main.rand.NextBool().ToDirectionInt();
        }

        public override string TexturePath => "Cascade/Content/Projectiles/Ambient/Comet";

        public override int MaxFrames => 3;

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
            Rectangle frameRectangle = StoredTexture.Frame(1, MaxFrames, 0, Frame % 3);
            Vector2 mainOrigin = frameRectangle.Size() / 2f;

            Color color = Color.Lerp(Color.White, Color.Lerp(Main.ColorOfTheSkies, Color.Black, 0.3f + (Depth / 15f)), 0.15f + (Depth / 15f)) * Opacity;
            DrawCosmostone(spriteBatch, GetDrawPositionBasedOnDepth(), frameRectangle, color, Rotation, mainOrigin, Scale / Depth, 0, 0f);
        }

        public void DrawCosmostone(SpriteBatch spriteBatch, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float worthless = 0f)
        {
            Texture2D glowmask = CascadeTextureRegistry.CometGlowmask.Value;

            Main.EntitySpriteDraw(StoredTexture, position, sourceRectangle, color, rotation, origin, scale, effects, worthless);

            Main.spriteBatch.PrepareForShaders();

            ManagedShader shader = ShaderManager.GetShader("Cascade.ManaPaletteShader");
            shader.TrySetParameter("globalTime", Main.GlobalTimeWrappedHourly);
            shader.TrySetParameter("flowCompactness", 2.0f);
            shader.TrySetParameter("gradientPrecision", 10f);
            shader.TrySetParameter("palette", new Vector4[] 
            {
                new Color(100, 216, 253).ToVector4(),
                new Color(1, 158, 252).ToVector4(),
                new Color(101, 91, 126).ToVector4(),
                new Color(1, 81, 252).ToVector4(),
                new Color(24, 10, 230).ToVector4(),
                new Color(101, 91, 126).ToVector4(),
                new Color(116, 55, 234).ToVector4(),
                new Color(199, 47, 228).ToVector4(),
                new Color(101, 91, 126).ToVector4(),
            });
            shader.Apply();
            Main.spriteBatch.Draw(glowmask, position, sourceRectangle, color, rotation, origin, scale, effects, worthless);
            Main.spriteBatch.ResetToDefault();
        }
    }
}
