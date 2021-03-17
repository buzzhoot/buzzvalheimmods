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
			{"odin_use","Raise your skill:"},
			{"odin_se_troll","TrollPet"},
			{"odin_se_troll_tooltip","summon a pet troll"},
			{"odin_mead_troll_name","Troll mead"},
			{"odin_mead_troll_desc","This item will summon a pet"},
			{"odin_pot_name","Odin's Pot"}
		};
		private static Dictionary<string, string> chinese = new Dictionary<string, string>() {
			{"odin_use","升级你的技能：" },
			{"odin_se_troll","宠物巨魔"},
			{"odin_se_troll_tooltip","召唤一只宠物巨魔"},
			{"odin_mead_troll_name","巨魔卷轴"},
			{"odin_mead_troll_desc","地精的商品，据说可以召唤一只巨魔打工人，但是他不会打我吧..."},
			{"odin_pot_name","品尝奥丁的魔酒"}
		};
		public static void init(string lang, Localization l)
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
			foreach (var el in t)
			{
				AddWord(new object[] { el.Key, el.Value });
			}
			DBG.blogInfo("Translation added");
		}
	}
}