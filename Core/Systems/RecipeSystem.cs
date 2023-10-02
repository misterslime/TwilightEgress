using CalamityMod.Items.Accessories;

namespace Cascade.Core.Systems
{
    public class RecipeSystem : ModSystem
    {
        private List<int> RecipesToBeDisabled;

        public override void PostAddRecipes()
        {
            RecipesToBeDisabled = new List<int>()
            {
                ModContent.ItemType<PlagueHive>()
            };

            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];

                // Disabling recipes.
                foreach (int result in RecipesToBeDisabled)
                    recipe.DisableCalamityRecipe(result);
            }
        } 
    }
}
