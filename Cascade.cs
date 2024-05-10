using CalamityMod.Cooldowns;
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

            // Mod Calls.
            if (Main.netMode != NetmodeID.Server)
            {
                var calamityMod = ModLoader.GetMod("CalamityMod");
                Main.QueueMainThreadAction(() =>
                {
                    calamityMod.Call("LoadParticleInstances", this);
                });
            }
        }

        public override void Unload()
        {
            Instance = null;
            UnloadLists();
            BuffHandler.StuffToUnload();
        }
    }
}