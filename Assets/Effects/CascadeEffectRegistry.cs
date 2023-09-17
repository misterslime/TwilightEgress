using Cascade.Content.Skies;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace Cascade.Assets.Effects
{
    public static class CascadeEffectRegistry
    {
        public static MiscShaderData IceQueenScrollingBackgroundShader => GameShaders.Misc["CalamityReworks:IceQueenScrollingBackgroundShader"];

        public static void LoadAllShaders()
        {
            AssetRepository assetRepo = ModContent.GetInstance<Cascade>().Assets;

            LoadRegularShaders(assetRepo);
            LoadAllScreenShaders(assetRepo);
        }

        private static void LoadRegularShaders(AssetRepository assetRepo)
        {
            Ref<Effect> iceQueenScrollingBackgroundShader = new Ref<Effect>(assetRepo.Request<Effect>("Assets/Effects/IceQueenScrollingBackgroundShader", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc["CalamityReworks:IceQueenScrollingBackgroundShader"] = new MiscShaderData(iceQueenScrollingBackgroundShader, "ScrollPass");
        }

        private static void LoadAllScreenShaders(AssetRepository assetRepo)
        {
            Filters.Scene["CalamityReworks:IceQueen"] = new Filter(new IceQueenScreenShaderData("FilterMiniTower").UseColor(Color.SkyBlue).UseOpacity(0.75f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityReworks:IceQueen"] = new IceQueenSky();

            Filters.Scene["CalamityReworks:CometNight"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(Color.Cyan).UseOpacity(0.35f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityReworks:CometNight"] = new CometNightSky();
        }
    }
}
