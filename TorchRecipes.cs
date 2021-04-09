using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria;

namespace ColorfulGel
{
    static class TorchRecipes
    {
        static internal List<Tuple<string, short, int>> GelMakeTorchesCount = new List<Tuple<string, short, int>>
        {
            MakeTuple( "Default",    ItemID.Torch, 1 ),
            MakeTuple( "Red",        ItemID.Torch, 2 ),
            MakeTuple( "Green",      ItemID.Torch, 2 ),
            MakeTuple( "Blue",       ItemID.Torch, 3 ),
            MakeTuple( "Yellow",     ItemID.Torch, 2 ),
            MakeTuple( "Purple",     ItemID.Torch, 5 ),
            MakeTuple( "Jungle",     ItemID.Torch, 4 ),
            MakeTuple( "Grey",       ItemID.Torch, 3 ),
            MakeTuple( "Lava",       ItemID.Torch, 8 ),
            MakeTuple( "Corrupt",    ItemID.Torch, 2 ),
            MakeTuple( "Crim",       ItemID.Torch, 2 ),
            MakeTuple( "Dungeon",    ItemID.Torch, 3 ),
            MakeTuple( "Ice",        ItemID.Torch, 1 ),
            MakeTuple( "Illuminant", ItemID.Torch, 5 ),
            MakeTuple( "Sand",       ItemID.Torch, 3 ),
        };

        static internal List<Tuple<short, string, int>> ItemRecipesAddGel = new List<Tuple<short, string, int>>
        {
            MakeTuple(ItemID.SpelunkerPotion,     "Yellow", 2),
            MakeTuple(ItemID.GravitationPotion,   "Purple", 2),
            MakeTuple(ItemID.IronskinPotion,      "Yellow", 1),
            MakeTuple(ItemID.LifeforcePotion,     "Red",    2),
            MakeTuple(ItemID.SwiftnessPotion,     "Green",  1),
            MakeTuple(ItemID.RegenerationPotion,  "Red",    1),
            MakeTuple(ItemID.HealingPotion,       "Red",    1),
            MakeTuple(ItemID.GreaterHealingPotion,"Red",    2),
            MakeTuple(ItemID.SuperHealingPotion,  "Red",    3),
            MakeTuple(ItemID.EndurancePotion,     "Grey",   2),

            MakeTuple(ItemID.FlaskofFire,         "Lava",   4),
        };

        internal static void AddRecipes(Mod mod) 
        {
            RecipeFinder rf = new RecipeFinder();
            rf.SetResult(ItemID.Torch, 3);
            rf.AddIngredient(ItemID.Gel);
            
            new RecipeEditor(rf.SearchRecipes()[0]).DeleteRecipe();

            ModRecipe r = new ModRecipe(mod);
            bool overhaulMod = ModLoader.GetMod("TerrariaOverhaul") != null;

            foreach (Tuple<string, short, int> rc in GelMakeTorchesCount)
            {
                if (rc.Item1 == "Blue" && overhaulMod) continue;
                r.AddGelIngredient(rc.Item1);
                r.AddRecipeGroup(RecipeGroupID.Wood);
                r.SetResult(rc.Item2, Math.Max(overhaulMod? rc.Item3 / 2 : rc.Item3 ,1));
                r.AddRecipe();
                r = new ModRecipe(mod);
            }

            foreach (Tuple<short, string, int> rc in ItemRecipesAddGel) 
            {
                RecipeFinder finder = new RecipeFinder();
                finder.SetResult(rc.Item1);
                foreach (Recipe recipe in finder.SearchRecipes()) 
                {
                    recipe.AddIngredient(ColorfulGel.GetGelItem(rc.Item2, rc.Item3));
                }
            }

            r.AddGelIngredient("Ice");
            r.AddRecipeGroup("Wood");
            r.SetResult(ItemID.IceTorch, 3);
            r.AddRecipe();
        }

        static Tuple<T1, T2, T3> MakeTuple<T1, T2, T3>(T1 i1, T2 i2, T3 i3) => new Tuple<T1, T2, T3>(i1,i2,i3);
    }
}
