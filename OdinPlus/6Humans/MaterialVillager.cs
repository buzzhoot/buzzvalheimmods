using System;
using UnityEngine;
using System.Collections.Generic;

namespace OdinPlus
{
	public class MaterialVillager : QuestVillager, Hoverable, Interactable, OdinInteractable
	{
		public readonly string[] m_materials = new string[] { "Wood", "Stone" };
		public string m_item = "";
		protected override void Awake()
		{
			base.Awake();
			var zdo = m_nview.GetZDO();
			m_item = zdo.GetString("Qmat","");
			if (m_item=="")
			{
				m_item=m_materials.GetRandomElement();
				zdo.Set("Qmat",m_item);
			}
		}
		public override void Choice0()
		{
			string n = string.Format("I could use some <color=yellow><b>{0}</b></color> to build our home",m_item);
			Say(n);
		}
		public override bool UseItem(Humanoid user, ItemDrop.ItemData item)
		{
			if (!IsQuestReady())
			{
				return false;
			}
			var inv = Player.m_localPlayer.GetInventory();
			string iname = Tweakers.GetItemData(m_item).m_shared.m_name;
			int count = Tweakers.GetItemData(m_item).m_shared.m_maxStackSize;
			if (inv.CountItems(iname)>=count)
			{
				inv.RemoveItem(iname,count);
				OdinData.AddCredits(30,true);
				Say("$op_human_thx");
				ResetQuestCD();
				return true;
			}
			else
			{
				Say("$op_human_noteought");
				return true;
			}
		}
	}
}
