using CalamityMod.NPCs.StormWeaver;
using Cascade.Core.Configs;

namespace Cascade.Content.EntityOverrides.Music
{
    internal class StormWeaverMusicOverride : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/SupercellRogue");

        public override bool IsSceneEffectActive(Player player) => Cascade.CanOverrideMusic(ModContent.NPCType<StormWeaverHead>()) && AudioConfig.Instance.OverrideCalamityMusic;

        public override SceneEffectPriority Priority => (SceneEffectPriority)10;
    }
}
