namespace Cascade
{
    public static partial class CascadeUtilities
    {
        /// <summary>
        /// Coordinates for the center of the screen.
        /// </summary>
        public static Vector2 ScreenCenter => Main.screenPosition + new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);

        public static SpriteEffects DirectionBasedSpriteEffects(this Entity entity)
            => entity.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        /// <summary>
        /// Prepares a <see cref="RasterizerState"/> with screen culling enabled. This is mainly used for improving performance when drawing.
        /// </summary>
        /// <returns><see cref="Main.Rasterizer"/> with ScissorTestEnable enabled and a ScissorRectangle that covers the width and height
        /// of the entire screen."/></returns>
        public static RasterizerState PrepareScissorRectangleState()
        {
            Main.Rasterizer.ScissorTestEnable = true;
            Main.instance.GraphicsDevice.ScissorRectangle = new(-5, -5, Main.screenWidth + 10, Main.screenHeight + 10);
            return Main.Rasterizer;
        }
    }
}

