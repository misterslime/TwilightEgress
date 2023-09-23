using Cascade.Content.Events.CosmostoneShowers;
using Cascade.Content.Projectiles.Ambient;
using Cascade.Content.Projectiles;
using static Cascade.Core.Systems.WorldSavingSystem;
using Cascade.Content.NPCs.CosmostoneShowers;

namespace Cascade.Core.Systems
{
    public class WorldUpdatingSystem : ModSystem
    {
        public override void PostUpdateWorld()
        {
            UpdateEvents();
            SpawnAmbientObjects();
        }

        private void UpdateEvents()
        {
            CosmostoneShowerEvent.UpdateEvent();
            CosmostoneShowerEvent.StartAndStopLightCosmoShowers();
        }

        private void SpawnAmbientObjects()
        {
            SpawnAsteroids();
            SpawnComets();
        }

        private void SpawnAsteroids()
        {
            // Asteroid spawning.
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                float xWorldPosition = ((Main.maxTilesX - 50) + 100) * 16f;
                float yWorldPosition = (Main.maxTilesY * 0.05f);
                Vector2 playerPositionInBounds = new Vector2(xWorldPosition, yWorldPosition);

                int closestPlayerIndex = Player.FindClosest(playerPositionInBounds, 1, 1);
                Player closestPlayer = Main.player[closestPlayerIndex];

                int spawnChance = CosmostoneShower ? 250 : LightCosmostoneShower ? 750 : 1500;
                if (closestPlayer.active && !closestPlayer.dead && closestPlayer.Center.Y <= Main.maxTilesY + 135f && Main.rand.NextBool(spawnChance))
                {
                    Vector2 cometSpawnPosition = closestPlayer.Center + Main.rand.NextVector2Circular(1500f, 1000f);
                    if (!Collision.SolidCollision(cometSpawnPosition, 160, 160))
                    {
                        int p = Projectile.NewProjectile(new EntitySource_WorldEvent(), cometSpawnPosition, Vector2.Zero, ModContent.ProjectileType<NPCSpawner>(), 0, 0f, Main.myPlayer, ModContent.NPCType<CosmostoneAsteroid>());
                        if (Main.projectile.IndexInRange(p))
                            NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p);
                    }
                }
            }
        }

        private void SpawnComets()
        {
            // Handle projectile spawning.
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // Some of this is derived from Vanilla's code to spawn fallen stars.
                // Essentially we scan the entire world's surface and then find any players close enough
                // to these boundraries.
                float xWorldPosition = ((Main.maxTilesX - 50) + 100) * 16f;
                float yWorldPosition = (Main.maxTilesY * 0.05f) * 16f;
                Vector2 playerPositionInBounds = new Vector2(xWorldPosition, yWorldPosition);

                int closestPlayerIndex = Player.FindClosest(playerPositionInBounds, 1, 1);
                Player closestPlayer = Main.player[closestPlayerIndex];

                int spawnChanceFactor = (int)(float)(10f * ((float)(Main.maxTilesX / 4200f)) * Star.starfallBoost);
                int timePassed = (int)Main.desiredWorldEventsUpdateRate;
                for (int i = 0; i < timePassed; i++)
                {
                    bool nightTimeWithNoCosmoEvents = !Main.dayTime && !LightCosmostoneShower && !CosmostoneShower;
                    int spawnChance = nightTimeWithNoCosmoEvents ? 480000 : LightCosmostoneShower ? 240000 : 80000;
                    if (!(Main.rand.NextFloat(spawnChance) < 10f * spawnChanceFactor))
                        continue;

                    if (closestPlayer.active && !closestPlayer.dead && closestPlayer.ZoneCometNight())
                    {
                        Vector2 cometSpawnPosition = closestPlayer.Center + new Vector2(Main.rand.NextFloat(-600f, 601f), -900f);
                        Vector2 cometVelocity = new Vector2(Main.rand.NextFloat(-8f, 9f), 10f);
                        // We do a little trolling :)))
                        int damage = Main.zenithWorld ? 900 : 300;
                        // Check to ensure collision with floating islands or anything in the sky doesn't occur.
                        if (!Collision.SolidCollision(playerPositionInBounds, 36, 36))
                        {
                            int p = Projectile.NewProjectile(new EntitySource_WorldEvent(), cometSpawnPosition, cometVelocity, ModContent.ProjectileType<Comet>(), damage, 0f, Main.myPlayer);
                            if (Main.projectile.IndexInRange(p))
                            {
                                // Forcefully sync the projectile to hopefully prevent any Multiplayer desync shenanigans.
                                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p);
                            }
                        }
                    }
                }
            }
        }
    }
}
