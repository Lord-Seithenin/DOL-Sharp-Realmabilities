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
using System.Reflection;

using DOL.Database;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;
using DOL.GS.PropertyCalc;

using log4net;

namespace DOL.GS.Spells
{
	/// <summary>
	/// Spell to change up to 3 property bonuses at once
	/// in one their specific given bonus category
	/// </summary>
	public abstract class PropertyChangingSpell : SpellHandler
	{
		/// <summary>
		/// Execute property changing spell
		/// </summary>
		/// <param name="target"></param>
		public override void FinishSpellCast(GameLiving target)
		{
			m_caster.Mana -= CalculateNeededPower(target);
			base.FinishSpellCast(target);
		}

		/// <summary>
		/// start changing effect on target
		/// </summary>
		/// <param name="effect"></param>
		public override void OnEffectStart(GameSpellEffect effect)
		{
			IPropertyIndexer bonuscat = GetBonusCategory(effect.Owner, BonusCategory1);

			int amount = (int)(Spell.Value * effect.Effectiveness);

			bonuscat[(int)Property1] += amount;

			if (Property2 != eProperty.Undefined)
			{
				bonuscat = GetBonusCategory(effect.Owner, BonusCategory2);
				bonuscat[(int)Property2] += amount;
			}
			if (Property3 != eProperty.Undefined)
			{
				bonuscat = GetBonusCategory(effect.Owner, BonusCategory3);
				bonuscat[(int)Property3] += amount;
			}

			SendUpdates(effect.Owner);

			eChatType toLiving = eChatType.CT_SpellPulse;
			eChatType toOther = eChatType.CT_SpellPulse;
			if (Spell.Pulse == 0 || !HasPositiveEffect)
			{
				toLiving = eChatType.CT_Spell;
				toOther = eChatType.CT_System;
				SendEffectAnimation(effect.Owner, 0, false, 1);
			}

			//messages are after buff and after "Your xxx has increased." messages
			MessageToLiving(effect.Owner, Spell.Message1, toLiving);
			Message.SystemToArea(effect.Owner, Util.MakeSentence(Spell.Message2, effect.Owner.GetName(0, false)), toOther, effect.Owner);

			if (ServerProperties.Properties.BUFF_RANGE > 0 && effect.Spell.Concentration > 0 && effect.SpellHandler.HasPositiveEffect && effect.Owner != effect.SpellHandler.Caster)
			{
				m_buffCheckAction = new BuffCheckAction(effect.SpellHandler.Caster, effect.Owner, effect);
				m_buffCheckAction.Start(BuffCheckAction.BUFFCHECKINTERVAL);
			}
		}

		BuffCheckAction m_buffCheckAction = null;

		/// <summary>
		/// When an applied effect expires.
		/// Duration spells only.
		/// </summary>
		/// <param name="effect">The expired effect</param>
		/// <param name="noMessages">true, when no messages should be sent to player and surrounding</param>
		/// <returns>immunity duration in milliseconds</returns>
		public override int OnEffectExpires(GameSpellEffect effect, bool noMessages)
		{
			if (!noMessages && Spell.Pulse == 0)
			{
				MessageToLiving(effect.Owner, Spell.Message3, eChatType.CT_SpellExpires);
				Message.SystemToArea(effect.Owner, Util.MakeSentence(Spell.Message4, effect.Owner.GetName(0, false)), eChatType.CT_SpellExpires, effect.Owner);
			}

			IPropertyIndexer bonuscat = GetBonusCategory(effect.Owner, BonusCategory1);
			bonuscat[(int) Property1] -= (int) (Spell.Value*effect.Effectiveness);

			if (Property2 != eProperty.Undefined)
			{
				bonuscat = GetBonusCategory(effect.Owner, BonusCategory2);
				bonuscat[(int) Property2] -= (int) (Spell.Value*effect.Effectiveness);
			}
			if (Property3 != eProperty.Undefined)
			{
				bonuscat = GetBonusCategory(effect.Owner, BonusCategory3);
				bonuscat[(int) Property3] -= (int) (Spell.Value*effect.Effectiveness);
			}

			SendUpdates(effect.Owner);

			if (m_buffCheckAction != null)
			{
				m_buffCheckAction.Stop();
				m_buffCheckAction = null;
			}
			return 0;
		}

		protected virtual void SendUpdates(GameLiving target)
		{
		}

		protected IPropertyIndexer GetBonusCategory(GameLiving target, int categoryid)
		{
			IPropertyIndexer bonuscat = null;
			switch (categoryid)
			{
				case 1:
					bonuscat = target.BuffBonusCategory1;
					break;
				case 2:
					bonuscat = target.BuffBonusCategory2;
					break;
				case 3:
					bonuscat = target.BuffBonusCategory3;
					break;
				case 4:
					bonuscat = target.BuffBonusCategory4;
					break;
				default:
					if (log.IsErrorEnabled)
						log.Error("BonusCategory not found " + categoryid + "!");
					break;
			}
			return bonuscat;
		}

		/// <summary>
		/// Property 1 which bonus value has to be changed
		/// </summary>
		public abstract eProperty Property1 { get; }

		/// <summary>
		/// Property 2 which bonus value has to be changed
		/// </summary>
		public virtual eProperty Property2
		{
			get { return eProperty.Undefined; }
		}

		/// <summary>
		/// Property 3 which bonus value has to be changed
		/// </summary>
		public virtual eProperty Property3
		{
			get { return eProperty.Undefined; }
		}

		/// <summary>
		/// Bonus Category where to change the Property1
		/// </summary>
		public virtual int BonusCategory1
		{
			get { return 1; }
		}

		/// <summary>
		/// Bonus Category where to change the Property2
		/// </summary>
		public virtual int BonusCategory2
		{
			get { return 1; }
		}

		/// <summary>
		/// Bonus Category where to change the Property3
		/// </summary>
		public virtual int BonusCategory3
		{
			get { return 1; }
		}

		public override void OnEffectRestored(GameSpellEffect effect, int[] vars)
		{
			IPropertyIndexer bonuscat = GetBonusCategory(effect.Owner, BonusCategory1);
			bonuscat[(int)Property1] += vars[1];

			if (Property2 != eProperty.Undefined)
			{
				bonuscat = GetBonusCategory(effect.Owner, BonusCategory2);
				bonuscat[(int)Property2] += vars[1];
			}
			if (Property3 != eProperty.Undefined)
			{
				bonuscat = GetBonusCategory(effect.Owner, BonusCategory3);
				bonuscat[(int)Property3] += vars[1];
			}
			SendUpdates(effect.Owner);
		}

		public override int OnRestoredEffectExpires(GameSpellEffect effect, int[] vars, bool noMessages)
		{
			if (!noMessages && Spell.Pulse == 0)
			{
				MessageToLiving(effect.Owner, Spell.Message3, eChatType.CT_SpellExpires);
				Message.SystemToArea(effect.Owner, Util.MakeSentence(Spell.Message4, effect.Owner.GetName(0, false)), eChatType.CT_SpellExpires, effect.Owner);
			}

			IPropertyIndexer bonuscat = GetBonusCategory(effect.Owner, BonusCategory1);
			bonuscat[(int)Property1] -= vars[1];

			if (Property2 != eProperty.Undefined)
			{
				bonuscat = GetBonusCategory(effect.Owner, BonusCategory2);
				bonuscat[(int)Property2] -= vars[1];
			}
			if (Property3 != eProperty.Undefined)
			{
				bonuscat = GetBonusCategory(effect.Owner, BonusCategory3);
				bonuscat[(int)Property3] -= vars[1];
			}
			SendUpdates(effect.Owner);

			return 0;

		}

		public override PlayerXEffect getSavedEffect(GameSpellEffect e)
		{
			PlayerXEffect eff = new PlayerXEffect();
			eff.Var1 = Spell.ID;
			eff.Duration = e.RemainingTime;
			eff.IsHandler = true;
			eff.Var2 = (int)(Spell.Value * e.Effectiveness);
			eff.SpellLine = SpellLine.KeyName;
			return eff;

		}

		// constructor
		public PropertyChangingSpell(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line)
		{
		}
	}

	public class BuffCheckAction : RegionAction 
	{
		public const int BUFFCHECKINTERVAL = 60000;//60 seconds

		private GameLiving m_caster = null;
		private GameLiving m_owner = null;
		private GameSpellEffect m_effect = null;

		public BuffCheckAction(GameLiving caster, GameLiving owner, GameSpellEffect effect)
			: base(caster)
		{
			m_caster = caster;
			m_owner = owner;
			m_effect = effect;
		}

		/// <summary>
		/// Called on every timer tick
		/// </summary>
		protected override void OnTick()
		{
			if (m_caster == null ||
				m_owner == null ||
				m_effect == null)
				return;

			if (WorldMgr.GetDistance(m_caster, m_owner) > ServerProperties.Properties.BUFF_RANGE)
				m_effect.Cancel(false);
			else
				Start(BUFFCHECKINTERVAL);
		}
	}
}