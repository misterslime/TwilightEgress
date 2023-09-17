using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.NormalNPCs;
using Cascade.Content.NPCs.CosmostoneShowers;
using System.Collections.Generic;

namespace Cascade.Content.Events.CometNight
{
    public class CometNightSpawnPool : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCometNight())
            {
                pool.Add(NPCID.EnchantedNightcrawler, 0.75f);
                pool.Add(NPCID.LightningBug, 0.75f);
                if (spawnInfo.Player.Center.Y <= Main.maxTilesY + 135f)
                {
                    pool.Add(ModContent.NPCType<Twinkler>(), 0.95f);
                    pool.Add(ModContent.NPCType<DwarfJellyfish>(), 0.7f);
                }

                // FUCK YOU FUCK YOU FUCK YOU FUCK YOU FUCK YOU FUCK YOU
                if (pool.ContainsKey(ModContent.NPCType<ShockstormShuttle>()))
                    pool.Remove(ModContent.NPCType<ShockstormShuttle>());
            }
        }
    }
}
