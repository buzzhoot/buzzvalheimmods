using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace OdinPlus
{
  [BepInPlugin("digitalroot.valheim.mods.odinplusremake", "OdinPlusRemake", "0.3.0")]
  [BepInIncompatibility("buzz.valheim.OdinPlus")]
  public class Plugin : BaseUnityPlugin
  {
    #region Config Var

    public static ConfigEntry<int> nexusID;
    public static ManualLogSource logger;
    public static ConfigEntry<KeyboardShortcut> KS_SecondInteractkey;
    public static ConfigEntry<string> CFG_ItemSellValue;
    public static ConfigEntry<Vector3> CFG_OdinPosition;
    public static ConfigEntry<bool> CFG_ForceOdinPosition;
    public static bool Set_FOP = false;

    #region InternalConfig

    public static int RaiseCost = 10;
    public static int RaiseFactor = 100;

    #endregion InternalConfig

    Harmony _harmony;

    #endregion

    public static GameObject OdinPlusRoot;

    #region Actions

    public static Action posZone;
    public static Action RegRPC;
    public static Action<ObjectDB> preODB;

    #endregion Actions

    #region Mono

    private void Awake()
    {
      logger = Logger;
      CFG_ItemSellValue = Config.Bind("Config", "ItemSellValue", "TrophyBlob:20;TrophyBoar:5;TrophyBonemass:50;TrophyDeathsquito:20;TrophyDeer:5;TrophyDragonQueen:50;TrophyDraugr:20;TrophyDraugrElite:30;TrophyDraugrFem:20;TrophyEikthyr:50;TrophyFenring:30;TrophyForestTroll:30;TrophyFrostTroll:20;TrophyGoblin:20;TrophyGoblinBrute:30;TrophyGoblinKing:50;TrophyGoblinShaman:20;TrophyGreydwarf:5;TrophyGreydwarfBrute:15;TrophyGreydwarfShaman:15;TrophyHatchling:20;TrophyLeech:15;TrophyLox:20;TrophyNeck:5;TrophySerpent:30;TrophySGolem:30;TrophySkeleton:10;TrophySkeletonPoison:30;TrophySurtling:20;TrophyTheElder:50;TrophyWolf:20;TrophyWraith:30;AncientSeed:5;BoneFragments:1;Chitin:5;WitheredBone:10;DragonEgg:40;GoblinTotem:20;OdinLegacy:20");
      nexusID = Config.Bind("General", "NexusID", 798, "Nexus mod ID for updates");
      KS_SecondInteractkey = Config.Bind("1Hotkeys", "Second Interact key", new KeyboardShortcut(KeyCode.G));
      CFG_OdinPosition = Config.Bind("2Server set only", "Odin position", Vector3.zero);
      CFG_ForceOdinPosition = Config.Bind("2Server set only", "Force Odin Position", false);

      RegRPC = ReigsterRpc;

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

      //-- init here
      OdinPlusRoot = new GameObject("OdinPlus");
      OdinPlusRoot.AddComponent<ResourceAssetManager>();
      OdinPlusRoot.AddComponent<OdinPlus>();

      //notice Debug
      OdinPlusRoot.AddComponent<DevTool>();

      DontDestroyOnLoad(OdinPlusRoot);
      DBG.blogInfo("OdinPlus Loaded");
    }

    public static void ReigsterRpc()
    {
      DBG.blogWarning("Starting reg rpc");
    }

    private void OnDestroy()
    {
      if (_harmony != null) _harmony.UnpatchSelf();
    }

    #endregion Mono

    #region patch

    #region StoreGui

    [HarmonyPatch(typeof(StoreGui), "Show")]
    private static class Prefix_StoreGui_Show
    {
      private static void Postfix(StoreGui __instance, Trader trader)
      {
        if (OdinPlus.traderNameList.Contains(trader.m_name))
        {
          OdinTrader.TweakGui(__instance, true);
        }
      }
    }

    [HarmonyPatch(typeof(StoreGui), "Hide")]
    private static class Prefix_StoreGui_Hide
    {
      private static void Prefix(StoreGui __instance)
      {
        var trader = Traverse.Create(__instance).Field<Trader>("m_trader").Value;
        if (OdinPlus.traderNameList.Contains(trader.m_name))
        {
          OdinTrader.TweakGui(__instance, false);
        }
      }
    }

    [HarmonyPatch(typeof(StoreGui), "GetPlayerCoins")]
    private static class Postfix_StoreGui_GetPlayerCoins
    {
      private static void Postfix(StoreGui __instance, ref int __result)
      {
        var t = Traverse.Create(__instance).Field<Trader>("m_trader").Value;
        if (t == null)
        {
          return;
        }

        string name = t.m_name;
        if (OdinPlus.traderNameList.Contains(name))
        {
          __result = OdinData.Credits;
        }
      }
    }

    [HarmonyPatch(typeof(StoreGui), "BuySelectedItem")]
    private static class Prefix_StoreGui_BuySelectedItem
    {
      private static bool Prefix(StoreGui __instance)
      {
        string name = Traverse.Create(__instance).Field<Trader>("m_trader").Value.m_name;
        if (OdinPlus.traderNameList.Contains(name))
        {
          var m_selectedItem = Traverse.Create(__instance).Field<Trader.TradeItem>("m_selectedItem").Value;
          int stack = Mathf.Min(m_selectedItem.m_stack, m_selectedItem.m_prefab.m_itemData.m_shared.m_maxStackSize);
          if (m_selectedItem == null || (m_selectedItem.m_price * stack - OdinData.Credits > 0))
          {
            return false;
          }

          int quality = m_selectedItem.m_prefab.m_itemData.m_quality;
          int variant = m_selectedItem.m_prefab.m_itemData.m_variant;
          if (Player.m_localPlayer.GetInventory().AddItem(m_selectedItem.m_prefab.name, stack, quality, variant, 0L, "") != null)
          {
            OdinData.RemoveCredits(m_selectedItem.m_price * stack); //?
            __instance.m_buyEffects.Create(__instance.gameObject.transform.position, Quaternion.identity);
            Player.m_localPlayer.ShowPickupMessage(m_selectedItem.m_prefab.m_itemData, m_selectedItem.m_prefab.m_itemData.m_stack);
            Traverse.Create(__instance).Method("FillList").GetValue();
            Gogan.LogEvent("Game", "BoughtItem", m_selectedItem.m_prefab.name, 0L);
          }

          return false;
        }

        return true;
      }
    }

    #endregion

    #region Player and Console and Fejd

    [HarmonyPatch(typeof(Player), "Update")]
    private static class Patch_Player_Update
    {
      private static void Postfix(Player __instance)
      {
        if (CheckPlayerNull())
        {
          return;
        }

        if (KS_SecondInteractkey.Value.IsDown() && __instance.GetHoverObject() != null)
        {
          if (__instance.GetHoverObject().GetComponent<OdinInteractable>() != null)
          {
            __instance.GetHoverObject().GetComponent<OdinInteractable>().SecondaryInteract(__instance);
            return;
          }

          if (__instance.GetHoverObject().GetComponentInParent<OdinInteractable>() != null)
          {
            __instance.GetHoverObject().GetComponentInParent<OdinInteractable>().SecondaryInteract(__instance);
          }
        }
      }
    }

    [HarmonyPatch(typeof(FejdStartup), "Start")]
    private static class FejdStartup_Start_Patch
    {
      private static void Postfix()
      {
        if (OdinPlus.isInit)
        {
          return;
        }

        OdinPlus.Init();
      }
    }

    [HarmonyPatch(typeof(Console), "InputText")]
    private static class Patch_Console_InputText
    {
      private static void Prefix()
      {
        if (DevTool.IsIns())
        {
          DevTool.InputCMD(Console.instance.m_input.text);
        }
      }
    }

    [HarmonyPatch(typeof(Chat), "InputText")]
    private static class Patch_Chat_InputText
    {
      private static void Prefix(Chat __instance)
      {
        if (Player.m_localPlayer != null && OdinPlus.isNPCInit)
        {
          string cmd = __instance.m_input.text;
          if (cmd.ToLower() == "/odinhere")
          {
            if (Set_FOP)
            {
              LocationManager.GetStartPos();
              return;
            }

            NpcManager.Root.transform.localPosition = Player.m_localPlayer.transform.localPosition + Vector3.forward * 4;
          }

          if (cmd.ToLower() == "/whereami")
          {
            var pos = Player.m_localPlayer.transform.position;
            string s = pos.x + "," + pos.y + "," + pos.z;
            DBG.InfoCT(s);
            DBG.cprt(s);
            //global::Console.instance.m_input.text=s;
            __instance.m_input.text = s;
            return;
          }

          if (cmd.ToLower() == "/whereodin")
          {
            var pos = NpcManager.Root.transform.localPosition;
            string s = pos.x + "," + pos.y + "," + pos.z;
            DBG.InfoCT(s);
            DBG.cprt(s);
            __instance.m_input.text = s;
            return;
          }

          if (cmd.ToLower() == "/setodin")
          {
            CFG_OdinPosition.Value = NpcManager.Root.transform.localPosition;
            return;
          }

          if (cmd.ToLower() == "/findfarm")
          {
            Game.instance.DiscoverClosestLocation("WoodFarm1", Player.m_localPlayer.transform.position, "Village", 0);
          }
        }
      }
    }

    #endregion

    #region Misc

    [HarmonyPatch(typeof(Localization), "SetupLanguage")]
    public static class MyLocalizationPatch
    {
      public static void Postfix(Localization __instance, string language)
      {
        //Debug.LogWarning(language);
        BuzzLocal.init(language, __instance);
        BuzzLocal.UpdateDictinary();
      }
    }

    [HarmonyPatch(typeof(PlayerProfile), "SavePlayerToDisk")]
    public static class PlayerProfile_SavePlayerData_Patch
    {
      public static void Prefix(PlayerProfile __instance)
      {
        if (CheckPlayerNull())
        {
          return;
        }

        OdinData.saveOdinData(Player.m_localPlayer.GetPlayerName() + "_" + ZNet.instance.GetWorldName());
      }
    }

    [HarmonyPatch(typeof(PlayerProfile), "LoadPlayerData")]
    private static class Patch_PlayerProfile_LoadPlayerData
    {
      private static void Postfix()
      {
        if (ZNet.instance == null)
        {
          return;
        }

        {
          if (CheckPlayerNull() || OdinPlus.m_instance.isLoaded)
          {
            return;
          }

          OdinData.loadOdinData(Player.m_localPlayer.GetPlayerName() + "_" + ZNet.instance.GetWorldName());
        }
      }
    }

    [HarmonyPatch(typeof(Tameable), "GetHoverText")]
    private static class Postfix_Tameable_GetHoverText
    {
      private static void Postfix(Tameable __instance, ref string __result)
      {
        if (__instance.gameObject.GetComponent<Character>().m_name == "$op_wolf_name")
        {
          __result += Localization.instance.Localize(String.Format("\n<color=yellow><b>[{0}]</b></color>$op_wolf_use", KS_SecondInteractkey.Value.MainKey.ToString()));
        }
      }
    }

    #endregion

    #region ZnetScene

    [HarmonyPatch(typeof(ZNetScene), "Awake")]
    private static class ZNetScene_Awake_Prefix
    {
      private static void Prefix(ZNetScene __instance)
      {
        OdinPlus.PreZNS(__instance);
      }
    }

    [HarmonyPriority(600)]
    [HarmonyBefore("buzz.valheim.AllTameable", "org.bepinex.plugins.creaturelevelcontrol")]
    [HarmonyPatch(typeof(ZNetScene), "Awake")]
    private static class ZNetScene_Awake_Patch
    {
      private static void Postfix(ZNetScene __instance)
      {
        //Pet.init(__instance);
        OdinPlus.PostZNS();
      }
    }

    [HarmonyPatch(typeof(ZNetScene), "Shutdown")]
    private static class ZNetScene_Shutdown_Patch
    {
      private static void Postfix()
      {
        if (ZNet.instance.IsDedicated() && ZNet.instance.IsServer())
        {
          OdinData.saveOdinData(ZNet.instance.GetWorldName());
        }

        OdinPlus.UnRegister();
        OdinPlus.Clear();
      }
    }

    #endregion

    #region ZoneSystem

    [HarmonyPatch(typeof(ZoneSystem), "Start")]
    private static class Postfix_ZoneSystem_Start
    {
      private static void Postfix()
      {
        if (posZone != null)
        {
          posZone();
        }

        OdinPlus.PostZone();
      }
    }

    [HarmonyPatch(typeof(ZoneSystem), "Start")]
    private static class Prefix_ZoneSystem_Start
    {
      private static void Prefix()
      {
        //LocationMarker.HackLoctaions();
      }
    }

    [HarmonyPatch(typeof(DungeonGenerator), "Awake")]
    private static class Postfix_DungeonDB_Awake
    {
      private static void Postfix(DungeonGenerator __instance)
      {
        if (__instance.GetComponent<ZNetView>())
        {
          __instance.gameObject.AddComponent<LocationMarker>();
        }
      }
    }

    [HarmonyPatch(typeof(LocationProxy), "Awake")]
    private static class Postfix_LocationProxy_Awake
    {
      private static void Postfix(LocationProxy __instance)
      {
        if (__instance.GetComponentInChildren<DungeonGenerator>(true) != null)
        {
          return;
        }

        __instance.gameObject.AddComponent<LocationMarker>();
      }
    }

    #endregion ZoneSystem

    #region ODB

    [HarmonyPatch(typeof(ObjectDB), "Awake")]
    private static class Prefix_ObjectDB_Awake
    {
      private static void Prefix(ObjectDB __instance)
      {
        preODB(__instance);
      }
    }


    [HarmonyPatch(typeof(ObjectDB), "Awake")]
    private static class Patch_ObjectDB_Awake
    {
      private static void Postfix(ObjectDB __instance)
      {
        OdinPlus.PostODB();
      }
    }

    #endregion

    #region Znet

    [HarmonyPatch(typeof(ZNet), "Awake")]
    private static class Postfix_ZNet_Awake
    {
      private static void Postfix()
      {
        RegRPC();
        LocationManager.RequestServerFop();
      }
    }

    #endregion znet

    #region container

    [HarmonyPatch(typeof(Container), "Interact")]
    private static class Postfix_Container_Interact
    {
      private static void Postfix(Container __instance, Humanoid character, bool hold)
      {
        var a = __instance.GetComponent<LegacyChest>();
        if (a)
        {
          a.OnOpen(character, hold);
        }
      }
    }

    #endregion container

    #region Charactor

    [HarmonyPatch(typeof(Character), "GetHoverText")]
    private static class Prefix_Character_GetHoverText
    {
      private static bool Prefix(Character __instance, ref string __result)
      {
        Component comp = __instance.GetComponent<HumanNPC>();
        if (comp)
        {
          __result = ((HumanNPC) comp).GetHoverText();
          return false;
        }

        return true;
      }
    }

    #endregion Charactor

    #endregion patch

    #region Tool

    public static bool CheckPlayerNull(bool log = false)
    {
      if (Player.m_localPlayer == null)
      {
        if (log)
        {
          DBG.blogWarning("Player is Null");
        }

        return true;
      }

      return false;
    }

    #endregion
  }
}
