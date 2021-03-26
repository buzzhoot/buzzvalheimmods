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
	public class DevTool : MonoBehaviour
	{
		#region Mono
		private void Update()
		{
			if (Player.m_localPlayer==null)
			{
				return;
			}
			if (Input.GetKeyDown(KeyCode.F8))
			{
				OdinPlus.m_instance.Reset();
			}
			if (Input.GetKeyDown(KeyCode.F6))
			{
				OdinPlus.UnRegister();
				Destroy(Plugin.OdinPlusRoot);
			}
			if (Input.GetKeyDown(KeyCode.F9))
			{
				PrintLocPrefab();
			}
			if (Input.GetKeyDown(KeyCode.Keypad1)&&Input.GetKeyDown(KeyCode.RightControl))
			{
				ZoneSystem.instance.SetGlobalKey("defeated_eikthyr");
			}
			if (Input.GetKeyDown(KeyCode.Keypad0)&&Input.GetKeyDown(KeyCode.RightControl))
			{
				ZoneSystem.instance.ResetGlobalKeys();
			}
			if (Input.GetKeyDown(KeyCode.Keypad2)&&Input.GetKeyDown(KeyCode.RightControl))
			{
				ZoneSystem.instance.SetGlobalKey("defeated_eikthyr");
				ZoneSystem.instance.SetGlobalKey("defeated_gdking");
			}
			if (Input.GetKeyDown(KeyCode.Keypad3)&&Input.GetKeyDown(KeyCode.RightControl))
			{
				ZoneSystem.instance.SetGlobalKey("defeated_eikthyr");
				ZoneSystem.instance.SetGlobalKey("defeated_gdking");
				ZoneSystem.instance.SetGlobalKey("defeated_bonemass");
			}
			if (Input.GetKeyDown(KeyCode.Keypad4)&&Input.GetKeyDown(KeyCode.RightControl))
			{
				ZoneSystem.instance.SetGlobalKey("defeated_eikthyr");
				ZoneSystem.instance.SetGlobalKey("defeated_gdking");
				ZoneSystem.instance.SetGlobalKey("defeated_bonemass");
				ZoneSystem.instance.SetGlobalKey("defeated_moder");
			}
		}
		
		#endregion Mono
		public static ZoneSystem.LocationInstance dbginsa;
		public static void findLoc()
		{
			Game.instance.DiscoverClosestLocation("Crypt4", Player.m_localPlayer.transform.position, "test", 1);
			Minimap.PinData pinData = Enumerable.First<Minimap.PinData>((List<Minimap.PinData>)Traverse.Create(Minimap.instance).Field("m_pins").GetValue(), (Minimap.PinData p) => p.m_type == Minimap.PinType.None && p.m_name == "");
			ZoneSystem.instance.FindClosestLocation("Crypt4", Player.m_localPlayer.transform.position, out dbginsa);
		}
		public static void findLocPrefab()
		{
			var a = ZoneSystem.instance.m_locations;
			int i = 0;
			foreach (var b in a)
			{
				if (b.m_prefabName.Contains("w"))
				{
					Debug.LogWarning(i);
				}
				i++;
			}
		}
		public static void PrintLocPrefab()
		{
			int i = 0;
			var a = ZoneSystem.instance.m_locations;
			string str = "LocList";
			foreach (var b in a)
			{
				str+=","+i+b.m_prefabName;
				i++;
			}
			Debug.LogWarning(str);
		}
		public static void ViewReward()
		{
			if (TaskManager.Root.transform.childCount == 0) { return; }
			if (TaskManager.Root.transform.GetChild(OdinData.Data.TaskCount - 1).GetComponent<DungeonTask>().Reward == null) { return; }
			GameCamera.instance.gameObject.transform.position = TaskManager.Root.transform.GetChild(OdinData.Data.TaskCount - 1).GetComponent<DungeonTask>().Reward.transform.position;
		}
		public static void finds()
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
		public static void Monster()
		{
			var a = GameObject.Instantiate(ZNetScene.instance.GetPrefab("Fenring"),Player.m_localPlayer.transform.position+Vector3.up+Vector3.forward*2,Quaternion.identity);
			Traverse.Create(a.GetComponent<Humanoid>()).Field<SEMan>("m_seman").Value.AddStatusEffect(OdinSE.MonsterSEList.ElementAt(4).Key);
		}


	}
}
