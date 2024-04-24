using Cascade.Common.Systems.PlanetoidSystem;

namespace Cascade.Assets.ExtraTextures
{
    public static class CascadeTextureRegistry
    {
        #region Objects
        public static readonly LazyAsset<Texture2D> EmptyPixel = MiscTexturesRegistry.InvisiblePixel;

        public static readonly Asset<Texture2D> SoftStar = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/GreyscaleObjects/SoftStar");

        public static readonly Asset<Texture2D> GreyscaleVortex = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/GreyscaleObjects/GreyscaleVortex");

        #region Lists
        public static Asset<Texture2D>[] Planetoids = new Asset<Texture2D>[PlanetoidSystem.planetoidsByType.Count];

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
            "Cascade/Assets/ExtraTextures/GreyscaleObjects/SmokeCloud",
            "Cascade/Assets/ExtraTextures/GreyscaleObjects/SmokeCloud2",
            "Cascade/Assets/ExtraTextures/GreyscaleObjects/SmokeCloud3",
            "Cascade/Assets/ExtraTextures/GreyscaleObjects/SmokeCloud4",
            "Cascade/Assets/ExtraTextures/GreyscaleObjects/SmokeCloud5",
            "Cascade/Assets/ExtraTextures/GreyscaleObjects/SmokeCloud6"
        };
        #endregion
        #endregion

        #region Noise
        public static readonly Asset<Texture2D> GreyscaleSeemlessNoise = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/GreyscaleGradients/GreyscaleSeemlessNoise");
        #endregion
    }
}
