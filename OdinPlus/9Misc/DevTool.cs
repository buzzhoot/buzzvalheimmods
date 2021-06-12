using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Globalization;
using UnityEngine.UI;
using System.Text.Json;
using System.Text.Json.Serialization;

//|| ||DEBUG How to
//Right Ctrl + keypad[0-5] global key sets (how many kinds of boss have you defeated,quest is based on that)
//5 unlock all tasks and location
//3d teleport, console type in /3dtele x,y,z  like this: /3dtele 329.4,5034.8,605.7
//console /ctast1 Treasure /ctask2 Dungeon /ctask3 Hunt
//!DONT PRESS RinghtCtrl+F[6-10] it's for dev debug most like ly will break the mod
//Thx for your help for testing!
namespace OdinPlus
{
	public class DevTool : MonoBehaviour
	{
		public static bool DisableSaving = false;
		public static List<string> UnLocal = new List<string>();
		public static DevTool instance;
		private Action postZone;

		#region Mono
		private void Awake()
		{
			instance = this;
			Plugin.posZone = (Action)Delegate.Combine(Plugin.posZone, (Action)Reg);
		}
		private void Reg()
		{
			if (ZNet.instance.IsServer())
			{
				ZRoutedRpc.instance.Register<string>("RPC_ServerSetGlobalKey", new Action<long, string>(RPC_ServerSetGlobalKey));
				ZRoutedRpc.instance.Register("RPC_ServerResetGlobalKey", new Action<long>(RPC_ServerResetGlobalKey));
				DBG.blogWarning("Server Reg:cheat Global Key");
			}

		}
		public static bool IsIns()
		{
			return instance != null;
		}
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Alpha0) && Input.GetKey(KeyCode.RightControl))
			{
				DisableSaving = true;
			}
			if (Player.m_localPlayer == null)
			{
				return;
			}
			if (Input.GetKeyDown(KeyCode.F8) && Input.GetKey(KeyCode.RightControl))
			{
				OdinPlus.m_instance.Reset();
			}
			if (Input.GetKeyDown(KeyCode.F6) && Input.GetKey(KeyCode.RightControl))
			{
				OdinPlus.UnRegister();
				Destroy(Plugin.OdinPlusRoot);
			}
			if (Input.GetKeyDown(KeyCode.F9) && Input.GetKey(KeyCode.RightControl))
			{
				PrintMeadsLoc();
			}
			if (Input.GetKeyDown(KeyCode.F10) && Input.GetKey(KeyCode.RightControl))
			{
				PrintUnLoc();
			}
			if (Input.GetKeyDown(KeyCode.Keypad0) && Input.GetKey(KeyCode.RightControl))
			{
				RequestResetGlobalKey();
			}
			if (Input.GetKeyDown(KeyCode.Keypad1) && Input.GetKey(KeyCode.RightControl))
			{
				RequestResetGlobalKey();
				RequestSetGlobalKey("defeated_eikthyr");
			}

			if (Input.GetKeyDown(KeyCode.Keypad2) && Input.GetKey(KeyCode.RightControl))
			{
				RequestResetGlobalKey();
				RequestSetGlobalKey("defeated_eikthyr");
				RequestSetGlobalKey("defeated_gdking");
			}
			if (Input.GetKeyDown(KeyCode.Keypad3) && Input.GetKey(KeyCode.RightControl))
			{
				RequestResetGlobalKey();
				RequestSetGlobalKey("defeated_eikthyr");
				RequestSetGlobalKey("defeated_gdking");
				RequestSetGlobalKey("defeated_bonemass");
			}
			if (Input.GetKeyDown(KeyCode.Keypad4) && Input.GetKey(KeyCode.RightControl))
			{
				RequestResetGlobalKey();
				RequestSetGlobalKey("defeated_eikthyr");
				RequestSetGlobalKey("defeated_gdking");
				RequestSetGlobalKey("defeated_bonemass");
				RequestSetGlobalKey("defeated_moder");
			}
			if (Input.GetKeyDown(KeyCode.Keypad5) && Input.GetKey(KeyCode.RightControl))
			{
				RequestResetGlobalKey();
				RequestSetGlobalKey("defeated_eikthyr");
				RequestSetGlobalKey("defeated_gdking");
				RequestSetGlobalKey("defeated_bonemass");
				RequestSetGlobalKey("defeated_moder");
				RequestSetGlobalKey("defeated_goblinking");
			}
			if (Input.GetKeyDown(KeyCode.Period) && Input.GetKey(KeyCode.RightControl))
			{
				GameCamera.instance.ToggleFreeFly();
			}
			if (Input.GetKeyDown(KeyCode.Keypad7) && Input.GetKey(KeyCode.RightControl))
			{

			}
			if (Input.GetKeyDown(KeyCode.Keypad8) && Input.GetKey(KeyCode.RightControl))
			{

			}
			if (Input.GetKeyDown(KeyCode.Keypad9) && Input.GetKey(KeyCode.RightControl))
			{

			}
			if (Input.GetKeyDown(KeyCode.Comma) && Input.GetKey(KeyCode.RightControl))
			{
				ShowWindow = !ShowWindow;
			}
		}

		#endregion Mono

		#region Gui
		Rect windowRect = new Rect(200, 200, 400, 800);
		public bool ShowWindow = false;
		public string lname = "HelloWorld";
		GUIStyle style;
		private void OnGUI()
		{
			if (ShowWindow)
			{
				style = new GUIStyle();
				style.fontSize = 30;
				style.normal.textColor = Color.white;
				GUI.backgroundColor = Color.black;
				windowRect = GUILayout.Window(1219, windowRect, DevWindow, "Buzz Odin Plus Debug");
			}
		}
		void DevWindow(int WindowID)
		{

			GUI.DragWindow(new Rect(0, 0, 4000, 20));

			GUILayout.Width(400);
			GUILayout.Height(50);
			GUILayout.Label("welcom to use odin plus debug tool", style);
			var player = Player.m_localPlayer;
			if (player != null)
			{
				var val = ZoneSystem.instance.GetZone(player.transform.position);
				GUILayout.Label("Zone: " + val.ToString(), style);
				GUILayout.Label("Location: " + (player.transform.position).ToString(), style);

				if (ZNet.instance != null)
				{
					if (ZNet.instance.IsServer())
					{
						var dic = Traverse.Create(ZoneSystem.instance).Field<Dictionary<Vector2i, ZoneSystem.LocationInstance>>("m_locationInstances").Value;

						if (dic.ContainsKey(val))
						{
							string locName = dic[ZoneSystem.instance.GetZone(player.transform.position)].m_location.m_prefabName;
							GUILayout.Label(locName, style);
						}
						else
						{
							GUILayout.Label("No location here", style);
						}
					}
					GUILayout.BeginHorizontal();
					
					lname = GUILayout.TextField(lname, 40);
					if (GUILayout.Button("FindLoctaion"))
					{
						Game.instance.DiscoverClosestLocation(lname, player.transform.position, lname, 0);
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					if (GUILayout.Button("Key0"))
					{
						DBG.blogWarning("Reseting global keys");
						RequestResetGlobalKey();
					}
					if (GUILayout.Button("Key1"))
					{
						RequestResetGlobalKey();
						RequestSetGlobalKey("defeated_eikthyr");
					}

					if (GUILayout.Button("Key2"))
					{
						RequestResetGlobalKey();
						RequestSetGlobalKey("defeated_eikthyr");
						RequestSetGlobalKey("defeated_gdking");
					}
					if (GUILayout.Button("Key3"))
					{
						RequestResetGlobalKey();
						RequestSetGlobalKey("defeated_eikthyr");
						RequestSetGlobalKey("defeated_gdking");
						RequestSetGlobalKey("defeated_bonemass");
					}
					if (GUILayout.Button("Key4"))
					{
						RequestResetGlobalKey();
						RequestSetGlobalKey("defeated_eikthyr");
						RequestSetGlobalKey("defeated_gdking");
						RequestSetGlobalKey("defeated_bonemass");
						RequestSetGlobalKey("defeated_moder");
					}
					if (GUILayout.Button("Key5"))
					{
						RequestResetGlobalKey();
						RequestSetGlobalKey("defeated_eikthyr");
						RequestSetGlobalKey("defeated_gdking");
						RequestSetGlobalKey("defeated_bonemass");
						RequestSetGlobalKey("defeated_moder");
						RequestSetGlobalKey("defeated_goblinking");
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
					if (GUILayout.Button("FreeCam"))
					{
						GameCamera.instance.ToggleFreeFly();
					}
					foreach (QuestType item in Enum.GetValues(typeof(QuestType)))
					{
						if (GUILayout.Button(item.ToString()))
						{
							QuestManager.instance.CreateQuest(item);
						}
					}
					GUILayout.EndHorizontal();
				}


			}
		}
		#endregion Gui

		#region ZoneSys
		public static ZoneSystem.LocationInstance dbginsa;
		public static void findLoc()
		{
			Game.instance.DiscoverClosestLocation("Crypt4", Player.m_localPlayer.transform.position, "test", 1);
			Minimap.PinData pinData = Enumerable.First<Minimap.PinData>((List<Minimap.PinData>)Traverse.Create(Minimap.instance).Field("m_pins").GetValue(), (Minimap.PinData p) => p.m_type == Minimap.PinType.None && p.m_name == "");
			ZoneSystem.instance.FindClosestLocation("Crypt4", Player.m_localPlayer.transform.position, out dbginsa);
		}
		public static void findLoc(int x, int y)
		{
			var dic = Traverse.Create(ZoneSystem.instance).Field<Dictionary<Vector2i, ZoneSystem.LocationInstance>>("m_locationInstances").Value;
			Vector2i val = new Vector2i(x, y);
			dbginsa = dic[val];
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
			QuestManager.instance.CheckKey();
			QuestManager.instance.CreateRandomQuest();
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
				if (CMD.StartsWith("/3dtele"))
				{
					CMD = CMD.Remove(0, 8);
					var pos = CMD.Split(new char[] { ',' });
					Player.m_localPlayer.transform.position = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
				}
				if (CMD.StartsWith("/ctask"))
				{
					CMD = CMD.Remove(0, 6);
					QuestManager.instance.CreateQuest((QuestType)int.Parse(CMD), Game.instance.GetPlayerProfile().GetCustomSpawnPoint());
				}
			}
		}



		#endregion Debug

		#region RpcCheats
		public static void RequestSetGlobalKey(string gkey)
		{
			ZRoutedRpc.instance.InvokeRoutedRPC("RPC_ServerSetGlobalKey", new object[] { gkey });
			DBG.blogWarning("Client Request Set Global key :" + gkey);

		}
		public static void RequestResetGlobalKey()
		{
			ZRoutedRpc.instance.InvokeRoutedRPC("RPC_ServerResetGlobalKey");
			DBG.blogWarning("Client Request Reset Global key");

		}
		public static void RPC_ServerResetGlobalKey(long sender)
		{
			ZoneSystem.instance.ResetGlobalKeys();
			DBG.blogWarning("Client Request Reset Global key");

		}
		public static void RPC_ServerSetGlobalKey(long sender, string gkey)
		{
			ZoneSystem.instance.SetGlobalKey(gkey);
			DBG.blogWarning("Server set Global key: " + gkey);
		}

		#endregion RpcCheats

		#region HumanNpc


		#endregion HumanNpc
		//End Class

		#region DebugSaving
		public static OdinData.DataTable SaveDataDebug;
		public static string SaveDataFileName;

		public static void LoadDebugSavingData()
		{
			string file = Path.Combine(Application.persistentDataPath, (SaveDataFileName + ".odinplus"));
			FileStream fileStream = new FileStream(@file, FileMode.Open, FileAccess.Read);
			BinaryReader binaryReader = new BinaryReader(fileStream);
			var str = binaryReader.ReadString();
			SaveDataDebug=JsonSerializer.Deserialize<OdinData.DataTable>(str);
			fileStream.Close();
		}


		#endregion DebugSaving
	}
}
