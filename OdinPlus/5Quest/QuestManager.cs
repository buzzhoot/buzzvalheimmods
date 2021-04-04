using System.Collections.Generic;
using System;
using UnityEngine;
namespace OdinPlus
{
	public class QuestManager
	{

		#region Variable

		#region Data
		public Dictionary<string, Quest> MyQuests = new Dictionary<string, Quest>();
		private Quest WaitQuest = null;

		#endregion Data
		#region CFG
		public const int MaxLevel = 3;
		#endregion CFG

		#region interal
		public static GameObject Root;
		public static int Level = 1;
		public static QuestManager instance;
		QuestProcesser questProcesser;
		#endregion interal

		#endregion Variable

		#region Main
		private void Awake()
		{
			instance = this;
			MyQuests = new Dictionary<string, Quest>();
			Plugin.RegRPC = (Action)Delegate.Combine(Plugin.RegRPC, (Action)ReigsterRpc);
		}
		private void Update()
		{

		}

		#endregion Main

		#region Rpc
		public void ReigsterRpc()
		{

			ZRoutedRpc.instance.Register<string, Vector3>("RPC_CreateQuestSucced", new Action<long, string, Vector3>(RPC_CreateQuestSucced));
			ZRoutedRpc.instance.Register("RPC_CreateQuestFailed", new Action<long>(RPC_CreateQuestFailed));
			DBG.blogWarning("QuestManager rpc reged");
		}
		public void RPC_CreateQuestSucced(long sender, string id, Vector3 pos)
		{
			var quest = WaitQuest;
			quest.ID = id;
			quest.m_realPostion = pos;
			questProcesser.Begin();
			DBG.blogWarning(string.Format("Client :Create Quest {0} {1} at {2}", id, quest.locName, pos));
		}
		public void RPC_CreateQuestFailed(long sender)
		{
			DBG.InfoCT("Try Agian,the dice may run out");
			DBG.blogError(string.Format("Cannot Place Quest :  {0}", WaitQuest.locName, WaitQuest.m_type));
			WaitQuest = null;
		}

		#endregion Rpc

		#region Feature
		public void CreateMuninQuest()
		{

		}
		public bool CreateLocQuest(string lname, QuestType type, Vector3 pos)
		{
			if (CanCreateQuest())
			{
				WaitQuest = new Quest();
				WaitQuest.locName = lname;
				WaitQuest.m_type = type;
				//XXXZRoutedRpc.instance.InvokeRoutedRPC("RPC_ServerFindLocation", new object[] { lname, pos });
				return true;
			}
			return false;
		}
		public bool CanCreateQuest()
		{
			if (WaitQuest != null)
			{
				DBG.InfoCT("$op_quest_failed_wait");
				return false;
			}
			return true;
		}
		public void CreateRandomTask()
		{

		}
		public void CreatTask(QuestType type)
		{
			WaitQuest = new Quest();
			WaitQuest.m_type = type;
			//ZRoutedRpc.instance.InvokeRoutedRPC("RPC_ServerFindLocation", new object[] { lname, pos });
		}
		private void Catalog()
		{
			var quest = WaitQuest;
			switch (quest.m_type)
			{
				case QuestType.Dungeon:
					questProcesser = DungeonQuestProcesser.Create(quest);
					break;
				case QuestType.Treasure:
					questProcesser = TreasureQuestProcesser.Create(quest);
					break;
				case QuestType.Hunt:
					questProcesser = HuntQuestProcesser.Create(quest);
					break;
				case QuestType.Search:
					questProcesser = SearchQuestProcesser.Create(quest);
					break;
			}
		}
		#endregion Feature
		#region Tool

		#endregion Tool
	}
}
