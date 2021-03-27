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
//NPC
{"odin_OdinLegacy_desc","Offer this to odin and get credits"},
{"odin_OdinLegacy_name","Odin Legacy"},
{"odin_god","ODIN"},
{"odin_use","Use 10 credits to raise your skill:"},
{"odin_pot_name","Odin's Pot"},
{"odin_inventory_full","Make a room! your inventory is full!"},
{"odin_buy","Buy"},
{"odin_munin_name","Munin"},
{"odin_shaman","Mystery shaman"},
{"odin_shaman_notenough","Need at least 3 ...."},
{"odin_shaman_use","?"},
{"odin_wolf_name","Magic Wolf"},
{"odin_ScrollWolf_name","Magic Wolf Head"},
{"odin_ScrollWolf_desc","\" Use this to summon a magic wolf with backpack...but watchout,he may disapper, i can't assure you anything.. \""},
{"odin_ScrollWolf_tooltip","Duration:<color=orange><b>30</b></color>mins \n Use your <color=orange><b>SecondaryKey</b></color> to open his backpack"},
{"odin_ScrollTroll_name","Magic Tr Headoll"},
{"odin_ScrollTroll_desc","\" Use this to summon troll...but watchout,he may disapper, i can't assure you anything.. \""},
{"odin_ScrollTroll_tooltip","Duration:<color=orange><b>5</b></color>mins\n CoolDown:<color=orange><b>10</b></color>mins \nUse your <color=orange><b>SecondaryKey</b></color> to make him force attack"},
{"odin_wolf_use","Open wolf pack"},
//Meads
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
{"odin_InvisibleMeadS_name","Invsiable Mead Small"},
{"odin_InvisibleMeadS_desc","Makes you Invisible!only effects to Monsters."},
{"odin_InvisibleMeadS_tooltip","Duration:<color=orange><b>60</b></color>s"},
{"odin_InvisibleMeadM_name","Invsiable Mead Medium"},
{"odin_InvisibleMeadM_desc","Makes you Invisible!only effects to Monsters."},
{"odin_InvisibleMeadM_tooltip","Duration:<color=orange><b>90</b></color>s"},
{"odin_InvisibleMeadL_name","Invsiable Mead Large"},
{"odin_InvisibleMeadL_desc","Makes you Invisible!only effects to Monsters."},
{"odin_InvisibleMeadL_tooltip","Duration:<color=orange><b>120</b></color>s"},
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
{"odin_SwordsMeadS_name","Swords Mead Small"},
{"odin_SwordsMeadS_desc","Makes your sword more powerful!"},
{"odin_SwordsMeadS_tooltip","Duration:<color=orange><b>60</b></color>s"},
{"odin_SwordsMeadM_name","Swords Mead Medium"},
{"odin_SwordsMeadM_desc","Makes your sword more powerful!"},
{"odin_SwordsMeadM_tooltip","Duration:<color=orange><b>150</b></color>s"},
{"odin_SwordsMeadL_name","Swords Mead Large"},
{"odin_SwordsMeadL_desc","Makes your sword more powerful!"},
{"odin_SwordsMeadL_tooltip","Duration:<color=orange><b>300</b></color>s"},
{"odin_SpeedMeadsL_name","Speed Mead"},
{"odin_SpeedMeadsL_desc","Makes you run faster!"},
{"odin_SpeedMeadsL_tooltip","Duration:<color=orange><b>300</b></color>s"},
{"odin_AxeMeadS_name","Axe Mead Small"},
{"odin_AxeMeadS_desc","Makes your Axe more powerful!"},
{"odin_AxeMeadS_tooltip","Duration:<color=orange><b>60</b></color>s"},
{"odin_AxeMeadM_name","Axe Mead Medium"},
{"odin_AxeMeadM_desc","Makes your Axe more powerful!"},
{"odin_AxeMeadM_tooltip","Duration:<color=orange><b>150</b></color>s"},
{"odin_AxeMeadL_name","Axe Mead Large"},
{"odin_AxeMeadL_desc","Makes your Axe more powerful!"},
{"odin_AxeMeadL_tooltip","Duration:<color=orange><b>300</b></color>s"},
//task
{"odin_hunt_target","(Hunt Target)"},
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
{"odin_InvisibleMeadS_name","隐身魔法酒(小)"},
{"odin_InvisibleMeadS_desc","使你隐身一段时间，你在想什么？当然仅仅对怪物有效"},
{"odin_InvisibleMeadS_tooltip","持续时间:<color=orange><b>60</b></color>秒"},
{"odin_InvisibleMeadM_name","隐身魔法酒(中)"},
{"odin_InvisibleMeadM_desc","使你隐身一段时间，你在想什么？当然仅仅对怪物有效"},
{"odin_InvisibleMeadM_tooltip","持续时间:<color=orange><b>90</b></color>秒"},
{"odin_InvisibleMeadL_name","隐身魔法酒(大)"},
{"odin_InvisibleMeadL_desc","使你隐身一段时间，你在想什么？当然仅仅对怪物有效"},
{"odin_InvisibleMeadL_tooltip","持续时间:<color=orange><b>120</b></color>秒"},
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
{"odin_SpeedMeadsL_tooltip","持续时间:<color=orange><b>300</b></color>秒"},
{"odin_AxeMeadS_name","伐木工魔法酒(小)"},
{"odin_AxeMeadS_desc","少盖房子多砍树"},
{"odin_AxeMeadS_tooltip","持续时间:<color=orange><b>60</b></color>秒"},
{"odin_AxeMeadM_name","伐木工魔法酒(中)"},
{"odin_AxeMeadM_desc","少盖房子多砍树"},
{"odin_AxeMeadM_tooltip","持续时间:<color=orange><b>150</b></color>秒"},
{"odin_AxeMeadL_name","伐木工魔法酒(大)"},
{"odin_AxeMeadL_desc","少盖房子多砍树"},
{"odin_AxeMeadL_tooltip","持续时间:<color=orange><b>300</b></color>秒"},
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