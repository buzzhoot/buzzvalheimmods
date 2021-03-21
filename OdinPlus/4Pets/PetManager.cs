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
		public static GameObject excObj;
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

			
			InitTroll();
			InitWolf();

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

			SetColor(go);

			if (hd.m_randomSets.Length > 1)
			{
				hd.m_randomSets = hd.m_randomSets.Skip(hd.m_randomSets.Length - 1).ToArray();
			}
			PetList.Add(name + "Pet", go);
			return;
		}


		#endregion

		#region Wolf
		private static void InitWolf()
		{
			var go = Instantiate(zns.GetPrefab("Wolf"), Root.transform);
			go.name = "WolfPet";
			var hum = go.GetComponent<Humanoid>();
			var mai = go.GetComponent<MonsterAI>();
			var tame = go.GetComponent<Tameable>();

			DestroyImmediate(go.GetComponent<Procreation>());
			DestroyImmediate(go.GetComponent<CharacterDrop>());

			var pw = go.AddComponent<PetWolf>();

			SetColor(go);

			//Humanoid

			hum.m_name += String.Format("\n<color=yellow><b>[{0}]</b></color>$odin_wolf_use", Plugin.KS_SecondInteractkey.Value.MainKey.ToString());
			hum.m_faction = Character.Faction.Players;
			//hum.SetLevel(4);
			//Ai Tweak
			mai.m_randomMoveInterval = 10000;
			mai.m_randomCircleInterval = 10000;
			mai.m_alertRange = 3;
			mai.m_viewRange = 3;
			mai.m_hearRange = 3;
			//Container
			var ctn = go.AddComponent<Container>();
			//pw.container = ctn;
			ctn.m_width = 2;
			ctn.m_height = 2;
			ctn.m_name = "WolfPack";//trans
			//ctn.m_destroyedLootPrefab = zns.GetPrefab("CargoCrate");
			ctn.m_bkg = zns.GetPrefab("CargoCrate").GetComponent<Container>().m_bkg;
			PetList.Add(go.name, go);

		}
		#endregion Wolf

		#region Feature
		public static void SummonPet(string name)
		{
			var ppfb = ZNetScene.instance.GetPrefab(name);
			var go = Instantiate(ppfb, Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward * 2f + Vector3.up, Quaternion.identity);
			go.GetComponent<Character>().SetLevel(4);
			DBG.InfoCT("You summoned a " + name);//trans
		}

		#endregion Feature

		#region Tool
		public static void initIndicator()
		{
			Indicator = new GameObject("Indicator");
			Indicator.transform.SetParent(Plugin.OdinPlusRoot.transform);
			Indicator.AddComponent<StaticTarget>();
			Indicator.AddComponent<CapsuleCollider>();
			Indicator.SetActive(false);
		}
		public static void SetColor(GameObject go)
		{
			var mat = go.GetComponentInChildren<Renderer>().material;
			mat.SetFloat("_Hue", 0.3f);
			mat.SetFloat("_Saturation", 0.5f);
			mat.EnableKeyword("_EMISSION");
			mat.SetColor("_EmissionColor", Color.HSVToRGB(0.3f, 0.5f, 0.3f) * 0.1f);
			mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

		}
		#endregion Tool
	}
}
