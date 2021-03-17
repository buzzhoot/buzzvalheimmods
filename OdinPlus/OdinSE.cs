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
		private void Awake()
		{
			initTroll();
		}
		#region Buzz_SE
		private static void initTroll()
		{
			var se = ScriptableObject.CreateInstance<SE_TrollHelper>();
			se.m_icon = OdinPlus.TrollHeadIcon;
			se.m_name = "$odin_se_troll";
			se.m_tooltip = "$odin_se_troll_tooltip";
			se.m_cooldownIcon = true;
			se.m_ttl = 300;
			SElist.Add("mead_troll",se);
		}

		#endregion
		public static void Register()
		{
			foreach (var se in SElist.Values)
			{
				ObjectDB.instance.m_StatusEffects.Add(se);
			}
			DBG.blogInfo("Register SE");
		}
	}
}