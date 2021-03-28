using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ColorfulGel
{
    static class ModRecipeExtension
    {
        public static void AddIngredient(this ModRecipe recipe, Item item)
        {
            FieldInfo numIngredientsField = typeof(ModRecipe).GetField("numIngredients", BindingFlags.NonPublic | BindingFlags.Instance);
            int numIngredients = (int)numIngredientsField.GetValue(recipe);
            recipe.requiredItem[numIngredients] = item;
            numIngredients++;
            numIngredientsField.SetValue(recipe, numIngredients);
        }

        public static void AddGelIngredient(this ModRecipe recipe, string color, int stack = 1)
        {
            AddGelIngredient(recipe, ColorfulGel.GelColors[color], stack);
        }

        public static void AddGelIngredient(this ModRecipe recipe, Color color, int stack = 1)
        {
            Item gel = new Item();
            gel.SetDefaults(ItemID.Gel);
            gel.stack = stack;
            gel.color = color;
            recipe.AddIngredient(gel);
        }
    }
}
