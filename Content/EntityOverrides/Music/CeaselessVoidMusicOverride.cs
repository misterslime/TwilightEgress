using CalamityMod.NPCs.CeaselessVoid;
using Cascade.Core.Configs;

namespace Cascade.Content.EntityOverrides.Music
{
    internal class CeaselessVoidMusicOverride : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/SecondLaw");

        public override bool IsSceneEffectActive(Player player) => Cascade.CanOverrideMusic(ModContent.NPCType<CeaselessVoid>()) && AudioConfig.Instance.OverrideCalamityMusic;

        public override SceneEffectPriority Priority => (SceneEffectPriority)10;
    }
}
