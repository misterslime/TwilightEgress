namespace Cascade.Content.Items.Materials
{
    public class Cosmostone : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.width = 18;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.noMelee = true;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.material = true;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.LightBlue.ToVector3());    
        }
    }
}
