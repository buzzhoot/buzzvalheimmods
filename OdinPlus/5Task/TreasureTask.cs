using System;
using System.Collections.Generic;
using UnityEngine;
namespace OdinPlus
{
	public class TreasureTask : OdinTask
	{
		private string[] m_tier1 = { "WoodHouse11", "WoodHouse6", "WoodHouse3", "WoodHouse4", "WoodHouse1" };
		private void Awake()
		{
			switch (Key)
			{

				case 0:
					break;
			}
			base.Begin();
		}
		private void InitTire1()
		{
			if(isMain)
			{
				int ind = m_tier1.Length.RollDice();
				string locName = m_tier1[ind];
				ZoneSystem.instance.FindClosestLocation(locName,Player.m_localPlayer.transform.position,out location);
				if(location.m_placed)
				{	var loc = location.m_location.m_location;
					var root = location.m_location.m_location.gameObject;
					if (root.transform.Find("Beehive")==null)
					{
						var go = Instantiate(ZNetScene.instance.GetPrefab("Beehive"),root.transform);
						go.transform.localPosition = root.FindObject("Beehive").transform.localPosition;
						return;
					}
				}
			}
		}
		private void AddChest()
		{
			
		}
	}
}