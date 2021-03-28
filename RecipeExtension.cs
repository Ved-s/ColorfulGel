using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Exceptions;
using Terraria.ID;

namespace ColorfulGel
{
    static class RecipeExtension
    {
        public static void AddIngredient(this Recipe recipe, Item item)
        {
            for (int i = 0; i < Recipe.maxRequirements; i++)
            {
                if (recipe.requiredItem[i].type == ItemID.None)
                {
                    recipe.requiredItem[i] = item;
                    return;
                }
                if (recipe.requiredItem[i].IsTheSameAs(item))
                {
                    recipe.requiredItem[i].stack += item.stack;
                    return;
                }
            }
            throw new RecipeException("Recipe already has maximum number of ingredients");
        }
    }
}
