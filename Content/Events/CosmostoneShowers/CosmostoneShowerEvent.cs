using CalamityMod.Events;
using CalamityMod.NPCs.NormalNPCs;
using Cascade.Content.NPCs.CosmostoneShowers.Asteroids;
using Cascade.Content.NPCs.CosmostoneShowers.Planetoids;
using Cascade.Content.Projectiles;
using Cascade.Content.Skies.SkyEntities;
using Cascade.Content.Skies.SkyEntities.StationaryAsteroids;
using Cascade.Content.Skies.SkyEntities.TravellingAsteroid;
using Cascade.Core.Globals.GlobalNPCs;
using Cascade.Core.Graphics.GraphicalObjects.SkyEntitySystem;
using Terraria.GameContent.Events;
using Terraria.Graphics;
using Terraria.ModLoader.IO;

namespace Cascade.Content.Events.CosmostoneShowers
{
    public class CosmostoneShowerEvent : EventHandler
    {
        public static bool CosmostoneShower { get; set; }

        private int ShiningStarSpawnChance
        {
            get
            {
                if (!EventIsActive || !CosmostoneShower)
                    return 0;
                return 100 * (int)Round(Lerp(1f, 0.4f, Star.starfallBoost / 3f), 0);
            }
        }

        private int TravellingAsteroidSpawnChance
        {
            get
            {
                if (!EventIsActive || !CosmostoneShower)
                    return 0;
                return 70 * (int)Round(Lerp(1f, 0.6f, Star.starfallBoost / 3f), 0);
            }
        }

        private int StationaryAsteroidSpawnChance
        {
            get
            {
                if (!EventIsActive || !CosmostoneShower)
                    return 0;
                return 55;
            }
        }

        private int CosmicGasSpawnChance
        {
            get
            {
                if (!EventIsActive || !CosmostoneShower)
                    return 0;
                return 10;
            }
        }

        private int SiriusSpawnChance
        {
            get
            {
                if (!EventIsActive || !CosmostoneShower)
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

        private const int MaxShiningStars = 75;

        private const int MaxTravellingAsteroids = 100;

        private const int MaxStationaryAsteroids = 25;

        private const int MaxCosmicGases = 50;

        public override bool EventIsActive => CosmostoneShower;

        public override void OnModLoad()
        {
            CascadeGlobalNPC.EditSpawnPoolEvent += EditSpawnPool;
        }

        public override void OnModUnload()
        {
            CascadeGlobalNPC.EditSpawnPoolEvent -= EditSpawnPool;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["CosmostoneShower"] = CosmostoneShower;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            CosmostoneShower = tag.GetBool("CosmostoneShower");
        }

        public override void UpdateEvent()
        {         
            bool shouldStopEvent = Main.bloodMoon || Main.pumpkinMoon || Main.snowMoon || BossRushEvent.BossRushActive;
            bool shouldIncreaseSpawnRate = LanternNight.NextNightIsLanternNight;

            // Start and stop the event.
            if (CascadeUtilities.JustTurnedToNight && !shouldStopEvent && !CosmostoneShower && Main.rand.NextBool(shouldIncreaseSpawnRate ? 7 : 15))
            {
                Main.NewText("The night sky glimmers with cosmic energy...", Color.DeepSkyBlue);
                CosmostoneShower = true;
            }

            if ((Main.dayTime && CosmostoneShower) || shouldStopEvent)
                CosmostoneShower = false;

            // Don't run any code past here if the event isn't active.
            if (!EventHandlerManager.SpecificEventIsActive<CosmostoneShowerEvent>())
                return;

            // Important entities.
            Entities_SpawnSpecialSpaceNPCs();
            
            // Visual objects.
            Visuals_SpawnAmbientSkyEntities();
        }

        public override void ResetEventStuff()
        {
            CosmostoneShower = false;
        }

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
                    { ModContent.NPCType<CometstoneAsteroidSmall>(), cometstoneChance * smallAsteroidChance },
                    { ModContent.NPCType<CometstoneAsteroidMedium>(), cometstoneChance * mediumAsteroidChance },
                    { ModContent.NPCType<CometstoneAsteroidLarge>(), cometstoneChance * largeAsteroidChance }
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

        private void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (!spawnInfo.Player.ZoneCosmostoneShowers() || spawnInfo.Invasion)
                return;

            // Space additions.
            if (spawnInfo.Sky)
            {
                // Clear the original spawn pool, successfully getting rid of Vanilla NPCs since
                // they can't be manually removed.
                pool.Clear();
            }

            // Surface additions.
            if (spawnInfo.Player.ZoneOverworldHeight)
            {
                pool.Add(NPCID.Firefly, 0.85f);
            }

            pool.Add(NPCID.EnchantedNightcrawler, 0.75f);

            // FUCK YOU
            if (pool.ContainsKey(ModContent.NPCType<ShockstormShuttle>()))
                pool.Remove(ModContent.NPCType<ShockstormShuttle>());
        }

        private void Visuals_SpawnAmbientSkyEntities()
        {
            if (Main.rand.NextBool(3))
            {
                Vector2 lightBallSpawnPos = Main.LocalPlayer.Center + Main.rand.NextVector2Circular(Main.screenWidth, Main.screenHeight);
                Vector2 velocity = Vector2.One.RotatedByRandom(Tau) * Main.rand.NextFloat(-0.3f, 0.3f);
                float scale = Main.rand.NextFloat(0.08f, 0.25f);
                float parallaxStrength = Main.rand.NextFloat(1f, 5f);
                int lifetime = Main.rand.Next(120, 180);

                AmbientLightBallParticle lightBall = new(lightBallSpawnPos, velocity, scale, 0f, 1f, parallaxStrength, lifetime, Color.CornflowerBlue);
                lightBall.SpawnCasParticle();
            }

            int totalStarLayers = 7;
            int totalAsteroidsLayers = 5;
            VirtualCamera virtualCamera = new(Main.LocalPlayer);

            // Shining Stars.
            if (SkyEntityManager.CountActiveSkyEntities<ShiningStar>() < MaxShiningStars && Main.rand.NextBool(ShiningStarSpawnChance))
            {
                for (int i = 0; i < totalStarLayers; i++)
                {
                    float x = virtualCamera.Center.X + Main.rand.NextFloat(-virtualCamera.Size.X, virtualCamera.Size.X) * 3f;
                    float y = (float)(Main.worldSurface * 16f) * Main.rand.NextFloat(-0.01f, 0.6f);
                    Vector2 position = new(x, y);

                    float maxScale = Main.rand.NextFloat(1f, 3f);
                    int lifespan = Main.rand.Next(120, 240);

                    float xStrectch = Main.rand.NextFloat(0.5f, 1.5f);
                    float yStretch = Main.rand.NextFloat(0.5f, 1.5f);
                    new ShiningStar(position, ShiningStarColors, maxScale, i + 5f, new Vector2(xStrectch, yStretch), lifespan).Spawn();
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
                { 3, cometstoneChance * smallAsteroidChance },
                { 4, cometstoneChance * mediumAsteroidChance },
                { 5, cometstoneChance * largeAsteroidChance }
            };

            // Horizontally-travelling Asteroids.
            int travellingAsteroids = 0;

            travellingAsteroids += SkyEntityManager.CountActiveSkyEntities<StationaryCosmostoneAsteroidSmall>();
            travellingAsteroids += SkyEntityManager.CountActiveSkyEntities<StationaryCosmostoneAsteroidMedium>();
            travellingAsteroids += SkyEntityManager.CountActiveSkyEntities<StationaryCosmostoneAsteroidLarge>();
            travellingAsteroids += SkyEntityManager.CountActiveSkyEntities<StationaryCometstoneAsteroidSmall>();
            travellingAsteroids += SkyEntityManager.CountActiveSkyEntities<StationaryCometstoneAsteroidMedium>();
            travellingAsteroids += SkyEntityManager.CountActiveSkyEntities<StationaryCometstoneAsteroidLarge>();

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
                            new TravellingCometstoneAsteroidSmall(position, velocity, maxScale, depth, speed * Main.rand.NextFloat(0.01f, 0.02f), lifespan).Spawn();
                            break;
                        case 4:
                            new TravellingCometstoneAsteroidMedium(position, velocity, maxScale, depth, speed * Main.rand.NextFloat(0.01f, 0.02f), lifespan).Spawn();
                            break;
                        case 5:
                            new TravellingCometstoneAsteroidLarge(position, velocity, maxScale, depth, speed * Main.rand.NextFloat(0.01f, 0.02f), lifespan).Spawn();
                            break;
                    }
                }
            }

            // Stationary, floating asteroids.
            int stationaryAsteroids = 0;

            stationaryAsteroids += SkyEntityManager.CountActiveSkyEntities<StationaryCosmostoneAsteroidSmall>();
            stationaryAsteroids += SkyEntityManager.CountActiveSkyEntities<StationaryCosmostoneAsteroidMedium>();
            stationaryAsteroids += SkyEntityManager.CountActiveSkyEntities<StationaryCosmostoneAsteroidLarge>();
            stationaryAsteroids += SkyEntityManager.CountActiveSkyEntities<StationaryCometstoneAsteroidSmall>();
            stationaryAsteroids += SkyEntityManager.CountActiveSkyEntities<StationaryCometstoneAsteroidMedium>();
            stationaryAsteroids += SkyEntityManager.CountActiveSkyEntities<StationaryCometstoneAsteroidLarge>();

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
                            new StationaryCometstoneAsteroidSmall(position, maxScale, depth, Main.rand.NextFloat(0.01f, 0.03f), lifespan).Spawn();
                            break;
                        case 4:
                            new StationaryCometstoneAsteroidMedium(position, maxScale, depth, Main.rand.NextFloat(0.01f, 0.03f), lifespan).Spawn();
                            break;
                        case 5:
                            new StationaryCometstoneAsteroidLarge(position, maxScale, depth, Main.rand.NextFloat(0.01f, 0.03f), lifespan).Spawn();
                            break;
                    }
                }
            }

            // Cosmic gases.
            // Makes the sky less monochromatic with its flat blue color.
            if (SkyEntityManager.CountActiveSkyEntities<CosmicGas>() < MaxCosmicGases && Main.rand.NextBool(CosmicGasSpawnChance))
            {
                float x = virtualCamera.Center.X + Main.rand.NextFloat(-virtualCamera.Size.X, virtualCamera.Size.X) * 2f;
                float y = (float)(Main.worldSurface * 16f) * Main.rand.NextFloat(-0.01f, 0.1f);
                Vector2 position = new(x, y);

                int numGas = Main.rand.Next(4, 7);
                for (int i = 0; i < numGas; i++)
                {
                    float individualPositionVariance = Main.rand.NextFloat(850f, 1250f);
                    position += Main.rand.NextVector2Circular(individualPositionVariance, individualPositionVariance);

                    float maxScale = Main.rand.NextFloat(12f, 18f);
                    int lifespan = Main.rand.Next(600, 1200);
                    float depth = Main.rand.NextFloat(5f, 200f);

                    new CosmicGas(position, ShiningStarColors, maxScale, depth, lifespan).Spawn();
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
