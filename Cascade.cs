using Cascade.Assets.Effects;
using Cascade.Core.Balancing;

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
        }

        public override void Unload()
        {
            Instance = null;
            CascadeBalancingChangesManager.Unload();
        }
    }
}