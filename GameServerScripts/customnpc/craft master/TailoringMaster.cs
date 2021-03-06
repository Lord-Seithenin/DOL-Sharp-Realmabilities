 //using DOL.Database;
//using DOL.GS.PacketHandler;

namespace DOL.GS.Scripts
{
	/// <summary>
	/// the master for armorcrafting
	/// </summary>
	[NPCGuildScript("Tailors Master")]
	public class TailoringMaster : CraftNPC
	{
		private static readonly eCraftingSkill[] m_trainedSkills = 
		{
			eCraftingSkill.ArmorCrafting,
			eCraftingSkill.ClothWorking,
			eCraftingSkill.Fletching,
			eCraftingSkill.LeatherCrafting,
			eCraftingSkill.SiegeCrafting,
			eCraftingSkill.Tailoring,
			eCraftingSkill.WeaponCrafting,
			eCraftingSkill.MetalWorking,
		};

		public override eCraftingSkill[] TrainedSkills
		{
			get { return m_trainedSkills; }
		}

		public override string GUILD_ORDER
		{
			get { return "Tailoring"; }
		}

		public override string GUILD_CRAFTERS
		{
			get { return "Tailors"; }
		}

		public override eCraftingSkill TheCraftingSkill
		{
			get { return eCraftingSkill.Tailoring; }
		}

		public override string InitialEntersentence
		{
			get { return "Would you like to join the Order of [" + GUILD_ORDER + "]? As a Taylor you can expect to sew cloth and leather armor. While you will excel in Tayloring and have good skills in Fletching, you can expect great Difficulty in Weapons crafting and Armor Crafting. A well trained Taylor also has a small bit of skill to perform Siege Crafting should it be needed "; }
		}
	}
}