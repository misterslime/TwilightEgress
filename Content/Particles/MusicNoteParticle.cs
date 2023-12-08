﻿namespace Cascade.Content.Particles
{
    public class MusicNoteParticle : Particle
    {
        private const int MusicNoteScaleChangeThreshold = 30;

        private const int BaseLifespan = 180;

        private Texture2D MusicNoteTexture;

        public override string Texture => Utilities.EmptyPixelPath;

        public override bool SetLifetime => true;

        public override bool UseCustomDraw => true;

        /// <param name="lifespan">The lifespan of this particle defaults to 180 ticks (3 seconds).
        /// Modifying this value will simply add to that value.</param>
        public MusicNoteParticle(Vector2 position, Vector2 velocity, int lifespan = 0)
        {
            Position = position;
            Velocity = velocity;
            Lifetime = BaseLifespan + lifespan;

            List<string> MusicNoteTexturePaths = new()
            {
                "Terraria/Images/Projectile_76",
                "Terraria/Images/Projectile_77",
                "Terraria/Images/Projectile_78",
            };

            MusicNoteTexture = ModContent.Request<Texture2D>(MusicNoteTexturePaths[Main.rand.Next(3)]).Value;

            Scale = 0f;
            Rotation = 0f;
        }

        public override void Update()
        {
            // Scale up a bit before scaling back down.
            if (Time is <= MusicNoteScaleChangeThreshold)
                Scale = Clamp(Scale + 0.04f, 0f, 1f);
            if (Time >= Lifetime - MusicNoteScaleChangeThreshold && Time <= Lifetime)
                Scale = Clamp(Scale - 0.04f, 0f, 1f);

            Velocity *= 0.98f;
            Rotation = Lerp(ToRadians(-15f), ToRadians(15f), SineInOutEasing(Time / 45f, 0));
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Vector2 drawPosition = Position - Main.screenPosition;
            spriteBatch.Draw(MusicNoteTexture, drawPosition, null, Color.White, Rotation, MusicNoteTexture.Size() / 2f, Scale, 0, 0f);
        }
    }
}