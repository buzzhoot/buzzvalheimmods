using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using BepInEx.Logging;
using UnityEngine;


namespace BuzzValheimTemplate
{
	[BepInPlugin("buzz.valhei.name", "name", "0.0.1")]
	public class Plugin:BaseUnityPlugin
	{
        #region Var
            
            //public static ConfigEntry<int> nexusID;
             private static ManualLogSource logger;
        #endregion
       
		private void Awake()
		{
		Plugin.logger = base.Logger;
        
        //Plugin.nexusID = base.Config.Bind<int>("General", "NexusID", 354, "Nexus mod ID for updates");
        Plugin.logger.LogInfo("name Loadded"); 
		}

    }
}
