using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace OdinPlus
{
	class PetManager : MonoBehaviour
	{
		#region var
		private static ZNetScene zns;
		private static GameObject Root;
		private static Dictionary<string, GameObject> PetList = new Dictionary<string, GameObject>();
		public static GameObject TrollIns;
		public static GameObject WolfIns;
		public static GameObject Indicator;
		public static bool isInit = false;
		//public static Pet instance;
		#endregion
		#region Main
		private void Awake()
		{
			initIndicator();
		}
		public static void Clear()
		{
			TrollIns = null;
			DBG.blogInfo("PetList Clear");
		}
		public static void Init()
		{

			zns = ZNetScene.instance;
			Root = new GameObject("PetPrefab");
			Root.transform.SetParent(OdinPlus.PrefabParent.transform);

			//notice Init Here
			InitTroll();

			OdinPlus.OdinPostRegister(PetList);
			isInit = true;
		}
		#endregion Main

		#region Troll
		private static void InitTroll()
		{
			string[] l = Plugin.CFG_Pets.Value.Split(new char[] { ',' });
			foreach (string name in l)
			{
				CreateTrollPrefab(name);
			}
		}
		private static void CreateTrollPrefab(string name)
		{
			if (zns.GetPrefab(name) == null)
			{
				DBG.blogWarning("can't find the prefab zns :" + name);
				return;
			}
			var go = Instantiate(zns.GetPrefab(name), Root.transform);
			go.name = name + "Pet";
			Tameable tame;
			if (!go.TryGetComponent<Tameable>(out tame))
			{
				tame = go.AddComponent<Tameable>();
			}
			go.AddComponent<PetTroll>();
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
		public static void initIndicator()
		{
			Indicator = new GameObject("Indicator");
			DontDestroyOnLoad(Indicator);
			Indicator.AddComponent<StaticTarget>();
			Indicator.AddComponent<CapsuleCollider>();
			Indicator.SetActive(false);
		}
		public static void SummonTroll(string name)
		{
			if (TrollIns != null)
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
		public static void SummonWolf()//add
		{

		}
		#endregion

		#region Wolf
		private static void InitWolf()
		{
			var go = Instantiate(zns.GetPrefab("Wolf"), Root.transform);
			go.name = "WolfPet";
			var hum=go.GetComponent<Humanoid>();
			var mai = go.GetComponent<MonsterAI>();
			var tame = go.GetComponent<Tameable>();
			var pw = go.AddComponent<PetWolf>();

			//-- mname
			//aiTweak
			//?tame
			var ctnRoot= new GameObject("fake root");
			var ctnopen= new GameObject("fake open");
			var ctnclose= new GameObject("fake close");
			var ctn = ctnRoot.AddComponent<Container>();
			ctnRoot.transform.SetParent(go.transform);
			ctn.m_open=ctnopen;
			ctn.m_closed=ctnclose;
			ctn.m_privacy=Container.PrivacySetting.Private;
			ctn.m_destroyedLootPrefab=zns.GetPrefab("CargoCreate");
			//ctn.m_defaultItems //addItem Caller
			//?should add Collider to this,then move it outside world
			//?should open and close be in the root
			//-- ctn bkg-width=height=
		}
		#endregion Wolf
	}
}
