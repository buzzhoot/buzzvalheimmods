using System.Reflection;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using BepInEx.Logging;
using UnityEngine;

namespace AllTameable
{
	public class PrefabManager : MonoBehaviour
	{
		public static Dictionary<int, GameObject> odbRegList = new Dictionary<int, GameObject>();
		public static Dictionary<int, GameObject> znsRegList = new Dictionary<int, GameObject>();
		public GameObject Root;
		public static bool isInit;
		#region Feature
		public  void Init()
		{
			isInit = true;
		}
		public  void PostODB()
		{
			ValRegister(ObjectDB.instance);
		}
		public  void PreZNS(ZNetScene zns)
		{
			ValRegister(zns);
		}
		public  void PostZNS()
		{
			if (!PetManager.isInit)
			{
				PetManager.Init();
			}
			ValRegister();
		}
		public  void Clear()
		{
			Plugin.petManager.Clear();
			UnRegister();
		}

		#endregion Feature

		#region Mono
		private void Awake()
		{
			Root = new GameObject("PrefabList");
			Root.transform.SetParent(Plugin.Root.transform);
			Root.SetActive(false);
		}
		#endregion Mono

		#region Tool
		public static void ValRegister(ObjectDB odb)
		{

			var m_itemByHash = Traverse.Create(odb).Field<Dictionary<int, GameObject>>("m_itemByHash").Value;
			foreach (var item in odbRegList)
			{
				m_itemByHash.Add(item.Key, item.Value);
				odb.m_items.Add(item.Value);
			}
			DBG.blogInfo("Register to ODB");
		}
		public static void ValRegister(ZNetScene zns)
		{
			foreach (var item in odbRegList.Values)
			{
				zns.m_prefabs.Add(item);
			}
			DBG.blogInfo("Register odb to zns");
		}
		public static void ValRegister()
		{
			var m_namedPrefabs = Traverse.Create(ZNetScene.instance).Field<Dictionary<int, GameObject>>("m_namedPrefabs").Value;
			foreach (var item in znsRegList)
			{
				ZNetScene.instance.m_prefabs.Add(item.Value);
				m_namedPrefabs.Add(item.Key, item.Value);
			}
			DBG.blogInfo("Register zns");
		}
		public static void PreRegister(Dictionary<string, GameObject> list, string name)
		{
			foreach (var item in list)
			{
				odbRegList.Add(item.Key.GetStableHashCode(), item.Value);
			}
			DBG.blogWarning("Register " + name + " for ODB");
		}
		public static void PostRegister(Dictionary<string, GameObject> list)
		{
			foreach (var item in list)
			{

				znsRegList.Add(item.Key.GetStableHashCode(), item.Value);
			}
		}
		public static void PostRegister(GameObject go)
		{
			znsRegList.Add(go.name.GetStableHashCode(), go);
		}
		public static void PreRegister(GameObject go)
		{
			odbRegList.Add(go.name.GetStableHashCode(), go);
		}
		public static void UnRegister()
		{
			var odb = ObjectDB.instance;
			var zns = ZNetScene.instance;
			var m_itemByHash = Traverse.Create(odb).Field<Dictionary<int, GameObject>>("m_itemByHash").Value;
			var m_namedPrefabs = Traverse.Create(ZNetScene.instance).Field<Dictionary<int, GameObject>>("m_namedPrefabs").Value;
			odb.m_items.RemoveList<int, GameObject>(odbRegList);
			m_itemByHash.RemoveList<int, GameObject>(odbRegList);
			zns.m_prefabs.RemoveList<int, GameObject>(odbRegList);
			m_namedPrefabs.RemoveList<int, GameObject>(odbRegList);
			zns.m_prefabs.RemoveList<int, GameObject>(znsRegList);
			m_namedPrefabs.RemoveList<int, GameObject>(znsRegList);
			DBG.blogInfo("UnRegister all list");
		}
		#endregion Tool


	}
}