using System.Reflection;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using BepInEx.Logging;
using UnityEngine;
//||X||may be setup NPC somewhere else 
//? Up Up UP
namespace OdinPlus
{

	public class OdinPlus : MonoBehaviour
	{
		#region List
		public static List<string> traderNameList = new List<string>();
		#endregion
		#region Ojects var
		public static GameObject Root;
		public static GameObject OdinPrefab;
		public static GameObject OdinNPCParent;
		public static OdinTrader m_odinTrader;
		public static GameObject PrefabParent;
		#endregion
		#region assets var
		public static Sprite OdinCreditIcon;
		public static Sprite[] OdinMeadsIcon;
		public static Sprite TrollHeadIcon;
		public static Sprite CoinsIcon;

		#endregion
		private void Awake()
		{
			Root = this.gameObject;
			PrefabParent = new GameObject("OdinPlusPrefabs");
			PrefabParent.SetActive(false);
			PrefabParent.transform.SetParent(Root.transform);
			OdinNPCParent = new GameObject("OdinNPCs");
			OdinNPCParent.SetActive(false);
			OdinNPCParent.transform.SetParent(Root.transform);
		}

		#region Tool
		public static void ProcessCommands(string inCommand)
		{
			if (inCommand.Length > 0)
			{
				if (inCommand.StartsWith(" "))
				{
					inCommand = inCommand.Remove(0, 1);
				}
				if (inCommand == "bzd")
				{
					//!init here
				}
				if (inCommand == "test")
				{
					Destroy(Plugin.OdinPlusRoot);
				}
			}
		}

		#endregion
		#region Feature
		public static void initAssets()//!Peformance
		{
			/* 			var splist = Resources.FindObjectsOfTypeAll<Sprite>();
						foreach (var sp in splist)
						{
							if (sp.name == "TrophyForestTroll")	{TrollHeadIcon = sp;}
							if (sp.name == "HelmetOdin") { OdinHelmetIcon = sp; }
						} */
			OdinCreditIcon = ObjectDB.instance.GetItemPrefab("HelmetOdin").GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0];
			OdinMeadsIcon.AddItem(OdinCreditIcon);
			TrollHeadIcon = ObjectDB.instance.GetItemPrefab("TrophyFrostTroll").GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0];
			CoinsIcon = ObjectDB.instance.GetItemPrefab("Coins").GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0];
		}
		public static void initPrefabs()
		{
			OdinPlus.initAssets();
			Pet.initIndicator();
			OdinSE.init();
			OdinMeads.init();
		}
		public static void Regsiter(ZNetScene zns)
		{
			OdinMeads.Register(zns);
		}
		
		public static void Regsiter(ObjectDB odb)
		{
			OdinSE.Register();
			OdinMeads.Register(odb);
		}
		public static void PostRegister()
		{
			Pet.Register();
		}
		public static void DebugRegister()
		{
			OdinSE.init();
			PostRegister();
			OdinMeads.PostRegister();
			Pet.Register();
		}
		public static void Init()
		{
			initAssets();
			initPrefabs();
			//initNPCs();
		}
		public static void DebugInit()
		{
			initAssets();
			initPrefabs();
			initNPCs();
			Pet.init(ZNetScene.instance);
		}
		public static void initNPCs()
		{
			var podin = ZNetScene.instance.GetPrefab("odin");
			var pfire = ZNetScene.instance.GetPrefab("fire_pit");
			var pcaul = ZNetScene.instance.GetPrefab("piece_cauldron");
			var odin = CopyChildren(podin);
			odin.transform.SetParent(OdinNPCParent.transform);
			var c = new GameObject("coll");
			c.AddComponent<CapsuleCollider>();
			c.transform.SetParent(odin.transform);
			c.transform.localScale = new Vector3(1, 2, 1);
			c.transform.localPosition = Vector3.up;
			var fire = CopyChildren(pfire);
			var caul = CopyChildren(pcaul);
			fire.transform.SetParent(OdinNPCParent.transform);
			caul.transform.SetParent(OdinNPCParent.transform);


			odin.transform.localPosition = new Vector3(0f, 0, 0f);
			fire.transform.localPosition = new Vector3(1.5f, 0, -0.5f);
			caul.transform.localPosition = new Vector3(1.5f, 0, -0.5f);

			Destroy(fire.transform.Find("PlayerBase").gameObject);
			fire.transform.Find("_enabled_high").gameObject.SetActive(true);
			caul.transform.Find("HaveFire").gameObject.SetActive(true);
			m_odinTrader = odin.AddComponent<OdinTrader>();
			//?init pot
			var caulStore = caul.AddComponent<OdinStore>();
			caulStore.m_name = "$odin_pot_name";
			traderNameList.Add(caulStore.m_name);
			caulStore.m_talkername = "$odin";
			caulStore.m_talker = odin;
			var tm = OdinMeads.MeadList[0].GetComponent<ItemDrop>();
			caulStore.m_items.Add(new Trader.TradeItem
			{
				m_prefab = tm,
				m_stack = 1,
				m_price = 1
			});

			OdinPrefab = odin;
			OdinNPCParent.SetActive(true);
		}
		#endregion
		#region Utilities
		public static GameObject CopyChildren(GameObject prefab)
		{
			int cc = prefab.transform.childCount;
			GameObject r = new GameObject(prefab.name);
			for (int i = 0; i < cc; i++)
			{
				var o = prefab.transform.GetChild(i).gameObject;
				var a = Instantiate(o, r.transform);
				a.name = o.name;
			}
			return r;
		}
		public static void CopyComponent(Component original, GameObject destination)
		{
			System.Type type = original.GetType();
			Component copy = destination.AddComponent(type);
			// Copied fields can be restricted with BindingFlags
			FieldInfo[] fields = type.GetFields();
			PropertyInfo[] props = type.GetProperties();
			foreach (FieldInfo field in fields)
			{
				field.SetValue(copy, field.GetValue(original));
			}
			//foreach(PropertyInfo p in props)
			//{
			//    props.SetValue(copy, props.GetValue(original));
			//}
			return;
		}

		#endregion
	}
}