using Cascade.Content.Skies;

namespace Cascade.Core.Systems
{
    public class SkyShaderLoadingSystem : ModSystem
    {
        public override void OnModLoad()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                // Ice Queen's screen shader.
                Filters.Scene["Cascade:IceQueen"] = new Filter(new IceQueenScreenShaderData("FilterMiniTower").UseColor(Color.SkyBlue).UseOpacity(0.75f), EffectPriority.VeryHigh);
                SkyManager.Instance["Cascade:IceQueen"] = new IceQueenSky();

                // Cosmostone Shower's screen shader.
                Filters.Scene["Cascade:CosmostoneShowers"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(Color.Lerp(default, Color.DeepSkyBlue, 0.4f)).UseOpacity(0.2f), EffectPriority.VeryHigh);
                SkyManager.Instance["Cascade:CosmostoneShowers"] = new CosmostoneShowersSky();
            }
        }
    }
}
