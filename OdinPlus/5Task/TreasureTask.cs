using System;
using System.Collections.Generic;

using UnityEngine;
using HarmonyLib;
namespace OdinPlus
{
	public class TreasureTask : OdinTask
	{
		#region  var
		#endregion  var

		#region Main
		private void Awake()
		{
			m_tier0 = new string[] { "WoodHouse11", "WoodHouse6", "WoodHouse3", "WoodHouse4", "WoodHouse1" };
			m_tier1 = new string[] { "WoodHouse11", "WoodHouse6", "WoodHouse3", "WoodHouse4", "WoodHouse1" };
			m_tier2 = new string[] { "WoodHouse11", "WoodHouse6", "WoodHouse3", "WoodHouse4", "WoodHouse1" };
			m_tier3 = new string[] { "WoodHouse11", "WoodHouse6", "WoodHouse3", "WoodHouse4", "WoodHouse1" };
			m_tier4 = new string[] { "WoodHouse11", "WoodHouse6", "WoodHouse3", "WoodHouse4", "WoodHouse1" };
			if (laoding)
			{
				return;
			}
			base.Begin();
		}
		#endregion Main

		#region Override Init
		protected override bool SetLocation()
		{
			bool result = base.SetLocation();
			HintStart = String.Format("There a chest burried under a  <color=yellow><b>[{0}]</b></color> near the location i marked for you,check your map ...", locName);
			return result;
		}
		protected override void InitTire0()
		{
			if (!isLoaded())
			{
				return;
			}
			if (isMain)
			{
				if (location.m_placed)
				{
					CheckHive();
					return;
				}
				m_isInit = false;
				return;
			}
			AddChest();
		}
		protected override void Discovery()
		{
			HintTarget = string.Format("Looks like you are close to the chest,look around find a <color=yellow><b>[{0}]</b></color>", locName);
			base.Discovery();
		}
		#endregion Override Init

		#region Tool
		private void AddChest()
		{
			Reward = Instantiate(ZNetScene.instance.GetPrefab("OdinChest" + (Key + 1).ToString()));
			float y  = - 1.5f;
			if (Key==0)
			{
				y = 0;
			}
			Reward.transform.localPosition = new Vector3(4f.RollDice(), y, 4f.RollDice()) + location.m_position;
			return;
		}
		private void CheckHive()
		{
			if (root.transform.Find("Beehive") == null)
			{
				var go = Instantiate(ZNetScene.instance.GetPrefab("Beehive"));
				if (root.FindObject("Beehive") != null)
				{
					go.transform.localPosition = root.FindObject("Beehive").transform.localPosition;
				}
				AddChest();
				return;
			}
			AddChest();
			return;
		}
		#endregion Tool

		#region Override tail

		#endregion Override
	}
}