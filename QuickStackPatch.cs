using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;

namespace ColorfulGel
{
    static class QuickStackPatch
    {
		public static void ApplyPatch() 
		{
			On.Terraria.UI.ChestUI.QuickStack += QuickStack;
			On.Terraria.Chest.PutItemInNearbyChest += PutItemInNearbyChest;
		}
		public static void RemovePatch()
		{
			On.Terraria.UI.ChestUI.QuickStack -= QuickStack;
			On.Terraria.Chest.PutItemInNearbyChest -= PutItemInNearbyChest;
		}


		public static void QuickStack(On.Terraria.UI.ChestUI.orig_QuickStack orig)
		{
			Player player = Main.player[Main.myPlayer];
			if (player.chest == -4)
			{
				ChestUI.MoveCoins(player.inventory, player.bank3.item);
			}
			else if (player.chest == -3)
			{
				ChestUI.MoveCoins(player.inventory, player.bank2.item);
			}
			else if (player.chest == -2)
			{
				ChestUI.MoveCoins(player.inventory, player.bank.item);
			}
			Item[] inventory = player.inventory;
			Item[] openChest = player.bank.item;
			if (player.chest > -1)
			{
				openChest = Main.chest[player.chest].item;
			}
			else if (player.chest == -2)
			{
				openChest = player.bank.item;
			}
			else if (player.chest == -3)
			{
				openChest = player.bank2.item;
			}
			else if (player.chest == -4)
			{
				openChest = player.bank3.item;
			}
			List<Item> possibleStackItems = new List<Item>();
			List<int>  possibleStackSlots = new List<int>();
			List<int>  quickStackIgnoreSlots = new List<int>();
			Dictionary<int, Item> dictionary = new Dictionary<int, Item>();
			List<int> removeItems = new List<int>();
			bool[] quickStacked = new bool[openChest.Length];
			for (int i = 0; i < 40; i++)
			{
				if (openChest[i].type > 0 && openChest[i].stack > 0 && openChest[i].maxStack > 1 && (openChest[i].type < 71 || openChest[i].type > 74))
				{
					possibleStackSlots.Add(i);
					possibleStackItems.Add(openChest[i]);
				}
				if (openChest[i].type == 0 || openChest[i].stack <= 0)
				{
					quickStackIgnoreSlots.Add(i);
				}
			}
			int num = 50;
			if (player.chest <= -2)
			{
				num += 4;
			}
			for (int j = 0; j < num; j++)
			{
				foreach (Item itm in possibleStackItems)
				{
					if (inventory[j].IsTheSameAs(itm) && !inventory[j].favorited)
					{
						dictionary.Add(j, inventory[j]);
					}
				}
			}
			for (int k = 0; k < possibleStackSlots.Count; k++)
			{
				int num2 = possibleStackSlots[k];
				Item itm2 = openChest[num2];
				foreach (KeyValuePair<int, Item> keyValuePair in dictionary)
				{
					if (keyValuePair.Value.IsTheSameAs(itm2) && inventory[keyValuePair.Key].IsTheSameAs(itm2))
					{
						int num3 = inventory[keyValuePair.Key].stack;
						int num4 = openChest[num2].maxStack - openChest[num2].stack;
						if (num4 == 0)
						{
							break;
						}
						if (num3 > num4)
						{
							num3 = num4;
						}
						Main.PlaySound(7, -1, -1, 1, 1f, 0f);
						openChest[num2].stack += num3;
						inventory[keyValuePair.Key].stack -= num3;
						if (inventory[keyValuePair.Key].stack == 0)
						{
							inventory[keyValuePair.Key].SetDefaults(0, false);
						}
						quickStacked[num2] = true;
					}
				}
			}
			foreach (KeyValuePair<int, Item> keyValuePair2 in dictionary)
			{
				if (inventory[keyValuePair2.Key].stack == 0)
				{
					removeItems.Add(keyValuePair2.Key);
				}
			}
			foreach (int key in removeItems)
			{
				dictionary.Remove(key);
			}
			for (int l = 0; l < quickStackIgnoreSlots.Count; l++)
			{
				int num5 = quickStackIgnoreSlots[l];
				bool flag = true;
				Item num6 = openChest[num5];
				foreach (KeyValuePair<int, Item> keyValuePair3 in dictionary)
				{
					if ((keyValuePair3.Value.IsTheSameAs(num6) && inventory[keyValuePair3.Key].IsTheSameAs(num6)) || (flag && inventory[keyValuePair3.Key].stack > 0))
					{
						Main.PlaySound(7, -1, -1, 1, 1f, 0f);
						if (flag)
						{
							num6 = keyValuePair3.Value;
							openChest[num5] = inventory[keyValuePair3.Key];
							inventory[keyValuePair3.Key] = new Item();
						}
						else
						{
							int num7 = inventory[keyValuePair3.Key].stack;
							int num8 = openChest[num5].maxStack - openChest[num5].stack;
							if (num8 == 0)
							{
								break;
							}
							if (num7 > num8)
							{
								num7 = num8;
							}
							openChest[num5].stack += num7;
							inventory[keyValuePair3.Key].stack -= num7;
							if (inventory[keyValuePair3.Key].stack == 0)
							{
								inventory[keyValuePair3.Key] = new Item();
							}
						}
						quickStacked[num5] = true;
						flag = false;
					}
				}
			}
			if (Main.netMode == 1 && player.chest >= 0)
			{
				for (int m = 0; m < quickStacked.Length; m++)
				{
					NetMessage.SendData(32, -1, -1, null, player.chest, (float)m, 0f, 0f, 0, 0, 0);
				}
			}
			possibleStackItems.Clear();
			possibleStackSlots.Clear();
			quickStackIgnoreSlots.Clear();
			dictionary.Clear();
			removeItems.Clear();
		}
		public static Item PutItemInNearbyChest(On.Terraria.Chest.orig_PutItemInNearbyChest orig, Item item, Vector2 position)
		{
			if (Main.netMode == 1)
			{
				return item;
			}
			for (int i = 0; i < 1000; i++)
			{
				bool flag = false;
				bool flag2 = false;
				if (Main.chest[i] != null && !IsPlayerInChest(i) && !Chest.isLocked(Main.chest[i].x, Main.chest[i].y) && (new Vector2((float)(Main.chest[i].x * 16 + 16), (float)(Main.chest[i].y * 16 + 16)) - position).Length() < 200f)
				{
					for (int j = 0; j < Main.chest[i].item.Length; j++)
					{
						if (Main.chest[i].item[j].type > 0 && Main.chest[i].item[j].stack > 0)
						{
							if (item.IsTheSameAs(Main.chest[i].item[j]))
							{
								flag = true;
								int num = Main.chest[i].item[j].maxStack - Main.chest[i].item[j].stack;
								if (num > 0)
								{
									if (num > item.stack)
									{
										num = item.stack;
									}
									item.stack -= num;
									Main.chest[i].item[j].stack += num;
									if (item.stack <= 0)
									{
										item.SetDefaults(0, false);
										return item;
									}
								}
							}
						}
						else
						{
							flag2 = true;
						}
					}
					if (flag && flag2 && item.stack > 0)
					{
						for (int k = 0; k < Main.chest[i].item.Length; k++)
						{
							if (Main.chest[i].item[k].type == 0 || Main.chest[i].item[k].stack == 0)
							{
								Main.chest[i].item[k] = item.Clone();
								item.SetDefaults(0, false);
								return item;
							}
						}
					}
				}
			}
			return item;
		}
		public static bool IsPlayerInChest(int i)
		{
			for (int j = 0; j < 255; j++)
			{
				if (Main.player[j].chest == i)
				{
					return true;
				}
			}
			return false;
		}

	}
}
