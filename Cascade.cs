using Cascade.Core.Players.BuffHandlers;

namespace Cascade
{
    public partial class Cascade : Mod
	{
        public static Mod CalamityMod { get; private set; }

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
                Main.QueueMainThreadAction(() =>
                {
                    CalamityMod.Call("LoadParticleInstances", this);
                });
            }
        }

        public override void Unload()
        {
            Instance = null;
            CalamityMod = null;
            UnloadLists();
            BuffHandler.StuffToUnload();
        }
    }
}