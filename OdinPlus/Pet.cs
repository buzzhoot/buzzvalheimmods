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
		public static Pet instance;
		#endregion
		private void Awake()
		{
			instance = this;
		}
		public static void init(ZNetScene instance)
		{
			zns = instance;
			PrefabsParent = new GameObject("PetPrefab");
			PrefabsParent.transform.SetParent(Plugin.PrefabParent.transform);
			string[] l = Plugin.CFG_Pets.Value.Split(new char[] { ',' });
			foreach (string name in l)
			{
				CreatePetPrefab(name);
			}
		}
		private static void CreatePetPrefab(string name)
		{
			if(zns.GetPrefab(name)==null){
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
			hd.m_faction = Character.Faction.Players;
			if (hd.m_randomSets.Length > 1)
			{
				hd.m_randomSets = hd.m_randomSets.Skip(hd.m_randomSets.Length - 1).ToArray();
			}
			PetList.Add(name+"Pet", go);			
			return;
		}
		public static bool GetPrefab(string name, out GameObject go)
		{
			if (PetList.ContainsKey(name))
			{
				go = PetList[name];
				return true;
			}
			go = null;
			return false;
		}
		public static void Clear()
		{
			foreach (var o in PetList)
			{
				GameObject.Destroy(o.Value);
			}
			petIns = null;
			PetList.Clear();
			DBG.blogInfo("PetList Clear");
		}
		public static void CmdHelper()
		{
			RaycastHit raycastHit;
			if (petIns != null && Input.GetKeyDown(KeyCode.BackQuote) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit))
			{
				Vector3 point = raycastHit.point;
				Debug.DrawRay(Player.m_localPlayer.transform.position, point, Color.white);
				if (Indicator.activeSelf)
				{
					Indicator.SetActive(false);
					Traverse.Create(petIns.GetComponent<MonsterAI>()).Field("m_targetStatic").SetValue(null);
					DBG.InfoCT("Stop pet attack");
					return;
				}
				Indicator.SetActive(true);
				Indicator.transform.position = raycastHit.point;
				ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", new object[] { raycastHit.point, 3, "attack here!", "" });
				Traverse.Create(petIns.GetComponent<MonsterAI>()).Field("m_targetStatic").SetValue(Indicator.GetComponent<StaticTarget>());
				DBG.InfoCT("Pet force attack");
				return;
			}
		}
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
			petIns = Instantiate(ppfb, Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward * 2f + Vector3.up, Quaternion.identity);
			DBG.InfoCT("You summoned a " + name +"pet");//trans
		}
	}
}
