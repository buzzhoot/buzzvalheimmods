using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace OdinPlus
{
	class Pet : MonoBehaviour
	{
		#region var
		private static ZNetScene zns;
		private static GameObject PrefabsParent;
		private static Dictionary<string, GameObject> PetList = new Dictionary<string, GameObject>();
		public static GameObject petIns;
		public static GameObject Indicator;
		public static bool isInit=false;
		//public static Pet instance;
		#endregion
		private void Awake()
		{
			initIndicator();
		}
		public static void Init()
		{
			
			zns = ZNetScene.instance;
			PrefabsParent = new GameObject("PetPrefab");
			PrefabsParent.transform.SetParent(OdinPlus.PrefabParent.transform);
			string[] l = Plugin.CFG_Pets.Value.Split(new char[] { ',' });
			foreach (string name in l)
			{
				CreatePetPrefab(name);
			}
			OdinPlus.OdinPostRegister(PetList);
			isInit=true;
		}
		private static void CreatePetPrefab(string name)
		{
			if (zns.GetPrefab(name) == null)
			{
				DBG.blogWarning("can't find the prefab zns :" + name);
				return;
			}
			var go = Instantiate(zns.GetPrefab(name), PrefabsParent.transform);
			go.name = name + "Pet";
			Tameable tame;
			if (!go.TryGetComponent<Tameable>(out tame))
			{
				tame = go.AddComponent<Tameable>();
			}
			go.AddComponent<PetHelper>();
			var hd = go.GetComponent<Humanoid>();
			DestroyImmediate(go.GetComponent<CharacterDrop>());
			hd.m_name = hd.m_name + " Pet";//trans
			hd.m_faction = Character.Faction.Players;
			if (hd.m_randomSets.Length > 1)
			{
				hd.m_randomSets = hd.m_randomSets.Skip(hd.m_randomSets.Length - 1).ToArray();
			}
			PetList.Add(name + "Pet", go);
			return;
		}
		#region Helper
		public static void initIndicator()
		{
			Indicator = new GameObject("Indicator");
			DontDestroyOnLoad(Indicator);
			Indicator.AddComponent<StaticTarget>();
			Indicator.AddComponent<CapsuleCollider>();
			Indicator.SetActive(false);
		}
		public static void SummonHelper(string name)
		{
			if (petIns != null)
			{
				DBG.InfoCT("You can have only one Pet");//trans
				return;
			}
			var ppfb = ZNetScene.instance.GetPrefab(name + "Pet");
			if (ppfb == null)
			{
				DBG.blogWarning("Pet spawned failed cannot find the prefab");
			}
			Instantiate(ppfb, Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward * 2f + Vector3.up, Quaternion.identity);
			DBG.InfoCT("You summoned a " + name + "pet");//trans
		}
		#endregion
		public static void Clear()
		{
			petIns = null;
			DBG.blogInfo("PetList Clear");
		}
	}
}
