using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace ColorfulGel
{
    static class TorchRecipes
    {
        static internal List<Tuple<string, short, int>> GelMakeTorchesCount = new List<Tuple<string, short, int>>
        {
            new Tuple<string, short, int>( "Default",    ItemID.Torch, 1 ),
            new Tuple<string, short, int>( "Red",        ItemID.Torch, 2 ),
            new Tuple<string, short, int>( "Green",      ItemID.Torch, 2 ),
            new Tuple<string, short, int>( "Blue",       ItemID.Torch, 3 ),
            new Tuple<string, short, int>( "Yellow",     ItemID.Torch, 2 ),
            new Tuple<string, short, int>( "Purple",     ItemID.Torch, 5 ),
            new Tuple<string, short, int>( "Jungle",     ItemID.Torch, 4 ),
            new Tuple<string, short, int>( "Grey",       ItemID.Torch, 3 ),
            new Tuple<string, short, int>( "Lava",       ItemID.Torch, 8 ),                                            
            new Tuple<string, short, int>( "Corrupt",    ItemID.Torch, 2 ),
            new Tuple<string, short, int>( "Crim",       ItemID.Torch, 2 ),
            new Tuple<string, short, int>( "Dungeon",    ItemID.Torch, 3 ),
            new Tuple<string, short, int>( "Ice",        ItemID.Torch, 1 ),
            new Tuple<string, short, int>( "Illuminant", ItemID.Torch, 5 ),
            new Tuple<string, short, int>( "Sand",       ItemID.Torch, 3 ),
        };

        internal static void AddRecipes(Mod mod) 
        {
            RecipeFinder rf = new RecipeFinder();
            rf.SetResult(ItemID.Torch, 3);
            rf.AddIngredient(ItemID.Gel);
            
            new RecipeEditor(rf.SearchRecipes()[0]).DeleteRecipe();

            ModRecipe r = new ModRecipe(mod);

            foreach (Tuple<string, short, int> rc in GelMakeTorchesCount)
            {
                r.AddGelIngredient(rc.Item1);
                r.AddRecipeGroup(RecipeGroupID.Wood);
                r.SetResult(rc.Item2, rc.Item3);
                r.AddRecipe();
                r = new ModRecipe(mod);
            }
        }
    }
}
