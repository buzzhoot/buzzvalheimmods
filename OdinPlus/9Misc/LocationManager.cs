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

		#region Mono
		private void Awake()
		{
			BlackList = OdinData.Data.BlackList;
			GetValDictionary();
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
		#endregion Feature

	}
}