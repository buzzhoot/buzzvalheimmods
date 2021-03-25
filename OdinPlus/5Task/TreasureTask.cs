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
			if (loading)
			{
				return;
			}
			m_type=TaskManager.TaskType.Treasure;
			if (!TaskManager.isMain)
			{
				m_tier0 = new string[] { "WoodHouse11", "WoodHouse6", "WoodHouse3", "WoodHouse4", "WoodHouse6", "WoodHouse7", "WoodHouse8", "WoodHouse9" };
				m_tier1 = new string[] { "WoodHouse3", "WoodHouse4", "Ruin2", "Ruins1", "ShipSetting01", "Runestone_Boars", "Runestone_Meadows", "Runestone_Greydwarfs", "Runestone_BlackForest" };
				m_tier2 = new string[] { "SwampRuinX", "SwampRuinY", "SwampHut5", "SwampHut1", "SwampHut2", "SwampHut3", "SwampHut4", "Runestone_Draugr" };
				m_tier3 = new string[] { "SwampRuinX", "SwampRuinY", "SwampHut5", "SwampHut1", "SwampHut2", "SwampHut3", "SwampHut4", "Runestone_Draugr" };//?
				m_tier4 = new string[] { "SwampRuinX", "SwampRuinY", "SwampHut5", "SwampHut1", "SwampHut2", "SwampHut3", "SwampHut4", "Runestone_Draugr" };//?
			}
			else
			{
				m_tier0 = new string[] { "WoodHouse2", "WoodHouse10" };
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
				LocationManager.GetLocationInstance(Id, out location);
				return;
			}
			AddChest();
		}
		protected override void InitTire1()
		{
			if (!isLoaded())
			{
				return;
			}
			AddChest();
		}
		protected override void InitTire2()
		{
			if (!isLoaded())
			{
				return;
			}
			AddChest();
		}
		protected override void InitTire3()
		{
			if (!isLoaded())
			{
				return;
			}
			AddChest();
		}
		protected override void InitTire4()
		{
			if (!isLoaded())
			{
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
			Reward = Instantiate(ZNetScene.instance.GetPrefab("LegacyChest" + (Key + 1).ToString()));
			float y = -1.5f;
			float x = 4f;
			float z = 3.999f;
			if (Key == 0)
			{
				y = 0;
				x = 2f;
				z = 1.999f;
			}
			Reward.transform.localPosition = new Vector3(x.RollDice(), y, z.RollDice()) + location.m_position;
			Reward.GetComponent<LegacyChest>().ID = this.Id;
			m_isInit = true;
			DBG.blogWarning("Placed LegacyChest at : " + Reward.transform.localPosition);
			return;
		}
		private void CheckHive()
		{
			if (!Tweakers.HasObject("Beehive", location.m_position))
			{
				var go = Instantiate(ZNetScene.instance.GetPrefab("Beehive"));

				//go.transform.localPosition = root.FindObject("Beehive").transform.localPosition + location.m_position;
				go.transform.localPosition = location.m_position;
				DBG.blogWarning("placed beehive at:" + go.transform.localPosition);
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