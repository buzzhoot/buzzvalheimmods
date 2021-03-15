using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OdinPlus
{
	class OdinMeads
	{
		private static ZNetScene zns;
		//private static Dictionary<string, GameObject> MeadList = new Dictionary<string, GameObject>();
		private static ObjectDB objectDB;
		public static List<GameObject> MeadList = new List<GameObject>();
		public static List<string> MeadNameList = new List<string>();

		private static GameObject PrefabsParent;
		public static void init()
		{
			PrefabsParent = new GameObject("MeadPrefabs");
			PrefabsParent.transform.SetParent(Plugin.PrefabParent.transform);
			PrefabsParent.SetActive(false);
			objectDB = ObjectDB.instance;
			zns = ZNetScene.instance;
		}
		public static void initSF()
		{

		}
		public static void CreateMeadPrefab(string name)
		{

		}
	}
}
