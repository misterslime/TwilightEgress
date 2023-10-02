using CalamityMod.Items;
using Cascade.Content.Items.Materials;

namespace Cascade.Content.Items.Weapons.Ranged
{
    public class LunarCresent : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 42;
            Item.damage = 20;
            Item.knockBack = 1f;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Arrow;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.UseSound = SoundID.Item5;
        }

        public override void AddRecipes()
	    {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CosmostoneBar>(), 8) //this could probably be made more interesting but this is functional for now
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			if (type == ProjectileID.WoodenArrowFriendly) {
				type = ProjectileID.JestersArrow; //placeholder until we make the custom convert
			}
		}
    }
}