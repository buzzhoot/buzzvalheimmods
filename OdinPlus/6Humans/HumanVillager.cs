using System;
using UnityEngine;
using System.Collections.Generic;

namespace OdinPlus
{
	public class HumanVillager : HumanNPC, Hoverable, Interactable, OdinInteractable
	{
		public static List<HumanVillager> HumanNPCS = new List<HumanVillager>();
        protected readonly float QuestCD =  1800;
        public float timer = 0;
		protected override void Awake()
		{
			if (HumanNPCS == null)
			{
				HumanNPCS = new List<HumanVillager>();
			}
			HumanNPCS.Add(this);
			base.Awake();
			m_hum.m_onDamaged = (Action<float, Character>)Delegate.Combine(m_hum.m_onDamaged, (Action<float, Character>)(Damage));

		}
		private void OnDestroy()
		{
			HumanNPCS.Remove(this);
		}
		private void Damage(float hit, Character character)
		{
			if (character == null)
			{
				return;
			}
			if (character.IsPlayer())
			{
				foreach (var item in HumanNPCS)
				{
					item.ChangeFaction(Player.m_localPlayer);
				}
			}
		}
	}
}
