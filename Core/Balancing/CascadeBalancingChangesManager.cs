using CalamityMod.Balancing;
using CalamityMod.NPCs.StormWeaver;
using Cascade.Content.DedicatedContent.Marv;

namespace Cascade.Core.Balancing
{
    public static class CascadeBalancingChangesManager 
    {
        internal static List<NPCBalancingChange> NPCSpecificBalancingChanges;

        internal static void Load()
        {
            List<int> thunderousFuryProjectiles = new List<int>()
            {
                ModContent.ProjectileType<ElectricSkyBolt>(),
                ModContent.ProjectileType<ElectricSkyBoltExplosion>(),
                ModContent.ProjectileType<ElectricSkyBoltMist>()
            };

            NPCSpecificBalancingChanges = new List<NPCBalancingChange>()
            {
                // Storm Weaver.
                new NPCBalancingChange(ModContent.NPCType<StormWeaverHead>(), Do(new ProjectileResistBalancingRule(0.5f, thunderousFuryProjectiles.ToArray()))),
                new NPCBalancingChange(ModContent.NPCType<StormWeaverBody>(), Do(new ProjectileResistBalancingRule(0.5f, thunderousFuryProjectiles.ToArray()))),
                new NPCBalancingChange(ModContent.NPCType<StormWeaverTail>(), Do(new ProjectileResistBalancingRule(0.5f, thunderousFuryProjectiles.ToArray())))
            };
        }

        internal static void Unload()
        {
            NPCSpecificBalancingChanges = null;
        }

        internal static void ApplyFromProjectile(NPC npc, ref NPC.HitModifiers modifiers, Projectile proj)
        {
            // Apply rules specific to NPCs. 
            foreach (NPCBalancingChange balancingChange in NPCSpecificBalancingChanges)
            {
                if (npc.type != balancingChange.NPCType)
                    continue;

                foreach (IBalancingRule balancingRule in balancingChange.BalancingRules)
                {
                    if (balancingRule.AppliesTo(npc, modifiers, proj))
                        balancingRule.ApplyBalancingChange(npc, ref modifiers);
                }
            }
        }

        internal static IBalancingRule[] Do(params IBalancingRule[] rules) => rules;
    }
}
