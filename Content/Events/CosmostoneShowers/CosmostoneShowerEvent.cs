﻿using CalamityMod.Events;
using CalamityMod.NPCs.NormalNPCs;
using Cascade.Content.NPCs.CosmostoneShowers.Asteroids;
using Cascade.Content.NPCs.CosmostoneShowers.Manaphages;
using Cascade.Content.NPCs.CosmostoneShowers.Planetoids;
using Cascade.Content.Projectiles;
using Cascade.Content.Skies.SkyEntities;
using Cascade.Content.Skies.SkyEntities.StationaryAsteroids;
using Cascade.Content.Skies.SkyEntities.TravellingAsteroid;
using Cascade.Core.Graphics.GraphicalObjects.SkyEntitySystem;
using Terraria.GameContent.Events;
using Terraria.Graphics;

namespace Cascade.Content.Events.CosmostoneShowers
{
    public class CosmostoneShowerEvent : EventHandler
    {
        private int ShiningStarSpawnChance
        {
            get
            {
                if (!EventIsActive)
                    return 0;
                return 12 * (int)Round(Lerp(1f, 0.4f, Star.starfallBoost / 3f), 0);
            }
        }

        private int TravellingAsteroidSpawnChance
        {
            get
            {
                if (!EventIsActive)
                    return 0;
                return 70 * (int)Round(Lerp(1f, 0.6f, Star.starfallBoost / 3f), 0);
            }
        }

        private int StationaryAsteroidSpawnChance
        {
            get
            {
                if (!EventIsActive)
                    return 0;
                return 55;
            }
        }

        private int SiriusSpawnChance
        {
            get
            {
                if (!EventIsActive)
                    return 0;

                int spawnChance = Main.tenthAnniversaryWorld ? 10000 : LanternNight.LanternsUp ? 50000 : 100000;
                return spawnChance * (int)Round(Lerp(1f, 0.4f, Star.starfallBoost / 3f));
            }
        }

        private Color ShiningStarColors
        {
            get
            {
                // Blues, reds and purples.
                Color firstColor = Utils.SelectRandom(Main.rand, 
                    Color.SkyBlue, Color.AliceBlue, Color.DeepSkyBlue, 
                    Color.Purple, Color.MediumPurple, Color.BlueViolet, 
                    Color.Violet, Color.PaleVioletRed, Color.MediumVioletRed);

                // Yellows, oranges and whites.
                Color secondColor = Utils.SelectRandom(Main.rand, 
                    Color.Yellow, Color.Goldenrod, Color.LightGoldenrodYellow, Color.LightYellow, 
                    Color.Orange, Color.OrangeRed, Color.White, Color.FloralWhite, Color.NavajoWhite);

                return Color.Lerp(firstColor, secondColor, Main.rand.NextFloat(0.1f, 1f));
            }
        }

        private const int MaxShiningStars = 500;

        private const int MaxTravellingAsteroids = 100;

        private const int MaxStationaryAsteroids = 25;

        public override bool PersistAfterLeavingWorld => true;

        public override bool PreUpdateEvent()
        {
            bool shouldStopEvent = Main.bloodMoon || Main.pumpkinMoon || Main.snowMoon || BossRushEvent.BossRushActive;
            bool shouldIncreaseSpawnRate = LanternNight.NextNightIsLanternNight;

            // Start and stop the event.
            if (CascadeUtilities.JustTurnedToNight && !shouldStopEvent && !EventIsActive && Main.rand.NextBool(shouldIncreaseSpawnRate ? 7 : 15))
            {
                Main.NewText("A mana-rich asteroid belt is travelling past the planet...", Color.DeepSkyBlue);
                EventHandlerManager.StartEvent<CosmostoneShowerEvent>();
            }

            if ((Main.dayTime && EventIsActive) || shouldStopEvent)
                EventHandlerManager.StopEvent<CosmostoneShowerEvent>();

            return true;
        }

        public override void UpdateEvent()
        {        
            // Important entities.
            Entities_SpawnSpecialSpaceNPCs();
            
            // Visual objects.
            Visuals_SpawnAmbientSkyEntities();
        }

        public override void EditEventSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (!spawnInfo.Player.ZoneCosmostoneShowers() || spawnInfo.Invasion)
                return;

            // Space additions.
            if (spawnInfo.Sky)
            {
                pool.Clear();
                pool.Add(ModContent.NPCType<Manaphage>(), 0.56f);
            }

            // Surface additions.
            if (spawnInfo.Player.ZoneOverworldHeight)
            {
                pool.Add(NPCID.Firefly, 0.85f);
                pool.Add(NPCID.EnchantedNightcrawler, 0.75f);
            }

            // FUCK YOU
            if (pool.ContainsKey(ModContent.NPCType<ShockstormShuttle>()))
                pool.Remove(ModContent.NPCType<ShockstormShuttle>());
        }

        // TODO:
        // Overall this entire spawning method once the Planetoid and Asteroid reworks are finished.
        // -fryzahh
        private void Entities_SpawnSpecialSpaceNPCs()
        {
            int asteroidSpawnChance = 125;
            int planetoidSpawnChance = 500;

            List<NPC> activePlanetoids = Cascade.BasePlanetoidInheriters.Where(p => p.active).ToList();
            List<NPC> activePlanetoidsOnScreen = new();

            // Get all active planetoids that are on-screen.
            foreach (NPC planetoid in activePlanetoids)
            {
                Rectangle planetoidBounds = new((int)planetoid.Center.X, (int)planetoid.Center.Y, (int)planetoid.localAI[0], (int)planetoid.localAI[1]);
                Rectangle screenBounds = new((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth + 100, Main.screenHeight + 100);
                if (planetoidBounds.Intersects(screenBounds))
                    activePlanetoidsOnScreen.Add(planetoid);
            }

            float xWorldPosition = ((Main.maxTilesX - 50) + 100) * 16f;
            float yWorldPosition = Main.maxTilesY * 0.057f;
            Vector2 playerPositionInBounds = new(xWorldPosition, yWorldPosition);

            int closestPlayerIndex = Player.FindClosest(playerPositionInBounds, 1, 1);
            Player closestPlayer = Main.player[closestPlayerIndex];

            // Asteroids.
            if (closestPlayer.active && !closestPlayer.dead && closestPlayer.Center.Y <= Main.maxTilesY + 1000f && Main.rand.NextBool(asteroidSpawnChance))
            {
                // Default spawn position.
                Vector2 asteroidSpawnPosition = closestPlayer.Center + Main.rand.NextVector2CircularEdge(Main.rand.NextFloat(1250f, 250f), Main.rand.NextFloat(600f, 200f));

                float cosmostoneChance = 1f;
                float cometstoneChance = 2f;
                float smallAsteroidChance = 4f;
                float mediumAsteroidChance = 2f;
                float largeAsteroidChance = 1f;

                Dictionary<int, float> asteroids = new Dictionary<int, float>
                {
                    { ModContent.NPCType<CosmostoneAsteroidSmall>(), cosmostoneChance * smallAsteroidChance },
                    { ModContent.NPCType<CosmostoneAsteroidMedium>(), cosmostoneChance * mediumAsteroidChance },
                    { ModContent.NPCType<CosmostoneAsteroidLarge>(), cosmostoneChance * largeAsteroidChance },
                    { ModContent.NPCType<CosmostoneGeode>(), cosmostoneChance * smallAsteroidChance * 0.5f },
                    { ModContent.NPCType<SilicateAsteroidSmall>(), cometstoneChance * smallAsteroidChance },
                    { ModContent.NPCType<SilicateAsteroidMedium>(), cometstoneChance * mediumAsteroidChance },
                    { ModContent.NPCType<SilicateAsteroidLarge>(), cometstoneChance * largeAsteroidChance }
                };

                // Search for any active Planetoids currently viewable on-screen.
                // Change the spawn position of asteroids to a radius around the center of these Planetoids if there are any active at the time.
                // This allows most asteroids to not just spawn directly inside of Planetoids or their radius (may be buggy if there are 
                // multiple Planetoids close to each other).
                foreach (NPC planetoid in activePlanetoidsOnScreen)
                {
                    float radiusAroundPlanetoid = planetoid.localAI[0] + planetoid.localAI[1] + Main.rand.NextFloat(1000f, 200f);
                    Vector2 planetoidPositionWithRadius = planetoid.Center + Vector2.UnitX.RotatedByRandom(Tau) * radiusAroundPlanetoid;
                    asteroidSpawnPosition = planetoidPositionWithRadius;
                }

                if (CascadeUtilities.ObligatoryNetmodeCheckForSpawningEntities() && !Collision.SolidCollision(asteroidSpawnPosition, 300, 300))
                {
                    int p = Projectile.NewProjectile(new EntitySource_WorldEvent(), asteroidSpawnPosition, Vector2.Zero, ModContent.ProjectileType<NPCSpawner>(), 0, 0f, Main.myPlayer, asteroids.RandomElementByWeight(e => e.Value).Key);
                    if (Main.projectile.IndexInRange(p))
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p);
                }
            }

            // Planetoids.
            if (closestPlayer.active && !closestPlayer.dead && closestPlayer.Center.Y <= Main.maxTilesY + 300f && closestPlayer.Center.Y >= Main.maxTilesY * 0.5f && Main.rand.NextBool(planetoidSpawnChance))
            {
                Vector2 planetoidSpawnPosition = closestPlayer.Center + Main.rand.NextVector2CircularEdge(Main.rand.NextFloat(2500f, 1500f), 600f);
                if (activePlanetoidsOnScreen.Count > 0)
                {
                    NPC otherPlanetoid = activePlanetoids.LastOrDefault();
                    if (otherPlanetoid.active)
                    {
                        float radiusAroundPlanetoid = otherPlanetoid.localAI[0] + otherPlanetoid.localAI[1] + Main.rand.NextFloat(2000f, 750f);
                        Vector2 planetoidPositionWithRadius = otherPlanetoid.Center + Vector2.UnitX.RotatedByRandom(Tau) * radiusAroundPlanetoid;
                        planetoidSpawnPosition = planetoidPositionWithRadius;
                    }
                }
                
                if (CascadeUtilities.ObligatoryNetmodeCheckForSpawningEntities() && !Collision.SolidCollision(planetoidSpawnPosition, 1600, 1600) && activePlanetoids.Count < 10)
                {
                    int p = Projectile.NewProjectile(new EntitySource_WorldEvent(), planetoidSpawnPosition, Vector2.Zero, ModContent.ProjectileType<NPCSpawner>(), 0, 0f, Main.myPlayer, ModContent.NPCType<GalileoPlanetoid>());
                    if (Main.projectile.IndexInRange(p))
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p);
                }
            }
        }

        private void Visuals_SpawnAmbientSkyEntities()
        {
            Color[] starColours = [Color.Violet, Color.DeepSkyBlue, Color.CornflowerBlue, Color.White, Color.Yellow, Color.Orange, Color.Red];

            int particles = Main.rand.Next(3) + 1;
            for (int i = 0; i < particles; i++)
            {
                Vector2 starSpawnPos = Main.LocalPlayer.Center + Main.rand.NextVector2Circular(Main.screenWidth, Main.screenHeight);
                Vector2 starVelocity = Vector2.One.RotatedByRandom(Tau) * Main.rand.NextFloat(-0.2f, 0.2f);
                float starScale = Main.rand.NextFloat(0.10f, 0.20f) * 2f;
                float parallaxStrength = Main.rand.NextFloat(1f, 5f);
                int starLifetime = Main.rand.Next(120, 180) * 2;

                //Color[] starColours = [Color.Violet, Color.DeepSkyBlue, Color.CornflowerBlue, Color.White, Color.Yellow, Color.Orange, Color.Red];
                Color starColor = CascadeUtilities.InterpolateColor(starColours, Main.rand.NextFloat());

                new AmbientStarParticle(starSpawnPos, starVelocity, starScale, 0f, 1f, parallaxStrength, starLifetime, starColor).SpawnCasParticle();
            }

            int totalStarLayers = 7;
            int totalAsteroidsLayers = 5;
            VirtualCamera virtualCamera = new(Main.LocalPlayer);

            // Shining Stars.
            if (SkyEntityManager.CountActiveSkyEntities<ShiningStar>() < MaxShiningStars && Main.rand.NextBool(ShiningStarSpawnChance))
            {
                for (int i = 0; i < totalStarLayers; i++)
                {
                    float x = virtualCamera.Center.X + Main.rand.NextFloat(-virtualCamera.Size.X, virtualCamera.Size.X) * 2f;
                    float y = (float)(Main.worldSurface * 16f) * Main.rand.NextFloat(-0.01f, 0.6f);
                    Vector2 position = new(x, y);

                    float maxScale = Main.rand.NextFloat(1f, 10f);
                    int lifespan = Main.rand.Next(120, 200);

                    float xStrectch = Main.rand.NextFloat(0.5f, 1.5f);
                    float yStretch = Main.rand.NextFloat(0.5f, 1.5f);

                    float depth = Main.rand.NextFloat() * 50f;

                    Color starColor = CascadeUtilities.InterpolateColor(starColours, Main.rand.NextFloat());

                    if (depth > 1f)
                        new ShiningStar(position, starColor, maxScale, depth, new Vector2(xStrectch, yStretch), lifespan).Spawn();
                    else
                        new ShiningStarParticle(position, starColor, maxScale, Main.rand.NextFloat(1f, 5f), new Vector2(xStrectch, yStretch), lifespan).SpawnCasParticle();
                }
            }

            // Weights for asteroid spawning
            float cosmostoneChance = 1f;
            float cometstoneChance = 2f;
            float smallAsteroidChance = 4f;
            float mediumAsteroidChance = 2f;
            float largeAsteroidChance = 1f;

            Dictionary<int, float> asteroids = new Dictionary<int, float>
            {
                { 0, cosmostoneChance * smallAsteroidChance },
                { 1, cosmostoneChance * mediumAsteroidChance },
                { 2, cosmostoneChance * largeAsteroidChance },
                { 3, cosmostoneChance * smallAsteroidChance * 0.5f },
                { 4, cometstoneChance * smallAsteroidChance },
                { 5, cometstoneChance * mediumAsteroidChance },
                { 6, cometstoneChance * largeAsteroidChance }
            };

            // Horizontally-travelling Asteroids.
            int travellingAsteroids = 0;

            travellingAsteroids += SkyEntityManager.CountActiveSkyEntities<TravellingCosmostoneAsteroidSmall>();
            travellingAsteroids += SkyEntityManager.CountActiveSkyEntities<TravellingCosmostoneAsteroidMedium>();
            travellingAsteroids += SkyEntityManager.CountActiveSkyEntities<TravellingCosmostoneAsteroidLarge>();
            travellingAsteroids += SkyEntityManager.CountActiveSkyEntities<TravellingCosmostoneGeode>();
            travellingAsteroids += SkyEntityManager.CountActiveSkyEntities<TravellingSilicateAsteroidSmall>();
            travellingAsteroids += SkyEntityManager.CountActiveSkyEntities<TravellingSilicateAsteroidMedium>();
            travellingAsteroids += SkyEntityManager.CountActiveSkyEntities<TravellingSilicateAsteroidLarge>();

            if (travellingAsteroids < MaxTravellingAsteroids && Main.rand.NextBool(TravellingAsteroidSpawnChance))
            {
                for (int i = 0; i < totalAsteroidsLayers; i++)
                {
                    float x = virtualCamera.Center.X - virtualCamera.Size.X - 1280f;
                    float y = (float)(Main.worldSurface * 16f) * Main.rand.NextFloat(-0.2f, 0.225f);
                    Vector2 position = new(x, y);

                    float speed = Main.rand.NextFloat(3f, 15f);
                    Vector2 velocity = Vector2.UnitX * speed;

                    float maxScale = Main.rand.NextFloat(0.5f, 2f);
                    float depth = i + 3f;
                    int lifespan = Main.rand.Next(1200, 1800);

                    int asteroid = asteroids.RandomElementByWeight(e => e.Value).Key;

                    switch (asteroid)
                    {
                        case 0:
                            new TravellingCosmostoneAsteroidSmall(position, velocity, maxScale, depth, speed * Main.rand.NextFloat(0.01f, 0.02f), lifespan).Spawn();
                            break;
                        case 1:
                            new TravellingCosmostoneAsteroidMedium(position, velocity, maxScale, depth, speed * Main.rand.NextFloat(0.01f, 0.02f), lifespan).Spawn();
                            break;
                        case 2:
                            new TravellingCosmostoneAsteroidLarge(position, velocity, maxScale, depth, speed * Main.rand.NextFloat(0.01f, 0.02f), lifespan).Spawn();
                            break;
                        case 3:
                            new TravellingCosmostoneGeode(position, velocity, maxScale, depth, speed * Main.rand.NextFloat(0.01f, 0.02f), lifespan).Spawn();
                            break;
                        case 4:
                            new TravellingSilicateAsteroidSmall(position, velocity, maxScale, depth, speed * Main.rand.NextFloat(0.01f, 0.02f), lifespan).Spawn();
                            break;
                        case 5:
                            new TravellingSilicateAsteroidMedium(position, velocity, maxScale, depth, speed * Main.rand.NextFloat(0.01f, 0.02f), lifespan).Spawn();
                            break;
                        case 6:
                            new TravellingSilicateAsteroidLarge(position, velocity, maxScale, depth, speed * Main.rand.NextFloat(0.01f, 0.02f), lifespan).Spawn();
                            break;
                    }
                }
            }

            // Stationary, floating asteroids.
            int stationaryAsteroids = 0;

            stationaryAsteroids += SkyEntityManager.CountActiveSkyEntities<StationaryCosmostoneAsteroidSmall>();
            stationaryAsteroids += SkyEntityManager.CountActiveSkyEntities<StationaryCosmostoneAsteroidMedium>();
            stationaryAsteroids += SkyEntityManager.CountActiveSkyEntities<StationaryCosmostoneAsteroidLarge>();
            stationaryAsteroids += SkyEntityManager.CountActiveSkyEntities<StationaryCosmostoneGeode>();
            stationaryAsteroids += SkyEntityManager.CountActiveSkyEntities<StationarySilicateAsteroidSmall>();
            stationaryAsteroids += SkyEntityManager.CountActiveSkyEntities<StationarySilicateAsteroidMedium>();
            stationaryAsteroids += SkyEntityManager.CountActiveSkyEntities<StationarySilicateAsteroidLarge>();

            if (stationaryAsteroids < MaxStationaryAsteroids && Main.rand.NextBool(StationaryAsteroidSpawnChance))
            {
                for (int i = 0; i < totalAsteroidsLayers; i++)
                {
                    float x = virtualCamera.Center.X + Main.rand.NextFloat(-virtualCamera.Size.X, virtualCamera.Size.X) * 3f;
                    float y = (float)(Main.worldSurface * 16f) * Main.rand.NextFloat(-0.01f, 0.225f);
                    Vector2 position = new(x, y);

                    float maxScale = Main.rand.NextFloat(0.5f, 2f);
                    int lifespan = Main.rand.Next(600, 1200);
                    float depth = i + 3f;

                    int asteroid = asteroids.RandomElementByWeight(e => e.Value).Key;
                    
                    switch (asteroid)
                    {
                        case 0:
                            new StationaryCosmostoneAsteroidSmall(position, maxScale, depth, Main.rand.NextFloat(0.01f, 0.03f), lifespan).Spawn();
                            break;
                        case 1:
                            new StationaryCosmostoneAsteroidMedium(position, maxScale, depth, Main.rand.NextFloat(0.01f, 0.03f), lifespan).Spawn();
                            break;
                        case 2:
                            new StationaryCosmostoneAsteroidLarge(position, maxScale, depth, Main.rand.NextFloat(0.01f, 0.03f), lifespan).Spawn();
                            break;
                        case 3:
                            new StationaryCosmostoneGeode(position, maxScale, depth, Main.rand.NextFloat(0.01f, 0.03f), lifespan).Spawn();
                            break;
                        case 4:
                            new StationarySilicateAsteroidSmall(position, maxScale, depth, Main.rand.NextFloat(0.01f, 0.03f), lifespan).Spawn();
                            break;
                        case 5:
                            new StationarySilicateAsteroidMedium(position, maxScale, depth, Main.rand.NextFloat(0.01f, 0.03f), lifespan).Spawn();
                            break;
                        case 6:
                            new StationarySilicateAsteroidLarge(position, maxScale, depth, Main.rand.NextFloat(0.01f, 0.03f), lifespan).Spawn();
                            break;
                    }
                }
            }

            // Have an extremely low chance for exactly one Sirius star to spawn.
            if (SkyEntityManager.CountActiveSkyEntities<Sirius>() < 1 && Main.rand.NextBool(SiriusSpawnChance))
            {
                int lifespan = Main.rand.Next(600, 1200);

                float x = virtualCamera.Center.X + Main.rand.NextFloat(-virtualCamera.Size.X, virtualCamera.Size.X) * 0.85f;
                float y = (float)(Main.worldSurface * 16f) * Main.rand.NextFloat(-0.01f, 0.08f);
                Vector2 position = new(x, y);

                new Sirius(position, Color.SkyBlue, 2f, lifespan).Spawn();
            }
        }
    }
}
