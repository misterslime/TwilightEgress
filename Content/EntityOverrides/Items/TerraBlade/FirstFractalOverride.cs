//this isn't functional and i don't feel right using it considering most of it is just 'Reobtainable' mod code so 
//we'll probably need to look over it again and maybe contact that guy before considering using this
/*
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using Terraria.GameContent.Creative;

namespace Cascade.Content.EntityOverrides.Items.TerraBlade
{
    public class FirstFractalOverride : ItemOverride
    {
        public override int TypeToOverride => ItemID.FirstFractal;

        public static readonly SoundStyle SwingSound = new SoundStyle("CalamityMod/Sounds/Item/TerratomereSwing");

        //this was from the 'Reobtainable' mod, but doesn't seem to work without the assembly changes they make, so im unsure if this is even worth it
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == 4722;
        }
        public override void SetDefaults(Item item)
        {
            item.noUseGraphic = false;
            //if(item.type == ItemID.FirstFractal) item.SetNameOverride("First Fractal");
        }

        //also from the 'Reobtainable' mod
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Deprecated[4722] = false;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[4722] = 1;
        }

        public override void AddRecipes()
        {
            Recipe.Create(ItemID.FirstFractal)
                .AddIngredient(ItemID.TerraBlade)
                .AddIngredient(ModContent.ItemType<CelestialClaymore>())
                .AddIngredient(ModContent.ItemType<Swordsplosion>())
                .AddIngredient(ModContent.ItemType<DivineGeode>(), 7)
                .AddTile(TileID.LunarCraftingStation)
                .Register();

        }
    }
} 
*/