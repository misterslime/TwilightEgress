using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;

namespace Cascade.Content.EntityOverrides.Items.HoneyComb
{
    public class PlagueHiveOverride : ItemOverride
    {
        public override int TypeToOverride => ModContent.ItemType<CalamityMod.Items.Accessories.PlagueHive>();

        public override void AddRecipes()
	    {
		    Recipe rec = Recipe.Create(TypeToOverride)
                .AddIngredient(ModContent.ItemType<AlchemicalFlask>(), 1)
                .AddIngredient(ItemID.HiveBackpack, 1)
                .AddIngredient(ItemID.HoneyComb, 1)
                .AddIngredient(ItemID.FragmentStardust, 10)
                .AddIngredient(ModContent.ItemType<InfectedArmorPlating>(), 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
	    }
        
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
	    {
            player.honeyCombItem = item;
            player.CascadePlayer_HoneyComb().beeFlight = 3;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
<<<<<<< HEAD
=======
            
>>>>>>> 4490480a44af693588ca5e235e62d463e91e69bd
            tooltips.Add(new(Mod, "HivePackEffect", "Increases the strength of friendly bees"));
            tooltips.Add(new(Mod,"HoneyCombEffect","Douses the user in honey when damaged\n7% increased flight time"));
        }
    }
}