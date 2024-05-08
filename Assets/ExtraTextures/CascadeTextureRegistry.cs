namespace Cascade.Assets.ExtraTextures
{
    public static class CascadeTextureRegistry
    {
        #region Objects
        public static readonly LazyAsset<Texture2D> EmptyPixel = MiscTexturesRegistry.InvisiblePixel;

        public static readonly Asset<Texture2D> SoftStar = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/GreyscaleObjects/SoftStar");

        public static readonly Asset<Texture2D> GreyscaleVortex = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/GreyscaleObjects/GreyscaleVortex");

        #region Lists
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

        #region Gradients
        public static readonly Asset<Texture2D> BlueCosmicGalaxy = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/Gradients/BlueCosmicGalaxy");

        public static readonly Asset<Texture2D> BlueCosmicGalaxyBlurred = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/Gradients/BlueCosmicGalaxyBlurred");

        public static readonly Asset<Texture2D> GrainyNoise = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/Gradients/GrainyNoise");

        public static readonly Asset<Texture2D> MeltyNoise = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/Gradients/MeltyNoise");

        public static readonly Asset<Texture2D> NeuronNebulaGalaxy = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/Gradients/NeuronNebulaGalaxy");

        public static readonly Asset<Texture2D> NeuronNebulaGalaxyBlurred = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/Gradients/NeuronNebulaGalaxy_Blurry");

        public static readonly Asset<Texture2D> PerlinNoise = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/Gradients/PerlinNoise");

        public static readonly Asset<Texture2D> PerlinNoise2 = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/Gradients/PerlinNoise2");

        public static readonly Asset<Texture2D> PerlinNoise3 = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/Gradients/PerlinNoise3");

        public static readonly Asset<Texture2D> PurpleBlueNebulaGalaxy = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/Gradients/PurpleBlueNebulaGalaxy");

        public static readonly Asset<Texture2D> PurpleBlueNebulaGalaxyBlurred = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/Gradients/PurpleBlueNebulaGalaxyBlurred");

        public static readonly Asset<Texture2D> SmudgyNoise = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/Gradients/SmudgyNoise");

        public static readonly Asset<Texture2D> StarryGalaxy = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/Gradients/StarryGalaxy");

        public static readonly Asset<Texture2D> SwirlyNoise = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/Gradients/SwirlyNoise");
        #endregion
    }
}
