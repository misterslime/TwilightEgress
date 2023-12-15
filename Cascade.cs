using CalamityMod.Cooldowns;
using Cascade.Core.Balancing;
using Cascade.Core.Players.BuffHandlers;

namespace Cascade
{
    public partial class Cascade : Mod
	{
        public Mod CalamityMod;

        public static Cascade Instance { get; private set; }

        public override void Load()
        {
            Instance = this;
            CalamityMod = ModLoader.GetMod("CalamityMod");

            // Cascade-specific loading.
            LoadLists();
            CascadeBalancingChangesManager.Load();

            // Mod Calls.
            if (Main.netMode != NetmodeID.Server)
            {
                var calamityMod = ModLoader.GetMod("CalamityMod");
                Main.QueueMainThreadAction(() =>
                {
                    calamityMod.Call("LoadParticleInstances", this);
                });
            }

            // Calamity-specific loading.
            CooldownRegistry.RegisterModCooldowns(this);
        }

        public override void Unload()
        {
            Instance = null;
            UnloadLists();
            CascadeBalancingChangesManager.Unload();
            BuffHandler.StuffToUnload();
        }
    }
}