using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using Terraria.GameContent.Creative;

namespace Cascade.Content.EntityOverrides.Items.TerraBlade
{
    public class FirstFractalOverride : ItemOverride
    {
        public override int TypeToOverride => ItemID.FirstFractal;

        public override void SetDefaults(Item item)
        {
            item.noUseGraphic = true;
            item.SetNameOverride("First Fractal");
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.Deprecated[TypeToOverride] = false;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[TypeToOverride] = 1;
        }

        public override void AddRecipes()
        {
            Recipe.Create(TypeToOverride)
                .AddIngredient(ItemID.TerraBlade)
                .AddIngredient(ModContent.ItemType<CelestialClaymore>())
                .AddIngredient(ModContent.ItemType<Swordsplosion>())
                .AddIngredient(ModContent.ItemType<DivineGeode>(), 7)
                .AddTile(TileID.LunarCraftingStation)
                .Register();

        }
    }
} 