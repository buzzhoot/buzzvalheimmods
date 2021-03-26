using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Globalization;
using UnityEngine.UI;

namespace OdinPlus
{
	public class DevTool
	{

		public static void findLoc()
		{
			Game.instance.DiscoverClosestLocation("Crypt4", Player.m_localPlayer.transform.position, "test", 1);
			Minimap.PinData pinData = Enumerable.First<Minimap.PinData>((List<Minimap.PinData>)Traverse.Create(Minimap.instance).Field("m_pins").GetValue(), (Minimap.PinData p) => p.m_type == Minimap.PinType.None && p.m_name == "");
			ZoneSystem.instance.FindClosestLocation("Crypt4", Player.m_localPlayer.transform.position, out Plugin.dbginsa);
		}
		public static void findLocPrefab()
		{
			var a = ZoneSystem.instance.m_locations;
			int i = 0;
			foreach (var b in a)
			{
				if (b.m_prefabName == "Vendor_BlackForest")
				{
					Debug.LogWarning(i);
					return;
				}
				i++;
			}
		}
		public static void ViewReward()
		{
			if (TaskManager.Root.transform.childCount == 0) { return; }
			if (TaskManager.Root.transform.GetChild(OdinData.Data.TaskCount - 1).GetComponent<DungeonTask>().Reward == null) { return; }
			GameCamera.instance.gameObject.transform.position = TaskManager.Root.transform.GetChild(OdinData.Data.TaskCount - 1).GetComponent<DungeonTask>().Reward.transform.position;
		}
		private static void finds()
		{
			var a = Resources.FindObjectsOfTypeAll<GameObject>();
			string s = "";
			foreach (var item in a)
			{
				if (item.name == "Beehive" && item.scene.name == "locations")
				{
					s += (char)34 + item.transform.parent.name + (char)34 + ",";
				}
			}
			Debug.LogWarning(s);
		}



	}
}
