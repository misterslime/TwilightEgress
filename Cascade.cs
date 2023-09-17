using Cascade.Assets.Effects;

namespace Cascade
{
    public class Cascade : Mod
	{
        public static Cascade Instance { get; private set; }

        public override void Load()
        {
            Instance = this;
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
        }
    }
}