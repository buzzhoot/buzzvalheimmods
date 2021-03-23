using System;
using System.Collections.Generic;

using UnityEngine;
using HarmonyLib;
namespace OdinPlus
{
	public class TreasureTask : OdinTask
	{
		#region  var
		private GameObject Chest;
		private Inventory inv;
		private Container ctn;
		#endregion  var
		
		#region Main
		private void Awake()
		{
			m_tier0 = new string[] { "WoodHouse11", "WoodHouse6", "WoodHouse3", "WoodHouse4", "WoodHouse1" };
			m_tier1 = new string[] { "WoodHouse11", "WoodHouse6", "WoodHouse3", "WoodHouse4", "WoodHouse1" };
			m_tier2 = new string[] { "WoodHouse11", "WoodHouse6", "WoodHouse3", "WoodHouse4", "WoodHouse1" };
			m_tier3 = new string[] { "WoodHouse11", "WoodHouse6", "WoodHouse3", "WoodHouse4", "WoodHouse1" };
			m_tier4 = new string[] { "WoodHouse11", "WoodHouse6", "WoodHouse3", "WoodHouse4", "WoodHouse1" };
			Reward = OdinPlus.PrefabParent.transform.Find("OdinLegacy"+(Key+1).ToString()).gameObject;
			base.Begin();
		}
		#endregion Main

		#region Override Init
		protected override void SetLocation()
		{
			base.SetLocation();
			HintStart = String.Format("There a chest burried under a  <color=yellow><b>[{0}]</b></color> near the location i marked for you,check your map ...", locName);
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
			if (ctn != null)
			{
				SetupInv();
				return;
			}
			Chest = Instantiate(ZNetScene.instance.GetPrefab("Chest"),OdinPlus.PrefabParent.transform);
			Chest.name = "Chest" + Id;
			Chest.transform.localPosition = new Vector3(2.RollDice(), -1.5f, 2.RollDice()) + location.m_position;
			DestroyImmediate(Chest.GetComponent<Rigidbody>());
			ctn = Chest.GetComponent<Container>();
			ctn.m_defaultItems.m_drops.Add(new DropTable.DropData { m_item = Reward, m_stackMax = 1, m_stackMin = 1, m_weight = 1 });
			Chest.transform.SetParent(ZNetScene.instance.transform);
			return;

		}
		private void SetupInv()
		{
			inv = ctn.GetInventory();
			if (inv == null) { return; }
			if (inv.NrOfItems()!=0)
			{
				m_isInit = true;
				return;
			}
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
		protected override void CheckTarget()
		{
			if (Chest == null)
			{
				FindChest();
			}
			if (inv.NrOfItems()!=0) { return; }
			Finish();
		}
		protected override void Clear()
		{
			ZNetScene.instance.Destroy(Chest);
			if (Reward != null) { DestroyImmediate(Reward); }
			base.Clear();
		}
		private void FindChest()
		{
			Chest = GameObject.Find("Chest" + Id);
			inv = Chest.GetComponent<Container>().GetInventory();
		}
		#endregion Override
	}
}