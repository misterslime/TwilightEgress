namespace Cascade.Content.Particles
{
    public abstract class ScreenParticle : Particle
    {
        /// <summary>
        /// The on-screen position a particle starts off at when it spawns. Usually set to <see cref="Main.screenPosition"/>.
        /// </summary>
        public Vector2 InitialScreenPosition;

        /// <summary>
        /// The strength of the parallax effect that is applied to a particle.
        /// </summary>
        public float ParallaxStrength;

        /// <summary>
        /// Whether or not you'd want to override this particle's draw method and manually draw it yourself.
        /// </summary>
        public virtual bool UseCustomScreenDrawing => false;

        /// <summary>
        /// Handles how the particle draws on-screen.
        /// </summary>
        /// <returns>The draw position of the particle.</returns>
        public Vector2 GetScreenDrawPosistion()
        {
            // Modify the position where the particle should be drawn by finding the difference between the current particle position and a lerp
            // between the screen position, the difference between the screen position and
            // the initial screen position when the particle is spawned, with the parallax strength acting as the lerp amount.
            Vector2 newScreenPosition = Position - Vector2.Lerp(Main.screenPosition, Main.screenPosition - 2 * (InitialScreenPosition - Main.screenPosition), ParallaxStrength);

            // Get the size of the screen and adjust it to the UI scale.
            Vector2 screenSize = new Vector2(Main.screenWidth, Main.screenHeight);
            Vector2 uiScreenSize = screenSize * Main.UIScale;

            // Adjust the new screen position to the UI screen size.
            while (newScreenPosition.X < 0)
                newScreenPosition.X += uiScreenSize.X;
            while (newScreenPosition.Y < 0)
                newScreenPosition.Y += uiScreenSize.Y;

            // Get the new drawing position, adjusting to the current screen size and player's current zoom level.
            Vector2 drawPosition = new Vector2(newScreenPosition.X % uiScreenSize.X, newScreenPosition.Y % uiScreenSize.Y) * Main.GameViewMatrix.Zoom;
            return drawPosition * 3f - ((screenSize * Main.GameViewMatrix.Zoom) - screenSize);
        }

        public sealed override void Draw(SpriteBatch spriteBatch)
        {
            // Redrawing the particles.
            if (UseCustomScreenDrawing)
                CustomScreenDrawing(spriteBatch);
            else
                spriteBatch.Draw(Texture, GetScreenDrawPosistion(), Frame, DrawColor * Opacity, Rotation, null, Scale, Direction.ToSpriteDirection());
        }


        /// <summary>
        /// Used for doing any sort of custom drawing. This should be overriden and used instead of Particle.CustomDrawing() if you do desire to do custom drawing.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void CustomScreenDrawing(SpriteBatch spriteBatch) { }
    }
}
