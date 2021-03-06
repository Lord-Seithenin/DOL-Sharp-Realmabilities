/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */
using System;
using System.Text;
using DOL.Database;

namespace DOL.GS
{
	/// <summary>
	/// 
	/// </summary>
	public class Spell : Skill
	{
		protected readonly string m_description = "";
		protected readonly string m_target = "";
		protected readonly string m_spelltype = "-";
		protected readonly int m_range = 0;
		protected readonly int m_radius = 0;
		protected readonly double m_value = 0;
		protected readonly double m_damage = 0;
		protected readonly eDamageType m_damageType = eDamageType.Natural;
		protected readonly byte m_concentration = 0;
		protected readonly int m_duration = 0;
		protected readonly int m_frequency = 0;
		protected readonly int m_pulse = 0;
		protected readonly int m_pulse_power = 0;
		protected readonly int m_power = 0;
		protected readonly int m_casttime = 0;
		protected readonly int m_recastdelay = 0;
		protected readonly int m_reshealth = 0;
		protected readonly int m_resmana = 0;
		protected readonly int m_lifedrain_return = 0;
		protected readonly int m_amnesia_chance = 0;
		protected readonly string m_message1 = "";
		protected readonly string m_message2 = "";
		protected readonly string m_message3 = "";
		protected readonly string m_message4 = "";
		protected readonly ushort m_effectID = 0;
		protected readonly ushort m_icon = 0;
		protected readonly int m_instrumentRequirement = 0;
		protected readonly int m_spellGroup = 0;
		protected readonly int m_effectGroup = 0;
		protected readonly int m_subSpellID = 0;
		protected readonly bool m_moveCast = false;
		protected readonly bool m_uninterruptible = false;


		#region member access properties

		public ushort ClientEffect
		{
			get { return m_effectID; }
		}

		public ushort Icon
		{
			get { return m_icon; }
		}

		public string Description
		{
			get { return m_description; }
		}

		public string Target
		{
			get { return m_target; }
		}

		public int Range
		{
			get { return m_range; }
		}

		public int Power
		{
			get { return m_power; }
		}

		public int CastTime
		{
			get { return m_casttime; }
		}

		public double Damage
		{
			get { return m_damage; }
		}

		public eDamageType DamageType
		{
			get { return m_damageType; }

		}

		public string SpellType
		{
			get { return m_spelltype; }
		}

		public int Duration
		{
			get { return m_duration; }
		}

		public int Frequency
		{
			get { return m_frequency; }
		}

		public int Pulse
		{
			get { return m_pulse; }
		}

		public int PulsePower
		{
			get { return m_pulse_power; }
		}

		public int Radius
		{
			get { return m_radius; }
		}

		public int RecastDelay
		{
			get { return m_recastdelay; }
		}

		public int ResurrectHealth
		{
			get { return m_reshealth; }
		}

		public int ResurrectMana
		{
			get { return m_resmana; }
		}

		public double Value
		{
			get { return m_value; }
		}

		public byte Concentration
		{
			get { return m_concentration; }
		}

		public int LifeDrainReturn
		{
			get { return m_lifedrain_return; }
		}

		public int AmnesiaChance
		{
			get { return m_amnesia_chance; }
		}

		public string Message1 { get { return m_message1; } }
		public string Message2 { get { return m_message2; } }
		public string Message3 { get { return m_message3; } }
		public string Message4 { get { return m_message4; } }

		public override eSkillPage SkillType
		{
			get { return eSkillPage.Spells; }
		}

		public int InstrumentRequirement
		{
			get { return m_instrumentRequirement; }
		}

		public int Group
		{
			get { return m_spellGroup; }
		}

		public int EffectGroup
		{
			get { return m_effectGroup; }
		}

		public int SubSpellID
		{
			get { return m_subSpellID; }
		}

		public bool MoveCast
		{
			get { return m_moveCast; }
		}

		public bool Uninterruptible
		{
			get { return m_uninterruptible; }
		}

		#endregion

		/// <summary>
		/// Returns the string representation of the Spell
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return new StringBuilder(32)
				.Append("Name=").Append(Name)
				.Append(", ID=").Append(ID)
				.Append(", SpellType=").Append(SpellType)
				.ToString();
		}

		public Spell(DBSpell dbspell, int requiredLevel)
			: base(dbspell.Name, (ushort)dbspell.SpellID, requiredLevel)
		{

			m_description = dbspell.Description;
			m_target = dbspell.Target;
			m_spelltype = dbspell.Type;
			m_range = dbspell.Range;
			m_radius = dbspell.Radius;
			m_value = dbspell.Value;
			m_damage = dbspell.Damage;
			m_damageType = (eDamageType)dbspell.DamageType;
			m_concentration = (byte)dbspell.Concentration;
			m_duration = dbspell.Duration * 1000;
			m_frequency = dbspell.Frequency * 100;
			m_pulse = dbspell.Pulse;
			m_pulse_power = dbspell.PulsePower;
			m_power = dbspell.Power;
			m_casttime = (int)(dbspell.CastTime * 1000);
			m_recastdelay = dbspell.RecastDelay * 1000;
			m_reshealth = dbspell.ResurrectHealth;
			m_resmana = dbspell.ResurrectMana;
			m_lifedrain_return = dbspell.LifeDrainReturn;
			m_amnesia_chance = dbspell.AmnesiaChance;
			m_message1 = dbspell.Message1;
			m_message2 = dbspell.Message2;
			m_message3 = dbspell.Message3;
			m_message4 = dbspell.Message4;
			m_effectID = (ushort)dbspell.ClientEffect;
			m_icon = (ushort)dbspell.Icon;
			m_instrumentRequirement = dbspell.InstrumentRequirement;
			m_spellGroup = dbspell.SpellGroup;
			m_effectGroup = dbspell.EffectGroup;
			m_subSpellID = dbspell.SubSpellID;
			m_moveCast = dbspell.MoveCast;
			m_uninterruptible = dbspell.Uninterruptible;
		}
	}
}
