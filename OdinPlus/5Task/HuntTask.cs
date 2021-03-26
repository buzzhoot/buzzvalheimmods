using System;
using System.Collections.Generic;
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
			m_tier1 = new string[] { "Ruin1"};
			m_tier2 = new string[] { "CopperMine3" };
			m_tier3 = new string[] { "CopperMine3" };
			m_tier4 = new string[] { "CopperMine3" };
			singleInit=true;
			base.Begin();
		}
		protected override bool SetLocation()
		{
			bool result = base.SetLocation();
			if (result)
			{
				HintStart = String.Format("There is a <color=yellow><b>[{0}]</b></color> near the location i marked for you,check your map ...", locName);
			}
			return result;
		}
		protected override void SetLocName()
		{
			locName = Monsters[Key];
			locName = Regex.Replace(locName, @"[_]", "");
		}
		protected override void Discovery()
		{
			HintTarget = string.Format("Looks like you are close to the <color=yellow><b>[{0}]</b></color> Watchout!", locName);
			base.Discovery();
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
			var pos = new Vector3(location.m_position.x, y+2, location.m_position.z-7f);
			Reward = Instantiate(ZNetScene.instance.GetPrefab(Monsters[Key] + "Hunt"),pos,Quaternion.identity);
			Reward.GetComponent<HuntTarget>().ID=Id;
			Reward.GetComponent<HuntTarget>().Setup(Key, Level);
			m_isInit=true;
			DBG.blogWarning("Placed "+Monsters[Key] + "Hunt" +" at : " + Reward.transform.localPosition);
		}
	}
}