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
		#region Consant

		#endregion Consant
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
			CreateHuntTargetMonster();

			OdinPlus.OdinPostRegister(PrefabList);
			isInit = true;
		}
		#endregion Mono

		#region Task
		private static void CreateHuntTargetMonster()
		{
			foreach (var item in QuestRef.HunterMonsterList)
			{
				PrefabList.Add(item + "Hunt", HuntTarget.CreateMonster(item));
			}
		}
		private static void CreateLegacyChest()
		{
			for (int i = 1; i < 11; i++)
			{
				var go = ZNetScene.instance.GetPrefab("OdinLegacy");
				GameObject Chest = Instantiate(ZNetScene.instance.GetPrefab("Chest"), Root.transform);
				Chest.name = "LegacyChest" + i;

				DestroyImmediate(Chest.GetComponent<Rigidbody>());
				var sp= Chest.AddComponent<StaticPhysics>();
				sp.m_pushUp=false;
				var ctn = Chest.GetComponent<Container>();
				Chest.AddComponent<LegacyChest>();
				var mat = Chest.GetComponentInChildren<Renderer>().material;

				mat.SetFloat("_Hue", 0.3f);
				mat.SetFloat("_Saturation", 0.5f);

				ctn.m_name = "LegacyChest";
				ctn.m_width = 1;
				ctn.m_height = 1;
				ctn.m_defaultItems.m_drops.Add(new DropTable.DropData { m_item = go, m_stackMax = i, m_stackMin = i, m_weight = 1 });
				//-?
				var fx = Instantiate(FxAssetManager.GetFxNN("BlueSmoke"), Chest.transform);

				PrefabList.Add(Chest.name, Chest);
			}
		}
		#endregion OdinLegcy
	}
}