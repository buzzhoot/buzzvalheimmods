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
		protected Inventory inv;
		#endregion  var
		#region Main
		private void Awake()
		{
			m_tier0 = new string[] { "WoodHouse11", "WoodHouse6", "WoodHouse3", "WoodHouse4", "WoodHouse1" };
			m_tier1 = new string[] { "WoodHouse11", "WoodHouse6", "WoodHouse3", "WoodHouse4", "WoodHouse1" };
			m_tier2 = new string[] { "WoodHouse11", "WoodHouse6", "WoodHouse3", "WoodHouse4", "WoodHouse1" };
			m_tier3 = new string[] { "WoodHouse11", "WoodHouse6", "WoodHouse3", "WoodHouse4", "WoodHouse1" };
			m_tier4 = new string[] { "WoodHouse11", "WoodHouse6", "WoodHouse3", "WoodHouse4", "WoodHouse1" };
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
			HintTarget = "Looks like you are close to the chest,look around find a <color=yellow><b>[{0}]</b></color>";
			base.Discovery();
		}
		#endregion Override Init

		#region Tool
		private void AddChest()
		{
			Chest = Instantiate(ZNetScene.instance.GetPrefab("Chest"), root.transform);
			Chest.transform.localPosition = new Vector3(0, -1.5f, 0);
			var stg = Chest.AddComponent<SnapToGround>();
			DestroyImmediate(Chest.GetComponent<Rigidbody>());
			stg.m_offset = -1.5f;
			var ctn = Chest.GetComponent<Container>();
			inv = Traverse.Create(ctn).Field<Inventory>("m_inventory").Value;
			var lgc = ObjectDB.instance.GetItemPrefab("OdinLegacy").GetComponent<ItemDrop>().m_itemData;
			lgc.m_quality = Key;
			inv.AddItem(lgc);
			m_isInit = true;
		}
		private void CheckHive()
		{
			if (root.transform.Find("Beehive") == null)
			{
				var go = Instantiate(ZNetScene.instance.GetPrefab("Beehive"), root.transform);
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
			if (inv.HaveItem("OdinLegacy")) { return; }
			Finish();
		}
		protected override void Clear()
		{
			ZNetScene.instance.Destroy(Chest);
			base.Clear();
		}
		#endregion Override
	}
}