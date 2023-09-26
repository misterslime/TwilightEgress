namespace Cascade.Content.EntityOverrides.Items.HoneyComb
{
    public class StingerNecklaceOverride : ItemOverride
    {
        public override int TypeToOverride => ItemID.StingerNecklace;

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
	    {
            player.CascadePlayer_HoneyComb().beeFlight = 1; 
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips){
            tooltips.Add(new(Mod,"BeeFlightEffect","7% increased flight time"));
        }
    }
}