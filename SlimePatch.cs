using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace ColorfulGel
{
    static class SlimePatch
    {
        internal static void ApplyPatch() 
        {
            On.Terraria.NPC.NPCLoot += NPC_NPCLoot;
        }
        internal static void RemovePatch()
        {
            On.Terraria.NPC.NPCLoot -= NPC_NPCLoot;
        }

        internal static Dictionary<int, string> PredefinedSlimeColors = new Dictionary<int, string>
        {
            {-5, "Grey"}, {-6, "Grey"}, {16, "Grey" }, {302, "Grey" },
            {81, "Corrupt"}, {-1, "Corrupt"},{-2, "Corrupt"},
            {183, "Crim"}, {-24, "Crim"},{-25, "Crim"},
            {71, "Dungeon" },
            {147, "Ice" }, { 184, "Ice" },
            {204, "Jungle" },
            {535, "Blue" }, {333, "Blue" },
            {334, "Green"},
            {335, "Red" },
            {336, "Yellow" },
            {138, "Illuminant" },
            {537, "Sand" },
            {59, "Lava"}
        };
        static List<int> SlimesDropNoGel = new List<int>() { 59, 71 };


        private static void NPC_NPCLoot(On.Terraria.NPC.orig_NPCLoot orig, NPC self)
        {
            orig(self);
            if (IsSlime(self)) OnSlimeLoot(self);
        }
        public static bool IsSlime(NPC npc) 
        {
            return npc.type == NPCID.BlueSlime || npc.type == NPCID.MotherSlime || npc.type == NPCID.IlluminantSlime || npc.type == NPCID.ToxicSludge || npc.type == NPCID.IceSlime || npc.type == NPCID.SpikedIceSlime || npc.type == NPCID.SlimedZombie || npc.type == NPCID.SpikedJungleSlime || npc.type == NPCID.SlimeMasked || (npc.type >= NPCID.SlimeRibbonWhite && npc.type <= NPCID.SlimeRibbonRed) || npc.type == NPCID.SlimeSpiked
            || PredefinedSlimeColors.ContainsKey(npc.netID);
            
                
        }
        public static void OnSlimeLoot(NPC npc) 
        {
            foreach (KeyValuePair<int, string> kvp in PredefinedSlimeColors)
            {
                if (npc.netID == kvp.Key)
                {
                    Item item;
                    if (SlimesDropNoGel.Contains(npc.netID)) 
                    {
                        item = Main.item[Item.NewItem(npc.position, ItemID.Gel)];
                    }
                    else item = SearchNewestItem(ItemID.Gel);
                    item.color = ColorfulGel.GelColors[kvp.Value];
                }
            }
        }
        public static Item SearchNewestItem(int type) 
        {
            int foundID = -1;
            int minTime = int.MaxValue;
            for (int j = 0; j < Main.item.Length; j++)
            {
                if (Main.item[j].spawnTime - Main.itemLockoutTime[j] < minTime && Main.item[j].type == type)
                {
                    minTime = Main.item[j].spawnTime - Main.itemLockoutTime[j];
                    foundID = j;
                }
            }
            return (foundID >= 0) ? Main.item[foundID] : null;
        }    
    }
}
