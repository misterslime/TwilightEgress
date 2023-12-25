namespace Cascade.Assets.ExtraTextures
{
    public static class CascadeTextureRegistry
    {
        #region Single Textures
        public static readonly Asset<Texture2D> EmptyPixel = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/EmptyPixel");

        public static readonly Asset<Texture2D> SoftStar = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/GreyscaleObjects/SoftStar");

        public static readonly Asset<Texture2D> GreyscaleVortex = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/GreyscaleObjects/GreyscaleVortex");
        #endregion

        #region Noise
        public static readonly Asset<Texture2D> GreyscaleSeemlessNoise = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/GreyscaleGradients/GreyscaleSeemlessNoise");
        #endregion

        #region Texture Paths/Special Cases
        public static readonly List<string> FourPointedStars = new()
        {
            "Cascade/Assets/ExtraTextures/GreyscaleObjects/FourPointedStar_Small",
            "Cascade/Assets/ExtraTextures/GreyscaleObjects/FourPointedStar_Small_2",
            "Cascade/Assets/ExtraTextures/GreyscaleObjects/FourPointedStar_Medium",
            "Cascade/Assets/ExtraTextures/GreyscaleObjects/FourPointedStar_Medium_2",
            "Cascade/Assets/ExtraTextures/GreyscaleObjects/FourPointedStar_Large",
            "Cascade/Assets/ExtraTextures/GreyscaleObjects/FourPointedStar_Large_2"
        };

        public static readonly List<string> Smokes = new()
        {
            "Cascade/Assets/ExtraTextures/GreyscaleObjects/Smoke",
            "Cascade/Assets/ExtraTextures/GreyscaleObjects/Smoke_2",
        };
        #endregion
    }
}
