using Cascade.Content.Buffs.Pets;

namespace Cascade.Content.DedicatedContent.Lynel
{
    public class EarmuffFruit : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.DefaultToVanitypet(ModContent.ProjectileType<EarPiercingBellbird>(), ModContent.BuffType<BellbirdBuff>());
        }

        public override bool? UseItem(Player player)
        {
            player.AddBuff(Item.buffType, 2);
            return base.UseItem(player);
        }
    }
}
