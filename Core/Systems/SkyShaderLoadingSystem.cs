﻿using TwilightEgress.Content.Skies;

namespace TwilightEgress.Core.Systems
{
    public class SkyShaderLoadingSystem : ModSystem
    {
        public override void OnModLoad()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                // Ice Queen's screen shader.
                Filters.Scene["TwilightEgress:IceQueen"] = new Filter(new IceQueenScreenShaderData("FilterMiniTower").UseColor(Color.SkyBlue).UseOpacity(0.75f), EffectPriority.VeryHigh);
                SkyManager.Instance["TwilightEgress:IceQueen"] = new IceQueenSky();

                // Cosmostone Shower's screen shader.
                Filters.Scene["TwilightEgress:CosmostoneShowers"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(Color.Lerp(default, Color.DeepSkyBlue, 0.4f)).UseOpacity(0.2f), EffectPriority.VeryHigh);
                SkyManager.Instance["TwilightEgress:CosmostoneShowers"] = new CosmostoneShowersSky();
            }
        }
    }
}
