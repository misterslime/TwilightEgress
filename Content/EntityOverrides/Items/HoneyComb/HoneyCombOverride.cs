namespace Cascade.Content.EntityOverrides.Items.HoneyComb
{
    public class HoneyCombOverride : ItemOverride
    {
        public override int TypeToOverride => ItemID.HoneyComb;

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
	    {
            player.CascadePlayer_HoneyComb().beeFlight = 1; 
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips){
            tooltips.Add(new(Mod,"BeeFlightEffect","10% increased flight time"));
        }
    }
}