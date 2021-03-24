using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using UnityEngine;
namespace OdinPlus
{

	public class PrefabManager : MonoBehaviour
	{
		#region Var
		public static bool isInit = false;
		private static ZNetScene zns;
		public static GameObject Root;
		public static Dictionary<string, GameObject> PrefabList = new Dictionary<string, GameObject>();
		#endregion Var

		#region Mono
		private void Awake()
		{

		}
		public static void Init()
		{
			zns = ZNetScene.instance;
			Root = new GameObject("OdinPrefab");
			Root.transform.SetParent(OdinPlus.PrefabParent.transform);

			CreateLegacyChest();

			OdinPlus.OdinPostRegister(PrefabList);
			isInit = true;
		}
		#endregion Mono

		#region OdinLegcy

		private static void CreateLegacyChest()
		{
			for (int i = 1; i < 6; i++)
			{
				var go = Instantiate(OdinItem.GetObject("OdinLegacy"), Root.transform);
				go.name = ("OidnLegacy"+ i);
				var lgc = go.GetComponent<ItemDrop>().m_itemData;
				lgc.m_quality = i;
				GameObject Chest = Instantiate(ZNetScene.instance.GetPrefab("Chest"), OdinPlus.PrefabParent.transform);
				Chest.name = "LegacyChest" + i;

				DestroyImmediate(Chest.GetComponent<Rigidbody>());
				var ctn = Chest.GetComponent<Container>();
				Chest.AddComponent<LegacyChest>();

				ctn.m_name = "LegacyChest";
				ctn.m_defaultItems.m_drops.Add(new DropTable.DropData { m_item = go, m_stackMax = 1, m_stackMin = 1, m_weight = 1 });

				PrefabList.Add(Chest.name, Chest);
				PrefabList.Add(go.name,go);
			}
		}
		#endregion OdinLegcy
	}
}