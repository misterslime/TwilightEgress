namespace Cascade.Content.EntityOverrides.Items.HoneyComb
{
    public class HoneyCombOverride : ItemOverride
    {
        public override int TypeToOverride => ItemID.HoneyComb;

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
	    {
            player.Cascade_BeeFlightTimeBoost().BeeFlightBoost = 1; 
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            item.InsertNewTooltipLine(tooltips, "Tooltip0", "BeeFlightEffect", "10% increased flight time");
        }
    }
}