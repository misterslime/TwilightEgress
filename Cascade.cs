using CalamityMod.Cooldowns;
using Cascade.Core.Balancing;
using Cascade.Core.Players.BuffHandlers;

namespace Cascade
{
    public class Cascade : Mod
	{
        public Mod CalamityMod;

        public static Cascade Instance { get; private set; }

        public override void Load()
        {
            Instance = this;
            CalamityMod = ModLoader.GetMod("CalamityMod");

            CascadeBalancingChangesManager.Load();

            if (Main.netMode != NetmodeID.Server)
            {
                var calamityMod = ModLoader.GetMod("CalamityMod");
                Main.QueueMainThreadAction(() =>
                {
                    calamityMod.Call("LoadParticleInstances", this);
                });
            }

            CooldownRegistry.RegisterModCooldowns(this);
        }

        public override void Unload()
        {
            Instance = null;
            CascadeBalancingChangesManager.Unload();
            BuffHandler.StuffToUnload();
        }
    }
}