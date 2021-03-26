using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace OdinPlus
{
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
			m_tier3 = new string[] { "SunkenCrypt4", };
			m_tier4 = new string[] { "SunkenCrypt4", };
			base.Begin();
		}
		#region OverRide
		protected override bool SetLocation()
		{
			bool result = base.SetLocation();
			HintStart = String.Format("There a chest in the dungeon <color=yellow><b>[{0}]</b></color> near the location i marked for you,check your map ...", locName);
			return result;
		}
		protected override void Discovery()
		{
			HintTarget = string.Format("Looks like you are close to the dungeon,look around find a <color=yellow><b>[{0}]</b></color>", locName);
			base.Discovery();
		}
		protected override void InitTire0()
		{
			if (!isLoaded())
			{
				return;
			}
			FindRoom();
		}
		protected override void InitTire1()
		{
			if (!isLoaded())
			{
				return;
			}
			FindRoom();
		}
		protected override void InitTire2()
		{
			if (!isLoaded())
			{
				return;
			}
			FindRoom();
		}
		protected override void InitTire3()
		{
			if (!isLoaded())
			{
				return;
			}
			FindRoom();
		}
		protected override void InitTire4()
		{
			if (!isLoaded())
			{
				return;
			}
			FindRoom();
		}

		#endregion OverRide
		private void FindRoom()
		{
			DungeonRoot = LocationManager.FindDungeon(location.m_position);
			if (DungeonRoot == null)
			{
				return;
			}
			Container[] ctn = DungeonRoot.GetComponentsInChildren<Container>();
			if (ctn.Length == 0)
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
			else
			{
				var c = ctn[ctn.Length.RollDice()];
				var pos = c.transform.position;
				var par = c.transform.parent;
				c.GetComponent<ZNetView>().Destroy();
				Reward = Instantiate(ZNetScene.instance.GetPrefab("LegacyChest" + (Key + 1).ToString()), pos + Vector3.up * 0.1f, Quaternion.identity);
				Reward.GetComponent<LegacyChest>().ID = this.Id;
				m_isInit = true;
				DBG.blogWarning("Placed LegacyChest at Dungeon ctn: " + Reward.transform.position);
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
	}
}