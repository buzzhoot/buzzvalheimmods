using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
using System.Reflection;


namespace OdinPlus
{
    public class BuzzLocal
    {
        private static Localization lcl;
        public static Dictionary<string, string> t; //= new Dictionary<string, string>();
        private static Dictionary<string, string> english = new Dictionary<string, string>() {
            {"odinuse","Raise your skill:"},
            {"odinse_troll","TrollPet"},
            {"odinse_troll_tooltip","summon a pet troll"},
            {"odinmead_troll_name","Troll mead"},
            {"odinmead_troll_desc","This item will summon a pet"}
        };
        private static Dictionary<string, string> chinese = new Dictionary<string, string>() {
            {"odinuse","升级你的技能：" },
            {"odinse_troll","宠物巨魔"},
            {"odinse_troll_tooltip","召唤一只宠物巨魔"}
        };
        public static void init(string lang,Localization l)
        {
            lcl = l;
            //string @str = PlayerPrefs.GetString("language", "");
            if (lang == "Chinese")
            {
                t = chinese;
            }
            else
            {
                t = english;
            }
        }
        public static void AddWord(object[] element)
        {
            MethodInfo meth = AccessTools.Method(typeof(Localization), "AddWord", null, null);
            meth.Invoke(lcl, element);
        }
        public static void UpdateDictinary()
        {
            var len = t.Count;
            foreach(var el in t)
            {
                AddWord(new object[]{ el.Key,el.Value});
            }
            DBG.blogInfo("Translation added");
        }
    }
}