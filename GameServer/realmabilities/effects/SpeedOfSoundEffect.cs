using System;
using System.Collections;
using DOL.GS.PacketHandler;
using DOL.GS.SkillHandler;
using DOL.GS.PropertyCalc;
using DOL.Events;

namespace DOL.GS.Effects
{
	/// <summary>
	/// Effect handler for Barrier Of Fortitude
	/// </summary> 
	public class SpeedOfSoundEffect : StaticEffect, IGameEffect
	{
		private const String m_delveString = "Gives immunity to stun/snare/root and mesmerize spells and provides unbreakeable speed.";
		private GamePlayer m_player;
		private Int32 m_effectDuration;
		private RegionTimer m_expireTimer;
		private UInt16 m_id;

		DOLEventHandler m_attackFinished = new DOLEventHandler(AttackFinished);


		/// <summary>
		/// Called when effect is to be started
		/// </summary>
		/// <param name="player">The player to start the effect for</param>
		/// <param name="duration">The effectduration in secounds</param>
		public void Start(GamePlayer player, int duration)
		{
			m_player = player;
			m_effectDuration = duration;

			StartTimers();
			m_player.TempProperties.setProperty("Charging", true);
			GameEventMgr.AddHandler(m_player, GameLivingEvent.AttackFinished, m_attackFinished);
			GameEventMgr.AddHandler(m_player, GameLivingEvent.CastFinished, m_attackFinished);
			m_player.BuffBonusMultCategory1.Set((int)eProperty.MaxSpeed, this, PropertyCalc.MaxSpeedCalculator.SPEED4);		
			m_player.Out.SendUpdateMaxSpeed();

			m_player.EffectList.Add(this);
		}

		/// <summary>
		/// Called when the effectowner attacked an enemy
		/// </summary>
		/// <param name="e">The event which was raised</param>
		/// <param name="sender">Sender of the event</param>
		/// <param name="args">EventArgs associated with the event</param>
		private  static void AttackFinished(DOLEvent e, object sender, EventArgs args)
		{
			GamePlayer player = (GamePlayer)sender;
			if (e == GameLivingEvent.CastFinished)
			{
				CastSpellEventArgs cfea = args as CastSpellEventArgs;

				if (cfea.SpellHandler.Caster != player)
					return;

				//cancel if the effectowner casts a non-positive spell
				if (!cfea.SpellHandler.HasPositiveEffect)
				{
					SpeedOfSoundEffect effect = (SpeedOfSoundEffect)player.EffectList.GetOfType(typeof(SpeedOfSoundEffect));
					if (effect != null)
						effect.Cancel(false);
				}
			}
			else if (e == GameLivingEvent.AttackFinished)
			{
				AttackFinishedEventArgs afargs = args as AttackFinishedEventArgs;
				if (afargs == null)
					return;

				if (afargs.AttackData.Attacker != player)
					return;

				switch (afargs.AttackData.AttackResult)
				{
					case GameLiving.eAttackResult.HitStyle:
					case GameLiving.eAttackResult.HitUnstyled:
					case GameLiving.eAttackResult.Blocked:
					case GameLiving.eAttackResult.Evaded:
					case GameLiving.eAttackResult.Fumbled:
					case GameLiving.eAttackResult.Missed:
					case GameLiving.eAttackResult.Parried:
						SpeedOfSoundEffect effect = (SpeedOfSoundEffect)player.EffectList.GetOfType(typeof(SpeedOfSoundEffect));
						if (effect != null)
							effect.Cancel(false);
						break;
				}
			}
		}

		/// <summary>
		/// Called when effect is to be cancelled
		/// </summary>
		/// <param name="playerCancel">Whether or not effect is player cancelled</param>
		public void Cancel(bool playerCancel)
		{
			StopTimers();
			m_player.TempProperties.removeProperty("Charging");
			m_player.BuffBonusMultCategory1.Remove((int)eProperty.MaxSpeed, this);
			m_player.Out.SendUpdateMaxSpeed();
			m_player.EffectList.Remove(this);
			GameEventMgr.RemoveHandler(m_player, GameLivingEvent.AttackFinished, m_attackFinished);
			GameEventMgr.RemoveHandler(m_player, GameLivingEvent.CastFinished, m_attackFinished);
		}

		/// <summary>
		/// Starts the timers for this effect
		/// </summary>
		private void StartTimers()
		{
			StopTimers();
			m_expireTimer = new RegionTimer(m_player, new RegionTimerCallback(ExpireCallback), m_effectDuration * 1000);
		}

		/// <summary>
		/// Stops the timers for this effect
		/// </summary>
		private void StopTimers()
		{

			if (m_expireTimer != null)
			{
				m_expireTimer.Stop();
				m_expireTimer = null;
			}
		}

		/// <summary>
		/// The callback for when the effect expires
		/// </summary>
		/// <param name="timer">The ObjectTimerCallback object</param>
		private int ExpireCallback(RegionTimer timer)
		{
			Cancel(false);

			return 0;
		}


		/// <summary>
		/// Name of the effect
		/// </summary>
		public string Name
		{
			get
			{
				return "Speed of Sound";
			}
		}

		/// <summary>
		/// Remaining time of the effect in milliseconds
		/// </summary>
		public Int32 RemainingTime
		{
			get
			{
				RegionTimer timer = m_expireTimer;
				if (timer == null || !timer.IsAlive)
					return 0;
				return timer.TimeUntilElapsed;
			}
		}

		/// <summary>
		/// Icon ID
		/// </summary>
		public UInt16 Icon
		{
			get
			{
				return 3020;
			}
		}

		/// <summary>
		/// Unique ID for identification in the effect list
		/// </summary>
		public UInt16 InternalID
		{
			get
			{
				return m_id;
			}
			set
			{
				m_id = value;
			}
		}

		/// <summary>
		/// Delve information
		/// </summary>
		public IList DelveInfo
		{
			get
			{
				IList delveInfoList = new ArrayList(10);
				delveInfoList.Add(m_delveString);
				delveInfoList.Add(" ");

				int seconds = (int)(RemainingTime / 1000);
				if (seconds > 0)
				{
					delveInfoList.Add(" ");
					delveInfoList.Add("- " + seconds + " seconds remaining.");
				}

				return delveInfoList;
			}
		}
	}
}