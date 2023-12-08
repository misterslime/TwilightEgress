namespace Cascade.Content.EntityOverrides.Items.HoneyComb
{
    public class StingerNecklaceOverride : ItemOverride
    {
        public override int TypeToOverride => ItemID.StingerNecklace;

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
	    {
            player.CascadePlayer_HoneyComb().BeeFlightBoost = 1; 
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            item.InsertNewTooltipLine(tooltips, "Tooltip1", "BeeFlightEffect", "7% increased flight time");
        }
    }
}