namespace Cascade
{
    public static partial class CascadeUtilities
    {
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

        public static Color HexToRBG(string hexColor)
        {
            int color = Convert.ToInt32(hexColor, 16);

            int r = (color & 0xff0000) >> 16;
            int g = (color & 0xff00) >> 8;
            int b = (color & 0xff);

            return new Color(r, g, b);
        }
    }
}
