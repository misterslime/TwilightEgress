using Cascade.Content.Projectiles.Rogue;

namespace Cascade.Content.Items.Weapons.Rogue
{
    public class HolidayHalberd : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Holiday Halberd");
            /* Tooltip.SetDefault("Normal attacks launch a halberd towards the mouse cursor\n" +
                "The halberd leaves a trail of baubles and explodes into multiple Red or Green Sacks on impact\n" +
                "Stealth strikes imbue the halberd with glacial energy and cause them to cast an unknown spell on impact\n" +
                "'Proceed.'"); */
        }

        public override void SetDefaults()
        {
            Item.width = 70;
            Item.height = 72;
            Item.damage = 50;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.knockBack = 4f;
            Item.crit = 4;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.shoot = ModContent.ProjectileType<HolidayHalbertHoldout>();
            Item.shootSpeed = 1f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {          
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
