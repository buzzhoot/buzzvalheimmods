using System;
using UnityEngine;
using System.Collections.Generic;

namespace OdinPlus
{
	public class HumanVillager : HumanNPC, Hoverable, Interactable, OdinInteractable
	{
		public static List<HumanVillager> Villagers = new List<HumanVillager>();
		protected readonly float QuestCD = 1800;
		public float timer = 0;
		public GameObject EXCobj;
		protected override void Awake()
		{
			if (Villagers == null)
			{
				Villagers = new List<HumanVillager>();
			}
			Villagers.Add(this);
			base.Awake();
			var zdo = m_nview.GetZDO();
			m_hum.m_onDamaged = (Action<float, Character>)Delegate.Combine(m_hum.m_onDamaged, (Action<float, Character>)(Damage));

		}

		private void OnDestroy()
		{

			Villagers.Remove(this);
		}
		private void Damage(float hit, Character character)
		{
			if (character == null)
			{
				return;
			}
			if (character.IsPlayer())
			{
				foreach (var item in Villagers)
				{
					item.ChangeFaction(Player.m_localPlayer);
				}
			}
		}
		public bool IsQuestReady()
		{
			DateTime d = new DateTime(this.m_nview.GetZDO().GetLong("QuestTime", (long)QuestCD));
			bool result = (ZNet.instance.GetTime() - d).TotalSeconds > (double)QuestCD;
			EXCobj.SetActive(result);
			return result;
		}
		public void ResetQuestCD()
		{
			m_nview.GetZDO().Set("QuestTime",ZNet.instance.GetTime().Ticks);
		}
	}
}
