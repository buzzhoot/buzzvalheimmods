using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;

namespace OdinPlus
{
	internal class LocationManager : MonoBehaviour
	{
		private static Dictionary<Vector2i, ZoneSystem.LocationInstance> m_locationInstances = new Dictionary<Vector2i, ZoneSystem.LocationInstance>();
		public static List<string> BlackList = new List<string>();
		public static LocationManager instance;

		#region Mono
		private void Awake()
		{
			initRPC();
			instance = this;
			if (ZNet.instance.IsServer())
			{
				BlackList = OdinData.Data.BlackList;
				GetValDictionary();
				/* ZoneSystem.LocationInstance temp;
				ZoneSystem.instance.FindClosestLocation("StartTemple", Vector3.zero, out temp);
				NpcManager.Root.transform.localPosition = temp.m_position + new Vector3(-6, 0, -8); */
			}
			GetStartPos();
		}
		private void Start()
		{
			
		}
		private void OnDestroy()
		{
			m_locationInstances.Clear();
			BlackList.Clear();
		}

		#endregion Mono

		#region Init
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
				m_locationInstances.Remove(Tweakers.Pak(item));
			}
		}
		public static void Clear()
		{
		}
		#endregion Init

		#region Feature
		public static void Remove(string id)
		{
			BlackList.Add(id);
			m_locationInstances.Remove(Tweakers.Pak(id));
		}

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
					id = Tweakers.DepakVector2i(item.Key);
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
		public void initRPC()
		{
			ZRoutedRpc.instance.Register<Vector3>("RPC_SetStartPos", new Action<long, Vector3>(this.RPC_SetStartPos));
			if (ZNet.instance.IsServer())
			{
				ZRoutedRpc.instance.Register("Rpc_GetStartPos", new Action<long>(this.Rpc_GetStartPos));
				return;
			}

		}
		public void GetStartPos()
		{
			ZRoutedRpc.instance.InvokeRoutedRPC("Rpc_GetStartPos", new object[] { });
		}
		private void Rpc_GetStartPos(long sender)
		{
			ZoneSystem.LocationInstance temp;
			ZoneSystem.instance.FindClosestLocation("StartTemple", Vector3.zero, out temp);
			ZRoutedRpc.instance.InvokeRoutedRPC(sender, "RPC_SetStartPos", new object[] { temp.m_position });
		}
		private void RPC_SetStartPos(long sender, Vector3 pos)
		{
			Debug.Log("a");
			NpcManager.Root.transform.localPosition = pos + new Vector3(-6, 0, -8);
		}

		#endregion RPC


	}
}