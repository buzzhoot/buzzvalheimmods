using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;

namespace OdinPlus
{
	internal class LocationManager : MonoBehaviour
	{
		private static Dictionary<Vector2i, ZoneSystem.LocationInstance> m_locationInstances = new Dictionary<Vector2i, ZoneSystem.LocationInstance>();
		public static List<string> BlackList = new List<string>();
		public static LocationManager instance;
		public static bool rpc = false;
		public static Vector3 OdinPostion = Vector3.zero;

		#region Main
		private void Awake()
		{
			instance = this;
			Plugin.RegRPC = (Action)Delegate.Combine(Plugin.RegRPC, (Action)initRPC);
		}
		public static void Init()
		{
			if (ZNet.instance.IsServer())
			{
				if (Plugin.CFG_OdinPosition.Value == Vector3.zero)
				{
					ZoneSystem.LocationInstance temp;
					ZoneSystem.instance.FindClosestLocation("StartTemple", Vector3.zero, out temp);
					OdinPostion = temp.m_position + new Vector3(-6, 0, -8);
					if (OdinPostion == Vector3.zero)
					{
						OdinPostion += Vector3.forward * 0.0001f;
					}
				}
				else
				{
					OdinPostion = Plugin.CFG_OdinPosition.Value;
				}
				BlackList = OdinData.Data.BlackList;
				GetValDictionary();
			}

		}
		public static void GetValDictionary()
		{
			var a = Traverse.Create(ZoneSystem.instance).Field<Dictionary<Vector2i, ZoneSystem.LocationInstance>>("m_locationInstances").Value;
			foreach (var item in a)
			{
				m_locationInstances.Add(item.Key, item.Value);
			}
		}

		public static void RemoveBlackList()
		{
			foreach (var item in BlackList)
			{
				m_locationInstances.Remove(item.ToV2I());
			}
		}
		public static void Clear()
		{
			BlackList.Clear();
			m_locationInstances.Clear();
		}
		#endregion Init

		#region Feature
		public static bool GetLocationInstance(string id, out ZoneSystem.LocationInstance li)
		{
			var a = Traverse.Create(ZoneSystem.instance).Field<Dictionary<Vector2i, ZoneSystem.LocationInstance>>("m_locationInstances").Value;
			var key = Tweakers.Pak(id);
			if (a.ContainsKey(key))
			{
				li = a[key];
				return true;
			}
			li = default(ZoneSystem.LocationInstance);
			return false;
		}
		public static bool FindClosestLocation(string name, Vector3 point, out string id)
		{

			float num = 999999f;
			id = "0_0";
			bool result = false;
			foreach (var item in m_locationInstances)
			{
				float num2 = Vector3.Distance(item.Value.m_position, point);
				if (item.Value.m_location.m_prefabName == name && num2 < num)
				{
					num = num2;
					id = item.Key.Pak();
					result = true;
				}
			}
			return result;
		}
		public static bool FindClosestLocation(string name, Vector3 point, out Vector3 pos)
		{

			float num = 999999f;

			bool result = false;
			foreach (var item in m_locationInstances)
			{
				float num2 = Vector3.Distance(item.Value.m_position, point);
				if (item.Value.m_location.m_prefabName == name && num2 < num)
				{
					pos = item.Value.m_position;
					num = num2;
					result = true;
				}
			}
			pos = Vector3.zero;
			return result;
		}
		public static bool FindClosestLocation(string name, Vector3 point, out string id, out Vector3 pos)
		{

			float num = 999999f;
			pos = Vector3.zero;
			id = "0_0";
			bool result = false;
			foreach (var item in m_locationInstances)
			{
				float num2 = Vector3.Distance(item.Value.m_position, point);
				if (item.Value.m_location.m_prefabName == name && num2 < num)
				{
					pos = item.Value.m_position;
					id = item.Key.Pak();
					num = num2;
					result = true;
				}
			}
			return result;
		}
		#endregion Feature

		#region Tool
		public static GameObject FindDungeon(Vector3 pos)
		{
			var loc = Location.GetLocation(pos);
			if (loc == null)
			{
				return null;
			}
			var dunPos = loc.transform.Find("Interior").transform.position;
			Collider[] array = Physics.OverlapBox(dunPos, new Vector3(60, 60, 60));
			DungeonGenerator comp;
			foreach (var item in array)
			{
				var c = item.transform;
				while (c.transform.parent != null)
				{
					if (c.TryGetComponent<DungeonGenerator>(out comp))
					{
						if (c.name.Contains("Clone"))
						{
							return c.gameObject;
						}
					}
					c = c.transform.parent;
				}

			}
			return null;
		}
		#endregion Tool

		#region RPC
		public void initRPC()//RPC
		{
			ZRoutedRpc.instance.Register<Vector3>("RPC_SetStartPos", new Action<long, Vector3>(this.RPC_SetStartPos));
			ZRoutedRpc.instance.Register<string, Vector3, string, int>("RPC_ClientInitDungeon", new RoutedMethod<string, Vector3, string, int>.Method(RPC_ClientInitDungeon));//XXX
			ZRoutedRpc.instance.Register<bool>("RPC_ReceiveServerFOP", new Action<long, bool>(RPC_ReceiveServerFOP));
			if (ZNet.instance.IsServer())
			{
				ZRoutedRpc.instance.Register("Rpc_GetStartPos", new Action<long>(this.Rpc_GetStartPos));
				ZRoutedRpc.instance.Register("RPC_SendServerFOP", new Action<long>(RPC_SendServerFOP));
			}

		}
		public static void GetStartPos()
		{
			ZRoutedRpc.instance.InvokeRoutedRPC("Rpc_GetStartPos", new object[] { });
		}
		private void Rpc_GetStartPos(long sender)
		{
			DBG.blogWarning("Server got odin postion request");
			if (Plugin.CFG_OdinPosition.Value != Vector3.zero)
			{
				OdinPostion = Plugin.CFG_OdinPosition.Value;
			}
			ZRoutedRpc.instance.InvokeRoutedRPC(sender, "RPC_SetStartPos", new object[] { OdinPostion });
		}
		private void RPC_SetStartPos(long sender, Vector3 pos)
		{
			DBG.blogWarning("client  got odin postion " + pos);
			NpcManager.Root.transform.localPosition = pos;
		}
		public static void RequestServerFop()
		{
			ZRoutedRpc.instance.InvokeRoutedRPC("RPC_SendServerFOP", new object[] { });
		}
		public static void RPC_SendServerFOP(long sender)
		{
			ZRoutedRpc.instance.InvokeRoutedRPC(sender, "RPC_ReceiveServerFOP", new object[] { Plugin.CFG_ForceOdinPosition.Value });
			DBG.blogWarning("Server Sent FOP:" + Plugin.CFG_ForceOdinPosition.Value);
		}
		public static void RPC_ReceiveServerFOP(long sender, bool result)
		{
			DBG.blogWarning("Client Got FOP:" + result);
			Plugin.Set_FOP = result;
		}
		#region Dungeon
		private void RPC_ClientInitDungeon(long sender, string name, Vector3 pos, string Id, int Key)
		{
			if (!AddDungeonChest(name, pos, Id, Key))
			{
				ZRoutedRpc.instance.InvokeRoutedRPC("RPC_ServerDisInitTask", new object[] { Id });
			}

		}
		private bool AddDungeonChest(string name, Vector3 pos, string Id, int Key)
		{
			var DungeonRoot = new GameObject();
			if (name == "GoblinCamp2")
			{
				var dunPos = pos;
				if (!AddChest(dunPos, Id, Key))
				{
					var Reward = OdinTask.PlacingChest(dunPos, Id, Key);
					DBG.blogWarning("Placed LegacyChest at Dungeon camp: " + Reward.transform.position);
					return true;
				}
				return true;
			}
			else
			{
				DungeonRoot = LocationManager.FindDungeon(pos);
			}
			if (DungeonRoot == null)
			{
				return false;
			}
			if (!AddChest(DungeonRoot.transform.position, Id, Key))
			{
				Room[] array = DungeonRoot.GetComponentsInChildren<Room>();
				if (array.Length == 0) { return false; }

				var array2 = array.Where(c => c.m_endCap != true).ToArray();

				if (array2 == null)
				{
					var a = array[array.Length.RollDice()];
					AddChest(a, Id, Key);
					return true;
				}
				AddChest(array2[array2.Length.RollDice()], Id, Key);
				return true;
			}
			return true;
		}
		private bool AddChest(Vector3 pos, string Id, int Key)
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
							var Reward = OdinTask.PlacingChest(comp.transform.position, comp.transform.rotation, Id, Key);
							comp.GetInventory().RemoveAll();
							comp.GetComponent<ZNetView>().Destroy();
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
		private void AddChest(Room room, string Id, int Key)
		{
			var y = room.GetComponentInChildren<RoomConnection>().transform.localPosition.y;
			var x = room.m_size.x / 2;
			var z = room.m_size.z / 2;
			var pos = new Vector3(0, y + 0.2f, 0) + room.transform.position;
			var chest = OdinTask.PlacingChest(pos, Id, Key);
			DBG.blogWarning("Placed LegacyChest at Dungeon room: " + chest.transform.localPosition);
			return;
		}

		#endregion Dungeon

		#endregion RPC

		#region New

		public static void Remove(string id)
		{
			BlackList.Add(id);
			m_locationInstances.Remove(id.ToV2I());
			//upd add a new list for remove elements;
		}
		public static void RPC_ServerFindLocation(long sender, string sender_locName, Vector3 sender_pos)
		{
			var _id = "0_0";
			var _pos = Vector3.zero;
			if (FindClosestLocation(sender_locName, sender_pos, out _id, out _pos))
			{
				ZRoutedRpc.instance.InvokeRoutedRPC(sender, "RPC_CreateQuestSucced", new object[] { _id, _pos});
				DBG.blogWarning(string.Format("Location found location {0} at {1}", sender_locName, _pos.ToString()));
				Remove(_id);
				return;
			}
			ZRoutedRpc.instance.InvokeRoutedRPC(sender, "RPC_CreateQuestFailed", new object[]{});
			DBG.blogWarning(String.Format("Location cant find location {0} at {1}",sender_locName,sender_pos));
			
		}
		#endregion New

	}
}