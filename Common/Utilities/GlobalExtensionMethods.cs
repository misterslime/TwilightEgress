using Cascade.Core.Globals;
using Cascade.Core.Globals.GlobalNPCs;
using Cascade.Core.Players;
using Cascade.Core.Players.BuffHandlers;
using Cascade.Core.Systems.PlanetoidSystem;

namespace Cascade
{
    public static class GlobalExtensionMethods
    {
        #region GlobalNPCs
        public static CascadeGlobalNPC Cascade(this NPC npc) => npc.GetGlobalNPC<CascadeGlobalNPC>();

        public static DebuffHandlerGlobalNPC Cascade_Buffs(this NPC npc) => npc.GetGlobalNPC<DebuffHandlerGlobalNPC>();
        #endregion

        #region GlobalProjectiles
        public static CascadeGlobalProjectile Cascade(this Projectile projectile) => projectile.GetGlobalProjectile<CascadeGlobalProjectile>();
        #endregion

        #region ModPlayers
        public static BuffHandler Cascade_Buffs(this Player player) => player.GetModPlayer<BuffHandler>();

        public static ResplendentRoarPlayer Cascade_ResplendentRoar(this Player player) => player.GetModPlayer<ResplendentRoarPlayer>();

        public static BeeFlightTimeBoostPlayer Cascade_BeeFlightTimeBoost(this Player player) => player.GetModPlayer<BeeFlightTimeBoostPlayer>();

        public static OrbitalGravityPlayer Cascade_OrbitalGravity(this Player player) => player.GetModPlayer<OrbitalGravityPlayer>();

        public static PlanetoidPlayer Cascade_Planetoid(this Player player) => player.GetModPlayer<PlanetoidPlayer>();
        #endregion
    }
}
