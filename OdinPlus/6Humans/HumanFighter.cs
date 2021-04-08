using System;
using System.Collections.Generic;
using UnityEngine;

namespace OdinPlus
{
	public class HumanFighter : HumanNPC, Hoverable, Interactable, OdinInteractable
	{
		protected override void Awake()
		{
			base.Awake();
			ChoiceList = new string[2] { "$op_talk", "$op_human_fight" };
			m_hum.m_onDeath = (Action)Delegate.Combine(m_hum.m_onDeath, (Action)onDeath);
		}
		public override void Choice0()
		{
			Say("Want a Fight?");
		}
		public void Choice1()
		{
			ChangeFaction(Character.Faction.Boss);
		}
		public void onDeath()
		{
            
		}
	}
}
