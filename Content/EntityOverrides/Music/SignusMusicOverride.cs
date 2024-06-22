using CalamityMod.NPCs.Signus;
using Cascade.Core.Configs;

namespace Cascade.Content.EntityOverrides.Music
{
    internal class SignusMusicOverride : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/YourSilhouette");

        public override bool IsSceneEffectActive(Player player) => Cascade.CanOverrideMusic(ModContent.NPCType<Signus>()) && AudioConfig.Instance.OverrideCalamityMusic;

        public override SceneEffectPriority Priority => (SceneEffectPriority)10;
    }
}
