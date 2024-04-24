using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;

namespace Cascade.Content.EntityOverrides.Items.HoneyComb
{
    public class PlagueHiveOverride : ItemOverride
    {
        public override int TypeToOverride => ModContent.ItemType<PlagueHive>();

        public override void AddRecipes()
	    {
            Recipe.Create(TypeToOverride)
                .AddIngredient(ModContent.ItemType<AlchemicalFlask>())
                .AddIngredient(ItemID.HiveBackpack)
                .AddIngredient(ItemID.HoneyComb)
                .AddIngredient(ItemID.FragmentStardust, 10)
                .AddIngredient(ModContent.ItemType<InfectedArmorPlating>(), 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
	    }
        
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
	    {
            player.honeyCombItem = item;
            player.strongBees = true;
            player.Cascade_BeeFlightTimeBoost().BeeFlightBoost = 3;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            Utilities.EditTooltipByNum(2, item, tooltips, (t) => 
            t.Text += "\nIncreases wing flight time by 7%" +
            "\nDouses the user in honey when damaged" +
            "\nIncreases the strength of friendly bees");
        }
    }
}