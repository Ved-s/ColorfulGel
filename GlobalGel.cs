using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
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
        }
        public override bool NeedsSaving(Item item)
        {
            return item.type == Terraria.ID.ItemID.Gel;
        }
        public override void NetSend(Item item, BinaryWriter writer)
        {
            if (item.type == ItemID.Gel)
            {
                writer.WriteRGB(item.color);
                writer.Write(item.color.A);
            }
        }
        public override void NetReceive(Item item, BinaryReader reader)
        {
            if (item.type == ItemID.Gel)
            {
                item.color = reader.ReadRGB();
                item.color.A = reader.ReadByte();
            }
        }
        
    }
}
