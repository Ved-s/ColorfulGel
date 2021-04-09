using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace ColorfulGel
{
    static class ItemSortingPatch
    {
        public static void ApplyPatch() 
        {
            On.Terraria.UI.ItemSorting.Sort += ItemSorting_Sort;
			
        }

        public static void RemovePatch() 
        {
            On.Terraria.UI.ItemSorting.Sort -= ItemSorting_Sort;
        }
        
        private static void ItemSorting_Sort(On.Terraria.UI.ItemSorting.orig_Sort orig, Terraria.Item[] inv, int[] ignoreSlots)
        {
			Sort(inv, ignoreSlots);
        }

		public static void Sort(Item[] inv, params int[] ignoreSlots)
		{
			SetupSortingPriorities();
			List<int> list = new List<int>();
			for (int i = 0; i < inv.Length; i++)
			{
				if (!ignoreSlots.Contains(i))
				{
					Item item = inv[i];
					if (item != null && item.stack != 0 && item.type != 0 && !item.favorited)
					{
						list.Add(i);
					}
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				Item item2 = inv[list[j]];
				if (item2.stack < item2.maxStack)
				{
					int num = item2.maxStack - item2.stack;
					for (int k = j; k < list.Count; k++)
					{
						if (j != k)
						{
							Item item3 = inv[list[k]];
							if (item2.IsTheSameAs(item3) && item3.stack != item3.maxStack)
							{
								int num2 = item3.stack;
								if (num < num2)
								{
									num2 = num;
								}
								item2.stack += num2;
								item3.stack -= num2;
								num -= num2;
								if (item3.stack == 0)
								{
									inv[list[k]] = new Item();
									list.Remove(list[k]);
									j--;
									k--;
									break;
								}
								if (num == 0)
								{
									break;
								}
							}
						}
					}
				}
			}
			List<int> list2 = new List<int>(list);
			for (int l = 0; l < inv.Length; l++)
			{
				if (!ignoreSlots.Contains(l) && !list2.Contains(l))
				{
					Item item4 = inv[l];
					if (item4 == null || item4.stack == 0 || item4.type == 0)
					{
						list2.Add(l);
					}
				}
			}
			list2.Sort();
			List<int> list3 = new List<int>();
			List<int> list4 = new List<int>();
			foreach (ItemSortingLayer itemSortingLayer in _layerList)
			{
				List<int> list5 = itemSortingLayer.SortingMethod(itemSortingLayer, inv, list);
				if (list5.Count > 0)
				{
					list4.Add(list5.Count);
				}
				list3.AddRange(list5);
			}
			list3.AddRange(list);
			List<Item> list6 = new List<Item>();
			foreach (int num3 in list3)
			{
				list6.Add(inv[num3]);
				inv[num3] = new Item();
			}
			float num4 = 1f / (float)list4.Count;
			float num5 = num4 / 2f;
			for (int m = 0; m < list6.Count; m++)
			{
				int num6 = list2[0];
				ItemSlot.SetGlow(num6, num5, Main.player[Main.myPlayer].chest != -1);
				List<int> list7 = list4;
				int num7 = list7[0];
				list7[0] = num7 - 1;
				if (list4[0] == 0)
				{
					list4.RemoveAt(0);
					num5 += num4;
				}
				inv[num6] = list6[m];
				list2.Remove(num6);
			}
		}
		public static void SetupSortingPriorities()
		{
			Player player = Main.player[Main.myPlayer];
			_layerList.Clear();
			List<float> list = new List<float>
			{
				player.meleeDamage,
				player.rangedDamage,
				player.magicDamage,
				player.minionDamage,
				player.thrownDamage
			};
			list.Sort((float x, float y) => y.CompareTo(x));
			for (int i = 0; i < 5; i++)
			{
				if (!_layerList.Contains(ItemSortingLayers.WeaponsMelee) && player.meleeDamage == list[0])
				{
					list.RemoveAt(0);
					_layerList.Add(ItemSortingLayers.WeaponsMelee);
				}
				if (!_layerList.Contains(ItemSortingLayers.WeaponsRanged) && player.rangedDamage == list[0])
				{
					list.RemoveAt(0);
					_layerList.Add(ItemSortingLayers.WeaponsRanged);
				}
				if (!_layerList.Contains(ItemSortingLayers.WeaponsMagic) && player.magicDamage == list[0])
				{
					list.RemoveAt(0);
					_layerList.Add(ItemSortingLayers.WeaponsMagic);
				}
				if (!_layerList.Contains(ItemSortingLayers.WeaponsMinions) && player.minionDamage == list[0])
				{
					list.RemoveAt(0);
					_layerList.Add(ItemSortingLayers.WeaponsMinions);
				}
				if (!_layerList.Contains(ItemSortingLayers.WeaponsThrown) && player.thrownDamage == list[0])
				{
					list.RemoveAt(0);
					_layerList.Add(ItemSortingLayers.WeaponsThrown);
				}
			}
			_layerList.Add(ItemSortingLayers.WeaponsAssorted);
			_layerList.Add(ItemSortingLayers.WeaponsAmmo);
			_layerList.Add(ItemSortingLayers.ToolsPicksaws);
			_layerList.Add(ItemSortingLayers.ToolsHamaxes);
			_layerList.Add(ItemSortingLayers.ToolsPickaxes);
			_layerList.Add(ItemSortingLayers.ToolsAxes);
			_layerList.Add(ItemSortingLayers.ToolsHammers);
			_layerList.Add(ItemSortingLayers.ToolsTerraforming);
			_layerList.Add(ItemSortingLayers.ToolsAmmoLeftovers);
			_layerList.Add(ItemSortingLayers.ArmorCombat);
			_layerList.Add(ItemSortingLayers.ArmorVanity);
			_layerList.Add(ItemSortingLayers.ArmorAccessories);
			_layerList.Add(ItemSortingLayers.EquipGrapple);
			_layerList.Add(ItemSortingLayers.EquipMount);
			_layerList.Add(ItemSortingLayers.EquipCart);
			_layerList.Add(ItemSortingLayers.EquipLightPet);
			_layerList.Add(ItemSortingLayers.EquipVanityPet);
			_layerList.Add(ItemSortingLayers.PotionsDyes);
			_layerList.Add(ItemSortingLayers.PotionsHairDyes);
			_layerList.Add(ItemSortingLayers.PotionsLife);
			_layerList.Add(ItemSortingLayers.PotionsMana);
			_layerList.Add(ItemSortingLayers.PotionsElixirs);
			_layerList.Add(ItemSortingLayers.PotionsBuffs);
			_layerList.Add(ItemSortingLayers.MiscValuables);
			_layerList.Add(ItemSortingLayers.MiscPainting);
			_layerList.Add(ItemSortingLayers.MiscWiring);
			_layerList.Add(ItemSortingLayers.MiscMaterials);
			_layerList.Add(ItemSortingLayers.MiscRopes);
			_layerList.Add(ItemSortingLayers.MiscExtractinator);
			_layerList.Add(ItemSortingLayers.LastMaterials);
			_layerList.Add(ItemSortingLayers.LastTilesImportant);
			_layerList.Add(ItemSortingLayers.LastTilesCommon);
			_layerList.Add(ItemSortingLayers.LastNotTrash);
			_layerList.Add(ItemSortingLayers.LastTrash);
		}
		public static List<ItemSortingLayer> _layerList = new List<ItemSortingLayer>();
		public static Dictionary<string, List<int>> _layerWhiteLists = new Dictionary<string, List<int>>();
		public class ItemSortingLayer
		{
			// Token: 0x06002675 RID: 9845 RVA: 0x0048D304 File Offset: 0x0048B504
			public ItemSortingLayer(string name, Func<ItemSortingLayer, Item[], List<int>, List<int>> method)
			{
				this.Name = name;
				this.SortingMethod = method;
			}

			// Token: 0x06002676 RID: 9846 RVA: 0x0048D31C File Offset: 0x0048B51C
			public void Validate(ref List<int> indexesSortable, Item[] inv)
			{
				List<int> list;
				if (_layerWhiteLists.TryGetValue(this.Name, out list))
				{
					indexesSortable = (from i in indexesSortable
									   where list.Contains(inv[i].netID)
									   select i).ToList<int>();
				}
			}

			// Token: 0x06002677 RID: 9847 RVA: 0x0048D368 File Offset: 0x0048B568
			public override string ToString()
			{
				return this.Name;
			}

			// Token: 0x0400414B RID: 16715
			public readonly string Name;

			// Token: 0x0400414C RID: 16716
			public readonly Func<ItemSortingLayer, Item[], List<int>, List<int>> SortingMethod;
		}
		public class ItemSortingLayers
		{
			// Token: 0x0400414D RID: 16717
			public static ItemSortingLayer WeaponsMelee = new ItemSortingLayer("Weapons - Melee", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].maxStack == 1 && inv[i].damage > 0 && inv[i].ammo == 0 && inv[i].melee && inv[i].pick < 1 && inv[i].hammer < 1 && inv[i].axe < 1
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[y].rare.CompareTo(inv[x].rare);
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x0400414E RID: 16718
			public static ItemSortingLayer WeaponsRanged = new ItemSortingLayer("Weapons - Ranged", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].maxStack == 1 && inv[i].damage > 0 && inv[i].ammo == 0 && inv[i].ranged
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[y].rare.CompareTo(inv[x].rare);
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x0400414F RID: 16719
			public static ItemSortingLayer WeaponsMagic = new ItemSortingLayer("Weapons - Magic", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].maxStack == 1 && inv[i].damage > 0 && inv[i].ammo == 0 && inv[i].magic
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[y].rare.CompareTo(inv[x].rare);
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x04004150 RID: 16720
			public static ItemSortingLayer WeaponsMinions = new ItemSortingLayer("Weapons - Minions", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].maxStack == 1 && inv[i].damage > 0 && inv[i].summon
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[y].rare.CompareTo(inv[x].rare);
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x04004151 RID: 16721
			public static ItemSortingLayer WeaponsThrown = new ItemSortingLayer("Weapons - Thrown", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].damage > 0 && (inv[i].ammo == 0 || inv[i].notAmmo) && inv[i].shoot > 0 && inv[i].thrown
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[y].rare.CompareTo(inv[x].rare);
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x04004152 RID: 16722
			public static ItemSortingLayer WeaponsAssorted = new ItemSortingLayer("Weapons - Assorted", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].damage > 0 && inv[i].ammo == 0 && inv[i].pick == 0 && inv[i].axe == 0 && inv[i].hammer == 0
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[y].rare.CompareTo(inv[x].rare);
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x04004153 RID: 16723
			public static ItemSortingLayer WeaponsAmmo = new ItemSortingLayer("Weapons - Ammo", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].ammo > 0 && inv[i].damage > 0
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[y].rare.CompareTo(inv[x].rare);
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x04004154 RID: 16724
			public static ItemSortingLayer ToolsPicksaws = new ItemSortingLayer("Tools - Picksaws", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].pick > 0 && inv[i].axe > 0
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort((int x, int y) => inv[x].pick.CompareTo(inv[y].pick));
				return list;
			});

			// Token: 0x04004155 RID: 16725
			public static ItemSortingLayer ToolsHamaxes = new ItemSortingLayer("Tools - Hamaxes", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].hammer > 0 && inv[i].axe > 0
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort((int x, int y) => inv[x].axe.CompareTo(inv[y].axe));
				return list;
			});

			// Token: 0x04004156 RID: 16726
			public static ItemSortingLayer ToolsPickaxes = new ItemSortingLayer("Tools - Pickaxes", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].pick > 0
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort((int x, int y) => inv[x].pick.CompareTo(inv[y].pick));
				return list;
			});

			// Token: 0x04004157 RID: 16727
			public static ItemSortingLayer ToolsAxes = new ItemSortingLayer("Tools - Axes", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].pick > 0
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort((int x, int y) => inv[x].axe.CompareTo(inv[y].axe));
				return list;
			});

			// Token: 0x04004158 RID: 16728
			public static ItemSortingLayer ToolsHammers = new ItemSortingLayer("Tools - Hammers", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].hammer > 0
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort((int x, int y) => inv[x].hammer.CompareTo(inv[y].hammer));
				return list;
			});

			// Token: 0x04004159 RID: 16729
			public static ItemSortingLayer ToolsTerraforming = new ItemSortingLayer("Tools - Terraforming", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].netID > 0 && ItemID.Sets.SortingPriorityTerraforming[inv[i].netID] > -1
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = ItemID.Sets.SortingPriorityTerraforming[inv[x].netID].CompareTo(ItemID.Sets.SortingPriorityTerraforming[inv[y].netID]);
					if (num == 0)
					{
						num = inv[y].stack.CompareTo(inv[x].stack);
					}
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x0400415A RID: 16730
			public static ItemSortingLayer ToolsAmmoLeftovers = new ItemSortingLayer("Weapons - Ammo Leftovers", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].ammo > 0
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[y].rare.CompareTo(inv[x].rare);
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x0400415B RID: 16731
			public static ItemSortingLayer ArmorCombat = new ItemSortingLayer("Armor - Combat", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where (inv[i].bodySlot >= 0 || inv[i].headSlot >= 0 || inv[i].legSlot >= 0) && !inv[i].vanity
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[y].rare.CompareTo(inv[x].rare);
					if (num == 0)
					{
						num = inv[x].netID.CompareTo(inv[y].netID);
					}
					return num;
				});
				return list;
			});

			// Token: 0x0400415C RID: 16732
			public static ItemSortingLayer ArmorVanity = new ItemSortingLayer("Armor - Vanity", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where (inv[i].bodySlot >= 0 || inv[i].headSlot >= 0 || inv[i].legSlot >= 0) && inv[i].vanity
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[y].rare.CompareTo(inv[x].rare);
					if (num == 0)
					{
						num = inv[x].netID.CompareTo(inv[y].netID);
					}
					return num;
				});
				return list;
			});

			// Token: 0x0400415D RID: 16733
			public static ItemSortingLayer ArmorAccessories = new ItemSortingLayer("Armor - Accessories", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].accessory
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[x].vanity.CompareTo(inv[y].vanity);
					if (num == 0)
					{
						num = inv[y].rare.CompareTo(inv[x].rare);
					}
					if (num == 0)
					{
						num = inv[x].netID.CompareTo(inv[y].netID);
					}
					return num;
				});
				return list;
			});

			// Token: 0x0400415E RID: 16734
			public static ItemSortingLayer EquipGrapple = new ItemSortingLayer("Equip - Grapple", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where Main.projHook[inv[i].shoot]
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[y].rare.CompareTo(inv[x].rare);
					if (num == 0)
					{
						num = inv[x].netID.CompareTo(inv[y].netID);
					}
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x0400415F RID: 16735
			public static ItemSortingLayer EquipMount = new ItemSortingLayer("Equip - Mount", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].mountType != -1 && !MountID.Sets.Cart[inv[i].mountType]
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[y].rare.CompareTo(inv[x].rare);
					if (num == 0)
					{
						num = inv[x].netID.CompareTo(inv[y].netID);
					}
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x04004160 RID: 16736
			public static ItemSortingLayer EquipCart = new ItemSortingLayer("Equip - Cart", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].mountType != -1 && MountID.Sets.Cart[inv[i].mountType]
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[y].rare.CompareTo(inv[x].rare);
					if (num == 0)
					{
						num = inv[x].netID.CompareTo(inv[y].netID);
					}
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x04004161 RID: 16737
			public static ItemSortingLayer EquipLightPet = new ItemSortingLayer("Equip - Light Pet", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].buffType > 0 && Main.lightPet[inv[i].buffType]
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[y].rare.CompareTo(inv[x].rare);
					if (num == 0)
					{
						num = inv[x].netID.CompareTo(inv[y].netID);
					}
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x04004162 RID: 16738
			public static ItemSortingLayer EquipVanityPet = new ItemSortingLayer("Equip - Vanity Pet", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].buffType > 0 && Main.vanityPet[inv[i].buffType]
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[y].rare.CompareTo(inv[x].rare);
					if (num == 0)
					{
						num = inv[x].netID.CompareTo(inv[y].netID);
					}
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x04004163 RID: 16739
			public static ItemSortingLayer PotionsLife = new ItemSortingLayer("Potions - Life", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].consumable && inv[i].healLife > 0 && inv[i].healMana < 1
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort((int x, int y) => inv[y].healLife.CompareTo(inv[x].healLife));
				return list;
			});

			// Token: 0x04004164 RID: 16740
			public static ItemSortingLayer PotionsMana = new ItemSortingLayer("Potions - Mana", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].consumable && inv[i].healLife < 1 && inv[i].healMana > 0
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort((int x, int y) => inv[y].healMana.CompareTo(inv[x].healMana));
				return list;
			});

			// Token: 0x04004165 RID: 16741
			public static ItemSortingLayer PotionsElixirs = new ItemSortingLayer("Potions - Elixirs", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].consumable && inv[i].healLife > 0 && inv[i].healMana > 0
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort((int x, int y) => inv[y].healLife.CompareTo(inv[x].healLife));
				return list;
			});

			// Token: 0x04004166 RID: 16742
			public static ItemSortingLayer PotionsBuffs = new ItemSortingLayer("Potions - Buffs", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].consumable && inv[i].buffType > 0
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[y].rare.CompareTo(inv[x].rare);
					if (num == 0)
					{
						num = inv[x].netID.CompareTo(inv[y].netID);
					}
					if (num == 0)
					{
						num = inv[y].stack.CompareTo(inv[x].stack);
					}
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x04004167 RID: 16743
			public static ItemSortingLayer PotionsDyes = new ItemSortingLayer("Potions - Dyes", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].dye > 0
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[y].rare.CompareTo(inv[x].rare);
					if (num == 0)
					{
						num = inv[y].dye.CompareTo(inv[x].dye);
					}
					if (num == 0)
					{
						num = inv[y].stack.CompareTo(inv[x].stack);
					}
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x04004168 RID: 16744
			public static ItemSortingLayer PotionsHairDyes = new ItemSortingLayer("Potions - Hair Dyes", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].hairDye >= 0
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[y].rare.CompareTo(inv[x].rare);
					if (num == 0)
					{
						num = inv[y].hairDye.CompareTo(inv[x].hairDye);
					}
					if (num == 0)
					{
						num = inv[y].stack.CompareTo(inv[x].stack);
					}
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x04004169 RID: 16745
			public static ItemSortingLayer MiscValuables = new ItemSortingLayer("Misc - Importants", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].netID > 0 && ItemID.Sets.SortingPriorityBossSpawns[inv[i].netID] > -1
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = ItemID.Sets.SortingPriorityBossSpawns[inv[x].netID].CompareTo(ItemID.Sets.SortingPriorityBossSpawns[inv[y].netID]);
					if (num == 0)
					{
						num = inv[y].stack.CompareTo(inv[x].stack);
					}
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x0400416A RID: 16746
			public static ItemSortingLayer MiscWiring = new ItemSortingLayer("Misc - Wiring", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where (inv[i].netID > 0 && ItemID.Sets.SortingPriorityWiring[inv[i].netID] > -1) || inv[i].mech
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = ItemID.Sets.SortingPriorityWiring[inv[y].netID].CompareTo(ItemID.Sets.SortingPriorityWiring[inv[x].netID]);
					if (num == 0)
					{
						num = inv[y].rare.CompareTo(inv[x].rare);
					}
					if (num == 0)
					{
						num = inv[y].netID.CompareTo(inv[x].netID);
					}
					if (num == 0)
					{
						num = inv[y].stack.CompareTo(inv[x].stack);
					}
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x0400416B RID: 16747
			public static ItemSortingLayer MiscMaterials = new ItemSortingLayer("Misc - Materials", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].netID > 0 && ItemID.Sets.SortingPriorityMaterials[inv[i].netID] > -1
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort((int x, int y) => ItemID.Sets.SortingPriorityMaterials[inv[y].netID].CompareTo(ItemID.Sets.SortingPriorityMaterials[inv[x].netID]));
				return list;
			});

			// Token: 0x0400416C RID: 16748
			public static ItemSortingLayer MiscExtractinator = new ItemSortingLayer("Misc - Extractinator", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].netID > 0 && ItemID.Sets.SortingPriorityExtractibles[inv[i].netID] > -1
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort((int x, int y) => ItemID.Sets.SortingPriorityExtractibles[inv[y].netID].CompareTo(ItemID.Sets.SortingPriorityExtractibles[inv[x].netID]));
				return list;
			});

			// Token: 0x0400416D RID: 16749
			public static ItemSortingLayer MiscPainting = new ItemSortingLayer("Misc - Painting", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where (inv[i].netID > 0 && ItemID.Sets.SortingPriorityPainting[inv[i].netID] > -1) || inv[i].paint > 0
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = ItemID.Sets.SortingPriorityPainting[inv[y].netID].CompareTo(ItemID.Sets.SortingPriorityPainting[inv[x].netID]);
					if (num == 0)
					{
						num = inv[x].paint.CompareTo(inv[y].paint);
					}
					if (num == 0)
					{
						num = inv[y].stack.CompareTo(inv[x].stack);
					}
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x0400416E RID: 16750
			public static ItemSortingLayer MiscRopes = new ItemSortingLayer("Misc - Ropes", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].netID > 0 && ItemID.Sets.SortingPriorityRopes[inv[i].netID] > -1
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort((int x, int y) => ItemID.Sets.SortingPriorityRopes[inv[y].netID].CompareTo(ItemID.Sets.SortingPriorityRopes[inv[x].netID]));
				return list;
			});

			// Token: 0x0400416F RID: 16751
			public static ItemSortingLayer LastMaterials = new ItemSortingLayer("Last - Materials", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].createTile < 0 && inv[i].createWall < 1
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[y].rare.CompareTo(inv[x].rare);
					if (num == 0)
					{
						num = inv[y].value.CompareTo(inv[x].value);
					}
					if (num == 0)
					{
						num = inv[y].stack.CompareTo(inv[x].stack);
					}
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x04004170 RID: 16752
			public static ItemSortingLayer LastTilesImportant = new ItemSortingLayer("Last - Tiles (Frame Important)", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].createTile >= 0 && Main.tileFrameImportant[inv[i].createTile]
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = string.Compare(inv[x].Name, inv[y].Name, StringComparison.OrdinalIgnoreCase);
					if (num == 0)
					{
						num = inv[y].stack.CompareTo(inv[x].stack);
					}
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x04004171 RID: 16753
			public static ItemSortingLayer LastTilesCommon = new ItemSortingLayer("Last - Tiles (Common), Walls", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].createWall > 0 || inv[i].createTile >= 0
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = string.Compare(inv[x].Name, inv[y].Name, StringComparison.OrdinalIgnoreCase);
					if (num == 0)
					{
						num = inv[y].stack.CompareTo(inv[x].stack);
					}
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x04004172 RID: 16754
			public static ItemSortingLayer LastNotTrash = new ItemSortingLayer("Last - Not Trash", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = (from i in itemsToSort
								  where inv[i].rare >= 0
								  select i).ToList<int>();
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[y].rare.CompareTo(inv[x].rare);
					if (num == 0)
					{
						num = string.Compare(inv[x].Name, inv[y].Name, StringComparison.OrdinalIgnoreCase);
					}
					if (num == 0)
					{
						num = inv[y].stack.CompareTo(inv[x].stack);
					}
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});

			// Token: 0x04004173 RID: 16755
			public static ItemSortingLayer LastTrash = new ItemSortingLayer("Last - Trash", delegate (ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
			{
				List<int> list = new List<int>(itemsToSort);
				layer.Validate(ref list, inv);
				foreach (int item in list)
				{
					itemsToSort.Remove(item);
				}
				list.Sort(delegate (int x, int y)
				{
					int num = inv[y].value.CompareTo(inv[x].value);
					if (num == 0)
					{
						num = inv[y].stack.CompareTo(inv[x].stack);
					}
					if (num == 0)
					{
						num = x.CompareTo(y);
					}
					return num;
				});
				return list;
			});
		}
	}
}
