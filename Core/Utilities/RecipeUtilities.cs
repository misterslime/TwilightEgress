namespace Cascade
{
    public static partial class Utilities
    {
        public static void DisableCalamityRecipe(this Recipe recipe, int recipeResult)
        {
            if (recipe.Mod == Cascade.Instance.CalamityMod && recipe.HasResult(recipeResult))
                recipe.DisableRecipe();
        }
    }
}
