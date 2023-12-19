using CalamityMod.Events;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.NormalNPCs;
using Cascade.Content.NPCs.CosmostoneShowers;
using Cascade.Content.Projectiles;
using Cascade.Content.Skies.SkyEntities;
using Cascade.Core.Globals.GlobalNPCs;
using Terraria.GameContent.Events;
using Terraria.Graphics;
using Terraria.ModLoader.IO;

namespace Cascade.Content.Events.CosmostoneShowers
{
    public class CosmostoneShowerEvent : EventHandler
    {
        public static bool CosmostoneShower { get; set; }

        public List<ShiningStar> ShiningStars;

        public List<TravellingAsteroid> TravellingAsteroids;

        public List<StationaryAsteroid> StationaryAsteroids;

        private int ShiningStarSpawnChance
        {
            get
            {
                if (!EventIsActive || !CosmostoneShower)
                    return 0;
                return 10;
            }
        }

        private int TravellingAsteroidSpawnChance
        {
            get
            {
                if (!EventIsActive || !CosmostoneShower)
                    return 0;
                return 45 * (int)Round(Lerp(1f, 0.4f, Star.starfallBoost / 3f), 0);
            }
        }

        private int StationaryAsteroidSpawnChance
        {
            get
            {
                if (!EventIsActive || !CosmostoneShower)
                    return 0;
                return 30;
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

        private const int MaxShiningStars = 150;

        private const int MaxTravellingAsteroids = 50;

        private const int MaxStationaryAsteroids = 25;

        public override bool EventIsActive => CosmostoneShower;

        public override void OnModLoad()
        {
            ShiningStars = new(MaxShiningStars);
            TravellingAsteroids = new(MaxTravellingAsteroids);
            StationaryAsteroids = new(MaxStationaryAsteroids);

            CascadeGlobalNPC.EditSpawnPoolEvent += EditSpawnPool;
        }

        public override void OnModUnload()
        {
            ShiningStars = null;
            TravellingAsteroids = null;
            StationaryAsteroids = null;

            CascadeGlobalNPC.EditSpawnPoolEvent -= EditSpawnPool;
        }

        public override void UpdateEvent()
        {         
            bool shouldStopEvent = Main.bloodMoon || Main.pumpkinMoon || Main.snowMoon || BossRushEvent.BossRushActive;
            bool shouldIncreaseSpawnRate = LanternNight.NextNightIsLanternNight;

            // Start and stop the event.
            if (Utilities.JustTurnedToNight && !shouldStopEvent && !CosmostoneShower && Main.rand.NextBool(shouldIncreaseSpawnRate ? 7 : 15))
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

        public override void SaveWorldData(TagCompound tag)
        {
            tag["CosmostoneShower"] = CosmostoneShower;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            CosmostoneShower = tag.GetBool("CosmostoneShower");
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

                if (Utilities.ObligatoryNetmodeCheckForSpawningEntities() && !Collision.SolidCollision(asteroidSpawnPosition, 300, 300))
                {
                    int p = Projectile.NewProjectile(new EntitySource_WorldEvent(), asteroidSpawnPosition, Vector2.Zero, ModContent.ProjectileType<NPCSpawner>(), 0, 0f, Main.myPlayer, ModContent.NPCType<CosmostoneAsteroid>());
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
                
                if (Utilities.ObligatoryNetmodeCheckForSpawningEntities() && !Collision.SolidCollision(planetoidSpawnPosition, 1600, 1600) && activePlanetoids.Count < 10)
                {
                    int p = Projectile.NewProjectile(new EntitySource_WorldEvent(), planetoidSpawnPosition, Vector2.Zero, ModContent.ProjectileType<NPCSpawner>(), 0, 0f, Main.myPlayer, ModContent.NPCType<GalileoPlanetoid>());
                    if (Main.projectile.IndexInRange(p))
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p);
                }
            }

            Vector2 dustPosition = closestPlayer.Center + new Vector2(0f, Main.maxTilesY + 10f);
            Utilities.CreateDustLoop(3, dustPosition, Vector2.Zero, DustID.Dirt);
        }

        private void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (!spawnInfo.Player.ZoneCosmostoneShowers() || spawnInfo.Invasion)
                return;

            // Space additions.
            if (spawnInfo.Player.ZoneSkyHeight)
            {
                // Clear the original spawn pool, successfully getting rid of Vanilla NPCs since
                // they can't be manually removed.
                pool.Clear();

                // Increase the spawn rate of some Astral Enemies with each defeat of a major boss,
                // indicating that the Astral Meteor is getting closer.

                // Twinklers.
                float twinklerSpawnChance =
                    DownedBossSystem.downedSlimeGod ? 3f :
                    NPC.downedBoss3 ? 2f :
                    (DownedBossSystem.downedHiveMind || DownedBossSystem.downedPerforator) ? 1f :
                    NPC.downedBoss2 ? 0.25f :
                    0f;

                // Astral Slimes.
                float astralSlimeSpawnChance =
                    DownedBossSystem.downedSlimeGod ? 0.2f :
                    (DownedBossSystem.downedHiveMind || DownedBossSystem.downedPerforator) ? 0.05f :
                    0f;

                // Astral Probes.
                float astralProbeSpawnChance =
                    DownedBossSystem.downedSlimeGod ? 0.2f :
                    NPC.downedBoss3 ? 0.1f :
                    (DownedBossSystem.downedHiveMind || DownedBossSystem.downedPerforator) ? 0.06f :
                    NPC.downedBoss2 ? 0.02f :
                    0f;

                pool.Add(ModContent.NPCType<Twinkler>(), twinklerSpawnChance);
                pool.Add(ModContent.NPCType<AstralSlime>(), astralSlimeSpawnChance);
                pool.Add(ModContent.NPCType<AstralProbe>(), astralProbeSpawnChance);
            }

            // Surface additions.
            if (spawnInfo.Player.ZoneOverworldHeight)
            {
                pool.Add(NPCID.LightningBug, 0.25f);
                pool.Add(NPCID.Firefly, 0.85f);
            }

            pool.Add(NPCID.EnchantedNightcrawler, 0.75f);

            // FUCK YOU
            if (pool.ContainsKey(ModContent.NPCType<ShockstormShuttle>()))
                pool.Remove(ModContent.NPCType<ShockstormShuttle>());
        }

        private void Visuals_SpawnAmbientSkyEntities()
        {
            int totalStarLayers = 7;
            int totalAsteroidsLayers = 3;
            VirtualCamera virtualCamera = new(Main.LocalPlayer);

            // Ensure lists are cleared properly.
            ShiningStars.RemoveAll(s => !s.Active || s.Time >= s.Lifespan);
            TravellingAsteroids.RemoveAll(a => !a.Active || a.Time >= a.Lifespan);
            StationaryAsteroids.RemoveAll(a => !a.Active || a.Time >= a.Lifespan);

            // Shining Stars.
            if (ShiningStars.Count < ShiningStars.Capacity)
            {
                for (int i = 0; i < totalStarLayers; i++)
                {
                    float x = virtualCamera.Center.X + Main.rand.NextFloat(-virtualCamera.Size.X, virtualCamera.Size.X) * 3f;
                    float y = (float)(Main.worldSurface * 16f) * Main.rand.NextFloat(-0.01f, 0.6f);
                    Vector2 position = new(x, y);

                    float maxScale = Main.rand.NextFloat(1f, 3f);
                    int lifespan = Main.rand.Next(120, 240);

                    ShiningStar shiningStar = new(position, ShiningStarColors, maxScale, i + 1.5f, new Vector2(1f, 1.5f), lifespan);
                    if (Main.rand.NextBool(ShiningStarSpawnChance))
                        shiningStar.Spawn();
                    ShiningStars.Add(shiningStar);
                }
            }

            // Horizontally-travelling Asteroids.
            if (TravellingAsteroids.Count < TravellingAsteroids.Capacity)
            {
                for (int i = 0; i < totalAsteroidsLayers; i++)
                {
                    float x = virtualCamera.Center.X - virtualCamera.Size.X - 1280f;
                    float y = (float)(Main.worldSurface * 16f) * Main.rand.NextFloat(-0.2f, 0.3f);
                    Vector2 position = new(x, y);

                    float speed = Main.rand.NextFloat(3f, 15f);
                    Vector2 velocity = Vector2.UnitX * speed;

                    float maxScale = Main.rand.NextFloat(0.5f, 3f);
                    float depth = i + 3f;
                    int lifespan = Main.rand.Next(1200, 1800);

                    TravellingAsteroid asteroid = new(position, velocity, maxScale, depth, speed * Main.rand.NextFloat(0.01f, 0.02f), lifespan);
                    if (Main.rand.NextBool(TravellingAsteroidSpawnChance))
                        asteroid.Spawn();
                    TravellingAsteroids.Add(asteroid);
                }
            }

            // Stationary, floating asteroids.
            if (StationaryAsteroids.Count < StationaryAsteroids.Capacity)
            {
                for (int i = 0; i < totalAsteroidsLayers; i++)
                {
                    float x = virtualCamera.Center.X + Main.rand.NextFloat(-virtualCamera.Size.X, virtualCamera.Size.X) * 3f;
                    float y = (float)(Main.worldSurface * 16f) * Main.rand.NextFloat(-0.01f, 0.6f);
                    Vector2 position = new(x, y);

                    float maxScale = Main.rand.NextFloat(1f, 5f);
                    int lifespan = Main.rand.Next(600, 1200);
                    float depth = i + 3f;

                    StationaryAsteroid stationaryAsteroid = new(position, maxScale, depth, Main.rand.NextFloat(0.01f, 0.03f), lifespan);
                    if (Main.rand.NextBool(StationaryAsteroidSpawnChance))
                        stationaryAsteroid.Spawn();
                    StationaryAsteroids.Add(stationaryAsteroid);
                }
            }
        }
    }
}
