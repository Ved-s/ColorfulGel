using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ColorfulGel
{
    class GlobalGel : GlobalItem
    {
        public override TagCompound Save(Item item)
        {
            TagCompound tc = new TagCompound();
            tc.Add("color", item.color.PackedValue);
            return tc;
        }

        public override void Load(Item item, TagCompound tag)
        {
            item.color.PackedValue = tag.Get<uint>("color");
            ColorfulGel.SetGelItemColorName(item);
        }

        public override bool NeedsSaving(Item item)
        {
            return item.type == Terraria.ID.ItemID.Gel;
        }
    }
}
