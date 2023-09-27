using Cascade.Assets.Effects;
using Cascade.Core.Balancing;

namespace Cascade
{
    public class Cascade : Mod
	{
        public static Cascade Instance { get; private set; }

        public override void Load()
        {
            Instance = this;
            CascadeBalancingChangesManager.Load();
            if (Main.netMode != NetmodeID.Server)
            {
                CascadeEffectRegistry.LoadAllShaders();

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