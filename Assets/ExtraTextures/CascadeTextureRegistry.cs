namespace Cascade.Assets.ExtraTextures
{
    public static class CascadeTextureRegistry
    {
        public static readonly Asset<Texture2D> EmptyPixel = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/EmptyPixel");

        public static readonly Asset<Texture2D> GreyscaleSeemlessNoise = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/GreyscaleGradients/GreyscaleSeemlessNoise");

        public static readonly Asset<Texture2D> GreyscaleStar = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/GreyscaleObjects/StarNonPixelated");

        public static readonly Asset<Texture2D> VanillaStar = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/GreyscaleObjects/Star");

        public static readonly Asset<Texture2D> VanillaStarSmall = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/GreyscaleObjects/Star1");

        public static readonly Asset<Texture2D> LightningSegment = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/GreyscaleObjects/LightningSegment");

        public static readonly Asset<Texture2D> LightningHalfCircle = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/GreyscaleObjects/HalfCircle");

        public static readonly Asset<Texture2D> GreyscaleVortex = ModContent.Request<Texture2D>("Cascade/Assets/ExtraTextures/GreyscaleObjects/GreyscaleVortex");
    }
}
