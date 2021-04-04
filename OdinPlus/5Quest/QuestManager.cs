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
		private static readonly string[] RefKeys = { "defeated_eikthyr", "defeated_gdking", "defeated_bonemass", "defeated_moder", "defeated_goblinking" };
		public static readonly int MaxLevel = 3;
		#endregion CFG

		#region In
		public bool isMain = false;
		public int Level = 1;
		public int GameKey = 0;
		#endregion In

		#region interal
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
			MyQuests = new Dictionary<string, Quest>();
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
			DBG.InfoCT("Try Agian,the dice is broken");
			DBG.blogError(string.Format("Cannot Place Quest :  {0}", WaitQuest.locName, WaitQuest.m_type));
			WaitQuest = null;
		}

		#endregion Rpc

		#region Feature
		public void CreateMuninQuest()
		{

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
		public void CreateRandomQuest()
		{

		}
		public Quest CreatQuest(QuestType type)
		{
			WaitQuest = new Quest();
			WaitQuest.m_type = type;
			WaitQuest.Key = GameKey;
			//upd ismain?
			//hack LEVEL
			SelectProcesser();
			questProcesser.Init();
			return WaitQuest;
		}
		private void SelectProcesser()
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
		public int CheckKey()
		{
			int result = 0;
			var keys = ZoneSystem.instance.GetGlobalKeys();
			foreach (var item in RefKeys)
			{
				if (keys.Contains(item)) { result += 1; }
			}
			GameKey = result;
			return result;
		}
		public bool HasQuest()
		{
			return !(MyQuests.Count == 0);
		}
		public int Count()
		{
			if (MyQuests == null)
			{
				return 0;
			}
			return MyQuests.Count;
		}
		public void PrintQuestList()
		{
			string n = "";
			foreach (var quest in MyQuests.Values)
			{
				n += quest.PrintData();
			}
			Tweakers.QuestTopicHugin("Quest List", n);
		}
		public void UpdateQuestList()
		{
			string n = "";
			foreach (var quest in MyQuests.Values)
			{
				n += quest.PrintData();
			}
			Tweakers.addHints(n);
		}
		public bool GiveUpQuest(int ind)
		{
			foreach (var quest in MyQuests.Values)
			{
				if (quest.m_index == ind)
				{
					quest.Giveup();
					DBG.blogInfo("Client give up quest" + ind);
					return true;
				}
			}
			return false;
		}
		#endregion Tool
	}
}
