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
            player.CascadePlayer_HoneyComb().BeeFlightBoost = 3;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            // This gets the exact line where the Journey Mode Research message is displayed.
            // Inserting it here causes the line to be pushed down, allowing our lines to appear
            // before it and seem like they're apart of the tooltip.

            // This obviously won't work for everything, since there are different types of tooltip
            // lines that are used for different kinds of items. You can check the line name list
            // at: https://docs.tmodloader.net/docs/stable/class_tooltip_line.html to get an idea
            // of where to insert new lines for your item.
            int index = tooltips.FindIndex(line => line.Name == "JourneyResearch");
            tooltips.InsertNewTooltipLine(index, "HivePackEffect", "Increases the strength of friendly bees");

            // Not incrementing here will cause new lines we insert to be pushed down under new ones,
            // similarly to what happens to the main index line we are inserting at.
            tooltips.InsertNewTooltipLine(index + 1, "HoneyCombEffect", "Douses the user in honey when damaged\nIncreases flight time by 7%");
            tooltips.InsertNewTooltipLine(index + 2, "FlavorText", "\'The hive has found its new host\'");
        }
    }
}