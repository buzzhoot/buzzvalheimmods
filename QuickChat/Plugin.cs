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
using System.Globalization;

namespace QuickChat
{
	[BepInPlugin("buzz.valheim.QuickChat", "QuickChat", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		public static ConfigEntry<int> nexusID;
		#region Config
		//public static ConfigEntry<int> nexusID;
		public static ConfigEntry<string> QC0;
		public static ConfigEntry<string> QC1;
		public static ConfigEntry<string> QC2;
		public static ConfigEntry<string> QC3;
		public static ConfigEntry<string> QC4;
		public static ConfigEntry<string> QC5;
		public static ConfigEntry<string> QC6;
		public static ConfigEntry<string> QC7;
		public static ConfigEntry<string> QC8;
		public static ConfigEntry<string> QC9;

		public static ConfigEntry<KeyboardShortcut> QK0;
		public static ConfigEntry<KeyboardShortcut> QK1;
		public static ConfigEntry<KeyboardShortcut> QK2;
		public static ConfigEntry<KeyboardShortcut> QK3;
		public static ConfigEntry<KeyboardShortcut> QK4;
		public static ConfigEntry<KeyboardShortcut> QK5;
		public static ConfigEntry<KeyboardShortcut> QK6;
		public static ConfigEntry<KeyboardShortcut> QK7;
		public static ConfigEntry<KeyboardShortcut> QK8;
		public static ConfigEntry<KeyboardShortcut> QK9;
		#endregion
		public static ManualLogSource logger;
		public static bool LastChat=false;
        public static bool LastCMD=false;

		private void Awake()
		{
			Plugin.logger = base.Logger;
			Plugin.nexusID = base.Config.Bind<int>("General", "NexusID", 534, "Nexus mod ID for updates");
			QC0 = base.Config.Bind<string>("1QuickChatText", "QuickChatText0", "cmd:imacheater");
			QC1 = base.Config.Bind<string>("1QuickChatText", "QuickChatText1", "/tod 0.5");
			QC2 = base.Config.Bind<string>("1QuickChatText", "QuickChatText2", "/env Clear");
			QC3 = base.Config.Bind<string>("1QuickChatText", "QuickChatText3", "/spawn CookedMeat;/spawn RawMeat");
			QC4 = base.Config.Bind<string>("1QuickChatText", "QuickChatText4", "cmd:debugmode");
			QC5 = base.Config.Bind<string>("1QuickChatText", "QuickChatText5", "/s i'am using Quick Chat!");
			QC6 = base.Config.Bind<string>("1QuickChatText", "QuickChatText6", "/w i'am using Quick Chat...");
			QC7 = base.Config.Bind<string>("1QuickChatText", "QuickChatText7", "/wave");
			QC8 = base.Config.Bind<string>("1QuickChatText", "QuickChatText8", "/shareMap");
			QC9 = base.Config.Bind<string>("1QuickChatText", "QuickChatText9", "cmd:spawn Hammer");
			QK0 = base.Config.Bind<KeyboardShortcut>("2QuickChatShorcut", "QuickChatShortcut0", new KeyboardShortcut(KeyCode.PageDown, KeyCode.Keypad0));
			QK1 = base.Config.Bind<KeyboardShortcut>("2QuickChatShorcut", "QuickChatShortcut1", new KeyboardShortcut(KeyCode.PageDown, KeyCode.Keypad1));
			QK2 = base.Config.Bind<KeyboardShortcut>("2QuickChatShorcut", "QuickChatShortcut2", new KeyboardShortcut(KeyCode.PageDown, KeyCode.Keypad2));
			QK3 = base.Config.Bind<KeyboardShortcut>("2QuickChatShorcut", "QuickChatShortcut3", new KeyboardShortcut(KeyCode.PageDown, KeyCode.Keypad3));
			QK4 = base.Config.Bind<KeyboardShortcut>("2QuickChatShorcut", "QuickChatShortcut4", new KeyboardShortcut(KeyCode.PageDown, KeyCode.Keypad4));
			QK5 = base.Config.Bind<KeyboardShortcut>("2QuickChatShorcut", "QuickChatShortcut5", new KeyboardShortcut(KeyCode.PageDown, KeyCode.Keypad5));
			QK6 = base.Config.Bind<KeyboardShortcut>("2QuickChatShorcut", "QuickChatShortcut6", new KeyboardShortcut(KeyCode.PageDown, KeyCode.Keypad6));
			QK7 = base.Config.Bind<KeyboardShortcut>("2QuickChatShorcut", "QuickChatShortcut7", new KeyboardShortcut(KeyCode.PageDown, KeyCode.Keypad7));
			QK8 = base.Config.Bind<KeyboardShortcut>("2QuickChatShorcut", "QuickChatShortcut8", new KeyboardShortcut(KeyCode.PageDown, KeyCode.Keypad8));
			QK9 = base.Config.Bind<KeyboardShortcut>("2QuickChatShorcut", "QuickChatShortcut9", new KeyboardShortcut(KeyCode.PageDown, KeyCode.Keypad9));
			Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
			DBG.blogInfo("Quick Chat Loadded");

		}
		private void Update() {
            if(CheckPlayerNull())
            {
                return;
            }
            if(Chat.instance.m_input.gameObject.activeSelf||global::Console.instance.m_chatWindow.gameObject.activeSelf)
            {
                return;
            }
            if (LastChat)
            {
                Traverse.Create(Chat.instance).Method("InputText").GetValue();
                Chat.instance.m_input.text="";
                LastChat=false;
                return;
            }
            if (LastCMD)
            {
                Traverse.Create(global::Console.instance).Method("InputText").GetValue();
                global::Console.instance.m_input.text="";
                Traverse.Create(Chat.instance).Method("InputText").GetValue();
                Chat.instance.m_input.text="";
                LastCMD=false;
                return;
            }
            if (QK0.Value.IsDown()) { handleText(QC0.Value); }
            if (QK1.Value.IsDown()) { handleText(QC1.Value); }
            if (QK2.Value.IsDown()) { handleText(QC2.Value); }
            if (QK3.Value.IsDown()) { handleText(QC3.Value); }
            if (QK4.Value.IsDown()) { handleText(QC4.Value); }
            if (QK5.Value.IsDown()) { handleText(QC5.Value); }
            if (QK6.Value.IsDown()) { handleText(QC6.Value); }
            if (QK7.Value.IsDown()) { handleText(QC7.Value); }
            if (QK8.Value.IsDown()) { handleText(QC8.Value); }
            if (QK9.Value.IsDown()) { handleText(QC9.Value); }
        }
        #region patch
		#endregion

		#region Tool
		public static bool CheckPlayerNull(bool log = false)
		{
			if (Player.m_localPlayer == null)
			{
				if (log) { DBG.blogWarning("Player is Null"); }

				return true;
			}
			return false;
		}
		#endregion

		#region Feature
		private void handleText(string text)
		{
            
            Chat.instance.m_chatWindow.gameObject.SetActive(true);
            Traverse.Create(Chat.instance).Field<float>("m_hideTimer").Value=0f;
            if (text.StartsWith("cmd:"))
            {
                global::Console.instance.m_input.text=text.Substring(4);
                Traverse.Create(Chat.instance).Field<string>("m_lastEntry").Value=text.Substring(4);
                Chat.instance.m_input.text="/s use console:"+text.Substring(4);
                LastCMD=true;
                return;
            }
            Chat.instance.m_input.text=text;
            LastChat=true;
		}
		#endregion

		#region Utilities

		#endregion
	}
}