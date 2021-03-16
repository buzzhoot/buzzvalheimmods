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

namespace PluginName
{
    [BepInPlugin("buzz.valheim.Plugin Name", "Plugin Name", "0.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        //public static ConfigEntry<int> nexusID;
        #region Config
        //public static ConfigEntry<int> nexusID;
        //public static ConfigEntry<KeyboardShortcut> KS_BuildNoCost;
        #endregion
        public static ManualLogSource logger;
        public static string LastCmd;

        private void Awake()
        {
            Plugin.logger = base.Logger;
            //Plugin.nexusID = base.Config.Bind<int>("General", "NexusID", 354, "Nexus mod ID for updates");
            //KS_BuildNoCost = base.Config.Bind<KeyboardShortcut>("1Hotkeys", "Build with no cost Hotkey", new KeyboardShortcut(KeyCode.RightControl, KeyCode.KeypadDivide));
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            DBG.blogInfo("Plugin Name Loadded");

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

        #region Tool
        public static void CheckConsole()
        {

            if (global::Console.instance != null && global::Console.instance.m_chatWindow.gameObject.activeInHierarchy)
            {
                string cmd = LastCmd;
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (cmd.Length > 0)
                    {
                        ProcessCommands(cmd);
                        return;
                    }
                }
                LastCmd = global::Console.instance.m_input.text;
            }
        }
        public static bool CheckPlayerNull(bool log = false)
        {
            if (Player.m_localPlayer == null)
            {
                if (log) { DBG.blogWarning("Player is Null"); }

                return true;
            }
            return false;
        }
        public static void ProcessCommands(string inCommand)
        {
            if (inCommand.Length > 0)
            {
                if (inCommand.StartsWith(" "))
                {
                    inCommand = inCommand.Remove(0, 1);
                }
                if (inCommand.StartsWith("/?")) { return; }
            }
        }
        #endregion

        #region Feature

        #endregion

        #region Utilities

        #endregion
    }
}