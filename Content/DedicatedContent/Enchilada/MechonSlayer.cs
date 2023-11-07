using CalamityMod.Items;
using CalamityMod.Rarities;

namespace Cascade.Content.DedicatedContent.Enchilada
{
    public class MechonSlayer : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 102;
            Item.height = 108;
            Item.damage = 300;
            Item.knockBack = 4f;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.shootSpeed = 10f;
        }
    }
}
