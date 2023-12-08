using CalamityMod.Events;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.NormalNPCs;
using Cascade.Content.NPCs.CosmostoneShowers;
using Cascade.Content.Particles.ScreenParticles;
using Cascade.Content.Projectiles;
using Cascade.Content.Projectiles.Ambient;
using Cascade.Core.Globals.GlobalNPCs;
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
            // Asteroid spawning.
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                float xWorldPosition = ((Main.maxTilesX - 50) + 100) * 16f;
                float yWorldPosition = (Main.maxTilesY * 0.05f);
                Vector2 playerPositionInBounds = new Vector2(xWorldPosition, yWorldPosition);

                int closestPlayerIndex = Player.FindClosest(playerPositionInBounds, 1, 1);
                Player closestPlayer = Main.player[closestPlayerIndex];

                int spawnChance = 250;
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
                    int spawnChance = 80000;
                    if (!(Main.rand.NextFloat(spawnChance) < 10f * spawnChanceFactor))
                        continue;

                    if (closestPlayer.active && !closestPlayer.dead && closestPlayer.ZoneCosmostoneShowers())
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
