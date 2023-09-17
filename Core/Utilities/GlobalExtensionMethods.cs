using Cascade.Core.GlobalInstances.GlobalNPCs;
using Cascade.Core.GlobalInstances.GlobalProjectiles;
using Cascade.Core.Players;
using Cascade.Core.Players.BuffHandlers;

namespace Cascade
{
    public static class GlobalExtensionMethods
    {
        public static CascadeGlobalNPC Cascade(this NPC npc) => npc.GetGlobalNPC<CascadeGlobalNPC>();

        public static CascadeGlobalProjectile Cascade(this Projectile projectile) => projectile.GetGlobalProjectile<CascadeGlobalProjectile>();

        public static DebuffHandlerPlayer Cascade_Debuff(this Player player) => player.GetModPlayer<DebuffHandlerPlayer>();

        public static MinionBuffHandler CascadePlayer_Minions(this Player player) => player.GetModPlayer<MinionBuffHandler>();

        public static ResplendentRoarPlayer CascadePlayer_ResplendantRoar(this Player player) => player.GetModPlayer<ResplendentRoarPlayer>();
    }
}
