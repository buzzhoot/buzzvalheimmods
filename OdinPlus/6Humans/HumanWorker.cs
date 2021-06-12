using System.Threading;
using System;

namespace OdinPlus
{
	public class HumanWorker : HumanVillager, Hoverable, Interactable, OdinInteractable
	{
		protected override void Awake()
		{
			base.Awake();
			ChoiceList = new string[2] { "$op_talk", "$op_human_message_hand" };
		}
		public override void Choice0()
		{
			Say("you got something for me?");//trans
		}
		public void Choice1()
		{
			if (OdinData.GetKey(m_nview.GetZDO().GetString("npcname")))
			{
				Say("Thx!");
                OdinData.AddCredits(10,true);
                return;
			}
			else
			{
                Say("you got something for me?");
                return;
			}
		}
	}
}
