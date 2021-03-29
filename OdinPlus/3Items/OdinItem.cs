using UnityEngine;
using System.Collections.Generic;

namespace OdinPlus
{
	class OdinItem : MonoBehaviour
	{
		//private static Dictionary<string, GameObject> MeadList = new Dictionary<string, GameObject>()
		private static GameObject MeadTasty;
		private static GameObject TrophyGoblinShaman;
		public static Dictionary<string, Sprite> PetItemList = new Dictionary<string, Sprite>{
			{"ScrollTroll", OdinPlus.TrollHeadIcon},
			{"ScrollWolf", OdinPlus.WolfHeadIcon}
			};
		public static Dictionary<string, GameObject> ObjectList = new Dictionary<string, GameObject>();
		
		public static GameObject Root;

		#region Mono
		private void Awake()
		{
			Root = new GameObject("ObjectList");
			Root.transform.SetParent(OdinPlus.PrefabParent.transform);
			Root.SetActive(false);

			var objectDB = ObjectDB.instance;
			MeadTasty = objectDB.GetItemPrefab("MeadTasty");
			TrophyGoblinShaman = objectDB.GetItemPrefab("TrophyGoblinShaman");

			InitLegacy();
			InitPetItem();

			OdinPlus.OdinPreRegister(ObjectList, nameof(ObjectList));

		}
		#endregion Mono
		#region PetItems
		private void InitPetItem()
		{
			foreach (var pet in PetItemList)
			{
				CreatePetItemPrefab(pet.Key, pet.Value);
			}
		}
		private void CreatePetItemPrefab(string name, Sprite icon)
		{
			GameObject go = Instantiate(MeadTasty, Root.transform);
			go.name = name;

			var id = go.GetComponent<ItemDrop>().m_itemData.m_shared;
			id.m_name = "$op_" + name + "_name";
			id.m_icons[0] = icon;
			id.m_description = "$op_" + name + "_desc";

			id.m_maxStackSize = 1;
			id.m_consumeStatusEffect = OdinSE.SElist[name];

			go.GetComponent<ItemDrop>().m_itemData.m_quality = 4;
			id.m_maxQuality = 5;

			ObjectList.Add(name, go);
		}
		#endregion PetItems

		#region Legacy
		private static void InitLegacy()
		{
			string name = "OdinLegacy";
			GameObject go = Instantiate(TrophyGoblinShaman, Root.transform);
			go.name = "OdinLegacy";
			var id = go.GetComponent<ItemDrop>().m_itemData.m_shared;
			id.m_name = "$op_" + name + "_name";
			id.m_icons[0] = OdinPlus.OdinLegacyIcon;
			id.m_description = "$op_" + name + "_desc";
			id.m_itemType = ItemDrop.ItemData.ItemType.None;

			id.m_maxStackSize = 10;
			id.m_maxQuality = 5;

			ObjectList.Add(name, go);

		}

		#endregion Legacy

		#region Tool
		public static ItemDrop.ItemData GetItemData(string name)
		{
			return ObjectList[name].GetComponent<ItemDrop>().m_itemData;
		}
		public static GameObject GetObject(string name)
		{
			return ObjectList[name];
		}

		#endregion Tool
	}
}