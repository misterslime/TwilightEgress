using CalamityMod.Balancing;
using CalamityMod.NPCs.StormWeaver;
using Cascade.Content.Items.Dedicated.Marv;
using ProjectileResistBalancingRule = Luminance.Core.Balancing.DefaultNPCBalancingRules.ProjectileResistBalancingRule;

namespace Cascade.Common.Balances
{
    public class CascadeBalancingManager : BalancingManager {

        internal List<int> thunderousFuryProjectiles = new List<int>() {
                ModContent.ProjectileType<ElectricSkyBolt>(),
                ModContent.ProjectileType<ElectricSkyBoltExplosion>(),
                ModContent.ProjectileType<ElectricSkyBoltMist>()
            };

        public CascadeBalancingManager() { }

        public override IEnumerable<NPCHitBalancingChange> GetNPCHitBalancingChanges() { 
            //stormweaver changes
            yield return new NPCHitBalancingChange(ModContent.NPCType<StormWeaverHead>(), new ProjectileResistBalancingRule(
                0.5f,
                BalancePriority.High,
                thunderousFuryProjectiles.ToArray()
                ));
            yield return new NPCHitBalancingChange(ModContent.NPCType<StormWeaverBody>(), new ProjectileResistBalancingRule(
                0.5f,
                BalancePriority.High,
                thunderousFuryProjectiles.ToArray()
                ));
            yield return new NPCHitBalancingChange(ModContent.NPCType<StormWeaverTail>(), new ProjectileResistBalancingRule(
                0.5f,
                BalancePriority.High,
                thunderousFuryProjectiles.ToArray()
                ));
        }
    }
}
