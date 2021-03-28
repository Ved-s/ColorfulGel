using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ColorfulGel
{
    class ColorfulGelGlobalRecipe : GlobalRecipe
    {

        public override bool RecipeAvailable(Recipe recipe)
        {
            List<Color> recipeColors = GetGelColorsFromItemArray(recipe.requiredItem);
            if (!recipeColors.Any()) return true;
            List<Color> foundColors = new List<Color>();
            foundColors.AddRange(GetGelColorsFromItemArray(Main.guideItem));
            foundColors.AddRange(GetGelColorsFromItemArray(Main.player[Main.myPlayer].inventory));
            foundColors.AddRange(GetGelColorsFromItemArray(GetOpenedChest()));
            return !recipeColors.Except(foundColors).Any();
        }


        private Item[] GetOpenedChest()
        {
            Item[] array = new Item[0];
            if (Main.player[Main.myPlayer].chest != -1)
            {
                if (Main.player[Main.myPlayer].chest > -1)
                {
                    array = Main.chest[Main.player[Main.myPlayer].chest].item;
                }
                else if (Main.player[Main.myPlayer].chest == -2)
                {
                    array = Main.player[Main.myPlayer].bank.item;
                }
                else if (Main.player[Main.myPlayer].chest == -3)
                {
                    array = Main.player[Main.myPlayer].bank2.item;
                }
                else if (Main.player[Main.myPlayer].chest == -4)
                {
                    array = Main.player[Main.myPlayer].bank3.item;
                }
            }
            return array;
        }
        private List<Color> GetGelColorsFromItemArray(params Item[] items)
        {
            List<Color> colors = new List<Color>();
            foreach (Item item in items)
            {
                if (item.type == ItemID.Gel) colors.Add(item.color);
            }
            return colors;
        }


    }
}
