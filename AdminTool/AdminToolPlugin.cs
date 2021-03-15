using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using BepInEx.Logging;
using UnityEngine;
using System.Globalization;
//=================TODO=============
//||X||Save Interval
//|| ||MouseTeleport
//||X||Free Camera
//|| ||QuickLoad
//============== EndTODO=============
namespace AdminTool
{
    [BepInPlugin("buzz.valheim.BuzzAdminTool", "BuzzAdminTool", "0.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        //public static ConfigEntry<int> nexusID;
        #region Config
        public static ConfigEntry<KeyboardShortcut> KS_BuildNoCost;
        public static ConfigEntry<KeyboardShortcut> KS_FlyingMode;
        public static ConfigEntry<KeyboardShortcut> KS_God;
        public static ConfigEntry<KeyboardShortcut> KS_KillAll;
        public static ConfigEntry<KeyboardShortcut> KS_LastCmd;
        public static ConfigEntry<string> QC0;
        public static ConfigEntry<string> QC1;
        public static ConfigEntry<string> QC2;
        public static ConfigEntry<string> QC3;
        public static ConfigEntry<string> QC4;
        public static ConfigEntry<string> QC5;
        public static ConfigEntry<string> QC6;
        public static ConfigEntry<string> QC7;
        public static ConfigEntry<string> QC8;

        public static ConfigEntry<KeyboardShortcut> QK0;
        public static ConfigEntry<KeyboardShortcut> QK1;
        public static ConfigEntry<KeyboardShortcut> QK2;
        public static ConfigEntry<KeyboardShortcut> QK3;
        public static ConfigEntry<KeyboardShortcut> QK4;
        public static ConfigEntry<KeyboardShortcut> QK5;
        public static ConfigEntry<KeyboardShortcut> QK6;
        public static ConfigEntry<KeyboardShortcut> QK7;
        public static ConfigEntry<KeyboardShortcut> QK8;


        public static ConfigEntry<bool> AutoAdmin;
        #endregion
    #region Internal var
                public static bool isBNC = false;
        public static bool isFM = false;
        public static bool isGod = false;
        public static bool isGhost = false;
        public static bool isInvis = false;
        public static bool isNoClip = false;
        public static bool isAdmin = false;
        public static bool isAutoAdmin;
    #endregion
    
        private static ManualLogSource logger;
        public string consoleLastMessage = string.Empty;
        private History consoleHistory = new History();
        public string LastCMD = string.Empty;

        private static string[] WeatherList = { "reset", "Clear", "Twilight_Clear", "Misty", "Darklands_dark", "Heath clear", "DeepForest Mist", "GDKing", "Rain", "LightRain", "ThunderStorm", "Eikthyr", "GoblinKing", "nofogts", "SwampRain", "Bonemass", "Snow", "Twilight_Snow", "Twilight_SnowStorm", "SnowStorm", "Moder", "Ashrain", "Crypt", "SunkenCrypt" };

        #region Mono
        private void Awake()
        {
            Plugin.logger = base.Logger;
            //Plugin.nexusID = base.Config.Bind<int>("General", "NexusID", 354, "Nexus mod ID for updates");
            KS_BuildNoCost = base.Config.Bind<KeyboardShortcut>("1Hotkeys", "Build with no cost Hotkey", new KeyboardShortcut(KeyCode.PageDown, KeyCode.KeypadDivide));
            KS_God = base.Config.Bind<KeyboardShortcut>("1Hotkeys", "Toogle God", new KeyboardShortcut(KeyCode.PageDown, KeyCode.KeypadMinus));
            KS_KillAll = base.Config.Bind<KeyboardShortcut>("1Hotkeys", "Kill All nearby Hotkey", new KeyboardShortcut(KeyCode.K));
            KS_FlyingMode = base.Config.Bind<KeyboardShortcut>("1Hotkeys", "Flying Mode Hotkey", new KeyboardShortcut(KeyCode.PageDown, KeyCode.KeypadMultiply));
            KS_LastCmd = base.Config.Bind<KeyboardShortcut>("1Hotkeys", "Excute last CMD", new KeyboardShortcut(KeyCode.PageDown, KeyCode.KeypadPlus));
            AutoAdmin = base.Config.Bind<bool>("2General", "Auto make you be admin", true);
            QC0 = base.Config.Bind<string>("3QC", "QC0", "");
            QC1 = base.Config.Bind<string>("3QC", "QC1", "/tod 0.5");
            QC2 = base.Config.Bind<string>("3QC", "QC2", "/env clear");
            QC3 = base.Config.Bind<string>("3QC", "QC3", "/spi CookedMeat 1");
            QC4 = base.Config.Bind<string>("3QC", "QC4", "/spi Troll 1");
            QC5 = base.Config.Bind<string>("3QC", "QC5", "");
            QC6 = base.Config.Bind<string>("3QC", "QC6", "");
            QC7 = base.Config.Bind<string>("3QC", "QC7", "");
            QC8 = base.Config.Bind<string>("3QC", "QC8", "");
            QK0 = base.Config.Bind<KeyboardShortcut>("3QC", "QK0", new KeyboardShortcut(KeyCode.PageDown, KeyCode.Keypad0));
            QK1 = base.Config.Bind<KeyboardShortcut>("3QC", "QK1", new KeyboardShortcut(KeyCode.PageDown,KeyCode.Keypad1));
            QK2 = base.Config.Bind<KeyboardShortcut>("3QC", "QK2", new KeyboardShortcut(KeyCode.PageDown, KeyCode.Keypad2));
            QK3 = base.Config.Bind<KeyboardShortcut>("3QC", "QK3", new KeyboardShortcut(KeyCode.PageDown, KeyCode.Keypad3));
            QK4 = base.Config.Bind<KeyboardShortcut>("3QC", "QK4", new KeyboardShortcut(KeyCode.PageDown, KeyCode.Keypad4));
            QK5 = base.Config.Bind<KeyboardShortcut>("3QC", "QK5", new KeyboardShortcut(KeyCode.PageDown, KeyCode.Keypad5));
            QK6 = base.Config.Bind<KeyboardShortcut>("3QC", "QK6", new KeyboardShortcut(KeyCode.PageDown, KeyCode.Keypad6));
            QK7 = base.Config.Bind<KeyboardShortcut>("3QC", "QK7", new KeyboardShortcut(KeyCode.PageDown, KeyCode.Keypad7));
            QK8 = base.Config.Bind<KeyboardShortcut>("3QC", "QK8", new KeyboardShortcut(KeyCode.PageDown, KeyCode.Keypad8));


            isAutoAdmin = AutoAdmin.Value;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
            Plugin.logger.LogInfo("Admin Loadded");

        }
        private void Update()
        {
            CheckConsole();
            if (CheckPlayerNull()) { return; }
            if (isAutoAdmin)
            {
                SetAdmin(true);
                if (isBNC == true) { isAutoAdmin = false; }
                return;
            }
            if (QK0.Value.IsDown()) { ProcessCommands(QC0.Value); }
            if (QK1.Value.IsDown()) { ProcessCommands(QC1.Value); }
            if (QK2.Value.IsDown()) { ProcessCommands(QC2.Value); }
            if (QK3.Value.IsDown()) { ProcessCommands(QC3.Value); }
            if (QK4.Value.IsDown()) { ProcessCommands(QC4.Value); }
            if (QK5.Value.IsDown()) { ProcessCommands(QC5.Value); }
            if (QK6.Value.IsDown()) { ProcessCommands(QC6.Value); }
            if (QK7.Value.IsDown()) { ProcessCommands(QC7.Value); }
            if (QK8.Value.IsDown()) { ProcessCommands(QC8.Value); }

            if (KS_FlyingMode.Value.IsDown()) { SetFly(); }
            if (KS_BuildNoCost.Value.IsDown()) { SetBNC(); }
            if (KS_God.Value.IsDown()) { SetGod(); }
            if (KS_KillAll.Value.IsDown()&&!global::Console.instance.m_chatWindow.gameObject.activeInHierarchy&&!Chat.instance.m_chatWindow.gameObject.activeInHierarchy) { KillNearBy(); }

            if (KS_LastCmd.Value.IsDown())
            {
                if (LastCMD != string.Empty)
                {
                    ProcessCommands(LastCMD);
                }
            }
        }
        #endregion

        #region patch
        [HarmonyPatch(typeof(Player), "UseStamina")]
        internal class AddStamina_Patch
        {
            private static void Prefix(ref float v)
            {
                if (isGod)
                {
                    v = 0f;
                }
            }
        }
        [HarmonyPatch(typeof(Chat), "SendPing")]
        private static class Chat_SendPing_Patch
        {
            private static bool Prefix(Chat __instance, ref Vector3 position)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    Player localPlayer = Player.m_localPlayer;
                    localPlayer.TeleportTo(position, localPlayer.transform.rotation, true);
                    return false;
                }
                return true;
            }
        }
        #endregion

        #region Tool
        public void CheckConsole()
        {

            //if (global::Console.instance != null && global::Console.instance.m_chatWindow.gameObject.activeInHierarchy)
            //{
            //    string cmd = LastCmd;
            //    if (Input.GetKeyDown(KeyCode.Return))
            //    {
            //        if (cmd.Length > 0)
            //        {
            //            ProcessCommands(cmd);
            //            return;
            //        }
            //    }
            //    LastCmd = global::Console.instance.m_input.text;
            //}
            ///
            if (global::Console.instance != null)
            {
                if (global::Console.instance.m_chatWindow.gameObject.activeInHierarchy)
                {
                    string text = global::Console.instance.m_input.text;
                    if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape))
                    {
                        consoleLastMessage = string.Empty;
                    }
                    if (!text.Equals(string.Empty) && !text.Equals(consoleLastMessage))
                    {
                        consoleLastMessage = text;
                    }
                    if (Input.GetKeyDown(KeyCode.Return) && text.Equals(string.Empty) && !consoleLastMessage.Equals(string.Empty))
                    {
                        consoleHistory.Add(consoleLastMessage);
                        ProcessCommands(consoleLastMessage);
                        LastCMD = consoleLastMessage;
                        consoleLastMessage = string.Empty;
                    }
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        global::Console.instance.m_input.text = consoleHistory.Fetch(text, true);
                        global::Console.instance.m_input.caretPosition = global::Console.instance.m_input.text.Length;
                    }
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        global::Console.instance.m_input.text = consoleHistory.Fetch(text, false);
                    }
                }
                if (Input.GetKeyDown(KeyCode.Slash) && !global::Console.IsVisible() && !Chat.instance.IsChatDialogWindowVisible() && !TextInput.IsVisible())
                {
                    global::Console.instance.m_chatWindow.gameObject.SetActive(true);
                    global::Console.instance.m_input.caretPosition = global::Console.instance.m_input.text.Length;
                }
            }
        }
        public static bool CheckPlayerNull(bool log = false)
        {
            if (Player.m_localPlayer == null)
            {
                if (log) { blogWarning("Player is Null"); }

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
                #region env
                if (inCommand.StartsWith("/env"))
                {
                    inCommand = inCommand.Remove(0, 5);
                    //float num1;
                    //if (!float.TryParse(inCommand, NumberStyles.Float, CultureInfo.InvariantCulture, out num1))
                    //{
                    //cprt("Wrong Number");
                    //return;
                    //}
                    if (inCommand == "list")
                    {
                        string wl = "pick from these ones,list: ";
                        foreach (var w in WeatherList)
                        {
                            wl += w;
                            wl += ",";
                        }
                        cprt(wl);
                        return;
                    }
                    if (EnvMan.instance)
                    {
                        if (WeatherList.Contains(inCommand))
                        {

                            if (inCommand == "reset")
                            {
                                EnvMan.instance.m_debugEnv = "";
                                cprt("reset enviroment");
                                return;
                            }
                            EnvMan.instance.m_debugEnv = inCommand;
                            cprt("set enviroment to " + inCommand);
                            return;
                        }
                        else
                        {
                            cprt("Wrong Word,Try /env list");
                        }
                    }
                    blogWarning("EnvMan is null");
                    return;
                }
                #endregion
                #region admin
                if (inCommand == "/amd")
                {
                    if (CheckPlayerNull())
                    {
                        return;
                    }
                    //isAdmin = true;
                    SetAdmin(true);
                }
                if (inCommand == "/oadm")
                {
                    if (CheckPlayerNull())
                    {
                        return;
                    }
                    //isAdmin = false;
                    SetAdmin(false);
                }
                if (inCommand == "/gst")
                {
                    if (CheckPlayerNull())
                    {
                        return;
                    }
                    SetGhost();
                }
                if (inCommand == "/rds")
                {
                    ItemDrop[] array2 = UnityEngine.Object.FindObjectsOfType<ItemDrop>();
                    int num = 0;
                    for (int i = 0; i < array2.Length; i++)
                    {
                        ZNetView component = array2[i].GetComponent<ZNetView>();
                        if (component && component.IsValid() && component.IsOwner())
                        {
                            component.Destroy();
                            num++;
                        }
                    }
                    InfoTL("Destoyed " + num.ToString() + " items");
                }
                if (inCommand == "/cam")
                {
                    GameCamera.instance.ToggleFreeFly();
                }
                #region noclip
                if (inCommand == "/noclip")
                {
                    if (CheckPlayerNull())
                    {
                        return;
                    }
                    SetClip();
                }
                #endregion

                #endregion
                #region Terrain
                if (inCommand.StartsWith("/tf"))
                {
                    inCommand = inCommand.Remove(0, 4);
                    string[] arg = inCommand.Split(new char[] { ',' });
                    if (arg.Length > 2 || arg.Length < 2 || !inCommand.Contains(","))
                    {
                        cprt("/tf syntax wrong");
                        return;
                    }
                    try
                    {
                        float x = float.Parse(arg[0]);
                        float y = float.Parse(arg[1]);
                        Terrain.Flatten(x, y);
                    }
                    catch (Exception e)
                    {
                        blogWarning("/tf failed :" + e);
                    }
                }
                if (inCommand.StartsWith("/trf"))
                {
                    inCommand = inCommand.Remove(0, 5);
                    cprt(inCommand);
                    try
                    {
                        Terrain.RemoveFlora(float.Parse(inCommand));
                    }
                    catch (Exception e)
                    {
                        blogWarning("/trf failed:" + e);
                    }

                }
                if (inCommand.StartsWith("/trst"))
                {
                    inCommand = inCommand.Remove(0, 6);
                    cprt(inCommand);
                    try
                    {
                        Terrain.Reset(Player.m_localPlayer.transform.position, float.Parse(inCommand));
                    }
                    catch (Exception e)
                    {
                        blogWarning("/trst failed:" + e);
                    }

                }
                #endregion
                #region tod
                if (inCommand.StartsWith("/tod"))
                {
                    float a = 0;
                    inCommand = inCommand.Remove(0, 5);
                    try { a = float.Parse(inCommand); }
                    catch (Exception)
                    {

                        cprt("Wrong Number");
                    }
                    if (a <= 0 || a >= 1)
                    {
                        EnvMan.instance.m_debugTimeOfDay = false;
                        return;
                    }
                    EnvMan.instance.m_debugTimeOfDay = true;
                    EnvMan.instance.m_debugTime = a;
                }
                #endregion
                #region tame
                if (inCommand == "/tame")
                {
                    Tameable.TameAllInArea(Player.m_localPlayer.transform.position, 30f);
                }
                #endregion
                #region list
                if (inCommand.StartsWith("/listi"))
                {
                    inCommand = inCommand.Remove(0, 7);
                    if (inCommand.Length > 0)
                    {
                        string list = "Result: ";
                        if (inCommand.StartsWith("?"))
                        {
                            foreach (GameObject g in ObjectDB.instance.m_items)
                            {
                                ItemDrop component = g.GetComponent<ItemDrop>();
                                list += component.name;
                                list += ",";
                                list = LimitLog(list);
                            }
                            list = LimitLog(list);
                            if (list != "")
                            {
                                Traverse.Create(MessageHud.instance).Method("AddLog", new object[] { list }).GetValue();
                                cprt(list);
                                cprt("Check your Message Log");
                                list = "";
                                return;
                            }
                            return;
                        }
                        using (List<GameObject>.Enumerator enumerator3 = ObjectDB.instance.m_items.GetEnumerator())
                        {
                            while (enumerator3.MoveNext())
                            {
                                GameObject g2 = enumerator3.Current;
                                ItemDrop c2 = g2.GetComponent<ItemDrop>();
                                if (c2.name.ToLower().Contains(inCommand.ToLower()))
                                {
                                    list += c2.name;
                                    list += ",";
                                    list = LimitLog(list);
                                }
                            }
                            list = LimitLog(list);
                            if (list != "")
                            {
                                Traverse.Create(MessageHud.instance).Method("AddLog", new object[] { list }).GetValue();
                                cprt(list);
                                cprt("Check your Message Log");
                                list = "";
                                return;
                            }
                            return;
                        }
                    }
                    return;
                }
                #endregion
                #region trader
                if (inCommand == "/trader")
                {
                    Game.instance.DiscoverClosestLocation("Vendor_BlackForest", Player.m_localPlayer.transform.position, "Merchant", 8);
                    Minimap.PinData pinData = Enumerable.First<Minimap.PinData>((List<Minimap.PinData>)Traverse.Create(Minimap.instance).Field("m_pins").GetValue(), (Minimap.PinData p) => p.m_type == Minimap.PinType.None && p.m_name == "");
                    return;
                }
                #endregion
                #region spi
                if (inCommand.StartsWith("/spi"))
                {
                    inCommand = inCommand.Remove(0, 5);
                    string[] c = inCommand.Split(new char[] {' '});
                    if (c.Length==1)
                    {
                        FT.SpawnPrefab(c[0], Player.m_localPlayer);
                        return;
                    }
                    if (c.Length == 2)
                    {
                        int amt = 1;
                        if(!int.TryParse(c[1], out amt)){
                            cprt("Wrong Syntax");
                            return; }
                        FT.SpawnPrefab(c[0], Player.m_localPlayer,amt);
                    }
                    else
                    {
                        cprt("Wrong Syntax");
                        return;
                    }
                }
                #endregion
                //end
            }
        }
        #endregion

        #region Feature
        private static void SetFly(bool info = true)
        {
            isFM = !isFM;
            Traverse.Create(Player.m_localPlayer).Field("m_debugFly").SetValue(isFM);
            Player.m_localPlayer.gameObject.GetComponent<ZNetView>().GetZDO().Set("DebugFly", isFM);
            if (!info)
            {
                return;
            }
            InfoCT(isFM ? "I am Bird!" : "StopFlying");
        }
        private static void SetBNC(bool info = true)
        {
            isBNC = !isBNC;
            Traverse.Create(Player.m_localPlayer).Field("m_noPlacementCost").SetValue(isBNC);
            Traverse.Create(Player.m_localPlayer).Method("UpdateAvailablePiecesList");
            if (!info)
            {
                return;
            }
            InfoCT(isBNC ? "I can build anything!!!" : "Need more material");
        }
        private static void KillNearBy(bool info = true)
        {
            if (!isGod) { return; }
            int num = 0;
            foreach (Character character in Character.GetAllCharacters())
            {
                if (!character.IsPlayer())
                {
                    HitData hitData = new HitData();
                    hitData.m_damage.m_damage = 99999f;
                    character.Damage(hitData);
                    num++;
                }
            }
            if (!info) { return; }
            InfoTL("killed" + num.ToString() + "Creatures");
        }
        private static void SetGod(bool info = true)
        {
            isGod = !isGod;
            Player.m_localPlayer.SetGodMode(isGod);
            if (!info) { return; }
            InfoCT(isGod ? "I AM GOD" : "i am a normal person");
        }
        private static void SetGhost(bool info = true)
        {
            isGhost = !isGhost;
            Player.m_localPlayer.SetGhostMode(isGhost);
            if (!info) { return; }
            InfoCT(isGhost ? "Ghost~~~" : "Human~~~~");
        }
        private static void SetClip(bool info = true)
        {
            isNoClip = !isNoClip;
            Collider[] componentsInChildren = Player.m_localPlayer.GetComponentsInChildren<Collider>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].enabled = !isNoClip;
            }
            InfoCT(isGhost ? "Nothing Stop me" : "Oh,Wall,Hurt!");
        }
        public static void SetAdmin(bool set, bool info = true)
        {
            isAdmin = set;
            isGod = !set;
            isGhost = !set;
            isBNC = !set;
            SetGhost(false);
            SetBNC(false);
            SetGod(false);
            if (info) { InfoCT(Player.m_localPlayer.GetPlayerName() + (isAdmin ? " became a admin" : " became a normal player")); }
        }
        #endregion

        #region Debug
        public static void cprt(string s)
        {
            global::Console.instance.Print(s);
        }
        public static void InfoTL(string s)
        {
            Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, s, 0, null);
        }
        public static void InfoCT(string s)
        {
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, s, 0, null);
        }
        public static void blogInfo(object o)
        {
            logger.LogInfo(o);
        }
        public static void blogWarning(object o)
        {
            logger.LogWarning(o);
        }
        #endregion Debug

        #region Utilities
        public static string LimitString(string str, int max = 60)
        {
            if (str.Length >= max)
            {
                cprt(str);
                str = "";
                return str;
            }
            return str;
        }
        public static string LimitLog(string str,int max = 49)
        {
            if (str.Length >= max)
            {
                var l = str.Remove(max);
                Traverse.Create(MessageHud.instance).Method("AddLog", new object[] { l }).GetValue();
                cprt(str);
                str = str.Remove(0,max);
                //blogWarning(str);
                return str;
            }
            return str;
        }
        private class History
        {
            public void Add(string item)
            {
                this.history.Add(item);
                this.index = 0;
            }
            public string Fetch(string current, bool next)
            {
                if (this.index == 0)
                {
                    this.current = current;
                }
                if (this.history.Count == 0)
                {
                    return current;
                }
                this.index += ((!next) ? 1 : -1);
                if (this.history.Count + this.index < 0 || this.history.Count + this.index > this.history.Count - 1)
                {
                    this.index = 0;
                    return this.current;
                }
                return this.history[this.history.Count + this.index];
            }
            private List<string> history = new List<string>();
            private int index;
            private string current;
        }
        #endregion
    }
    #region terrian
    public static class Terrain
    {
        public static void Reset(Vector3 centerLocation, float radius)
        {
            radius = Mathf.Clamp(radius, 2f, 50f);
            try
            {
                foreach (TerrainModifier terrainModifier in TerrainModifier.GetAllInstances())
                {
                    if (terrainModifier != null && Utils.DistanceXZ(Player.m_localPlayer.transform.position, terrainModifier.transform.position) < radius)
                    {
                        ZNetView component = terrainModifier.GetComponent<ZNetView>();
                        if (component != null && component.IsValid())
                        {
                            component.ClaimOwnership();
                            component.Destroy();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        public static void Flatten(float radiusX, float radiusY)
        {
            if (radiusY <= 0f)
            {
                radiusY = radiusX;
            }
            else if (radiusX <= 0f)
            {
                radiusX = radiusY;
            }
            if (radiusX > 80f)
            {
                radiusX = 80f;
                Plugin.blogWarning("X Length Capped At 80");
            }
            if (radiusY > 80f)
            {
                radiusY = 80f;
                Plugin.blogWarning("Y Length Capped At 80");
            }
            if (radiusX <= 0f)
            {
                radiusX = 1f;
                radiusY = 1f;
                Plugin.blogWarning("Must Be Greater Than Zero");
            }
            Transform transform = Player.m_localPlayer.transform;
            Vector3 position = transform.position;
            //Log.Message("Attempting To Flatten...");
            GameObject prefab = ZNetScene.instance.GetPrefab("raise");
            if (prefab.GetComponent<Piece>() == null || prefab == null)
            {
                Plugin.blogWarning("Flatten Creation Corrupted : Cancelling!");
                return;
            }
            TerrainModifier.SetTriggerOnPlaced(true);
            int num = 0;
            while ((float)num < radiusX)
            {
                int num2 = 0;
                while ((float)num2 < radiusY)
                {
                    Terrain.SpawnFloor(position + Vector3.down * 0.5f + transform.forward * (float)num + transform.right * (float)num2, transform.rotation, prefab);
                    num2++;
                }
                num++;
            }
            TerrainModifier.SetTriggerOnPlaced(false);
            //Plugin.blogWarning("Flatten Radius Required");
        }
        public static void RemoveFlora(float radius)
        {
            Collider[] array = Physics.OverlapBox(Player.m_localPlayer.transform.position, new Vector3(radius, radius, radius));
            List<string> list = new List<string>();
            list.Add("tree");
            list.Add("rock");
            list.Add("beech");
            list.Add("log");
            list.Add("bush");
            if (array.Length == 0) { return; }
            Collider[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                GameObject gameObject = array2[i].gameObject;
                GameObject gameObject2 = gameObject.transform.parent.gameObject;
                GameObject gameObject3 = null;
                string text = gameObject2.name.ToLower();
                string text2 = gameObject.name.ToLower();
                foreach (string value in list)
                {
                    if (text2.Contains(value))
                    {
                        gameObject3 = gameObject;
                        break;
                    }
                    if (!(gameObject2 == null) && text.Contains(value))
                    {
                        gameObject3 = gameObject2;
                        break;
                    }
                }
                if (!(gameObject3 == null))
                {
                    try
                    {
                        ZNetScene.instance.Destroy(gameObject3);
                    }
                    catch (Exception e)
                    {
                        Plugin.blogWarning("RemoveFlora FAILED:" + e);
                    }
                }
            }
        }
        public static void SpawnFloor(Vector3 position, Quaternion rotation, GameObject piece)
        {
            try
            {
                UnityEngine.Object.Instantiate<GameObject>(piece, position, rotation);
            }
            catch (Exception e)
            {
                Plugin.blogWarning("SpawnFloor Failed:" + e);
            }
        }
    }
    #endregion
    public static class Util
    {
        public static void SetPrivateField(this object obj, string fieldName, object value)
        {
            obj.GetType().GetField(fieldName).SetValue(obj, value);
        }
        public static void InvokePrivateMethod(this object obj, string methodName, object[] methodParams)
        {
            obj.GetType().GetMethod(methodName).Invoke(obj, methodParams);
        }
        public static T GetPrivateField<T>(this object obj, string fieldName)
        {
            return (T)((object)obj.GetType().GetField(fieldName).GetValue(obj));
        }
    }
}