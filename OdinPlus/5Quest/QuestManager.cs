using System.Collections.Generic;
using System;
using UnityEngine;
namespace OdinPlus
{
	public class QuestManager : MonoBehaviour
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
			CheckPlace();
		}
		private void CheckPlace()
		{
			var lmList = LocationMarker.MarkList;
			foreach (var item in MyQuests.Keys)
			{
				if (lmList.ContainsKey(item))
				{
					var lm = lmList[item];
					var quest = MyQuests[item];
					SelectProcesser(quest);
					questProcesser.Place(lm);
					return;
					//HELP Do i need yield return? but will Lead to a new questprocser(Multi-thread)
				}
			}
		}
		public void Clear()
		{
			MyQuests.Clear();
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
			CancelWaitError();
			var quest = WaitQuest;
			quest.ID = id;
			quest.m_realPostion = pos;
			questProcesser.Init();
			DBG.blogWarning(string.Format("Client :Create Quest {0} {1} at {2}", id, quest.locName, pos));
		}
		public void RPC_CreateQuestFailed(long sender)
		{
			CancelWaitError();
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
			UnityEngine.Random.InitState((int)Time.time);
			QuestType[] a = new QuestType[] { QuestType.Treasure };
			switch (CheckKey())
			{
				case 0:
					a = new QuestType[] { QuestType.Search, QuestType.Treasure };
					break;
				case 1:
					a = new QuestType[] { QuestType.Treasure, QuestType.Dungeon, QuestType.Search };
					break;
				case 2:
					a = new QuestType[] { QuestType.Treasure, QuestType.Hunt, QuestType.Dungeon, QuestType.Search };
					break;
				case 3:
					a = new QuestType[] { QuestType.Treasure, QuestType.Dungeon, QuestType.Hunt, QuestType.Search };
					break;
				case 4:
					a = new QuestType[] { QuestType.Treasure, QuestType.Dungeon, QuestType.Hunt, QuestType.Search };
					break;
				case 5:
					a = new QuestType[] { QuestType.Treasure, QuestType.Dungeon, QuestType.Hunt, QuestType.Search };
					break;
			}
			int l = a.Length;
			if (1f.RollDice() < 0.1)
			{
				CreateQuest(QuestType.Search);
				return;
			}
			DBG.blogWarning("Dice Rolled");
			instance.CreateQuest(a[l.RollDice()]);
		}
		public Quest CreateQuest(QuestType type)
		{
			return CreateQuest(type,Game.instance.GetPlayerProfile().GetCustomSpawnPoint());
		}

		public Quest CreateQuest(QuestType type, Vector3 pos)
		{
			if (MyQuests == null)
			{
				MyQuests = new Dictionary<string, Quest>();
			}
			//upd multiple overloads
			WaitQuest = new Quest();
			WaitQuest.m_type = type;
			GameKey = CheckKey();
			WaitQuest.Key = GameKey;
			WaitQuest.m_realPostion = pos;
			//upd ismain?
			//hack LEVEL
			SelectProcesser(WaitQuest);
			questProcesser.Init();
			return WaitQuest;
		}
		public bool GiveUpQuest(int ind)
		{
			foreach (var quest in MyQuests.Values)
			{
				if (quest.m_index == ind)
				{
					quest.Giveup();
					MyQuests.Remove(quest.ID);
					UpdateQuestList();
					DBG.blogInfo("Client give up quest" + ind);
					return true;
				}
			}
			return false;
		}
		public Quest GetQuest(string p_id)
		{
			foreach (var id in MyQuests.Keys)
			{
				if (id == p_id)
				{
					return MyQuests[id];
				}
			}
			return null;
		}

		#endregion Feature

		#region Tool
		private void SelectProcesser(Quest quest)
		{
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
		private void ShowWaitError()
		{
			DBG.InfoCT("There maybe something wrong with the server,please try again later");
		}
		public void CancelWaitError()
		{
			CancelInvoke("ShowWaitError");
		}
		#endregion Tool

		#region SaveLoad
		public void Save()
		{
			var data = OdinData.Data.Quests;
			data = new List<Quest>();
			foreach (var quest in MyQuests.Values)
			{
				data.Add(quest);
			}
		}
		public void Load()
		{
			foreach (var quest in OdinData.Data.Quests)
			{
				MyQuests.Add(quest.ID, quest);
			}
		}
		#endregion SaveLoad
	}
}
