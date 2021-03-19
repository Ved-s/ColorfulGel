using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace ColorfulGel
{
    static class ItemPatch
    {
		public static void UpdateItem(Item thisItem, int i)
		{
			if (Main.itemLockoutTime[i] > 0)
			{
				Main.itemLockoutTime[i]--;
				return;
			}

            if (!thisItem.active)
			{
				return;
			}
			if (thisItem.instanced)
			{
				if (Main.netMode == NetmodeID.Server)
				{
                    thisItem.active = false;
					return;
				}
                thisItem.keepTime = 600;
			}
			if (Main.netMode == NetmodeID.SinglePlayer)
			{
                thisItem.owner = Main.myPlayer;
			}
			float num = 0.1f;
			float num2 = 7f;
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				int num3 = (int)(thisItem.position.X + (float)(thisItem.width / 2)) / 16;
				int num4 = (int)(thisItem.position.Y + (float)(thisItem.height / 2)) / 16;
				if (num3 >= 0 && num4 >= 0 && num3 < Main.maxTilesX && num4 < Main.maxTilesY && Main.tile[num3, num4] == null)
				{
					num = 0f;
                    thisItem.velocity.X = 0f;
                    thisItem.velocity.Y = 0f;
				}
			}
			if (thisItem.honeyWet)
			{
				num = 0.05f;
				num2 = 3f;
			}
			else if (thisItem.wet)
			{
				num2 = 5f;
				num = 0.08f;
			}
			if (thisItem.ownTime > 0)
			{
                thisItem.ownTime--;
			}
			else
			{
                thisItem.ownIgnore = -1;
			}
			if (thisItem.keepTime > 0)
			{
                thisItem.keepTime--;
			}
			if (thisItem.beingGrabbed)
			{
                thisItem.isBeingGrabbed = true;
			}
			else
			{
                thisItem.isBeingGrabbed = false;
			}
			Vector2 vector = thisItem.velocity * 0.5f;
			if (!thisItem.beingGrabbed)
			{
				bool flag = true;
				if (thisItem.type - 71 <= 3)
				{
					flag = false;
				}
				if (ItemID.Sets.NebulaPickup[thisItem.type])
				{
					flag = false;
				}
				if (thisItem.owner == Main.myPlayer && flag && (thisItem.createTile >= TileID.Dirt || thisItem.createWall > 0 || (thisItem.ammo > 0 && !thisItem.notAmmo) || thisItem.consumable || thisItem.type >= ItemID.EmptyBucket && thisItem.type <= ItemID.LavaBucket || thisItem.type == ItemID.HoneyBucket || thisItem.type == ItemID.Wire || thisItem.dye > 0 || thisItem.paint > 0 || thisItem.material) && thisItem.stack < thisItem.maxStack)
				{
					for (int j = i + 1; j < 400; j++)
					{
						if (Main.item[j].active && thisItem.IsTheSameAs(Main.item[j]) && Main.item[j].stack > 0 && Main.item[j].owner == thisItem.owner && Math.Abs(thisItem.position.X + (float)(thisItem.width / 2) - (Main.item[j].position.X + (float)(Main.item[j].width / 2))) + Math.Abs(thisItem.position.Y + (float)(thisItem.height / 2) - (Main.item[j].position.Y + (float)(Main.item[j].height / 2))) < 30f)
						{
                            thisItem.position = (thisItem.position + Main.item[j].position) / 2f;
                            thisItem.velocity = (thisItem.velocity + Main.item[j].velocity) / 2f;
							int num5 = Main.item[j].stack;
							if (num5 > thisItem.maxStack - thisItem.stack)
							{
								num5 = thisItem.maxStack - thisItem.stack;
							}
							Main.item[j].stack -= num5;
                            thisItem.stack += num5;
							if (Main.item[j].stack <= 0)
							{
								Main.item[j].SetDefaults(0, false);
								Main.item[j].active = false;
							}
							if (Main.netMode != NetmodeID.SinglePlayer && thisItem.owner == Main.myPlayer)
							{
								NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i, 0f, 0f, 0f, 0, 0, 0);
								NetMessage.SendData(MessageID.SyncItem, -1, -1, null, j, 0f, 0f, 0f, 0, 0, 0);
							}
						}
					}
				}
				if (Main.netMode != NetmodeID.Server && Main.expertMode && thisItem.owner == Main.myPlayer && thisItem.type >= ItemID.CopperCoin && thisItem.type <= ItemID.PlatinumCoin)
				{
					Rectangle rectangle = new Rectangle((int)thisItem.position.X, (int)thisItem.position.Y, thisItem.width, thisItem.height);
					for (int k = 0; k < 200; k++)
					{
						if (Main.npc[k].active && Main.npc[k].lifeMax > 5 && !Main.npc[k].friendly && !Main.npc[k].immortal && !Main.npc[k].dontTakeDamage)
						{
							float num6 = (float)thisItem.stack;
							float num7 = 1f;
							if (thisItem.type == ItemID.SilverCoin)
							{
								num7 = 100f;
							}
							if (thisItem.type == ItemID.GoldCoin)
							{
								num7 = 10000f;
							}
							if (thisItem.type == ItemID.PlatinumCoin)
							{
								num7 = 1000000f;
							}
							num6 *= num7;
							float extraValue = Main.npc[k].extraValue;
							int num8 = Main.npc[k].realLife;
							if (num8 >= 0 && Main.npc[num8].active)
							{
								extraValue = Main.npc[num8].extraValue;
							}
							else
							{
								num8 = -1;
							}
							if (extraValue < num6)
							{
								Rectangle rectangle2 = new Rectangle((int)Main.npc[k].position.X, (int)Main.npc[k].position.Y, Main.npc[k].width, Main.npc[k].height);
								if (rectangle.Intersects(rectangle2))
								{
									float num9 = (float)Main.rand.Next(50, 76) * 0.01f;
									if (thisItem.type == ItemID.CopperCoin)
									{
										num9 += (float)Main.rand.Next(51) * 0.01f;
									}
									if (thisItem.type == ItemID.SilverCoin)
									{
										num9 += (float)Main.rand.Next(26) * 0.01f;
									}
									if (num9 > 1f)
									{
										num9 = 1f;
									}
									int num10 = (int)((float)thisItem.stack * num9);
									if (num10 < 1)
									{
										num10 = 1;
									}
									if (num10 > thisItem.stack)
									{
										num10 = thisItem.stack;
									}
                                    thisItem.stack -= num10;
									float num11 = (float)num10 * num7;
									int num12 = k;
									if (num8 >= 0)
									{
										num12 = num8;
									}
									Main.npc[num12].extraValue += num11;
									if (Main.netMode == NetmodeID.SinglePlayer)
									{
                                        Main.npc[num12].moneyPing(thisItem.position);
									}
									else
									{
                                        NetMessage.SendData(MessageID.SyncExtraValue, -1, -1, null, num12, num11, thisItem.position.X, thisItem.position.Y, 0, 0, 0);
									}
									if (thisItem.stack <= 0)
									{
                                        thisItem.SetDefaults(0, false);
                                        thisItem.active = false;
									}
									NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i, 0f, 0f, 0f, 0, 0, 0);
								}
							}
						}
					}
				}
				ItemLoader.Update(thisItem, ref num, ref num2);
				if (ItemID.Sets.ItemNoGravity[thisItem.type])
				{
                    thisItem.velocity.X = thisItem.velocity.X * 0.95f;
					if ((double)thisItem.velocity.X < 0.1 && (double)thisItem.velocity.X > -0.1)
					{
                        thisItem.velocity.X = 0f;
					}
                    thisItem.velocity.Y = thisItem.velocity.Y * 0.95f;
					if ((double)thisItem.velocity.Y < 0.1 && (double)thisItem.velocity.Y > -0.1)
					{
                        thisItem.velocity.Y = 0f;
					}
				}
				else
				{
                    thisItem.velocity.Y = thisItem.velocity.Y + num;
					if (thisItem.velocity.Y > num2)
					{
                        thisItem.velocity.Y = num2;
					}
                    thisItem.velocity.X = thisItem.velocity.X * 0.95f;
					if ((double)thisItem.velocity.X < 0.1 && (double)thisItem.velocity.X > -0.1)
					{
                        thisItem.velocity.X = 0f;
					}
				}
				bool flag2 = Collision.LavaCollision(thisItem.position, thisItem.width, thisItem.height);
				if (flag2)
				{
                    thisItem.lavaWet = true;
				}
				bool flag3 = Collision.WetCollision(thisItem.position, thisItem.width, thisItem.height);
				if (Collision.honey)
				{
                    thisItem.honeyWet = true;
				}
				if (flag3)
				{
					if (!thisItem.wet)
					{
						if (thisItem.wetCount == 0)
						{
                            thisItem.wetCount = 20;
							if (!flag2)
							{
								if (thisItem.honeyWet)
								{
									for (int l = 0; l < 5; l++)
									{
										int num13 = Dust.NewDust(new Vector2(thisItem.position.X - 6f, thisItem.position.Y + (float)(thisItem.height / 2) - 8f), thisItem.width + 12, 24, 152, 0f, 0f, 0, default(Color), 1f);
										Dust dust = Main.dust[num13];
										dust.velocity.Y = dust.velocity.Y - 1f;
										Dust dust2 = Main.dust[num13];
										dust2.velocity.X = dust2.velocity.X * 2.5f;
										Main.dust[num13].scale = 1.3f;
										Main.dust[num13].alpha = 100;
										Main.dust[num13].noGravity = true;
									}
                                    Main.PlaySound(SoundID.Splash, (int)thisItem.position.X, (int)thisItem.position.Y, 1, 1f, 0f);
								}
								else
								{
									for (int m = 0; m < 10; m++)
									{
										int num14 = Dust.NewDust(new Vector2(thisItem.position.X - 6f, thisItem.position.Y + (float)(thisItem.height / 2) - 8f), thisItem.width + 12, 24, Dust.dustWater(), 0f, 0f, 0, default(Color), 1f);
										Dust dust3 = Main.dust[num14];
										dust3.velocity.Y = dust3.velocity.Y - 4f;
										Dust dust4 = Main.dust[num14];
										dust4.velocity.X = dust4.velocity.X * 2.5f;
										Main.dust[num14].scale *= 0.8f;
										Main.dust[num14].alpha = 100;
										Main.dust[num14].noGravity = true;
									}
                                    Main.PlaySound(SoundID.Splash, (int)thisItem.position.X, (int)thisItem.position.Y, 1, 1f, 0f);
								}
							}
							else
							{
								for (int n = 0; n < 5; n++)
								{
									int num15 = Dust.NewDust(new Vector2(thisItem.position.X - 6f, thisItem.position.Y + (float)(thisItem.height / 2) - 8f), thisItem.width + 12, 24, 35, 0f, 0f, 0, default(Color), 1f);
									Dust dust5 = Main.dust[num15];
									dust5.velocity.Y = dust5.velocity.Y - 1.5f;
									Dust dust6 = Main.dust[num15];
									dust6.velocity.X = dust6.velocity.X * 2.5f;
									Main.dust[num15].scale = 1.3f;
									Main.dust[num15].alpha = 100;
									Main.dust[num15].noGravity = true;
								}
                                Main.PlaySound(SoundID.Splash, (int)thisItem.position.X, (int)thisItem.position.Y, 1, 1f, 0f);
							}
						}
                        thisItem.wet = true;
					}
				}
				else if (thisItem.wet)
				{
                    thisItem.wet = false;
					byte wetCount = thisItem.wetCount;
				}
				if (!thisItem.wet)
				{
                    thisItem.lavaWet = false;
                    thisItem.honeyWet = false;
				}
				if (thisItem.wetCount > 0)
				{
                    thisItem.wetCount -= 1;
				}
				if (thisItem.wet)
				{
					if (thisItem.wet)
					{
						Vector2 velocity = thisItem.velocity;
                        thisItem.velocity = Collision.TileCollision(thisItem.position, thisItem.velocity, thisItem.width, thisItem.height, false, false, 1);
						if (thisItem.velocity.X != velocity.X)
						{
							vector.X = thisItem.velocity.X;
						}
						if (thisItem.velocity.Y != velocity.Y)
						{
							vector.Y = thisItem.velocity.Y;
						}
					}
				}
				else
				{
                    thisItem.velocity = Collision.TileCollision(thisItem.position, thisItem.velocity, thisItem.width, thisItem.height, false, false, 1);
				}
				Vector4 vector2 = Collision.SlopeCollision(thisItem.position, thisItem.velocity, thisItem.width, thisItem.height, num, false);
                thisItem.position.X = vector2.X;
                thisItem.position.Y = vector2.Y;
                thisItem.velocity.X = vector2.Z;
                thisItem.velocity.Y = vector2.W;
                Collision.StepConveyorBelt(thisItem, 1f);
				if (thisItem.lavaWet)
				{
					if (thisItem.type == ItemID.GuideVoodooDoll)
					{
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
                            thisItem.active = false;
                            thisItem.type = ItemID.None;
                            thisItem.stack = 0;
							for (int num16 = 0; num16 < 200; num16++)
							{
								if (Main.npc[num16].active && Main.npc[num16].type == NPCID.Guide)
								{
									if (Main.netMode == NetmodeID.Server)
									{
										NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, num16, 9999f, 10f, -(float)Main.npc[num16].direction, 0, 0, 0);
									}
									Main.npc[num16].StrikeNPCNoInteraction(9999, 10f, -Main.npc[num16].direction, false, false, false);
                                    NPC.SpawnWOF(thisItem.position);
								}
							}
							NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i, 0f, 0f, 0f, 0, 0, 0);
						}
					}
					else if (thisItem.owner == Main.myPlayer && thisItem.type != ItemID.FireblossomSeeds && thisItem.type != ItemID.Fireblossom && thisItem.type != ItemID.Obsidian && thisItem.type != ItemID.Hellstone && thisItem.type != ItemID.HellstoneBar && thisItem.type != ItemID.LivingFireBlock && thisItem.rare == ItemRarityID.White || ItemLoader.CanBurnInLava(thisItem))
					{
                        thisItem.active = false;
                        thisItem.type = ItemID.None;
                        thisItem.stack = 0;
						if (Main.netMode != NetmodeID.SinglePlayer)
						{
							NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i, 0f, 0f, 0f, 0, 0, 0);
						}
					}
				}
				if (thisItem.type == ItemID.EnchantedNightcrawler)
				{
					float num17 = (float)Main.rand.Next(90, 111) * 0.01f;
					num17 *= (Main.essScale + 0.5f) / 2f;
                    Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.3f * num17, 0.1f * num17, 0.25f * num17);
				}
				else if (thisItem.type == ItemID.SoulofLight || thisItem.type == ItemID.NebulaPickup2)
				{
					float num18 = (float)Main.rand.Next(90, 111) * 0.01f;
					num18 *= Main.essScale;
                    Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.5f * num18, 0.1f * num18, 0.25f * num18);
				}
				else if (thisItem.type == ItemID.SoulofNight || thisItem.type == ItemID.NebulaPickup3)
				{
					float num19 = (float)Main.rand.Next(90, 111) * 0.01f;
					num19 *= Main.essScale;
                    Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.25f * num19, 0.1f * num19, 0.5f * num19);
				}
				else if (thisItem.type == ItemID.SoulofFright || thisItem.type == ItemID.NebulaPickup1)
				{
					float num20 = (float)Main.rand.Next(90, 111) * 0.01f;
					num20 *= Main.essScale;
                    Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.5f * num20, 0.3f * num20, 0.05f * num20);
				}
				else if (thisItem.type == ItemID.SoulofMight)
				{
					float num21 = (float)Main.rand.Next(90, 111) * 0.01f;
					num21 *= Main.essScale;
                    Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.1f * num21, 0.1f * num21, 0.6f * num21);
				}
				else if (thisItem.type == ItemID.SoulofFlight)
				{
					float num22 = (float)Main.rand.Next(90, 111) * 0.01f;
					num22 *= Main.essScale;
                    Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.1f * num22, 0.3f * num22, 0.5f * num22);
				}
				else if (thisItem.type == ItemID.SoulofSight)
				{
					float num23 = (float)Main.rand.Next(90, 111) * 0.01f;
					num23 *= Main.essScale;
                    Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.1f * num23, 0.5f * num23, 0.2f * num23);
				}
				else if (thisItem.type == ItemID.Heart || thisItem.type == ItemID.CandyApple || thisItem.type == ItemID.CandyCane)
				{
					float num24 = (float)Main.rand.Next(90, 111) * 0.01f;
					num24 *= Main.essScale * 0.5f;
                    Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.5f * num24, 0.1f * num24, 0.1f * num24);
				}
				else if (thisItem.type == ItemID.Star || thisItem.type == ItemID.SoulCake || thisItem.type == ItemID.SugarPlum)
				{
					float num25 = (float)Main.rand.Next(90, 111) * 0.01f;
					num25 *= Main.essScale * 0.5f;
                    Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.1f * num25, 0.1f * num25, 0.5f * num25);
				}
				else if (thisItem.type == ItemID.CursedFlame)
				{
					float num26 = (float)Main.rand.Next(90, 111) * 0.01f;
					num26 *= Main.essScale * 0.2f;
                    Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.5f * num26, 1f * num26, 0.1f * num26);
				}
				else if (thisItem.type == ItemID.Ichor)
				{
					float num27 = (float)Main.rand.Next(90, 111) * 0.01f;
					num27 *= Main.essScale * 0.2f;
                    Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 1f * num27, 1f * num27, 0.1f * num27);
				}
				else if (thisItem.type == ItemID.FragmentVortex)
				{
					Lighting.AddLight(thisItem.Center, new Vector3(0.2f, 0.4f, 0.5f) * Main.essScale);
				}
				else if (thisItem.type == ItemID.FragmentNebula)
				{
					Lighting.AddLight(thisItem.Center, new Vector3(0.4f, 0.2f, 0.5f) * Main.essScale);
				}
				else if (thisItem.type == ItemID.FragmentSolar)
				{
					Lighting.AddLight(thisItem.Center, new Vector3(0.5f, 0.4f, 0.2f) * Main.essScale);
				}
				else if (thisItem.type == ItemID.FragmentStardust)
				{
					Lighting.AddLight(thisItem.Center, new Vector3(0.2f, 0.2f, 0.5f) * Main.essScale);
				}
				if (thisItem.type == ItemID.FallenStar && Main.dayTime)
				{
					for (int num28 = 0; num28 < 10; num28++)
					{
                        Dust.NewDust(thisItem.position, thisItem.width, thisItem.height, 15, thisItem.velocity.X, thisItem.velocity.Y, 150, default(Color), 1.2f);
					}
					for (int num29 = 0; num29 < 3; num29++)
					{
                        Gore.NewGore(thisItem.position, new Vector2(thisItem.velocity.X, thisItem.velocity.Y), Main.rand.Next(16, 18), 1f);
					}
                    thisItem.active = false;
                    thisItem.type = ItemID.None;
                    thisItem.stack = 0;
					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i, 0f, 0f, 0f, 0, 0, 0);
					}
				}
				if (thisItem.type == ItemID.DD2EnergyCrystal && !DD2Event.Ongoing)
				{
					int num30 = Main.rand.Next(18, 24);
					for (int num31 = 0; num31 < num30; num31++)
					{
						int num32 = Dust.NewDust(thisItem.Center, 0, 0, 61, 0f, 0f, 0, default(Color), 1.7f);
						Main.dust[num32].velocity *= 8f;
						Dust dust7 = Main.dust[num32];
						dust7.velocity.Y = dust7.velocity.Y - 1f;
						Main.dust[num32].position = Vector2.Lerp(Main.dust[num32].position, thisItem.Center, 0.5f);
						Main.dust[num32].noGravity = true;
						Main.dust[num32].noLight = true;
					}
                    thisItem.active = false;
                    thisItem.type = ItemID.None;
                    thisItem.stack = 0;
					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i, 0f, 0f, 0f, 0, 0, 0);
					}
				}
			}
			else
			{
                thisItem.beingGrabbed = false;
			}
			if (thisItem.type == ItemID.PixieDust)
			{
				if (Main.rand.Next(6) == 0)
				{
					int num33 = Dust.NewDust(thisItem.position, thisItem.width, thisItem.height, 55, 0f, 0f, 200, thisItem.color, 1f);
					Main.dust[num33].velocity *= 0.3f;
					Main.dust[num33].scale *= 0.5f;
				}
			}
			else if (thisItem.type == ItemID.DD2EnergyCrystal)
			{
				Lighting.AddLight(thisItem.Center, 0.1f, 0.3f, 0.1f);
			}
			else if (thisItem.type == ItemID.AmethystGemsparkBlock)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.75f, 0f, 0.75f);
			}
			else if (thisItem.type == ItemID.SapphireGemsparkBlock)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0f, 0f, 0.75f);
			}
			else if (thisItem.type == ItemID.TopazGemsparkBlock)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.75f, 0.75f, 0f);
			}
			else if (thisItem.type == ItemID.EmeraldGemsparkBlock)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0f, 0.75f, 0f);
			}
			else if (thisItem.type == ItemID.RubyGemsparkBlock)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.75f, 0f, 0f);
			}
			else if (thisItem.type == ItemID.DiamondGemsparkBlock)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.75f, 0.75f, 0.75f);
			}
			else if (thisItem.type == ItemID.AmberGemsparkBlock)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.75f, 0.375f, 0f);
			}
			else if (thisItem.type == ItemID.AmethystGemsparkWall)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.6f, 0f, 0.6f);
			}
			else if (thisItem.type == ItemID.SapphireGemsparkWall)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0f, 0f, 0.6f);
			}
			else if (thisItem.type == ItemID.TopazGemsparkWall)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.6f, 0.6f, 0f);
			}
			else if (thisItem.type == ItemID.EmeraldGemsparkWall)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0f, 0.6f, 0f);
			}
			else if (thisItem.type == ItemID.RubyGemsparkWall)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.6f, 0f, 0f);
			}
			else if (thisItem.type == ItemID.DiamondGemsparkWall)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.6f, 0.6f, 0.6f);
			}
			else if (thisItem.type == ItemID.AmberGemsparkWall)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.6f, 0.375f, 0f);
			}
			else if (thisItem.type == ItemID.Torch || thisItem.type == ItemID.Candle)
			{
				if (!thisItem.wet)
				{
                    Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 1f, 0.95f, 0.8f);
				}
			}
			else if (thisItem.type == ItemID.LivingFireBlock)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.7f, 0.65f, 0.55f);
			}
			else if (thisItem.type == ItemID.CursedTorch)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.85f, 1f, 0.7f);
			}
			else if (thisItem.type == ItemID.IceTorch)
			{
				if (!thisItem.wet)
				{
                    Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.7f, 0.85f, 1f);
				}
			}
			else if (thisItem.type == ItemID.IchorTorch)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 1.25f, 1.25f, 0.8f);
			}
			else if (thisItem.type == ItemID.RainbowTorch)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), (float)Main.DiscoR / 255f, (float)Main.DiscoG / 255f, (float)Main.DiscoB / 255f);
			}
			else if (thisItem.type == ItemID.BoneTorch)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.95f, 0.65f, 1.3f);
			}
			else if (thisItem.type == ItemID.UltrabrightTorch)
			{
				float r = 0.75f;
				float g = 1.3499999f;
				float b = 1.5f;
				if (!thisItem.wet)
				{
                    Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), r, g, b);
				}
			}
			else if (thisItem.type >= ItemID.BlueTorch && thisItem.type <= ItemID.YellowTorch)
			{
				if (!thisItem.wet)
				{
					float r2 = 0f;
					float g2 = 0f;
					float b2 = 0f;
					int num34 = thisItem.type - 426;
					if (num34 == 1)
					{
						r2 = 0.1f;
						g2 = 0.2f;
						b2 = 1.1f;
					}
					if (num34 == 2)
					{
						r2 = 1f;
						g2 = 0.1f;
						b2 = 0.1f;
					}
					if (num34 == 3)
					{
						r2 = 0f;
						g2 = 1f;
						b2 = 0.1f;
					}
					if (num34 == 4)
					{
						r2 = 0.9f;
						g2 = 0f;
						b2 = 0.9f;
					}
					if (num34 == 5)
					{
						r2 = 1.3f;
						g2 = 1.3f;
						b2 = 1.3f;
					}
					if (num34 == 6)
					{
						r2 = 0.9f;
						g2 = 0.9f;
						b2 = 0f;
					}
                    Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), r2, g2, b2);
				}
			}
			else if (thisItem.type == ItemID.NebulaAxe || thisItem.type == ItemID.NebulaChainsaw || thisItem.type == ItemID.NebulaDrill || thisItem.type == ItemID.NebulaHammer || thisItem.type == ItemID.NebulaPickaxe || thisItem.type == ItemID.NebulaHelmet || thisItem.type == ItemID.NebulaBreastplate || thisItem.type == ItemID.NebulaLeggings || thisItem.type == ItemID.LunarHamaxeNebula)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.4f, 0.16f, 0.36f);
			}
			else if (thisItem.type == ItemID.VortexAxe || thisItem.type == ItemID.VortexChainsaw || thisItem.type == ItemID.VortexDrill || thisItem.type == ItemID.VortexHammer || thisItem.type == ItemID.VortexPickaxe || thisItem.type == ItemID.VortexHelmet || thisItem.type == ItemID.VortexBreastplate || thisItem.type == ItemID.VortexLeggings || thisItem.type == ItemID.LunarHamaxeVortex)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0f, 0.36f, 0.4f);
			}
			else if (thisItem.type == ItemID.SolarFlareAxe || thisItem.type == ItemID.SolarFlareChainsaw || thisItem.type == ItemID.SolarFlareDrill || thisItem.type == ItemID.SolarFlareHammer || thisItem.type == ItemID.SolarFlarePickaxe || thisItem.type == ItemID.SolarFlareHelmet || thisItem.type == ItemID.SolarFlareBreastplate || thisItem.type == ItemID.SolarFlareLeggings || thisItem.type == ItemID.LunarHamaxeSolar)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)(thisItem.width / 2)) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.5f, 0.25f, 0.05f);
			}
			else if (thisItem.type == ItemID.StardustAxe || thisItem.type == ItemID.StardustChainsaw || thisItem.type == ItemID.StardustDrill || thisItem.type == ItemID.StardustHammer || thisItem.type == ItemID.StardustPickaxe || thisItem.type == ItemID.StardustHelmet || thisItem.type == ItemID.StardustBreastplate || thisItem.type == ItemID.StardustLeggings || thisItem.type == ItemID.LunarHamaxeStardust)
			{
				Lighting.AddLight(thisItem.Center, 0.3f, 0.3f, 0.2f);
			}
			else if (thisItem.type == ItemID.FlamingArrow)
			{
				if (!thisItem.wet)
				{
                    Lighting.AddLight((int)((thisItem.position.X + (float)thisItem.width) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 1f, 0.75f, 0.55f);
				}
			}
			else if (thisItem.type == ItemID.FrostburnArrow)
			{
				if (!thisItem.wet)
				{
                    Lighting.AddLight((int)((thisItem.position.X + (float)thisItem.width) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.35f, 0.65f, 1f);
				}
			}
			else if (thisItem.type == ItemID.Glowstick)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)thisItem.width) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.7f, 1f, 0.8f);
			}
			else if (thisItem.type == ItemID.StickyGlowstick)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)thisItem.width) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.7f, 0.8f, 1f);
			}
			else if (thisItem.type == ItemID.BouncyGlowstick)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)thisItem.width) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 1f, 0.6f, 0.85f);
			}
			else if (thisItem.type == ItemID.SpelunkerGlowstick)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)thisItem.width) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 1.05f, 0.95f, 0.55f);
			}
			else if (thisItem.type == ItemID.JungleSpores)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)thisItem.width) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.55f, 0.75f, 0.6f);
			}
			else if (thisItem.type == ItemID.GlowingMushroom)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)thisItem.width) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.15f, 0.45f, 0.9f);
			}
			else if (thisItem.type == ItemID.FallenStar)
			{
                Lighting.AddLight((int)((thisItem.position.X + (float)thisItem.width) / 16f), (int)((thisItem.position.Y + (float)(thisItem.height / 2)) / 16f), 0.8f, 0.7f, 0.1f);
			}
			if (thisItem.type == ItemID.FallenStar)
			{
				if (Main.rand.Next(25) == 0)
				{
                    Dust.NewDust(thisItem.position, thisItem.width, thisItem.height, 58, thisItem.velocity.X * 0.5f, thisItem.velocity.Y * 0.5f, 150, default(Color), 1.2f);
				}
				if (Main.rand.Next(50) == 0)
				{
                    Gore.NewGore(thisItem.position, new Vector2(thisItem.velocity.X * 0.2f, thisItem.velocity.Y * 0.2f), Main.rand.Next(16, 18), 1f);
				}
			}
			if (thisItem.spawnTime < 2147483646)
			{
				if (thisItem.type == ItemID.Heart || thisItem.type == ItemID.Star || thisItem.type == ItemID.CandyCane || thisItem.type == ItemID.SugarPlum || thisItem.type == ItemID.CandyApple || thisItem.type == ItemID.SoulCake)
				{
                    thisItem.spawnTime += 4;
				}
                thisItem.spawnTime++;
			}
			ItemLoader.PostUpdate(thisItem);
			if (Main.netMode == NetmodeID.Server && thisItem.owner != Main.myPlayer)
			{
                thisItem.release++;
				if (thisItem.release >= 300)
				{
                    thisItem.release = 0;
                    NetMessage.SendData(MessageID.ResetItemOwner, thisItem.owner, -1, null, i, 0f, 0f, 0f, 0, 0, 0);
				}
			}
			if (thisItem.wet)
			{
                thisItem.position += vector;
			}
			else
			{
                thisItem.position += thisItem.velocity;
			}
			if (thisItem.noGrabDelay > 0)
			{
                thisItem.noGrabDelay--;
			}
		}
		public static void ApplyPatch() 
		{
			On.Terraria.Item.IsTheSameAs += Item_IsTheSameAs;
			On.Terraria.Item.IsNotTheSameAs += Item_IsNotTheSameAs;
			On.Terraria.Item.UpdateItem += Item_UpdateItem;
		}
		public static void RemovePatch() 
		{
			On.Terraria.Item.IsTheSameAs -= Item_IsTheSameAs;
			On.Terraria.Item.IsNotTheSameAs -= Item_IsNotTheSameAs;
			On.Terraria.Item.UpdateItem -= Item_UpdateItem;
		}

		private static void Item_UpdateItem(On.Terraria.Item.orig_UpdateItem orig, Terraria.Item self, int i)
		{
			ItemPatch.UpdateItem(self, i);
		}
		private static bool Item_IsNotTheSameAs(On.Terraria.Item.orig_IsNotTheSameAs orig, Terraria.Item self, Terraria.Item compareItem)
		{
			bool b = orig(self, compareItem);
			if (self.type == ItemID.Gel && compareItem.type == Terraria.ID.ItemID.Gel)
			{
				return b && self.color != compareItem.color;
			}
			return b;
		}
		private static bool Item_IsTheSameAs(On.Terraria.Item.orig_IsTheSameAs orig, Terraria.Item self, Terraria.Item compareItem)
		{
			bool b = orig(self, compareItem);
			if (self.type == Terraria.ID.ItemID.Gel && compareItem.type == Terraria.ID.ItemID.Gel)
			{
				return b && self.color == compareItem.color;
			}
			return b;
		}
	}
}
