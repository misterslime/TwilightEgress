using Cascade.Core.Graphics.GraphicalObjects.SkyEntitySystem;

namespace Cascade.Content.Skies.SkyEntities.TravellingAsteroid
{
    public class TravellingCometstoneAsteroidLarge : SkyEntity
    {
        public float RotationSpeed;

        public float RotationDirection;

        public TravellingCometstoneAsteroidLarge(Vector2 position, Vector2 velocity, float scale, float depth, float rotationSpeed, int lifespan)
        {
            Position = position;
            Velocity = velocity;
            Scale = scale;
            Depth = depth;
            RotationSpeed = rotationSpeed;
            Lifespan = lifespan;

            Opacity = 0f;
            Frame = Main.rand.Next(2);
            Rotation = Main.rand.NextFloat(TwoPi);
        }

        public override string TexturePath => "Cascade/Content/NPCs/CosmostoneShowers/Asteroids/CometstoneAsteroidLarge";

        public override int MaxFrames => 2;

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

            Rotation += RotationSpeed * Velocity.X * 0.03f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle frameRectangle = StoredTexture.Frame(1, MaxFrames, 0, Frame % 3);
            Vector2 mainOrigin = frameRectangle.Size() / 2f;

            Color color = Color.Lerp(Color.White, Color.Lerp(Main.ColorOfTheSkies, Color.Black, 0.3f + Depth / 10f), 0.15f + Depth / 10f) * Opacity;
            Main.EntitySpriteDraw(StoredTexture, GetDrawPositionBasedOnDepth(), frameRectangle, color, Rotation, mainOrigin, Scale / Depth, 0, 0f);
        }
    }
}
