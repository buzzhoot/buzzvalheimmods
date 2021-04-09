using System;

namespace OdinPlus
{
	public class QuestVillager : HumanVillager
	{
		protected virtual void Start()
		{
			EXCobj.SetActive(IsQuestReady());
		}   
	}
}
