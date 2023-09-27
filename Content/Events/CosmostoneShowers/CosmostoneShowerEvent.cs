using CalamityMod.Events;
using Cascade.Content.Particles.ScreenParticles;
using Terraria.GameContent.Events;
using static Cascade.Core.Systems.WorldSavingSystem;

namespace Cascade.Content.Events.CosmostoneShowers
{
    public static class CosmostoneShowerEvent
    {
        public static void UpdateEvent()
        {
            StartAndStopEvent();
            if (!CosmostoneShower)
                return;

            ParticleVisuals();
        }

        public static void StartAndStopEvent()
        {
            bool shouldStopEvent = Main.bloodMoon || Main.pumpkinMoon || Main.snowMoon || BossRushEvent.BossRushActive;
            bool shouldIncreaseSpawnRate = LanternNight.NextNightIsLanternNight;
            if (Utilities.JustTurnedToNight && !shouldStopEvent && !CosmostoneShower && Main.rand.NextBool(shouldIncreaseSpawnRate ? 7 : 15))
            {
                Main.NewText("The night sky glimmers with cosmic energy...", Color.DeepSkyBlue);
                CosmostoneShower = true;
            }

            if ((Main.dayTime && CosmostoneShower) || shouldStopEvent)
                CosmostoneShower = false;
        }

        public static void StartAndStopLightCosmoShowers()
        {
            bool shouldIncreaseSpawnRate = LanternNight.NextNightIsLanternNight;
            if (Utilities.JustTurnedToNight && !CosmostoneShower && !LightCosmostoneShower && Main.rand.NextBool(shouldIncreaseSpawnRate ? 3 : 5))
                LightCosmostoneShower = true;

            if (Main.dayTime && LightCosmostoneShower)
                LightCosmostoneShower = false;
        }

        public static void ParticleVisuals()
        {
            // Particle spawning.
            Vector2 spawnPosition = new Vector2(Main.rand.NextFloat(Main.screenWidth), Main.rand.NextFloat(Main.screenHeight));
            Color color = Color.CornflowerBlue;
            float scale = Main.rand.NextFloat(0.05f, 2f);
            int lifetime = Main.rand.Next(120, 180);
            CometNightStarParticle starParticle = new(spawnPosition, Main.screenPosition, scale * 0.05f, color, scale, lifetime);

            if (Main.rand.NextBool(15) && Main.LocalPlayer.ZoneSkyHeight)
                GeneralParticleHandler.SpawnParticle(starParticle);
            if (Main.rand.NextBool(100) && Main.LocalPlayer.ZoneOverworldHeight)
                GeneralParticleHandler.SpawnParticle(starParticle);
        }
    }
}
