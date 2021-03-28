using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;

namespace OdinPlus
{
	public class HuntTask : OdinTask
	{
		public static string[] Monsters = new string[] { "Troll", "Draugr_Elite", "Fenring", "GoblinBrute" };
		private void Awake()
		{
			if (loading)
			{
				return;
			}
			m_type = TaskManager.TaskType.Hunt;
			m_tier0 = new string[] { "Runestone_Greydwarfs" };
			m_tier1 = new string[] { "Ruin1", "Runestone_Greydwarfs" };
			m_tier2 = new string[] { "Runestone_Draugr" };
			m_tier3 = new string[] { "Waymarker02" };
			m_tier4 = new string[] { "Runestone_Plains" };
			m_tier5 = new string[] { "Runestone_Greydwarfs", "Ruin1", "Runestone_Greydwarfs", "Runestone_Draugr", "Waymarker02", "Runestone_Plains" };
			base.Begin();
		}
		protected override bool SetLocation()
		{
			bool result = base.SetLocation();
			if (result)
			{
				SetMonsterName();
			}
			return result;
		}
		private void SetMonsterName()
		{
			if (Key == 5)
			{
				Key = (Key-1).RollDice();
			}
			locName = Monsters[Key - 1];
			locName = Regex.Replace(locName, @"[_]", "");
		}
		protected override void InitAll()
		{
			if (!isLoaded())
			{
				return;
			}
			AddMonster();
		}
		private void AddMonster()
		{

			float y = 0;
			ZoneSystem.instance.FindFloor(location.m_position, out y);
			var pos = new Vector3(location.m_position.x, y + 2, location.m_position.z + 5f);
			Reward = Instantiate(ZNetScene.instance.GetPrefab(Monsters[Key - 1] + "Hunt"), pos, Quaternion.identity);
			Reward.GetComponent<HuntTarget>().ID = Id;
			Reward.GetComponent<HuntTarget>().Setup(Key, Level);
			m_isInit = true;
			DBG.blogWarning("Placed " + Monsters[Key - 1] + "Hunt" + " at : " + Reward.transform.localPosition);
		}
	}
}