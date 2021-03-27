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
			{"odin_ScrolTroll_name","Troll mead"},
			{"odin_ScrolTroll_desc","This item will summon a pet"},
			{"odin_pot_name","Odin's Pot"},
{"odin_ExpMeadS_name","Exp Mead Small"},
{"odin_ExpMeadS_desc","Makes you level up faster!"},
{"odin_ExpMeadS_tooltip","Duration:<color=orange><b>300</b></color>s"},
{"odin_ExpMeadM_name","Exp Mead Medium"},
{"odin_ExpMeadM_desc","Makes you level up faster!"},
{"odin_ExpMeadM_tooltip","Duration:<color=orange><b>450</b></color>s"},
{"odin_ExpMeadL_name","Exp Mead Large"},
{"odin_ExpMeadL_desc","Makes you level up faster!"},
{"odin_ExpMeadL_tooltip","Duration:<color=orange><b>600</b></color>s"},
{"odin_WeightMeadS_name","Weight Mead Small"},
{"odin_WeightMeadS_desc","Add more carry weight"},
{"odin_WeightMeadS_tooltip","Weight Add:<color=orange><b>100</b></color>\nDuration:<color=orange><b>300</b></color>s"},
{"odin_WeightMeadM_name","Weight Mead Medium"},
{"odin_WeightMeadM_desc","Add more carry weight"},
{"odin_WeightMeadM_tooltip","Weight Add:<color=orange><b>150</b></color>\nDuration:<color=orange><b>300</b></color>s"},
{"odin_WeightMeadL_name","Weight Mead Large"},
{"odin_WeightMeadL_desc","Add more carry weight"},
{"odin_WeightMeadL_tooltip","Duration:<color=orange><b>300</b></color>\nDuration:<color=orange><b>300</b></color>s"},
{"odin_InvisableMeadS_name","Invsiable Mead Small"},
{"odin_InvisableMeadS_desc","Makes you Invisable!only effects to Monsters."},
{"odin_InvisableMeadS_tooltip","Duration:<color=orange><b>60</b></color>s"},
{"odin_InvisableMeadM_name","Invsiable Mead Medium"},
{"odin_InvisableMeadM_desc","Makes you Invisable!only effects to Monsters."},
{"odin_InvisableMeadM_tooltip","Duration:<color=orange><b>90</b></color>s"},
{"odin_InvisableMeadL_name","Invsiable Mead Large"},
{"odin_InvisableMeadL_desc","Makes you Invisable!only effects to Monsters."},
{"odin_InvisableMeadL_tooltip","Duration:<color=orange><b>120</b></color>s"},
{"odin_PickaxeMeadS_name","Pickaxe Mead Small"},
{"odin_PickaxeMeadS_desc","Makes your pickaxe more powerful!"},
{"odin_PickaxeMeadS_tooltip","Duration:<color=orange><b>60</b></color>s"},
{"odin_PickaxeMeadM_name","Pickaxe Mead Medium"},
{"odin_PickaxeMeadM_desc","Makes your pickaxe more powerful!"},
{"odin_PickaxeMeadM_tooltip","Duration:<color=orange><b>150</b></color>s"},
{"odin_PickaxeMeadL_name","Pickaxe Mead Large"},
{"odin_PickaxeMeadL_desc","Makes your pickaxe more powerful!"},
{"odin_PickaxeMeadL_tooltip","Duration:<color=orange><b>300</b></color>s"},
{"odin_BowsMeadS_name","Bow Mead Small"},
{"odin_BowsMeadS_desc","Makes your bow more powerful!"},
{"odin_BowsMeadS_tooltip","Duration:<color=orange><b>60</b></color>s"},
{"odin_BowsMeadM_name","Bow Mead Medium"},
{"odin_BowsMeadM_desc","Makes your bow more powerful!"},
{"odin_BowsMeadM_tooltip","Duration:<color=orange><b>150</b></color>s"},
{"odin_BowsMeadL_name","Bow Mead Large"},
{"odin_BowsMeadL_desc","Makes your bow more powerful!"},
{"odin_BowsMeadL_tooltip","Duration:<color=orange><b>300</b></color>s"},
{"odin_SwordsMeadS_name","Exp Mead Small"},
{"odin_SwordsMeadS_desc","Makes your sword more powerful!"},
{"odin_SwordsMeadS_tooltip","Duration:<color=orange><b>60</b></color>s"},
{"odin_SwordsMeadM_name","Exp Mead Medium"},
{"odin_SwordsMeadM_desc","Makes your sword more powerful!"},
{"odin_SwordsMeadM_tooltip","Duration:<color=orange><b>150</b></color>s"},
{"odin_SwordsMeadL_name","Exp Mead Large"},
{"odin_SwordsMeadL_desc","Makes your sword more powerful!"},
{"odin_SwordsMeadL_tooltip","Duration:<color=orange><b>300</b></color>s"},
{"odin_SpeedMeadsL_name","Speed Mead"},
{"odin_SpeedMeadsL_desc","Makes you run faster!"},
{"odin_SpeedMeadsL_tooltip","Duration:<color=orange><b>300</b></color>s"}
		};
		private static Dictionary<string, string> chinese = new Dictionary<string, string>() {
			{"odin_use","升级你的技能：" },
			{"odin_se_troll","宠物巨魔"},
			{"odin_se_troll_tooltip","召唤一只宠物巨魔"},
			{"odin_ScrolTroll_name","巨魔卷轴"},
			{"odin_ScrolTroll_desc","地精的商品，据说可以召唤一只巨魔打工人，但是他不会打我吧..."},
			{"odin_pot_name","品尝奥丁的魔酒"},
{"odin_ExpMeadS_name","经验魔法酒(小)"},
{"odin_ExpMeadS_desc","加快你的升级速度"},
{"odin_ExpMeadS_tooltip","持续时间:<color=orange><b>300</b></color>秒"},
{"odin_ExpMeadM_name","经验魔法酒(中)"},
{"odin_ExpMeadM_desc","加快你的升级速度"},
{"odin_ExpMeadM_tooltip","持续时间:<color=orange><b>450</b></color>秒"},
{"odin_ExpMeadL_name","经验魔法酒(大)"},
{"odin_ExpMeadL_desc","加快你的升级速度"},
{"odin_ExpMeadL_tooltip","持续时间:<color=orange><b>600</b></color>秒"},
{"odin_WeightMeadS_name","负重魔法酒(小)"},
{"odin_WeightMeadS_desc","增加你的负重"},
{"odin_WeightMeadS_tooltip","负重:<color=orange><b>100</b></color>\n持续时间:<color=orange><b>300</b></color>秒"},
{"odin_WeightMeadM_name","负重魔法酒(中)"},
{"odin_WeightMeadM_desc","增加你的负重"},
{"odin_WeightMeadM_tooltip","负重:<color=orange><b>150</b></color>\n持续时间:<color=orange><b>300</b></color>秒"},
{"odin_WeightMeadL_name","负重魔法酒(大)"},
{"odin_WeightMeadL_desc","增加你的负重"},
{"odin_WeightMeadL_tooltip","持续时间:<color=orange><b>300</b></color>\n持续时间:<color=orange><b>300</b></color>秒"},
{"odin_InvisableMeadS_name","隐身魔法酒(小)"},
{"odin_InvisableMeadS_desc","使你隐身一段时间，你在想什么？当然仅仅对怪物有效"},
{"odin_InvisableMeadS_tooltip","持续时间:<color=orange><b>60</b></color>秒"},
{"odin_InvisableMeadM_name","隐身魔法酒(中)"},
{"odin_InvisableMeadM_desc","使你隐身一段时间，你在想什么？当然仅仅对怪物有效"},
{"odin_InvisableMeadM_tooltip","持续时间:<color=orange><b>90</b></color>秒"},
{"odin_InvisableMeadL_name","隐身魔法酒(大)"},
{"odin_InvisableMeadL_desc","使你隐身一段时间，你在想什么？当然仅仅对怪物有效"},
{"odin_InvisableMeadL_tooltip","持续时间:<color=orange><b>120</b></color>秒"},
{"odin_PickaxeMeadS_name","矿工魔法酒(小)"},
{"odin_PickaxeMeadS_desc","变身老黄金矿工"},
{"odin_PickaxeMeadS_tooltip","持续时间:<color=orange><b>60</b></color>秒"},
{"odin_PickaxeMeadM_name","矿工魔法酒(中)"},
{"odin_PickaxeMeadM_desc","变身老黄金矿工"},
{"odin_PickaxeMeadM_tooltip","持续时间:<color=orange><b>150</b></color>秒"},
{"odin_PickaxeMeadL_name","矿工魔法酒(大)"},
{"odin_PickaxeMeadL_desc","变身老黄金矿工"},
{"odin_PickaxeMeadL_tooltip","持续时间:<color=orange><b>300</b></color>秒"},
{"odin_BowsMeadS_name","射手魔法酒(小)"},
{"odin_BowsMeadS_desc","变身豌豆射手"},
{"odin_BowsMeadS_tooltip","持续时间:<color=orange><b>60</b></color>秒"},
{"odin_BowsMeadM_name","射手魔法酒(中)"},
{"odin_BowsMeadM_desc","变身豌豆射手"},
{"odin_BowsMeadM_tooltip","持续时间:<color=orange><b>150</b></color>秒"},
{"odin_BowsMeadL_name","射手魔法酒(大)"},
{"odin_BowsMeadL_desc","变身豌豆射手"},
{"odin_BowsMeadL_tooltip","持续时间:<color=orange><b>300</b></color>秒"},
{"odin_SwordsMeadS_name","剑魔法酒(小)"},
{"odin_SwordsMeadS_desc","变身<color=purple><b>基</b></color>剑大师"},
{"odin_SwordsMeadS_tooltip","持续时间:<color=orange><b>60</b></color>秒"},
{"odin_SwordsMeadM_name","剑魔法酒(中)"},
{"odin_SwordsMeadM_desc","变身<color=purple><b>基</b></color>剑大师"},
{"odin_SwordsMeadM_tooltip","持续时间:<color=orange><b>150</b></color>秒"},
{"odin_SwordsMeadL_name","剑魔法酒(大)"},
{"odin_SwordsMeadL_desc","变身<color=purple><b>基</b></color>剑大师"},
{"odin_SwordsMeadL_tooltip","持续时间:<color=orange><b>300</b></color>秒"},
{"odin_SpeedMeadsL_name","猎豹魔法酒"},
{"odin_SpeedMeadsL_desc","变身 香港记者...+1s"},
{"odin_SpeedMeadsL_tooltip","持续时间:<color=orange><b>300</b></color>秒"}
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