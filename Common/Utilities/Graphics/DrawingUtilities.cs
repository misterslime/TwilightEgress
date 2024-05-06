namespace Cascade
{
    public static partial class CascadeUtilities
    {
        /// <summary>
        /// Coordinates for the center of the screen.
        /// </summary>
        public static Vector2 ScreenCenter => Main.screenPosition + new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);

        /// <summary>
        /// Flips the sprite of an entity based on its <see cref="Entity.direction"/> field.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static SpriteEffects DirectionBasedSpriteEffects(this Entity entity)
            => entity.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        /// <summary>
        /// The palette used for mana flowing through Cosmostone Asteroids.
        /// </summary>
        public static readonly Vector4[] CosmostonePalette =
        {
            new Color(96, 188, 246).ToVector4(),
            new Color(81, 158, 245).ToVector4(),
            new Color(76, 131, 242).ToVector4(),
            new Color(3, 96, 243).ToVector4(),
            new Color(48, 65, 197).ToVector4(),
            new Color(104, 94, 228).ToVector4(),
            new Color(157, 113, 239).ToVector4(),
        };

        /// <summary>
        /// Converts a hexidecimal code into RGB values.
        /// </summary>
        /// <param name="hexCode">The hexidecimal code of the color you'd like to convert.</param>
        /// <returns>A <see cref="Color"/> instance with the correct RGB values from the hex code.</returns>
        public static Color HexToRGB(string hexCode)
        {
            int color = Convert.ToInt32(hexCode, 16);

            int r = (color & 0xff0000) >> 16;
            int g = (color & 0xff00) >> 8;
            int b = (color & 0xff);

            return new Color(r, g, b);
        }

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

