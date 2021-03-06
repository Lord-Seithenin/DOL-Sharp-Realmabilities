using DOL.AI.Brain;
using DOL.GS.PacketHandler;

namespace DOL.GS.Keeps
{
	public class TemplateMgr
	{
		public static void RefreshTemplate(GameKeepGuard guard)
		{
			SetGuardRealm(guard);
			SetGuardGuild(guard);
			SetGuardRespawn(guard);
			SetGuardGender(guard);
			SetGuardModel(guard);
			SetGuardName(guard);
			SetBlockEvadeParryChance(guard);
			SetGuardBrain(guard);
			SetGuardSpeed(guard);
			SetGuardLevel(guard);
			SetGuardResists(guard);
			ClothingMgr.EquipGuard(guard);
			ClothingMgr.SetEmblem(guard);
		}

		private static void SetGuardRealm(GameKeepGuard guard)
		{
			if (guard.Component != null)
				guard.Realm = guard.Component.Keep.Realm;
			else guard.Realm = (byte)guard.CurrentZone.GetRealm();
		}

		private static void SetGuardGuild(GameKeepGuard guard)
		{
			if (guard.Component == null)
				guard.GuildName = "";
			else if (guard.Component.Keep.Guild == null)
				guard.GuildName = "";
			else guard.GuildName = guard.Component.Keep.Guild.Name;
		}

		private static void SetGuardRespawn(GameKeepGuard guard)
		{
			if (guard is GuardLord || guard is FrontierHastener)
				guard.RespawnInterval = 1000;
			else guard.RespawnInterval = Util.Random(5, 25) * 60 * 1000;
		}

		private static byte GetBaseLevel(GameKeepGuard guard)
		{
			if (guard.Component == null)
			{
				if (guard is GuardLord)
					return 75;
				else return 65;
			}
			if (guard is GuardLord)
			{
				if (guard.Component.Keep is GameKeep)
					return (byte)(guard.Component.Keep.BaseLevel + ((guard.Component.Keep.BaseLevel / 10) + 1) * 2);
				else return (byte)(guard.Component.Keep.BaseLevel + ((guard.Component.Keep.BaseLevel / 10) + 1));
			}
			return guard.Component.Keep.BaseLevel;
		}

		public static void SetGuardLevel(GameKeepGuard guard)
		{
			if (guard is FrontierHastener)
			{
				guard.Level = 1;
			}
			else
			{
				int bonusLevel = 0;
				if (guard.Component != null)
					bonusLevel = guard.Component.Keep.Level;
				guard.Level = (byte)(GetBaseLevel(guard) + bonusLevel);
			}
		}

		private static void SetGuardGender(GameKeepGuard guard)
		{
			if (Util.Chance(50))
				guard.IsMale = true;
			else guard.IsMale = false;
		}

		/// <summary>
		/// Sets a guards model
		/// </summary>
		/// <param name="guard">The guard object</param>
		private static void SetGuardModel(GameKeepGuard guard)
		{
			ushort AlbionHastener = 244;
			ushort MidgardHastener = 16;
			ushort HiberniaHastener = 1910;

			if (guard is FrontierHastener)
			{
				switch (guard.Realm)
				{
					case 0:
					case 1:
						{
							guard.Model = AlbionHastener;
							guard.Size = 35;
							break;
						}
					case 2:
						{
							guard.Model = MidgardHastener;
							guard.Size = 30;
							guard.Flags ^= (uint)GameNPC.eFlags.TRANSPARENT;
							break;
						}
					case 3:
						{
							guard.Model = HiberniaHastener;
							guard.Size = 50;
							break;
						}
				}
				return;
			}

			#region AlbionClassModels
			ushort BritonMale = 32;
			ushort BritonFemale = 35;
			ushort HighlanderMale = 39;
			ushort HighlanderFemale = 43;
			ushort SaracenMale = 48;
			ushort SaracenFemale = 52;
			ushort AvalonianMale = 61;
			ushort AvalonianFemale = 65;
			ushort IcconuMale = 716;
			ushort IcconuFemale = 724;
			ushort HalfOgreMale = 1008;
			ushort HalfOgreFemale = 1020;
			#endregion
			#region MidgardClassModels
			ushort TrollMale = 137;
			ushort TrollFemale = 145;
			ushort NorseMale = 503;
			ushort NorseFemale = 507;
			ushort KoboldMale = 169;
			ushort KoboldFemale = 177;
			ushort DwarfMale = 185;
			ushort DwarfFemale = 193;
			ushort ValkynMale = 773;
			ushort ValkynFemale = 781;
			ushort FrostalfMale = 1051;
			ushort FrostalfFemale = 1063;
			#endregion
			#region HiberniaClassModels
			ushort FirbolgMale = 286;
			ushort FirbolgFemale = 294;
			ushort CeltMale = 302;
			ushort CeltFemale = 310;
			ushort LurikeenMale = 318;
			ushort LurikeenFemale = 326;
			ushort ElfMale = 334;
			ushort ElfFemale = 342;
			ushort SharMale = 1075;
			ushort SharFemale = 1087;
			ushort SylvianMale = 700;
			ushort SylvianFemale = 708;
			#endregion

			switch ((eRealm)guard.Realm)
			{
				#region None
				case eRealm.None:
				#endregion
				#region Albion
				case eRealm.Albion:
					{
						if (guard is GuardArcher)
						{
							if (guard.IsMale)
							{
								switch (Util.Random(0, 3))
								{
									case 0: guard.Model = SaracenMale; break;//Saracen Male
									case 1: guard.Model = HighlanderMale; break;//Highlander Male
									case 2: guard.Model = BritonMale; break;//Briton Male
									case 3: guard.Model = IcconuMale; break;//Icconu Male
								}
							}
							else
							{
								switch (Util.Random(0, 3))
								{
									case 0: guard.Model = SaracenFemale; break;//Saracen Female
									case 1: guard.Model = HighlanderFemale; break;//Highlander Female
									case 2: guard.Model = BritonFemale; break;//Briton Female
									case 3: guard.Model = IcconuFemale; break;//Icconu Female
								}
							}
						}
						else if (guard is GuardCaster)
						{
							if (guard.IsMale)
							{
								switch (Util.Random(0, 2))
								{
									case 0: guard.Model = AvalonianMale; break;//Avalonian Male
									case 1: guard.Model = BritonMale; break;//Briton Male
									case 2: guard.Model = HalfOgreMale; break;//Half Ogre Male
								}
							}
							else
							{
								switch (Util.Random(0, 2))
								{
									case 0: guard.Model = AvalonianFemale; break;//Avalonian Female
									case 1: guard.Model = BritonFemale; break;//Briton Female
									case 2: guard.Model = HalfOgreFemale; break;//Half Ogre Female
								}
							}
						}
						else if (guard is GuardFighter)
						{
							if (guard.IsMale)
							{
								switch (Util.Random(0, 5))
								{
									case 0: guard.Model = HighlanderMale; break;//Highlander Male
									case 1: guard.Model = BritonMale; break;//Briton Male
									case 2: guard.Model = SaracenMale; break;//Saracen Male
									case 3: guard.Model = AvalonianMale; break;//Avalonian Male
									case 4: guard.Model = HalfOgreMale; break;//Half Ogre Male
									case 5: guard.Model = IcconuMale; break;//Icconu Male
								}
							}
							else
							{
								switch (Util.Random(0, 5))
								{
									case 0: guard.Model = HighlanderFemale; break;//Highlander Female
									case 1: guard.Model = BritonFemale; break;//Briton Female
									case 2: guard.Model = SaracenFemale; break;//Saracen Female
									case 3: guard.Model = AvalonianFemale; break;//Avalonian Female
									case 4: guard.Model = HalfOgreFemale; break;//Half Ogre Female
									case 5: guard.Model = IcconuFemale; break;//Icconu Female
								}
							}
						}
						else if (guard is GuardHealer)
						{
							if (guard.IsMale)
							{
								switch (Util.Random(0, 2))
								{
									case 0: guard.Model = HighlanderMale; break;//Highlander Male
									case 1: guard.Model = BritonMale; break;//Briton Male
									case 2: guard.Model = AvalonianMale; break;//Avalonian Male
								}
							}
							else
							{
								switch (Util.Random(0, 2))
								{
									case 0: guard.Model = HighlanderFemale; break;//Highlander Female
									case 1: guard.Model = BritonFemale; break;//Briton Female
									case 2: guard.Model = AvalonianFemale; break;//Avalonian Female
								}
							}
						}
						else if (guard is GuardLord)
						{
							if (guard.IsMale)
							{
								switch (Util.Random(0, 2))
								{
									case 0: guard.Model = HighlanderMale; break;//Highlander Male
									case 1: guard.Model = BritonMale; break;//Briton Male
									case 2: guard.Model = AvalonianMale; break;//Avalonian Male
								}
							}
							else
							{
								switch (Util.Random(0, 2))
								{
									case 0: guard.Model = HighlanderFemale; break;//Highlander Female
									case 1: guard.Model = BritonFemale; break;//Briton Female
									case 2: guard.Model = AvalonianFemale; break;//Avalonian Female
								}
							}
						}
						else if (guard is GuardStealther)
						{
							if (guard.IsMale)
							{
								switch (Util.Random(0, 2))
								{
									case 0: guard.Model = SaracenMale; break;//Saracen Male
									case 1: guard.Model = BritonMale; break;//Briton Male
									case 2: guard.Model = IcconuMale; break;//Icconu Male
								}
							}
							else
							{
								switch (Util.Random(0, 2))
								{
									case 0: guard.Model = SaracenFemale; break;//Saracen Female
									case 1: guard.Model = BritonFemale; break;//Briton Female
									case 2: guard.Model = IcconuFemale; break;//Icconu Female
								}
							}
						}
						break;
					}
				#endregion
				#region Midgard
				case eRealm.Midgard:
					{
						if (guard is GuardArcher)
						{
							if (guard.IsMale)
							{
								switch (Util.Random(0, 4))
								{
									case 0: guard.Model = NorseMale; break;//Norse Male
									case 1: guard.Model = KoboldMale; break;//Kobold Male
									case 2: guard.Model = DwarfMale; break;//Dwarf Male
									case 3: guard.Model = ValkynMale; break;//Valkyn Male
									case 4: guard.Model = FrostalfMale; break;//Frostalf Male
								}
							}
							else
							{
								switch (Util.Random(0, 4))
								{
									case 0: guard.Model = NorseFemale; break;//Norse Female
									case 1: guard.Model = KoboldFemale; break;//Kobold Female
									case 2: guard.Model = DwarfFemale; break;//Dwarf Female
									case 3: guard.Model = ValkynFemale; break;//Valkyn Female
									case 4: guard.Model = FrostalfFemale; break;//Frostalf Female
								}
							}
						}
						else if (guard is GuardCaster)
						{
							if (guard.IsMale)
							{
								switch (Util.Random(0, 3))
								{
									case 0: guard.Model = KoboldMale; break;//Kobold Male
									case 1: guard.Model = NorseMale; break;//Norse Male
									case 2: guard.Model = DwarfMale; break;//Dwarf Male
									case 3: guard.Model = FrostalfMale; break;//Frostalf Male
								}
							}
							else
							{
								switch (Util.Random(0, 3))
								{
									case 0: guard.Model = KoboldFemale; break;//Kobold Female
									case 1: guard.Model = NorseFemale; break;//Norse Female
									case 2: guard.Model = DwarfFemale; break;//Dwarf Female
									case 3: guard.Model = FrostalfFemale; break;//Frostalf Female
								}
							}
						}
						else if (guard is GuardFighter)
						{
							if (guard.IsMale)
							{
								switch (Util.Random(0, 4))
								{
									case 0: guard.Model = TrollMale; break;//Troll Male
									case 1: guard.Model = NorseMale; break;//Norse Male
									case 2: guard.Model = DwarfMale; break;//Dwarf Male
									case 3: guard.Model = KoboldMale; break;//Kobold Male
									case 4: guard.Model = ValkynMale; break;//Valkyn Male
								}
							}
							else
							{
								switch (Util.Random(0, 4))
								{
									case 0: guard.Model = TrollFemale; break;//Troll Female
									case 1: guard.Model = NorseFemale; break;//Norse Female
									case 2: guard.Model = DwarfFemale; break;//Dwarf Female
									case 3: guard.Model = KoboldFemale; break;//Kobold Female
									case 4: guard.Model = ValkynFemale; break;//Valkyn Female
								}
							}
						}
						else if (guard is GuardHealer)
						{
							if (guard.IsMale)
							{
								switch (Util.Random(0, 2))
								{
									case 0: guard.Model = DwarfMale; break;//Dwarf Male
									case 1: guard.Model = NorseMale; break;//Norse Male
									case 2: guard.Model = FrostalfMale; break;//Frostalf Male
								}
							}
							else
							{
								switch (Util.Random(0, 2))
								{
									case 0: guard.Model = DwarfFemale; break;//Dwarf Female
									case 1: guard.Model = NorseFemale; break;//Norse Female
									case 2: guard.Model = FrostalfFemale; break;//Frostalf Female
								}
							}
						}
						else if (guard is GuardLord)
						{
							if (guard.IsMale)
							{
								switch (Util.Random(0, 3))
								{
									case 0: guard.Model = DwarfMale; break;//Dwarf Male
									case 1: guard.Model = NorseMale; break;//Norse Male
									case 2: guard.Model = TrollMale; break;//Troll Male
									case 3: guard.Model = KoboldMale; break;//Kobold Male
								}
							}
							else
							{
								switch (Util.Random(0, 3))
								{
									case 0: guard.Model = DwarfFemale; break;//Dwarf Female
									case 1: guard.Model = NorseFemale; break;//Norse Female
									case 2: guard.Model = TrollFemale; break;//Troll Female
									case 3: guard.Model = KoboldFemale; break;//Kobold Female
								}
							}
						}
						else if (guard is GuardStealther)
						{
							if (guard.IsMale)
							{
								switch (Util.Random(0, 2))
								{
									case 0: guard.Model = KoboldMale; break;//Kobold Male
									case 1: guard.Model = NorseMale; break;//Norse Male
									case 2: guard.Model = ValkynMale; break;//Valkyn Male
								}
							}
							else
							{
								switch (Util.Random(0, 2))
								{
									case 0: guard.Model = KoboldFemale; break;//Kobold Female
									case 1: guard.Model = NorseFemale; break;//Norse Female
									case 2: guard.Model = ValkynFemale; break;//Valkyn Female
								}
							}
						}
						break;
					}
				#endregion
				#region Hibernia
				case eRealm.Hibernia:
					{
						if (guard is GuardArcher)
						{
							if (guard.IsMale)
							{
								switch (Util.Random(0, 3))
								{
									case 0: guard.Model = LurikeenMale; break;//Lurikeen Male
									case 1: guard.Model = ElfMale; break;//Elf Male
									case 2: guard.Model = CeltMale; break;//Celt Male
									case 3: guard.Model = SharMale; break;//Shar Male
								}
							}
							else
							{
								switch (Util.Random(0, 3))
								{
									case 0: guard.Model = LurikeenFemale; break;//Lurikeen Female
									case 1: guard.Model = ElfFemale; break;//Elf Female
									case 2: guard.Model = CeltFemale; break;//Celt Female
									case 3: guard.Model = SharFemale; break;//Shar Female
								}
							}
						}
						else if (guard is GuardCaster)
						{
							if (guard.IsMale)
							{
								switch (Util.Random(0, 1))
								{
									case 0: guard.Model = ElfMale; break;//Elf Male
									case 1: guard.Model = LurikeenMale; break;//Lurikeen Male
								}
							}
							else
							{
								switch (Util.Random(0, 1))
								{
									case 0: guard.Model = ElfFemale; break;//Elf Female
									case 1: guard.Model = LurikeenFemale; break;//Lurikeen Female
								}
							}
						}
						else if (guard is GuardFighter)
						{
							if (guard.IsMale)
							{
								switch (Util.Random(0, 3))
								{
									case 0: guard.Model = FirbolgMale; break;//Firbolg Male
									case 1: guard.Model = LurikeenMale; break;//Lurikeen Male
									case 2: guard.Model = CeltMale; break;//Celt Male
									case 3: guard.Model = SharMale; break;//Shar Male
								}
							}
							else
							{
								switch (Util.Random(0, 3))
								{
									case 0: guard.Model = FirbolgFemale; break;//Firbolg Female
									case 1: guard.Model = LurikeenFemale; break;//Lurikeen Female
									case 2: guard.Model = CeltFemale; break;//Celt Female
									case 3: guard.Model = SharFemale; break;//Shar Female
								}
							}
						}
						else if (guard is GuardHealer)
						{
							if (guard.IsMale)
							{
								switch (Util.Random(0, 2))
								{
									case 0: guard.Model = CeltMale; break;//Celt Male
									case 1: guard.Model = FirbolgMale; break;//Firbolg Male
									case 2: guard.Model = SylvianMale; break;//Sylvian Male
								}
							}
							else
							{
								switch (Util.Random(0, 2))
								{
									case 0: guard.Model = CeltFemale; break;//Celt Female
									case 1: guard.Model = FirbolgFemale; break;//Firbolg Female
									case 2: guard.Model = SylvianFemale; break;//Sylvian Female
								}
							}
						}
						else if (guard is GuardLord)
						{
							if (guard.IsMale)
							{
								switch (Util.Random(0, 3))
								{
									case 0: guard.Model = CeltMale; break;//Celt Male
									case 1: guard.Model = FirbolgMale; break;//Firbolg Male
									case 2: guard.Model = LurikeenMale; break;//Lurikeen Male
									case 3: guard.Model = ElfMale; break;//Elf Male
								}
							}
							else
							{
								switch (Util.Random(0, 3))
								{
									case 0: guard.Model = CeltFemale; break;//Celt Female
									case 1: guard.Model = FirbolgFemale; break;//Firbolg Female
									case 2: guard.Model = LurikeenFemale; break;//Lurikeen Female
									case 3: guard.Model = ElfFemale; break;//Elf Female
								}
							}
						}
						else if (guard is GuardStealther)
						{
							if (guard.IsMale)
							{
								switch (Util.Random(0, 1))
								{
									case 0: guard.Model = ElfMale; break;//Elf Male
									case 1: guard.Model = LurikeenMale; break;//Lurikeen Male
								}
							}
							else
							{
								switch (Util.Random(0, 1))
								{
									case 0: guard.Model = ElfFemale; break;//Elf Female
									case 1: guard.Model = LurikeenFemale; break;//Lurikeen Female
								}
							}
						}
						break;
					}
				#endregion
			}
		}

		/// <summary>
		/// Sets a guards name
		/// </summary>
		/// <param name="guard">The guard object</param>
		private static void SetGuardName(GameKeepGuard guard)
		{
			if (guard is FrontierHastener)
			{
				guard.Name = "Hastener";
				return;
			}
			if (guard is GuardLord)
			{
				if (guard.Component == null)
				{
					guard.Name = "Commander of " + guard.CurrentZone.Description;
					return;
				}
				else if (guard.IsTowerGuard)
				{
					guard.Name = "Tower Captain";
					return;
				}
			}
			switch ((eRealm)guard.Realm)
			{
				#region Albion / None
				case eRealm.None:
				case eRealm.Albion:
					{
						if (guard is GuardArcher)
						{
							guard.Name = "Scout";
						}
						else if (guard is GuardCaster)
						{
							guard.Name = "Wizard";
						}
						else if (guard is GuardFighter)
						{
							if (guard.IsMale)
								guard.Name = "Armsman";
							else guard.Name = "Armswoman";
						}
						else if (guard is GuardHealer)
						{
							guard.Name = "Cleric";
						}
						else if (guard is GuardLord)
						{
							if (guard.IsMale)
								guard.Name = guard.Component.Keep.Name + " Lord";
							else guard.Name = guard.Component.Keep.Name + " Lady";
						}
						else if (guard is GuardStealther)
						{
							guard.Name = "Infiltrator";
						}
						break;
					}
				#endregion
				#region Midgard
				case eRealm.Midgard:
					{
						if (guard is GuardArcher)
						{
							guard.Name = "Hunter";
						}
						else if (guard is GuardCaster)
						{
							guard.Name = "Runemaster";
						}
						else if (guard is GuardFighter)
						{
							guard.Name = "Huscarl";
						}
						else if (guard is GuardHealer)
						{
							guard.Name = "Healer";
						}
						else if (guard is GuardLord)
						{
							guard.Name = guard.Component.Keep.Name + " Jarl";
						}
						else if (guard is GuardStealther)
						{
							guard.Name = "Shadowblade";
						}
						break;
					}
				#endregion
				#region Hibernia
				case eRealm.Hibernia:
					{
						if (guard is GuardArcher)
						{
							guard.Name = "Ranger";
						}
						else if (guard is GuardCaster)
						{
							guard.Name = "Eldritch";
						}
						else if (guard is GuardFighter)
						{
							guard.Name = "Guardian";
						}
						else if (guard is GuardHealer)
						{
							guard.Name = "Druid";
						}
						else if (guard is GuardLord)
						{
							guard.Name = guard.Component.Keep.Name + " Chieftain";
						}
						else if (guard is GuardStealther)
						{
							guard.Name = "Nightshade";
						}
						break;
					}
				#endregion
			}
			if ((eRealm)guard.Realm == eRealm.None)
				guard.Name = "Renegade " + guard.Name;
			if (guard.IsPortalKeepGuard)
				guard.Name = "Master " + guard.Name;
		}

		/// <summary>
		/// Sets a guards Block, Parry and Evade change
		/// </summary>
		/// <param name="guard">The guard object</param>
		private static void SetBlockEvadeParryChance(GameKeepGuard guard)
		{
			if (guard is GuardLord)
			{
				guard.BlockChance = 10;
				guard.EvadeChance = 10;
				guard.ParryChance = 10;
			}
			else if (guard is GuardStealther)
			{
				guard.EvadeChance = 30;
			}
			else if (guard is GuardFighter)
			{
				guard.BlockChance = 10;
				guard.ParryChance = 10;
			}
			else if (guard is GuardHealer)
			{
				guard.BlockChance = 10;
			}
			else if (guard is GuardArcher)
			{
				if (guard.Realm == (byte)eRealm.Albion)
					guard.BlockChance = 10;
				guard.EvadeChance = 10;
			}
		}

		/// <summary>
		/// Sets the guards brain
		/// </summary>
		/// <param name="guard">The guard object</param>
		public static void SetGuardBrain(GameKeepGuard guard)
		{
			if (guard.Brain is KeepGuardBrain)
				return;
			KeepGuardBrain brain = new KeepGuardBrain();
			if (guard is GuardCaster)
				brain = new CasterBrain();
			else if (guard is GuardHealer)
				brain = new HealerBrain();
			else if (guard is GuardLord)
				brain = new LordBrain();
			guard.AddBrain(brain);
			brain.guard = guard;
		}

		/// <summary>
		/// Sets the guards speed
		/// </summary>
		/// <param name="guard">The guard object</param>
		public static void SetGuardSpeed(GameKeepGuard guard)
		{
			if (guard.IsPortalKeepGuard)
				guard.MaxSpeedBase = 575;
			if ((guard is GuardLord && guard.Component != null)||
	guard is GuardStaticArcher ||
	guard is GuardStaticCaster)
				guard.MaxSpeedBase = 0;
			else guard.MaxSpeedBase = 350;
		}

		/// <summary>
		/// Sets a guards resists
		/// </summary>
		/// <param name="guard">The guard object</param>
		private static void SetGuardResists(GameKeepGuard guard)
		{
			for (int i = (int)eProperty.Resist_First; i <= (int)eProperty.Resist_Last; i++)
			{
				if (guard is GuardLord)
					guard.BuffBonusCategory1[i] = 35;
				else guard.BuffBonusCategory1[i] = 26;
			}
		}
	}
}
