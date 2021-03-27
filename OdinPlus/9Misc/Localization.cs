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
{"op_buy","Buy"},
{"op_crd","Credits"},
{"op_god","Odin"},
{"op_god_nocrd","Hard work is the only way to get reward"},
{"op_god_randomitem","I need Something useful...like "},
{"op_god_takeoffer","Nice,bring back more"},
{"op_inventory_full","Make a room! your inventory is full!"},
{"op_munin_c1","Accept Side Quest"},
{"op_munin_c2","Give up Quest"},
{"op_munin_c3","Change Quest Level"},
{"op_munin_c4","Show me QuestList"},
{"op_munin_cd","My brother is finding Quest for you, wait for"},
{"op_munin_giveup","Which Quest you want to give up?"},
{"op_munin_name","Munin"},
{"op_munin_noq","You don't have Quest "},
{"op_munin_notask","You don't have any Quest"},
{"op_munin_notenough","Not eoungh...bring more!"},
{"op_munin_takeoffer","Nice!"},
{"op_munin_task_lvl","Current Quest Level:"},
{"op_munin_taskfull","You have too many quests,clear some before you want more"},
{"op_munin_tasknum_a","Tasks"},
{"op_munin_tasknum_b","You have"},
{"op_munin_wait_hug","Wait for Hugin,he will tell you something"},
{"op_OdinLegacy_desc","Offer this to odin and get credits"},
{"op_OdinLegacy_name","Odin Legacy"},
{"op_offer","Offer your items"},
{"op_pot_name","Odin's Pot"},
{"op_pot_open","Want something magic,Warrior?"},
{"op_raise","I made you stronger,warrior"},
{"op_ScrollTroll_desc","\" Use this to summon troll...but watchout,he may disapper, i can't assure you anything.. \""},
{"op_ScrollTroll_name","Magic Tr Headoll"},
{"op_ScrollTroll_tooltip","Duration:<color=orange><b>5</b></color>mins\n CoolDown:<color=orange><b>10</b></color>mins \nUse your <color=orange><b>SecondaryKey</b></color> to make him force attack"},
{"op_ScrollWolf_desc","\" Use this to summon a magic wolf with backpack...but watchout,he may disapper, i can't assure you anything.. \""},
{"op_ScrollWolf_name","Magic Wolf Head"},
{"op_ScrollWolf_tooltip","Duration:<color=orange><b>30</b></color>mins \n Use your <color=orange><b>SecondaryKey</b></color> to open his backpack"},
{"op_shaman","Mystery shaman"},
{"op_shaman_no","Hmm that's something new,can't take that right now"},
{"op_shaman_notenough","Need at least 3 ...."},
{"op_shaman_offer","Offer your trophies(3 wolf or 3 troll)"},
{"op_shaman_use","?"},
{"op_switch","Switch Choice"},
{"op_use","Use 10 credits to raise your skill:"},
{"op_wolf_name","Magic Wolf"},
{"op_wolf_use","Open wolf pack"},
{"op_wrong_num","ops wrong number"},
//Meads
{"op_ExpMeadS_name","Exp Mead Small"},
{"op_ExpMeadS_desc","Makes you level up faster!"},
{"op_ExpMeadS_tooltip","Duration:<color=orange><b>300</b></color>s"},
{"op_ExpMeadM_name","Exp Mead Medium"},
{"op_ExpMeadM_desc","Makes you level up faster!"},
{"op_ExpMeadM_tooltip","Duration:<color=orange><b>450</b></color>s"},
{"op_ExpMeadL_name","Exp Mead Large"},
{"op_ExpMeadL_desc","Makes you level up faster!"},
{"op_ExpMeadL_tooltip","Duration:<color=orange><b>600</b></color>s"},
{"op_WeightMeadS_name","Weight Mead Small"},
{"op_WeightMeadS_desc","Add more carry weight"},
{"op_WeightMeadS_tooltip","Weight Add:<color=orange><b>100</b></color>\nDuration:<color=orange><b>300</b></color>s"},
{"op_WeightMeadM_name","Weight Mead Medium"},
{"op_WeightMeadM_desc","Add more carry weight"},
{"op_WeightMeadM_tooltip","Weight Add:<color=orange><b>150</b></color>\nDuration:<color=orange><b>300</b></color>s"},
{"op_WeightMeadL_name","Weight Mead Large"},
{"op_WeightMeadL_desc","Add more carry weight"},
{"op_WeightMeadL_tooltip","Duration:<color=orange><b>300</b></color>\nDuration:<color=orange><b>300</b></color>s"},
{"op_InvisibleMeadS_name","Invsiable Mead Small"},
{"op_InvisibleMeadS_desc","Makes you Invisible!only effects to Monsters."},
{"op_InvisibleMeadS_tooltip","Duration:<color=orange><b>60</b></color>s"},
{"op_InvisibleMeadM_name","Invsiable Mead Medium"},
{"op_InvisibleMeadM_desc","Makes you Invisible!only effects to Monsters."},
{"op_InvisibleMeadM_tooltip","Duration:<color=orange><b>90</b></color>s"},
{"op_InvisibleMeadL_name","Invsiable Mead Large"},
{"op_InvisibleMeadL_desc","Makes you Invisible!only effects to Monsters."},
{"op_InvisibleMeadL_tooltip","Duration:<color=orange><b>120</b></color>s"},
{"op_PickaxeMeadS_name","Pickaxe Mead Small"},
{"op_PickaxeMeadS_desc","Makes your pickaxe more powerful!"},
{"op_PickaxeMeadS_tooltip","Duration:<color=orange><b>60</b></color>s"},
{"op_PickaxeMeadM_name","Pickaxe Mead Medium"},
{"op_PickaxeMeadM_desc","Makes your pickaxe more powerful!"},
{"op_PickaxeMeadM_tooltip","Duration:<color=orange><b>150</b></color>s"},
{"op_PickaxeMeadL_name","Pickaxe Mead Large"},
{"op_PickaxeMeadL_desc","Makes your pickaxe more powerful!"},
{"op_PickaxeMeadL_tooltip","Duration:<color=orange><b>300</b></color>s"},
{"op_BowsMeadS_name","Bow Mead Small"},
{"op_BowsMeadS_desc","Makes your bow more powerful!"},
{"op_BowsMeadS_tooltip","Duration:<color=orange><b>60</b></color>s"},
{"op_BowsMeadM_name","Bow Mead Medium"},
{"op_BowsMeadM_desc","Makes your bow more powerful!"},
{"op_BowsMeadM_tooltip","Duration:<color=orange><b>150</b></color>s"},
{"op_BowsMeadL_name","Bow Mead Large"},
{"op_BowsMeadL_desc","Makes your bow more powerful!"},
{"op_BowsMeadL_tooltip","Duration:<color=orange><b>300</b></color>s"},
{"op_SwordsMeadS_name","Swords Mead Small"},
{"op_SwordsMeadS_desc","Makes your sword more powerful!"},
{"op_SwordsMeadS_tooltip","Duration:<color=orange><b>60</b></color>s"},
{"op_SwordsMeadM_name","Swords Mead Medium"},
{"op_SwordsMeadM_desc","Makes your sword more powerful!"},
{"op_SwordsMeadM_tooltip","Duration:<color=orange><b>150</b></color>s"},
{"op_SwordsMeadL_name","Swords Mead Large"},
{"op_SwordsMeadL_desc","Makes your sword more powerful!"},
{"op_SwordsMeadL_tooltip","Duration:<color=orange><b>300</b></color>s"},
{"op_SpeedMeadsL_name","Speed Mead"},
{"op_SpeedMeadsL_desc","Makes you run faster!"},
{"op_SpeedMeadsL_tooltip","Duration:<color=orange><b>300</b></color>s"},
{"op_AxeMeadS_name","Axe Mead Small"},
{"op_AxeMeadS_desc","Makes your Axe more powerful!"},
{"op_AxeMeadS_tooltip","Duration:<color=orange><b>60</b></color>s"},
{"op_AxeMeadM_name","Axe Mead Medium"},
{"op_AxeMeadM_desc","Makes your Axe more powerful!"},
{"op_AxeMeadM_tooltip","Duration:<color=orange><b>150</b></color>s"},
{"op_AxeMeadL_name","Axe Mead Large"},
{"op_AxeMeadL_desc","Makes your Axe more powerful!"},
{"op_AxeMeadL_tooltip","Duration:<color=orange><b>300</b></color>s"},
//task
{"op_hunt_target","(Hunt Target)"},
		};
		private static Dictionary<string, string> chinese = new Dictionary<string, string>() {
			{"op_use","升级你的技能：" },
			{"op_se_troll","宠物巨魔"},
			{"op_se_troll_tooltip","召唤一只宠物巨魔"},
			{"op_ScrolTroll_name","巨魔卷轴"},
			{"op_ScrolTroll_desc","地精的商品，据说可以召唤一只巨魔打工人，但是他不会打我吧..."},
			{"op_pot_name","品尝奥丁的魔酒"},
{"op_ExpMeadS_name","经验魔法酒(小)"},
{"op_ExpMeadS_desc","加快你的升级速度"},
{"op_ExpMeadS_tooltip","持续时间:<color=orange><b>300</b></color>秒"},
{"op_ExpMeadM_name","经验魔法酒(中)"},
{"op_ExpMeadM_desc","加快你的升级速度"},
{"op_ExpMeadM_tooltip","持续时间:<color=orange><b>450</b></color>秒"},
{"op_ExpMeadL_name","经验魔法酒(大)"},
{"op_ExpMeadL_desc","加快你的升级速度"},
{"op_ExpMeadL_tooltip","持续时间:<color=orange><b>600</b></color>秒"},
{"op_WeightMeadS_name","负重魔法酒(小)"},
{"op_WeightMeadS_desc","增加你的负重"},
{"op_WeightMeadS_tooltip","负重:<color=orange><b>100</b></color>\n持续时间:<color=orange><b>300</b></color>秒"},
{"op_WeightMeadM_name","负重魔法酒(中)"},
{"op_WeightMeadM_desc","增加你的负重"},
{"op_WeightMeadM_tooltip","负重:<color=orange><b>150</b></color>\n持续时间:<color=orange><b>300</b></color>秒"},
{"op_WeightMeadL_name","负重魔法酒(大)"},
{"op_WeightMeadL_desc","增加你的负重"},
{"op_WeightMeadL_tooltip","持续时间:<color=orange><b>300</b></color>\n持续时间:<color=orange><b>300</b></color>秒"},
{"op_InvisibleMeadS_name","隐身魔法酒(小)"},
{"op_InvisibleMeadS_desc","使你隐身一段时间，你在想什么？当然仅仅对怪物有效"},
{"op_InvisibleMeadS_tooltip","持续时间:<color=orange><b>60</b></color>秒"},
{"op_InvisibleMeadM_name","隐身魔法酒(中)"},
{"op_InvisibleMeadM_desc","使你隐身一段时间，你在想什么？当然仅仅对怪物有效"},
{"op_InvisibleMeadM_tooltip","持续时间:<color=orange><b>90</b></color>秒"},
{"op_InvisibleMeadL_name","隐身魔法酒(大)"},
{"op_InvisibleMeadL_desc","使你隐身一段时间，你在想什么？当然仅仅对怪物有效"},
{"op_InvisibleMeadL_tooltip","持续时间:<color=orange><b>120</b></color>秒"},
{"op_PickaxeMeadS_name","矿工魔法酒(小)"},
{"op_PickaxeMeadS_desc","变身老黄金矿工"},
{"op_PickaxeMeadS_tooltip","持续时间:<color=orange><b>60</b></color>秒"},
{"op_PickaxeMeadM_name","矿工魔法酒(中)"},
{"op_PickaxeMeadM_desc","变身老黄金矿工"},
{"op_PickaxeMeadM_tooltip","持续时间:<color=orange><b>150</b></color>秒"},
{"op_PickaxeMeadL_name","矿工魔法酒(大)"},
{"op_PickaxeMeadL_desc","变身老黄金矿工"},
{"op_PickaxeMeadL_tooltip","持续时间:<color=orange><b>300</b></color>秒"},
{"op_BowsMeadS_name","射手魔法酒(小)"},
{"op_BowsMeadS_desc","变身豌豆射手"},
{"op_BowsMeadS_tooltip","持续时间:<color=orange><b>60</b></color>秒"},
{"op_BowsMeadM_name","射手魔法酒(中)"},
{"op_BowsMeadM_desc","变身豌豆射手"},
{"op_BowsMeadM_tooltip","持续时间:<color=orange><b>150</b></color>秒"},
{"op_BowsMeadL_name","射手魔法酒(大)"},
{"op_BowsMeadL_desc","变身豌豆射手"},
{"op_BowsMeadL_tooltip","持续时间:<color=orange><b>300</b></color>秒"},
{"op_SwordsMeadS_name","剑魔法酒(小)"},
{"op_SwordsMeadS_desc","变身<color=purple><b>基</b></color>剑大师"},
{"op_SwordsMeadS_tooltip","持续时间:<color=orange><b>60</b></color>秒"},
{"op_SwordsMeadM_name","剑魔法酒(中)"},
{"op_SwordsMeadM_desc","变身<color=purple><b>基</b></color>剑大师"},
{"op_SwordsMeadM_tooltip","持续时间:<color=orange><b>150</b></color>秒"},
{"op_SwordsMeadL_name","剑魔法酒(大)"},
{"op_SwordsMeadL_desc","变身<color=purple><b>基</b></color>剑大师"},
{"op_SwordsMeadL_tooltip","持续时间:<color=orange><b>300</b></color>秒"},
{"op_SpeedMeadsL_name","猎豹魔法酒"},
{"op_SpeedMeadsL_desc","变身 香港记者...+1s"},
{"op_SpeedMeadsL_tooltip","持续时间:<color=orange><b>300</b></color>秒"},
{"op_AxeMeadS_name","伐木工魔法酒(小)"},
{"op_AxeMeadS_desc","少盖房子多砍树"},
{"op_AxeMeadS_tooltip","持续时间:<color=orange><b>60</b></color>秒"},
{"op_AxeMeadM_name","伐木工魔法酒(中)"},
{"op_AxeMeadM_desc","少盖房子多砍树"},
{"op_AxeMeadM_tooltip","持续时间:<color=orange><b>150</b></color>秒"},
{"op_AxeMeadL_name","伐木工魔法酒(大)"},
{"op_AxeMeadL_desc","少盖房子多砍树"},
{"op_AxeMeadL_tooltip","持续时间:<color=orange><b>300</b></color>秒"},
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