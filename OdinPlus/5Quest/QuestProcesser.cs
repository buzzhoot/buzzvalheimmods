using System;
using UnityEngine;
namespace OdinPlus
{
	public class QuestProcesser
	{
		protected Quest quest;
		public static QuestProcesser Create(Quest inq)
		{
			switch (inq.m_type)
			{
				case QuestType.Dungeon:
					return new DungeonQuestProcesser(inq);
				case QuestType.Treasure:
					return new TreasureQuestProcesser(inq);
				case QuestType.Hunt:
					return new HuntQuestProcesser(inq);
				case QuestType.Search:
					return new SearchQuestProcesser(inq);
				default:
					return new QuestProcesser(inq);
			}
		}
		public void SetQuest(Quest inq)
		{
			quest = inq;
		}
		public QuestProcesser(Quest inq)
		{
			quest = inq;
		}
		public QuestProcesser() { }
		public virtual void Init()
		{
			var list1 = QuestRef.LocDic[quest.GetQuestType()];
			var list2 = list1[quest.Key];
			quest.locName = list2.GetRandomElement();
			QuestManager.instance.Invoke("ShowWaitError", 10);
			ZRoutedRpc.instance.InvokeRoutedRPC("RPC_ServerFindLocation", new object[] { quest.locName, quest.m_realPostion });
		}
		public virtual void Begin()
		{
			QuestManager.instance.MyQuests.Add(quest.ID, quest);
			quest.m_ownerName = Player.m_localPlayer.GetPlayerName();
			quest.Begin();
		}
		public virtual void Place(LocationMarker lm)
		{
			lm.Used();
		}
		public virtual void Finish()
		{
			quest.Finish();
		}

	}
}