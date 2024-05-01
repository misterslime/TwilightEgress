using Cascade.Content.Items.Materials;

namespace Cascade.Content.Items.Placeable
{
    public class CometstoneBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.CometstoneBrick>());
            Item.width = 12;
            Item.height = 12;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Cometstone>(2)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
