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
		//private static Dictionary<string, GameObject> MeadList = new Dictionary<string, GameObject>()
		private static GameObject MeadTasty;
		public static List<GameObject> MeadList = new List<GameObject>();
		public static List<string> MeadNameList = new List<string> { "mead_troll" };
		public static Dictionary<string, Sprite> PetMeadList = new Dictionary<string, Sprite>();
		public static Dictionary<string,GameObject> MeadPrefabs = new Dictionary<string, GameObject>();
		private static GameObject PrefabsParent;
		public static void init()
		{
			PrefabsParent = new GameObject("MeadPrefabs");
			PrefabsParent.transform.SetParent(OdinPlus.PrefabParent.transform);
			PrefabsParent.SetActive(false);

			var objectDB = ObjectDB.instance;
			MeadTasty = objectDB.GetItemPrefab("MeadTasty");
			PetMeadList.Add("mead_troll", OdinPlus.TrollHeadIcon);

			foreach (var pet in PetMeadList)
			{
				CreatePetMeadPrefab(pet.Key, pet.Value);
			}
		}
		public static void CreatePetMeadPrefab(string name, Sprite icon)
		{
			GameObject go = Instantiate(MeadTasty, PrefabsParent.transform);
			go.name = name;
			var id = go.GetComponent<ItemDrop>().m_itemData.m_shared;
			id.m_name = "$odin_" + name + "_name";
			id.m_icons[0] = icon;
			id.m_description = "$odin_" + name + "_desc";
			id.m_consumeStatusEffect = OdinSE.SElist[name];
			MeadList.Add(go);
			MeadPrefabs.Add(name,go);
		}
		public static void Register(ZNetScene zns)
		{
			foreach (var go in MeadList)
			{
				zns.m_prefabs.Add(go);

			}
			DBG.blogWarning("Register Meads for ZNS");
		}
		public static void Register(ObjectDB odb)
		{
			var m_itemByHash = Traverse.Create(odb).Field<Dictionary<int, GameObject>>("m_itemByHash").Value;
			foreach (var go in MeadList)
			{
				m_itemByHash.Add(go.name.GetStableHashCode(), go);
				odb.m_items.Add(go);
			}
			DBG.blogWarning("Register Meads for ODB");
		}
		public static void PostRegister()
		{
			var fd = Traverse.Create(ZNetScene.instance).Field<Dictionary<int, GameObject>>("m_namedPrefabs");
			foreach (var item in MeadPrefabs)
			{
				fd.Value.Add(item.Key.GetStableHashCode(), item.Value);
			}
		}
	}
}
