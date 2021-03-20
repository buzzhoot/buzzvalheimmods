using System.Reflection;
using UnityEngine;

namespace OdinPlus
{

	class NpcManager : MonoBehaviour
	{
		public static bool IsInit = false;
		public static GameObject Root;
		
		public static GameObject terrain;
		public static OdinGod m_odinGod;
		public static OdinTrader m_odinPot;
		/* 		public static OdinTrader m_odinChest;
				public static OdinShaman m_odinShaman;
				public static OdinTrader m_shamanChest;
				public static OdinMunin m_odinMunin;
				public static OdinGoblin m_odinGoblin; */

		#region Main
		private void Awake()
		{
			Init();
		}
		private void OnDestroy()
		{
			Clear();
		}
		public static void Init()
		{
			Root = new GameObject("OdinNPCs"); ;
			Root.SetActive(false);
			Root.transform.SetParent(OdinPlus.Root.transform);
			Root.transform.localRotation = Quaternion.Euler(new Vector3(0, -42, 0));
			Root.transform.localPosition = new Vector3(-6,0.2f,-8);

			

			InitOdinGod();
			InitOdinPot();
			InitOdinChest();
			InitShaman();
			InitMunin();

			Root.SetActive(true);
			IsInit = true;
		}
		public static void Clear()
		{
			m_odinGod.RestTerrian();
			IsInit = false;
			Destroy(Root);
		}

		#endregion Main	
		#region NPCs
		private static void InitTerrain()
		{	
			terrain= new GameObject("terrain");
			var tm=terrain.AddComponent<TerrainModifier>();
			tm.m_paintRadius=2.5f;
		}
		private static void InitOdinGod()
		{
			var podin = ZNetScene.instance.GetPrefab("odin");
			var odin = Instantiate(podin, Root.transform);

			DestroyImmediate(odin.GetComponent<ZNetView>());
			DestroyImmediate(odin.GetComponent<ZSyncTransform>());
			DestroyImmediate(odin.GetComponent<Odin>());
			DestroyImmediate(odin.GetComponent<Rigidbody>());
			m_odinGod = odin.AddComponent<OdinGod>();
			odin.transform.localPosition = new Vector3(0f, 0, 0f);
		}
		private static void InitOdinPot()
		{

			var pfire = ZNetScene.instance.GetPrefab("fire_pit");
			var pcaul = ZNetScene.instance.GetPrefab("piece_cauldron");
			var fire = CopyChildren(pfire);
			var caul = CopyChildren(pcaul);
			fire.transform.SetParent(Root.transform);
			caul.transform.SetParent(Root.transform);

			fire.transform.localPosition = new Vector3(1.5f, 0, -0.5f);
			caul.transform.localPosition = new Vector3(1.5f, 0, -0.5f);

			Destroy(fire.transform.Find("PlayerBase").gameObject);
			fire.transform.Find("_enabled_high").gameObject.SetActive(true);
			caul.transform.Find("HaveFire").gameObject.SetActive(true);

			m_odinPot = caul.AddComponent<OdinTrader>();
			m_odinPot.m_name = "$odin_pot_name";
			OdinPlus.traderNameList.Add(m_odinPot.m_name);
			m_odinPot.m_talker = m_odinGod.gameObject;

			foreach (var item in OdinMeads.MeadList.Values)
			{
				m_odinPot.m_items.Add(new Trader.TradeItem
				{
					m_prefab = item.GetComponent<ItemDrop>(),
					m_stack = 1,
					m_price = 1
				});
			}
		}
		private static void InitOdinChest()
		{
		}
		private static void InitShaman()
		{
			var prefab = ZNetScene.instance.GetPrefab("GoblinShaman");
			var go = Instantiate(prefab, Root.transform);

			DestroyImmediate(prefab.GetComponent<RandomAnimation>());

			
			DestroyImmediate(prefab.GetComponent<ZNetView>());
			DestroyImmediate(prefab.GetComponent<ZSyncAnimation>());
			DestroyImmediate(prefab.GetComponent<ZSyncTransform>());
			DestroyImmediate(prefab.GetComponent<Humanoid>());
			


		}
		private static void InitMunin()
		{

		}
		private static void InitGoblin()
		{

		}

		#endregion NPCs
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

		#endregion Utilities
	}
}
