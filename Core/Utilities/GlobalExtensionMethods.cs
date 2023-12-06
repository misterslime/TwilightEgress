using Cascade.Core.Globals;
using Cascade.Core.Globals.GlobalNPCs;
using Cascade.Core.Players;
using Cascade.Core.Players.BuffHandlers;

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

        public static ResplendentRoarPlayer CascadePlayer_ResplendantRoar(this Player player) => player.GetModPlayer<ResplendentRoarPlayer>();

        public static HoneyCombPlayer CascadePlayer_HoneyComb(this Player player) => player.GetModPlayer<HoneyCombPlayer>();
        #endregion
    }
}
