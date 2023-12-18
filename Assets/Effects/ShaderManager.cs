using Cascade.Content.Skies;

namespace Cascade.Assets.Effects
{
    public class ShaderManager : ModSystem
    {
        public static Dictionary<string, MiscShaderData> Shaders;

        public static Dictionary<string, Filter> ScreenShaders;

        #region Texture Shaders
        private static MiscShaderData IceQueenScrollingBackgroundShader => GameShaders.Misc["Cascade:IceQueenScrollingBackgroundShader"];

        private static MiscShaderData NoisyVignetteShader => GameShaders.Misc["Cascade:NoisyVignette"];

        #endregion

        #region Screen Shaders
        private static Filter EllipticalVignetteShader => Filters.Scene["Cascade:EllipticalVignette"];

        private static Filter ChromaticAbberationShader => Filters.Scene["Cascade:ChromaticAbberation"];

        private static Filter BlackHoleShader => Filters.Scene["Cascade:BlackHole"];
        #endregion

        public override void OnModLoad()
        {
            AssetRepository assetRepo = Cascade.Instance.Assets;
            if (Main.netMode != NetmodeID.Server)
            {
                LoadRegularShaders(assetRepo);
                LoadAllScreenShaders(assetRepo);
            }

            /* Keeping shaders registered in the dictionaries creates a simple way of 
             * accessing them without things looking messy, like how this system worked prior.
             */
       
            Shaders = new()
            {
                {
                    "IceQueenSkyShader",
                    IceQueenScrollingBackgroundShader
                },

                {
                    "NoisyVignetteShader",
                    NoisyVignetteShader
                }
            };

            ScreenShaders = new()
            {
                {
                    "EllipticalVignetteShader",
                    EllipticalVignetteShader
                },

                {
                    "ChromaticAbberationShader",
                    ChromaticAbberationShader
                },

                {
                    "BlackHoleShader",
                    BlackHoleShader
                }
            };
        }

        public override void OnModUnload()
        {
            Shaders = null;
            ScreenShaders = null;
        }

        private void LoadRegularShaders(AssetRepository assetRepo)
        {
            // Ice Queen Sky.
            Ref<Effect> iceQueenScrollingBackgroundShader = new Ref<Effect>(assetRepo.Request<Effect>("Assets/Effects/SkyEffects/IceQueenScrollingBackgroundShader", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc["Cascade:IceQueenScrollingBackgroundShader"] = new MiscShaderData(iceQueenScrollingBackgroundShader, "ScrollPass");

            // Noisy Vignette.
            Ref<Effect> noisyVignetteShader = new(assetRepo.Request<Effect>("Assets/Effects/Overlays/NoisyVignette", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc["Cascade:NoisyVignette"] = new(noisyVignetteShader, "NoisyVignettePass");

        }

        private void LoadAllScreenShaders(AssetRepository assetRepo)
        {
            // Elliptical vignette.
            Ref<Effect> ellipticalVignetteShader = new(assetRepo.Request<Effect>("Assets/Effects/Overlays/EllipticalVignette", AssetRequestMode.ImmediateLoad).Value);
            Filters.Scene["Cascade:EllipticalVignette"] = new(new(ellipticalVignetteShader, "VignettePass"), EffectPriority.VeryHigh);

            // Chromatic abberation.
            Ref<Effect> chromaticAbberationShader = new(assetRepo.Request<Effect>("Assets/Effects/Overlays/ChromaticAbberation", AssetRequestMode.ImmediateLoad).Value);
            Filters.Scene["Cascade:ChromaticAbberation"] = new(new(chromaticAbberationShader, "ChromaAbberationPass"), EffectPriority.VeryHigh);

            // Black hole.
            Ref<Effect> blackHoleShader = new(assetRepo.Request<Effect>("Assets/Effects/Shapes/BlackHole", AssetRequestMode.ImmediateLoad).Value);
            Filters.Scene["Cascade:BlackHole"] = new(new(blackHoleShader, "BlackHolePass"), EffectPriority.VeryHigh);

            // Ice Queen's screen shader; currently unused.
            Filters.Scene["Cascade:IceQueen"] = new Filter(new IceQueenScreenShaderData("FilterMiniTower").UseColor(Color.SkyBlue).UseOpacity(0.75f), EffectPriority.VeryHigh);
            SkyManager.Instance["Cascade:IceQueen"] = new IceQueenSky();

            // Cosmostone Shower's screen shader.
            Filters.Scene["Cascade:CosmostoneShowers"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(Color.Lerp(default, Color.DeepSkyBlue, 0.4f)).UseOpacity(0.2f), EffectPriority.VeryHigh);
            SkyManager.Instance["Cascade:CosmostoneShowers"] = new CosmostoneShowersSky();
        }
    }
}
