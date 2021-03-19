using Microsoft.Xna.Framework;
using Terraria.ID;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace ColorfulGel
{
	public class ColorfulGel : Mod
	{
        public static Dictionary<string, Color> GelColors = new Dictionary<string, Color>()
        {            
            { "Red",        new Color(255, 30,  0,   100)},
            { "Blue",       new Color(0,   80,  255, 100)},
            { "Green",      new Color(0,   220, 40,  100)},
            { "Yellow",     new Color(255, 255, 0,   100)},
            { "Purple",     new Color(200, 0,   255, 150)},
            { "Jungle",     new Color(143, 215, 93,  100)},
                                           
            { "Lava",       new Color(247, 127, 62,  150)},
            { "Grey",       new Color(80,  80,  80,  150)},
            { "Corrupt",    new Color(131, 73,  245, 150)},
            { "Crim",       new Color(117, 16,  16,  150)},
            { "Dungeon",    new Color(37,  73,  89,  150)},
            { "Ice",        new Color(139, 203, 232, 150)},
            { "Illuminant", new Color(230, 151, 220, 150)},
            { "Sand",       new Color(222, 180, 118, 150)},

            { "Default" , default }
        };

        public override void Load()
        {
            base.Load();
            ItemPatch.ApplyPatch();
            SlimePatch.ApplyPatch();
        }
        public override void Unload()
        {
            base.Unload();
            ItemPatch.RemovePatch();
            SlimePatch.RemovePatch();
        }
        public override void AddRecipes()
        {
            base.AddRecipes();

            TorchRecipes.AddRecipes(this);
        }

        public static void SetGelItemColorName(Item item) 
        {
            if (item.type != ItemID.Gel) return;
            string gelcolor = null;
            foreach (KeyValuePair<string, Color> kvp in ColorfulGel.GelColors)
            {
                if (item.color == kvp.Value)
                {
                    gelcolor = kvp.Key;
                    break;
                }
            }
            if (gelcolor != null) item.SetNameOverride(Lang.GetItemNameValue(ItemID.Gel) + " (" + gelcolor + ")");
        }

}

}