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

		#region Main

		#endregion Main
		private void Awake()
		{
			OdinMeads.ValMeadsName.Add("exp_mead");
			initTrollSE();
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

		#region Buzz_SE
		private void initTrollSE()
		{
			var se = ScriptableObject.CreateInstance<SE_TrollHelper>();
			se.m_icon = OdinPlus.TrollHeadIcon;
			se.m_name = "$odin_se_troll";
			se.m_tooltip = "$odin_se_troll_tooltip";
			se.m_cooldownIcon = true;
			se.m_ttl = 300;
			SElist.Add("scroll_troll", se);
		}

		#endregion

		#region Val_SE
		private void initValSE()
		{
			foreach (var item in OdinMeads.ValMeadsName)
			{
				CreateValSE(item);
			}
		}
		private void CreateValSE(string name)
		{
			var se = ScriptableObject.CreateInstance<SE_Stats>();
			se.m_icon = OdinPlus.OdinSEIcon[0];
			se.m_name = "$odin_se_" + name;
			se.m_tooltip = "$odin_" + name + "_tooltip";
			
			se.m_ttl=5;

			se.m_raiseSkill=Skills.SkillType.All;
			se.m_raiseSkillModifier=100;
			
			SElist.Add(name, se);
		}
		#endregion Val_SE

	}

}