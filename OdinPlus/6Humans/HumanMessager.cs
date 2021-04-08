using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace OdinPlus
{
	public class HumanMessager : HumanVillager, Hoverable, Interactable, OdinInteractable
	{

		protected override void Awake()
		{
			base.Awake();
			ChoiceList = new string[2] { "$op_talk", "$op_human_quest_take" };
			var zdo = m_nview.GetZDO();
			zdo.GetFloat("QuestTimer", 0);
		}
		public override void Choice0()
		{
			Say("I need some help");//trans
		}
		public void Choice1()
		{
			var key = m_nview.GetZDO().GetString("npcname");
			OdinData.AddKey(key);
			PlaceQuestHuman(key);
			Say("Thx, you can find him near this location");
		}
		private void PlaceQuestHuman(string key)
		{
			//add
		}

	}
}
