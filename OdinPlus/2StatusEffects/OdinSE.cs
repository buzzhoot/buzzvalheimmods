using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OdinPlus
{
	class OdinSE : MonoBehaviour
	{
		public static Dictionary<string, StatusEffect> SElist = new Dictionary<string, StatusEffect>();
		public static Dictionary<string, StatusEffect> BuzzList = new Dictionary<string, StatusEffect>();

		public static Dictionary<string, StatusEffect> ValList = new Dictionary<string, StatusEffect>();
		public static Dictionary<string, SEData> ValDataList = new Dictionary<string, SEData>();

		#region Main
		private void Awake()
		{
			SetupValSE();

			initTrollSE();
			initWolfSE();
			initValSE();
		}
		public static void Register()
		{
			foreach (var se in SElist.Values)
			{
				ObjectDB.instance.m_StatusEffects.Add(se);
			}
			DBG.blogInfo("Register SE");
		}

		#endregion Main

		#region Pet_SE
		private void initTrollSE()
		{
			var se = ScriptableObject.CreateInstance<SE_SumonPet>();
			se.name = "SE_Troll";
			se.m_icon = OdinPlus.TrollHeadIcon;
			se.m_name = "$odin_se_troll";
			se.m_tooltip = "$odin_se_troll_tooltip";
			se.m_cooldownIcon = true;
			se.m_ttl = 300;
			se.PetName = "TrollPet";
			SElist.Add("scroll_troll", se);
		}
		private void initWolfSE()
		{
			var se = ScriptableObject.CreateInstance<SE_SumonPet>();
			se.name = "SE_Wolf";
			se.m_icon = OdinPlus.WolfHeadIcon;
			se.m_name = "$odin_se_wolf";
			se.m_tooltip = "$odin_se_wolf_tooltip";
			se.m_cooldownIcon = true;
			se.m_ttl = 300;
			se.PetName = "WolfPet";
			SElist.Add("scroll_wolf", se);
		}

		#endregion

		#region Val_SE
		private void initValSE()
		{
			foreach (var item in ValDataList)
			{
				CreateValSE(item.Key, item.Value);
			}
		}
		private void CreateValSE(string name, SEData data)
		{
			var se = ScriptableObject.CreateInstance<SE_Stats>();
			se.name = name;
			se.m_icon = OdinPlus.OdinSEIcon[0];
			se.m_name = "$odin_se_" + name;
			se.m_tooltip = "$odin_" + name + "_tooltip";

			se.m_ttl = data.m_ttl;
			se.m_tickInterval = data.m_tickInterval;
			se.m_healthPerTickMinHealthPercentage = data.m_healthPerTickMinHealthPercentage;
			se.m_healthPerTick = data.m_healthPerTick;
			se.m_healthOverTime = data.m_healthOverTime;
			se.m_healthOverTimeDuration = data.m_healthOverTimeDuration;
			se.m_healthOverTimeInterval = data.m_healthOverTimeInterval = 5f;
			se.m_staminaOverTimeDuration = data.m_staminaOverTimeDuration;
			se.m_staminaDrainPerSec = data.m_staminaDrainPerSec;
			se.m_runStaminaDrainModifier = data.m_runStaminaDrainModifier;
			se.m_jumpStaminaUseModifier = data.m_jumpStaminaUseModifier;
			se.m_healthRegenMultiplier = data.m_healthRegenMultiplier = 1f;
			se.m_staminaRegenMultiplier = data.m_staminaRegenMultiplier = 1f;
			se.m_raiseSkill = data.m_raiseSkill;
			se.m_raiseSkillModifier = data.m_raiseSkillModifier;
			//?se.m_mods = new List<HitData.DamageModPair>() = data.m_mods = new List<HitData.DamageModPair>();
			se.m_modifyAttackSkill = data.m_modifyAttackSkill;
			se.m_damageModifier = data.m_damageModifier = 1f;
			se.m_noiseModifier = data.m_noiseModifier;
			se.m_stealthModifier = data.m_stealthModifier;
			se.m_addMaxCarryWeight = data.m_addMaxCarryWeight;

			SElist.Add(name, se);
		}
		#region Se manager
		public class SEData
		{
			public float m_ttl;
			public float m_tickInterval;
			public float m_healthPerTickMinHealthPercentage;
			public float m_healthPerTick;
			public float m_healthOverTime;
			public float m_healthOverTimeDuration;
			public float m_healthOverTimeInterval = 5f;
			public float m_staminaOverTimeDuration;
			public float m_staminaDrainPerSec;
			public float m_runStaminaDrainModifier;
			public float m_jumpStaminaUseModifier;
			public float m_healthRegenMultiplier = 1f;
			public float m_staminaRegenMultiplier = 1f;
			public Skills.SkillType m_raiseSkill;
			public float m_raiseSkillModifier;
			public List<HitData.DamageModPair> m_mods = new List<HitData.DamageModPair>();
			public Skills.SkillType m_modifyAttackSkill;
			public float m_damageModifier = 1f;
			public float m_noiseModifier;
			public float m_stealthModifier;
			public float m_addMaxCarryWeight;
		}

		public static void SetupValSE()
		{
			ValDataList.Add("ExpMeadS", new SEData() { m_ttl = 300, m_raiseSkill = Skills.SkillType.All, m_raiseSkillModifier = 50 });
			ValDataList.Add("ExpMeadM", new SEData() { m_ttl = 450, m_raiseSkill = Skills.SkillType.All, m_raiseSkillModifier = 75 });
			ValDataList.Add("ExpMeadL", new SEData() { m_ttl = 600, m_raiseSkill = Skills.SkillType.All, m_raiseSkillModifier = 125 });
			ValDataList.Add("WeightMeadS", new SEData() { m_ttl = 300, m_addMaxCarryWeight = 100 });
			ValDataList.Add("WeightMeadM", new SEData() { m_ttl = 300, m_addMaxCarryWeight = 150 });
			ValDataList.Add("WeightMeadL", new SEData() { m_ttl = 300, m_addMaxCarryWeight = 300 });
			ValDataList.Add("InvisableMeadS", new SEData() { m_ttl = 60, m_noiseModifier = -1, m_stealthModifier = -1 });
			ValDataList.Add("InvisableMeadM", new SEData() { m_ttl = 90, m_noiseModifier = -1, m_stealthModifier = -1 });
			ValDataList.Add("InvisableMeadL", new SEData() { m_ttl = 120, m_noiseModifier = -1, m_stealthModifier = -1 });
			ValDataList.Add("PickaxeMeadS", new SEData() { m_ttl = 300, m_modifyAttackSkill = Skills.SkillType.Pickaxes, m_damageModifier = 1.5f });
			ValDataList.Add("PickaxeMeadM", new SEData() { m_ttl = 300, m_modifyAttackSkill = Skills.SkillType.Pickaxes, m_damageModifier = 2f });
			ValDataList.Add("PickaxeMeadL", new SEData() { m_ttl = 300, m_modifyAttackSkill = Skills.SkillType.Pickaxes, m_damageModifier = 2.5f });
			ValDataList.Add("BowsMeadS", new SEData() { m_ttl = 300, m_modifyAttackSkill = Skills.SkillType.Bows, m_damageModifier = 1.5f });
			ValDataList.Add("BowsMeadM", new SEData() { m_ttl = 300, m_modifyAttackSkill = Skills.SkillType.Bows, m_damageModifier = 2f });
			ValDataList.Add("BowsMeadL", new SEData() { m_ttl = 300, m_modifyAttackSkill = Skills.SkillType.Bows, m_damageModifier = 2.5f });
			ValDataList.Add("SwordsMeadS", new SEData() { m_ttl = 300, m_modifyAttackSkill = Skills.SkillType.Swords, m_damageModifier = 1.5f });
			ValDataList.Add("SwordsMeadM", new SEData() { m_ttl = 300, m_modifyAttackSkill = Skills.SkillType.Swords, m_damageModifier = 2f });
			ValDataList.Add("SwordsMeadL", new SEData() { m_ttl = 300, m_modifyAttackSkill = Skills.SkillType.Swords, m_damageModifier = 2.5f });
		}
		#endregion Se manager

		#endregion Val_SE

	}

}