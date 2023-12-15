using CalamityMod.Events;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.World.Planets;
using Cascade.Content.NPCs.CosmostoneShowers;
using Cascade.Content.Particles.ScreenParticles;
using Cascade.Content.Projectiles;
using Cascade.Content.Projectiles.Ambient;
using Cascade.Core.BaseEntities.ModNPCs;
using Cascade.Core.Globals.GlobalNPCs;
using Cascade.Core.Systems;
using System.Linq.Expressions;
using Terraria.GameContent.Events;
using Terraria.ModLoader.IO;

namespace Cascade.Content.Events.CosmostoneShowers
{
    public class CosmostoneShowerEvent : EventHandler
    {
        public static bool CosmostoneShower { get; set; }

        public override bool EventIsActive => CosmostoneShower;

        public override void OnModLoad()
        {
            CascadeGlobalNPC.EditSpawnPoolEvent += EditSpawnPool;
        }

        public override void OnModUnload()
        {
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

            HandleEventStuff();
            ParticleVisuals();
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

        public void HandleEventStuff()
        {
            SpawnSpaceEntities();
        }

        private void SpawnSpaceEntities()
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
            float yWorldPosition = (Main.maxTilesY * 0.057f);
            Vector2 playerPositionInBounds = new Vector2(xWorldPosition, yWorldPosition);

            int closestPlayerIndex = Player.FindClosest(playerPositionInBounds, 1, 1);
            Player closestPlayer = Main.player[closestPlayerIndex];

            // Asteroids.
            if (closestPlayer.active && !closestPlayer.dead && Main.rand.NextBool(asteroidSpawnChance))
            {
                // Default spawn position.
                Vector2 asteroidSpawnPosition = closestPlayer.Center + Main.rand.NextVector2Circular(1500f, 800f);

                // Search for any activve Planetoids currently viewable on-screen.
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
            if (closestPlayer.active && !closestPlayer.dead && closestPlayer.Center.Y <= Main.maxTilesY + 135f && Main.rand.NextBool(planetoidSpawnChance))
            {
                Vector2 planetoidSpawnPosition = closestPlayer.Center + Main.rand.NextVector2CircularEdge(Main.rand.NextFloat(2500f, 1500f), Main.rand.NextFloat(1600f, 800f));
                if (Utilities.ObligatoryNetmodeCheckForSpawningEntities() && !Collision.SolidCollision(planetoidSpawnPosition, 1600, 1600) && activePlanetoids.Count < 5)
                {
                    int p = Projectile.NewProjectile(new EntitySource_WorldEvent(), planetoidSpawnPosition, Vector2.Zero, ModContent.ProjectileType<NPCSpawner>(), 0, 0f, Main.myPlayer, ModContent.NPCType<GalileoPlanetoid>());
                    if (Main.projectile.IndexInRange(p))
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p);
                }
            }
        }

        private void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (!spawnInfo.Player.ZoneCosmostoneShowers())
                return;

            pool.Add(NPCID.EnchantedNightcrawler, 0.75f);
            pool.Add(NPCID.LightningBug, 0.75f);
            if (spawnInfo.Player.Center.Y <= Main.maxTilesY + 135f)
            {
                pool.Add(ModContent.NPCType<Twinkler>(), 0.95f);
                pool.Add(ModContent.NPCType<DwarfJellyfish>(), 0.7f);
            }

            // FUCK YOU
            if (pool.ContainsKey(ModContent.NPCType<ShockstormShuttle>()))
                pool.Remove(ModContent.NPCType<ShockstormShuttle>());
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
