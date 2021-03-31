using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace OdinPlus
{
	//!Add Level for dungeon set.
	public class DungeonTask : OdinTask
	{
		private GameObject DungeonRoot;
		private void Awake()
		{
			if (loading)
			{
				return;
			}
			m_type = TaskManager.TaskType.Dungeon;
			m_tier0 = new string[] { "Crypt3" };
			m_tier1 = new string[] { "Crypt3", "Crypt2", "Crypt4" };
			m_tier2 = new string[] { "SunkenCrypt4" };
			m_tier3 = new string[] { "SunkenCrypt4" };
			m_tier4 = new string[] { "GoblinCamp2" };
			m_tier5 = new string[] { "Crypt3", "Crypt2", "Crypt4", "SunkenCrypt4", "GoblinCamp2" };
			base.Begin();
		}
		#region OverRide

		protected override void InitAll()
		{
			if (!isLoaded())
			{
				return;
			}
			if (ZNet.instance.IsLocalInstance())
			{
				FindRoom();
				return;
			}
			var pl = PeersInArea();
			if (pl != null)
			{
				ZRoutedRpc.instance.InvokeRoutedRPC(pl[0], "RPC_ClientInitDungeon", new object[] { locName, location.m_position, Id, Key });
				m_isInit = true;
			}
			return;
		}

		#endregion OverRide
		private void FindRoom()
		{
			if (location.m_location.m_prefabName == "GoblinCamp2")
			{
				var dunPos = location.m_position;
				if (!AddChest(dunPos))
				{
					Reward = Instantiate(ZNetScene.instance.GetPrefab("LegacyChest" + (Key + 1).ToString()), dunPos, Quaternion.identity);
					Reward.GetComponent<LegacyChest>().ID = this.Id;
					m_isInit = true;
					DBG.blogWarning("Placed LegacyChest at Dungeon camp: " + Reward.transform.position);
					return;
				}
				return;
			}
			else
			{
				DungeonRoot = LocationManager.FindDungeon(location.m_position);
			}
			if (DungeonRoot == null)
			{
				return;
			}
			if (!AddChest(DungeonRoot.transform.position))
			{
				Room[] array = DungeonRoot.GetComponentsInChildren<Room>();
				if (array.Length == 0) { return; }

				var array2 = array.Where(c => c.m_endCap != true).ToArray();

				if (array2 == null)
				{
					var a = array[array.Length.RollDice()];
					AddChest(a);
					return;
				}
				AddChest(array2[array2.Length.RollDice()]);
				return;
			}
		}
		private void AddChest(Room room)
		{
			var y = room.GetComponentInChildren<RoomConnection>().transform.localPosition.y;
			var x = room.m_size.x / 2;
			var z = room.m_size.z / 2;
			var pos = new Vector3(0, y + 0.2f, 0) + room.transform.position;
			Reward = Instantiate(ZNetScene.instance.GetPrefab("LegacyChest" + (Key + 1).ToString()));
			Reward.transform.localPosition = pos;
			Reward.GetComponent<LegacyChest>().ID = this.Id;
			m_isInit = true;
			DBG.blogWarning("Placed LegacyChest at Dungeon room: " + Reward.transform.localPosition);
			return;
		}
		private bool AddChest(Vector3 pos)
		{
			Collider[] array = Physics.OverlapBox(pos, new Vector3(60, 60, 60));
			Container comp;
			foreach (var item in array)
			{
				var ci = item.transform;
				while (ci.transform.parent != null)
				{
					if (ci.TryGetComponent<Container>(out comp))
					{
						if (ci.name.Contains("Clone"))
						{
							Reward = Instantiate(ZNetScene.instance.GetPrefab("LegacyChest" + (Key + 1).ToString()), comp.transform.position, Quaternion.identity);
							comp.GetInventory().RemoveAll();
							comp.GetComponent<ZNetView>().Destroy();
							Reward.GetComponent<LegacyChest>().ID = this.Id;
							m_isInit = true;
							DBG.blogWarning("Placed LegacyChest at Dungeon ctn: " + Reward.transform.position);
							return true;
						}
					}
					ci = ci.transform.parent;
				}
			}
			DBG.blogWarning("Cant Find Chest in dungeon");
			return false;
		}
	}
}