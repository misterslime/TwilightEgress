using Cascade.Core.Systems;
using Cascade.Content.Projectiles.Ambient;
using Cascade.Content.Projectiles;
using Cascade.Content.Particles.ScreenParticles;
using Cascade.Content.NPCs.CosmostoneShowers;

namespace Cascade.Content.Events.CometNight
{
    public class CometNightSystem : ModSystem
    {
        public override void PostUpdateWorld()
        {
            Player localPlayer = Main.LocalPlayer;

            if (AmbientEventHandler.CometNight)
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
                        if (!(Main.rand.NextFloat(80000) < 10f * spawnChanceFactor))
                            continue;

                        if (closestPlayer.active && !closestPlayer.dead && closestPlayer.ZoneCometNight())
                        {
                            Vector2 cometSpawnPosition = closestPlayer.Center + new Vector2(Main.rand.NextFloat(-600f, 601f), -900f);
                            Vector2 cometVelocity = new Vector2(Main.rand.NextFloat(-8f, 9f), 10f);
                            // We do a little trolling :)))
                            int damage = Main.getGoodWorld ? 900 : 300;
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

                // Asteroid spawning.
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float xWorldPosition = ((Main.maxTilesX - 50) + 100) * 16f;
                    float yWorldPosition = (Main.maxTilesY * 0.05f);
                    Vector2 playerPositionInBounds = new Vector2(xWorldPosition, yWorldPosition);

                    int closestPlayerIndex = Player.FindClosest(playerPositionInBounds, 1, 1);
                    Player closestPlayer = Main.player[closestPlayerIndex];

                    if (closestPlayer.active && !closestPlayer.dead && closestPlayer.Center.Y <= Main.maxTilesY + 135f && Main.rand.NextBool(250))
                    {
                        Vector2 cometSpawnPosition = closestPlayer.Center + Main.rand.NextVector2Circular(1500f, 1000f);
                        if (!Collision.SolidCollision(cometSpawnPosition, 160, 160))
                        {
                            int p = Projectile.NewProjectile(new EntitySource_WorldEvent(), cometSpawnPosition, Vector2.Zero, ModContent.ProjectileType<NPCSpawner>(), 0, 0f, Main.myPlayer, ModContent.NPCType<Asteroid>());
                            if (Main.projectile.IndexInRange(p))
                                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p);
                        }
                    }
                }

                // Particle spawning.
                Vector2 spawnPosition = new Vector2(Main.rand.NextFloat(Main.screenWidth), Main.rand.NextFloat(Main.screenHeight));
                Color color = Color.CornflowerBlue;
                float scale = Main.rand.NextFloat(0.05f, 2f);
                int lifetime = Main.rand.Next(120, 180);
                CometNightStarParticle starParticle = new(spawnPosition, Main.screenPosition, scale * 0.15f, color, scale, lifetime);

                if (Main.rand.NextBool(15) && localPlayer.ZoneSkyHeight)
                    Utilities.SpawnParticleBetter(starParticle);
                if (Main.rand.NextBool(100) && localPlayer.ZoneOverworldHeight)
                    Utilities.SpawnParticleBetter(starParticle);
            }
        }
    }
}
