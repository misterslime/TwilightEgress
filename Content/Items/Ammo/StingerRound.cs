using Cascade.Content.Projectiles.Ranged.Ammo;

namespace Cascade.Content.Items.Ammo
{
    public class StingerRound : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 14;
            Item.damage = 6;
            Item.knockBack = 2;
            Item.DamageType = DamageClass.Ranged;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.value = Item.sellPrice(copper: 1);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<StingerRoundProjectile>();
            Item.ammo = AmmoID.Bullet;
        }
    }
}
