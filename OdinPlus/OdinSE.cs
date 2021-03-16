using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OdinPlus
{
	class OdinSE : MonoBehaviour
	{
		public static Dictionary<string,StatusEffect> SElist = new Dictionary<string, StatusEffect>();
		public static void init()
		{
			Sprite odinicon = ObjectDB.instance.GetItemPrefab("HelmetOdin").GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0];
			var mt = ObjectDB.instance.GetItemPrefab("MeadTasty");
			var id = mt.GetComponent<ItemDrop>().m_itemData;
			SElist.Add("mead_troll",ScriptableObject.CreateInstance<SE_TrollHelper>());
			foreach (var see in SElist)
			{
				var se=see.Value;
				se.m_icon = odinicon;
				se.m_name = "$odinse_troll";
				se.m_tooltip = "$odinse_troll_tooltip";
				se.m_cooldownIcon = true;
				se.m_ttl = 30;
				//id.m_shared.m_consumeStatusEffect = se;
			}
		}
		public static void Register()
		{
            foreach(var se in SElist.Values){
                ObjectDB.instance.m_StatusEffects.Add(se);
            }
		}
	}
}
