using System;
using UnityEngine;
namespace OdinPlus
{
	public class QuestProcesser
	{
		protected Quest quest;
		public static QuestProcesser Create(Quest inq)
		{
			return new QuestProcesser(inq);
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

		}
		public virtual void Begin() { }

	}
}