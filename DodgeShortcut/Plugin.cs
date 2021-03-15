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
namespace DodgeShortcut
{
    [BepInPlugin("buzz.valheim.DodgeShortcut", "DodgeShortcut", "1.1.1")]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<int> nexusID;
        public static ConfigEntry<KeyboardShortcut> KS_Dodge_Forward;
        public static ConfigEntry<KeyboardShortcut> KS_Dodge_Backward;
        public static ConfigEntry<KeyboardShortcut> KS_Dodge_Left;
        public static ConfigEntry<KeyboardShortcut> KS_Dodge_Right;
        public static ConfigEntry<KeyboardShortcut> KS_Dodge;
        #region Config
        //public static ConfigEntry<int> nexusID;
        //public static ConfigEntry<KeyboardShortcut> KS_BuildNoCost;
        #endregion
        public static ManualLogSource logger;
        public static string LastCmd;

        private void Awake()
        {
            Plugin.logger = base.Logger;
            Plugin.nexusID = base.Config.Bind<int>("General", "NexusID", 424, "Nexus mod ID for updates");
            KS_Dodge_Forward = base.Config.Bind<KeyboardShortcut>("1Hotkeys", "1Dodge Forward 向后翻滚", new KeyboardShortcut(KeyCode.W, KeyCode.LeftAlt));
            KS_Dodge_Backward = base.Config.Bind<KeyboardShortcut>("1Hotkeys", "2Dodge Backward 向前翻滚", new KeyboardShortcut(KeyCode.S, KeyCode.LeftAlt));
            KS_Dodge_Left = base.Config.Bind<KeyboardShortcut>("1Hotkeys", "3Dodge Left 向左翻滚", new KeyboardShortcut(KeyCode.A, KeyCode.LeftAlt));
            KS_Dodge_Right= base.Config.Bind<KeyboardShortcut>("1Hotkeys", "4Dodge Right 向右翻滚", new KeyboardShortcut(KeyCode.D, KeyCode.LeftAlt));
            KS_Dodge = base.Config.Bind<KeyboardShortcut>("1Hotkeys", "5Dodge where you run to向你的奔跑方向翻滚", new KeyboardShortcut(KeyCode.LeftAlt));
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            DBG.blogInfo("DodgeShortcut Loadded");

        }
        #region patch
        //[HarmonyPatch(typeof(GAME), "Shutdown")]
        //private static class Patch
        //{
        //    private static void PreFix()
        //    {
        //    }
        //}
        #endregion
        [HarmonyPatch(typeof(Player), "Update")]
        private static class DodgePatch
        {
            private static void Prefix()
            {
                if (CheckPlayerNull()||Player.m_localPlayer.IsTeleporting()|| global::Console.instance.m_chatWindow.gameObject.activeInHierarchy||Chat.instance.m_chatWindow.gameObject.activeInHierarchy||Player.m_localPlayer.InPlaceMode()||TextInput.IsVisible() ||StoreGui.IsVisible() ||InventoryGui.IsVisible()||Menu.IsVisible())
                 { return; }
                Vector3 dir = Traverse.Create(Player.m_localPlayer).Field<Vector3>("m_lookDir").Value;
                Vector3 rdir = Traverse.Create(Player.m_localPlayer).Field<Vector3>("m_moveDir").Value;
                dir.y = 0;
                rdir.y = 0 ;
                rdir=rdir.normalized;
                dir = dir.normalized;
                if (Input.GetKeyDown(KS_Dodge.Value.MainKey))
                {
                    Dodge(rdir);
                    return;
                }
                if (KS_Dodge_Forward.Value.IsDown())
                {
                    Dodge(dir);
                }
                if (KS_Dodge_Backward.Value.IsDown())
                {
                    Dodge(dir * -1);
                }
                if (KS_Dodge_Right.Value.IsDown())
                {
                    Dodge(Quaternion.Euler(0, 90, 0) * dir);
                }
                if (KS_Dodge_Left.Value.IsDown())
                {
                    Dodge(Quaternion.Euler(0,90, 0) * dir*-1);
                }
            }

        }
        
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
        private static void Dodge(Vector3 dir)
        {
            Traverse.Create(Player.m_localPlayer).Method("Dodge", new object[] { dir }).GetValue();
        }
        #endregion

        #region Feature

        #endregion

        #region Utilities

        #endregion
    }
}