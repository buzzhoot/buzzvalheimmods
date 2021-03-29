using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

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
		public static bool DisableSaving = false;
		public static List<string> UnLocal = new List<string>();
		#region Mono
		private void Update()
		{
			if (Player.m_localPlayer == null)
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
				PrintMeadsLoc();
			}
			if (Input.GetKeyDown(KeyCode.F10))
			{
				PrintUnLoc();
			}
			if (Input.GetKeyDown(KeyCode.Keypad0) && Input.GetKeyDown(KeyCode.RightControl))
			{
				ZoneSystem.instance.ResetGlobalKeys();
			}
			if (Input.GetKeyDown(KeyCode.Keypad1) && Input.GetKey(KeyCode.RightControl))
			{
				ZoneSystem.instance.ResetGlobalKeys();
				ZoneSystem.instance.SetGlobalKey("defeated_eikthyr");
			}

			if (Input.GetKeyDown(KeyCode.Keypad2) && Input.GetKey(KeyCode.RightControl))
			{
				ZoneSystem.instance.ResetGlobalKeys();
				ZoneSystem.instance.SetGlobalKey("defeated_eikthyr");
				ZoneSystem.instance.SetGlobalKey("defeated_gdking");
			}
			if (Input.GetKeyDown(KeyCode.Keypad3) && Input.GetKey(KeyCode.RightControl))
			{
				ZoneSystem.instance.ResetGlobalKeys();
				ZoneSystem.instance.SetGlobalKey("defeated_eikthyr");
				ZoneSystem.instance.SetGlobalKey("defeated_gdking");
				ZoneSystem.instance.SetGlobalKey("defeated_bonemass");
			}
			if (Input.GetKeyDown(KeyCode.Keypad4) && Input.GetKey(KeyCode.RightControl))
			{
				ZoneSystem.instance.ResetGlobalKeys();
				ZoneSystem.instance.SetGlobalKey("defeated_eikthyr");
				ZoneSystem.instance.SetGlobalKey("defeated_gdking");
				ZoneSystem.instance.SetGlobalKey("defeated_bonemass");
				ZoneSystem.instance.SetGlobalKey("defeated_moder");
			}
			if (Input.GetKeyDown(KeyCode.Keypad5) && Input.GetKey(KeyCode.RightControl))
			{
				ZoneSystem.instance.ResetGlobalKeys();
				ZoneSystem.instance.SetGlobalKey("defeated_eikthyr");
				ZoneSystem.instance.SetGlobalKey("defeated_gdking");
				ZoneSystem.instance.SetGlobalKey("defeated_bonemass");
				ZoneSystem.instance.SetGlobalKey("defeated_moder");
				ZoneSystem.instance.SetGlobalKey("defeated_goblinking");
			}
			if (Input.GetKeyDown(KeyCode.Keypad6) && Input.GetKey(KeyCode.RightControl))
			{
				DBG.a();
			}
		}

		#endregion Mono
		#region ZoneSys
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
				str += "," + b.m_prefabName;
				i++;
			}
			Debug.LogWarning(str);
		}
		#endregion ZoneSys

		#region Task
		public static void ViewReward()
		{
			//if (TaskManager.Root.transform.childCount == 0) { return; }
			//if (TaskManager.Root.transform.GetChild(OdinData.Data.TaskCount - 1).GetComponent<OdinTask>().Reward == null) { return; }
			//GameCamera.instance.gameObject.transform.position = TaskManager.Root.transform.GetChild(OdinData.Data.TaskCount - 1).GetComponent<DungeonTask>().Reward.transform.position;
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
			var a = GameObject.Instantiate(ZNetScene.instance.GetPrefab("Fenring"), Player.m_localPlayer.transform.position + Vector3.up + Vector3.forward * 2, Quaternion.identity);
			Traverse.Create(a.GetComponent<Humanoid>()).Field<SEMan>("m_seman").Value.AddStatusEffect(OdinSE.MonsterSEList.ElementAt(4).Key);
		}
		#endregion Task

		#region Print
		#region Print Tool
		/* 						var s = "";
					s += DevTool.PrintStringArray(m_tier0);
					s += DevTool.PrintStringArray(m_tier1);
					s += DevTool.PrintStringArray(m_tier2);
					s += DevTool.PrintStringArray(m_tier3);
					s += DevTool.PrintStringArray(m_tier4);
					DBG.blogWarning(s); */
		#endregion Print Tool
		public static string PrintStringArray(string[] list)
		{
			string s = "";
			foreach (var item in list)
			{
				s += string.Format("\"{0}\",", item);
			}
			Debug.Log(s);
			return s;
		}
		public static void PrintUnLoc()
		{
			string s = "";
			foreach (var item in UnLocal)
			{
				s += item + ",";
			}
			Debug.Log(s);
		}
		/* 		[HarmonyTranspiler]
				[HarmonyPatch(typeof(Localization),"Translate")]
				public static IEnumerable<CodeInstruction> TranslateTranspiler(IEnumerable<CodeInstruction> instructions)
				{
					List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
					//DumpIL(codes);
					for (int i = 0; i < codes.Count(); i++)
					{
						if (codes[i].opcode==OpCodes.Ret&&codes[i+1].opcode==OpCodes.Ldstr)
						{
							string s = (string)codes[i+4].operand;
							UnLocal+=s+",";
						}
					}
					return codes.AsEnumerable();
				} */
		[HarmonyPatch(typeof(Localization), "Translate")]
		private static class Postfix_Localization_Translate
		{
			private static void Postfix(ref string __result)
			{
				if (__result.StartsWith("["))
				{
					if (UnLocal.Contains(__result))
					{
						return;
					}
					UnLocal.Add(__result);
				}
			}
		}

		private static void PrintMeadsList()
		{
			string s = "";
			foreach (var item in OdinMeads.MeadList.Keys)
			{
				s += item + ",";
			}
			DBG.blogWarning(s);
		}
		private static void PrintMeadsLoc()
		{
			string s = "";
			foreach (var item in OdinMeads.MeadList.Keys)
			{
				DBG.blogWarning("op_" + item + "_name");
				DBG.blogWarning("op_" + item + "_desc");
				DBG.blogWarning("op_" + item + "_tooltip");
			}
		}
		#endregion Print

		#region Debug
		public static void TestA()
		{
			TaskManager.CheckKey();
			TaskManager.CreateRandomTask();
		}
		public static void TestB()
		{
			GameCamera.instance.ToggleFreeFly();
		}
		public static void TestC()
		{
			DevTool.ViewReward();
		}
		public static void InputCMD(string CMD)
		{
			if (CMD.Length > 0)
			{
				if (CMD.StartsWith(" "))
				{
					CMD = CMD.Remove(0, 1);
				}
				if (CMD == "testa")
				{
					DevTool.TestA();
				}
				if (CMD == "testb")
				{
					DevTool.TestB();
				}
				if (CMD == "testc")
				{
					DevTool.TestC();
				}
				if (CMD.StartsWith("/ctask"))
				{
					CMD=CMD.Remove(0,6);
					TaskManager.instance.CreateTask((TaskManager.TaskType)int.Parse(CMD));
					Debug.Log("creatin task");
				}
			}
		}
		[HarmonyPatch(typeof(Console), "InputText")]
		private static class Patch_Console_InputText
		{
			private static void Prefix()
			{
				//InputCMD(global::Console.instance.m_input.text);
			}
		}


		#endregion Debug

		//End Class
	}
}
