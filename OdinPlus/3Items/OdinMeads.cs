using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HarmonyLib;

namespace OdinPlus
{
	class OdinMeads : MonoBehaviour
	{
		//private static Dictionary<string, GameObject> MeadList = new Dictionary<string, GameObject>()
		private static GameObject MeadTasty;
		public static Dictionary<string, GameObject> MeadList = new Dictionary<string, GameObject>();
		private static GameObject Root;
		private void Awake()
		{
			Root = new GameObject("MeadList");
			Root.transform.SetParent(OdinPlus.PrefabParent.transform);
			Root.SetActive(false);

			var objectDB = ObjectDB.instance;
			MeadTasty = objectDB.GetItemPrefab("MeadTasty");

			//notice Init Here

			InitValMead();

			OdinPlus.OdinPreRegister(MeadList,nameof(MeadList));
		}
		
		public static void InitValMead()
		{
			foreach (var item in OdinSE.ValDataList.Keys)
			{
				CreateValMeadPrefab(item);
			}
		}
		public static void CreateValMeadPrefab(string name)
		{
			GameObject go = Instantiate(MeadTasty, Root.transform);
			go.name = name;
			var id = go.GetComponent<ItemDrop>().m_itemData.m_shared;
			id.m_name = "$odin_" + name + "_name";
			id.m_icons[0] = OdinPlus.OdinMeadsIcon[0];
			id.m_description = "$odin_" + name + "_desc";
			
			id.m_consumeStatusEffect = OdinSE.SElist[name];
			
			MeadList.Add(name, go);
		}
	}
}
