using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HarmonyLib;

namespace OdinPlus
{
	class OdinMeads : MonoBehaviour
	{
		//private static ZNetScene zns;
		//private static Dictionary<string, GameObject> MeadList = new Dictionary<string, GameObject>()
		private static GameObject MeadTasty;
		public static List<GameObject> MeadList = new List<GameObject>();
		public static List<string> MeadNameList = new List<string>();

		private static GameObject PrefabsParent;
		public static void init()
		{
			PrefabsParent = new GameObject("MeadPrefabs");
			PrefabsParent.transform.SetParent(Plugin.PrefabParent.transform);
			PrefabsParent.SetActive(false);
			var objectDB = ObjectDB.instance;
			//zns = ZNetScene.instance;
			MeadTasty = objectDB.GetItemPrefab("MeadTasty");

			MeadNameList.Add("mead_troll");
			foreach (var m in MeadNameList)
			{
				CreateMeadPrefab(m);
			}
		}
		public static void CreateMeadPrefab(string name)
		{
			GameObject go = Instantiate(MeadTasty, PrefabsParent.transform);
			go.name = name;
			var id = go.GetComponent<ItemDrop>().m_itemData.m_shared;
			id.m_name = "$odin" + name + "_name";
			id.m_description = "$odin" + name + "_desc";
			id.m_consumeStatusEffect = OdinSE.SElist[name];
			MeadList.Add(go);
		}
		public static void Register(ZNetScene zns)
		{
			foreach (var go in MeadList)
			{
				zns.m_prefabs.Add(go);
			}
		}
		public static void Register(ObjectDB odb)
		{
			var m_itemByHash = Traverse.Create(odb).Field<Dictionary<int, GameObject>>("m_itemByHash").Value;
			foreach (var go in MeadList)
			{
				m_itemByHash.Add(go.name.GetStableHashCode(), go);
				odb.m_items.Add(go);
			}
		}
	}
}
