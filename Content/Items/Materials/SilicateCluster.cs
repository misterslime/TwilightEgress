namespace Cascade.Content.Items.Materials
{
    public class SilicateCluster : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(6, 16));
            ItemID.Sets.AnimatesAsSoul[Type] = true;

            ItemID.Sets.ItemNoGravity[Type] = true;

            Item.ResearchUnlockCount = 100;
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
