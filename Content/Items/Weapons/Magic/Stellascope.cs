using Cascade.Content.Projectiles.Magic;

namespace Cascade.Content.Items.Weapons.Magic
{
    public class Stellascope : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 25;
            Item.height = 8;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 24;
            Item.useTime = Item.useAnimation = 20;
            Item.knockBack = 0;
            Item.mana = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<StellascopeHoldout>();
            Item.channel = true;
            Item.value = 12504;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.rare = ItemRarityID.Green;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI);
            return false;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
    }
}
