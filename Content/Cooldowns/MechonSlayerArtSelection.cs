using CalamityMod.Cooldowns;

namespace Cascade.Content.Cooldowns
{
    public class MechonSlayerArtSelection : CooldownHandler
    {
        public static new string ID => "MechonSlayerArtSelection";

        public override bool ShouldDisplay => true;

        public override bool ShouldPlayEndSound => true;

        public override LocalizedText DisplayName => Language.GetText($"Mods.Cascade.UI.Cooldowns.{ID}");

        public override string Texture => "Cascade/Content/Cooldowns/MechonSlayerArtSelection";

        public override Color OutlineColor => Color.DarkBlue;

        public override Color CooldownStartColor => Color.Lerp(Color.LightSkyBlue, Color.Cyan, 1f - instance.Completion);

        public override Color CooldownEndColor => CooldownStartColor;

        public override SoundStyle? EndSound => SoundID.DD2_DarkMageCastHeal;
    }
}
