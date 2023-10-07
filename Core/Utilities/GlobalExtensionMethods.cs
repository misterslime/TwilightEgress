﻿using Cascade.Core.Globals;
using Cascade.Core.Globals.GlobalNPCs;
using Cascade.Core.Players;
using Cascade.Core.Players.BuffHandlers;

namespace Cascade
{
    public static class GlobalExtensionMethods
    {
        public static CascadeGlobalNPC Cascade(this NPC npc) => npc.GetGlobalNPC<CascadeGlobalNPC>();

        public static DebuffHandlerGlobalNPC Cascade_Debuff(this NPC npc) => npc.GetGlobalNPC<DebuffHandlerGlobalNPC>();

        public static CascadeGlobalProjectile Cascade(this Projectile projectile) => projectile.GetGlobalProjectile<CascadeGlobalProjectile>();

        public static DebuffHandlerPlayer Cascade_Debuff(this Player player) => player.GetModPlayer<DebuffHandlerPlayer>();

        public static MinionBuffHandler CascadePlayer_Minions(this Player player) => player.GetModPlayer<MinionBuffHandler>();

        public static ResplendentRoarPlayer CascadePlayer_ResplendantRoar(this Player player) => player.GetModPlayer<ResplendentRoarPlayer>();

        public static HoneyCombPlayer CascadePlayer_HoneyComb(this Player player) => player.GetModPlayer<HoneyCombPlayer>();
    }
}
