using System;
using System.Collections.Generic;
using UnityEngine;
namespace OdinPlus
{
	public class FxAssetManager : MonoBehaviour
	{
		#region Var	
		#region Member
		public static Transform Root;
		public static bool isInit = false;
		#endregion Member
		#region Lists
		public static Dictionary<string, GameObject> FxNNList;
		#endregion Lists
		#endregion Var

		#region Main
		private void Awake()
		{
			Root = new GameObject("Assets").transform;
			Root.SetParent(OdinPlus.PrefabParent.transform);
		}
		public static void Init()
		{
			if (isInit)
			{
				return;
			}
			SetupFxNN();
			isInit = true;
		}
		#endregion Main

		#region Tool
		private static GameObject InsVal(string prefab, string name)
		{
			var a = GameObject.Instantiate(ZNetScene.instance.GetPrefab(prefab), Root);
			a.name = name;
			return a;
		}
		private static GameObject InsVal(string prefab, string par, string name)
		{
			var a = GameObject.Instantiate(ZNetScene.instance.GetPrefab(prefab).FindObject(par), Root);
			a.name = name;
			return a;
		}
		private static GameObject ValFXcc(string prefab, string name, Color col, int whichList = 0)
		{
			var go = InsVal(prefab, name);
			go.GetComponent<Renderer>().material.color = col;
			selectList(go, name, whichList);
			return go;
		}
		private static GameObject ValFXcc(string prefab, string par, string name, Color col, int whichList = 0)
		{
			var go = InsVal(prefab, par, name);
			go.GetComponent<Renderer>().material.color = col;
			selectList(go, name, whichList);
			return go;
		}
		private static void selectList(GameObject go, string name, int whichList)
		{
			switch (whichList)
			{
				case 0:
					break;
				case 1:
					FxNNList.Add(name, go);
					break;
			}
		}

		#endregion Tool

		#region Feature	
		public static GameObject GetFxNN(string name)
		{
			return FxNNList[name];
		}
		#endregion Feature

		#region FxNN
		private static void SetupFxNN()
		{
			ValFXcc("odin", "odinsmoke", "RedSmoke", Color.red, 1);
			ValFXcc("odin", "odinsmoke", "BlueSmoke", Color.blue, 1);
			ValFXcc("odin", "odinsmoke", "YellowSmoke", Color.yellow, 1);
			ValFXcc("odin", "odinsmoke", "GreenSmoke", Color.green, 1);
		}
		#endregion Fx
	}
}