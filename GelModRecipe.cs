using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ColorfulGel
{
    class GelModRecipe : ModRecipe
    {
        public List<Color> RequiredGelColors = new List<Color>();

        public GelModRecipe(Mod mod) : base(mod) 
        {

        }

        public void AddIngredient(Item item) 
        {
            FieldInfo numIngredientsField = base.GetType().BaseType.GetField("numIngredients", BindingFlags.NonPublic | BindingFlags.Instance);
            int numIngredients = (int)numIngredientsField.GetValue(this);

            requiredItem[numIngredients] = item;

            numIngredients++;
            numIngredientsField.SetValue(this, numIngredients);

        }
        public void AddGelIngredient(string color, int stack = 1)
        {
            AddGelIngredient(ColorfulGel.GelColors[color], stack);
        }
        public void AddGelIngredient(Color color, int stack = 1)
        {
            Item gel = new Item();
            gel.SetDefaults(ItemID.Gel);
            gel.stack = stack;
            gel.color = color;
            this.AddIngredient(gel);
            RequiredGelColors.Add(color);
        }

        public override bool RecipeAvailable()
        {
            return IsRequiredGel(Main.guideItem)
                || InventoryHasAllRequiredGel(Main.player[Main.myPlayer].inventory)
                || InventoryHasAllRequiredGel(GetOpenedChest());

        }

        private bool IsRequiredGel(Item item) 
        {
            return item.type == ItemID.Gel && RequiredGelColors.Contains(item.color);
        }
        private bool InventoryHasAllRequiredGel(Item[] items) 
        {
            List<Color> colors = new List<Color>(RequiredGelColors);
            foreach (Item i in items) 
            {
                if (i.type == ItemID.Gel && colors.Contains(i.color)) colors.Remove(i.color);
                if (colors.Count == 0) return true;
            }
            return false;
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
    }
}
