using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace OdinPlus
{
	public class DungeonTask : OdinTask
	{
		private void Awake()
		{
			if (loading)
			{
				return;
			}
			m_type = TaskManager.TaskType.Dungeon;
			m_tier1 = new string[] { "Crypt2"};
			m_tier2 = new string[] { "SwampRuinX", "SwampRuinY", "SwampHut5", "SwampHut1", "SwampHut2", "SwampHut3", "SwampHut4", "Runestone_Draugr" };
			m_tier3 = new string[] { "SwampRuinX", "SwampRuinY", "SwampHut5", "SwampHut1", "SwampHut2", "SwampHut3", "SwampHut4", "Runestone_Draugr" };

			base.Begin();
		}
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
		private void FindRoom()
		{
			root = LocationManager.FindDungeon(location.m_position);
			if (root == null)
			{
				return;
			}
			Room[] array = root.GetComponentsInChildren<Room>();
			var array2 = array.Where(c => c.m_endCap == true).ToArray();
			if (array2 == null)
			{
				var a = array[array.Length.RollDice()];
				AddChest(a);
				return;
			}
			AddChest(array2[array2.Length.RollDice()]);
			return;

		}
		private void AddChest(Room room)
		{
			var y = room.GetComponentInChildren<RoomConnection>().transform.localPosition.y;
			var x = room.m_size.x - 2;
			var z = room.m_size.z - 2;
			var pos = new Vector3(x.RollDice(-x), y, z.RollDice(-z));
			Reward = Instantiate(ZNetScene.instance.GetPrefab("LegacyChest" + (Key + 1).ToString()), room.transform);
			Reward.transform.localPosition = pos;
			Reward.GetComponent<LegacyChest>().ID = this.Id;
			m_isInit = true;
			DBG.blogWarning("Placed LegacyChest at Dungeon: " + Reward.transform.localPosition);
			return;
		}
	}
}