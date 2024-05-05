namespace Cascade.Content.Items.Materials
{
    public class SilicateCluster : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 16));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;

            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.noMelee = true;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.material = true;
        }
    }
}
