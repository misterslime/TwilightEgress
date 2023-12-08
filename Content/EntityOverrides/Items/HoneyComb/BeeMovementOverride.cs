namespace Cascade.Content.EntityOverrides.Items.HoneyComb
{
    public class BeeMovementOverride : ItemOverride
    {
        public override int TypeToOverride => ItemID.SweetheartNecklace;
        public override int[] AdditionalOverrideTypes => new int[]
        {
            ItemID.BeeCloak,
            ItemID.HoneyBalloon
        };

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
	    {
            player.CascadePlayer_HoneyComb().BeeFlightBoost = 2; 
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            item.InsertNewTooltipLine(tooltips, "Tooltip0", "BeeFlightEffect", "12% increased flight time");
        }
    }
}