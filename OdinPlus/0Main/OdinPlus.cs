using System.Reflection;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using BepInEx.Logging;
using UnityEngine;
//||X||may be setup NPC somewhere else 
//? Hi i am the manager ,spwan npc Somewhere Else;
namespace OdinPlus
{

	public class OdinPlus : MonoBehaviour
	{
		#region data var
		public static bool isInit = false;
		public static bool isNPCInit = false;
		public static bool isPetInit = false;
		public static OdinPlus m_instance;
		#endregion
		#region List
		public static List<string> traderNameList = new List<string>();
		//public static List<int> preRegList = new List<int>();
		public static Dictionary<int, GameObject> odbRegList = new Dictionary<int, GameObject>();
		public static Dictionary<int, GameObject> znsRegList = new Dictionary<int, GameObject>();

		#endregion
		#region Ojects var
		public static GameObject Root;
		public static GameObject OdinPrefab;
		public static GameObject OdinNPCParent;
		public static OdinGod m_OdinGod;
		public static GameObject PrefabParent;
		#endregion
		#region assets var

		public static Sprite OdinCreditIcon;
		public static List<Sprite> OdinMeadsIcon = new List<Sprite>();
		public static List<Sprite> OdinSEIcon = new List<Sprite>();
		public static Sprite TrollHeadIcon;
		public static Sprite WolfHeadIcon;
		public static Sprite CoinsIcon;
		public static Sprite OdinLegacyIcon;

		#endregion

		#region Mono
		private void Awake()
		{
			m_instance = this;
			Root = this.gameObject;
			PrefabParent = new GameObject("OdinPlusPrefabs");
			PrefabParent.SetActive(false);
			PrefabParent.transform.SetParent(Root.transform);
		}
		#endregion Mono

		#region Patch
		public static void Init()
		{
			initAssets();
			Root.AddComponent<OdinSE>();
			Root.AddComponent<OdinMeads>();
			Root.AddComponent<OdinItem>();
			Root.AddComponent<PetManager>();
			Root.AddComponent<TaskManager>();
			isInit = true;
		}
		public static void PostODB()
		{
			ValRegister(ObjectDB.instance);
		}
		public static void PreZNS(ZNetScene zns)
		{
			ValRegister(zns);
		}
		public static void PostZNS()
		{
			if (!PetManager.isInit)
			{
				PetManager.Init();
			}
			ValRegister();
		}
		public static void InitNPC()
		{
			Root.AddComponent<NpcManager>();
			isNPCInit = true;
		}
		public static void Clear()
		{
			PetManager.Clear();
			Destroy(Root.GetComponent<NpcManager>());
			isNPCInit = false;
		}
		#endregion Patch

		#region Tool
		public static void ProcessCommands(string inCommand)
		{
			if (inCommand.Length > 0)
			{
				if (inCommand.StartsWith(" "))
				{
					inCommand = inCommand.Remove(0, 1);
				}
				if (inCommand == "testa")
				{
					Plugin.TestA();
				}
				if (inCommand == "testb")
				{
					Plugin.TestB();
				}
			}
		}

		#endregion Tool

		#region Assets
		public static void initAssets()
		{
			OdinCreditIcon = ObjectDB.instance.GetItemPrefab("HelmetOdin").GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0];
			OdinMeadsIcon.Add(ObjectDB.instance.GetItemPrefab("MeadTasty").GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0]);
			OdinSEIcon.Add(OdinCreditIcon);
			TrollHeadIcon = ObjectDB.instance.GetItemPrefab("TrophyFrostTroll").GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0];
			WolfHeadIcon = ObjectDB.instance.GetItemPrefab("TrophyWolf").GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0];
			CoinsIcon = ObjectDB.instance.GetItemPrefab("Coins").GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0];
			OdinLegacyIcon = OdinCreditIcon.LoadSpriteFromTexture(Util.LoadTextureRaw(Util.GetResource(Assembly.GetCallingAssembly(), "OdinPlus.Resources.OdinLegacy.png")), 100f);
		}
		#endregion Assets

		#region Feature
		public static void ValRegister(ObjectDB odb)
		{
			OdinSE.Register();
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
		public static void OdinPreRegister(Dictionary<string, GameObject> list, string name)
		{
			foreach (var item in list)
			{
				odbRegList.Add(item.Key.GetStableHashCode(), item.Value);
			}
			DBG.blogInfo("Register " + name + " for ODB");
		}
		public static void OdinPostRegister(Dictionary<string, GameObject> list)
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
			DBG.blogWarning("UnRegister all list");
		}
		#endregion Feature
		#region Debug
		public void Reset()
		{
			initAssets();
			Plugin.OdinPlusRoot.AddComponent<OdinSE>();
			Plugin.OdinPlusRoot.AddComponent<OdinMeads>();
			Plugin.OdinPlusRoot.AddComponent<OdinItem>();
			Plugin.OdinPlusRoot.AddComponent<PetManager>();
			Plugin.OdinPlusRoot.AddComponent<TaskManager>();
			isInit = true;
			PostODB();
			var m_namedPrefabs = Traverse.Create(ZNetScene.instance).Field<Dictionary<int, GameObject>>("m_namedPrefabs").Value;
			foreach (var item in odbRegList)
			{
				ZNetScene.instance.m_prefabs.Add(item.Value);
				m_namedPrefabs.Add(item.Key, item.Value);
			}
			PostZNS();
			NpcManager.RavenPrefab=Tutorial.instance.m_ravenPrefab.transform.Find("Munin").gameObject;
			InitNPC();

		}
		#endregion Debug

	}
}