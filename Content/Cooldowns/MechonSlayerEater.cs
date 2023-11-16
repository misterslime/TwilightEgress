using CalamityMod.Cooldowns;

namespace Cascade.Content.Cooldowns
{
    public class MechonSlayerEater : CooldownHandler
    {
        public static new string ID => "MechonSlayerEater";

        public override bool ShouldDisplay => true;

        public override bool ShouldPlayEndSound => true;

        public override LocalizedText DisplayName => Language.GetText($"Mods.Cascade.UI.Cooldowns.{ID}");

        public override string Texture => "Cascade/Content/Cooldowns/MechonSlayerEater";

        public override string OutlineTexture => "Cascade/Content/Cooldowns/MechonSlayerArtSelectionOutline";

        public override string OverlayTexture => "Cascade/Content/Cooldowns/MechonSlayerArtSelectionOutline";

        public override Color OutlineColor => Color.LightSlateGray;

        public override Color CooldownStartColor => Color.Lerp(Color.SkyBlue, Color.DarkGray, 1f - instance.Completion);

        public override Color CooldownEndColor => CooldownStartColor;

        public override SoundStyle? EndSound => SoundID.DD2_DarkMageAttack;
    }
}
