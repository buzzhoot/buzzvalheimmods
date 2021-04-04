using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OdinPlus
{
	public enum QuestType { Treasure = 1, Hunt = 2, Dungeon = 3, Search = 4 };
	[Serializable]
	public class Quest
	{
		#region Varable

		#region Data
		public string locName = "";
		public string ID = "0_0";
		public QuestType m_type;
		public Vector3 m_realPostion;
		public bool hasPIN = false;
		public Vector3 m_pinPosition;
		public float m_range;

		#endregion Data

		#region Message
		public string HintTarget = "";
		public string HintStart = "";
		public string m_message = "";
		#endregion Message

		#region in
		public string QuestName;
		public int m_index;
		public bool isMain;
		public int Level;
		public int Key;
		#endregion in

		#endregion Varable
		private void SetPin()
		{
			Minimap.instance.DiscoverLocation(m_pinPosition, Minimap.PinType.Icon3, m_message);
		}
		public void SetLocName()
		{
			locName = Regex.Replace(locName, @"[\d-]", string.Empty);
			locName = Regex.Replace(locName, @"[_]", "");
		}
		private void SetQuestName()
		{
			QuestName = locName + " " + m_type.ToString();
		}
		private void SetPosition()
		{
			m_pinPosition = m_realPostion.GetRandomLocation(m_range);
		}
		public void SetRange(int range)
		{
			m_range = range.RollDice();
		}
		private void RemovePin()
		{
			Minimap.instance.RemovePin(m_pinPosition, 10);
		}
		public void SendPing()
		{
			Chat.instance.SendPing(m_pinPosition);
		}
		private void Discovery()
		{
			Tweakers.QuestHintHugin((isMain ? "Main" : "$op_quest_side") + "$op_quest_quest " + m_index + " : " + QuestName, HintTarget);
		}
		public QuestType GetQuestType()
		{
			return m_type;
		}
		public string PrintData()
		{
			string n = "\n" + (isMain ? "$op_quest_main" : " $op_quest_side ");
			n += String.Format(" $op_quest_quest [<color=yellow><b>{0}</b></color>] : {1}", m_index, QuestName);
			return n;
		}
		private void SetHintStart()
		{
			switch (GetQuestType())
			{
				case QuestType.Hunt:
					//HintStart = String.Format("There is a <color=yellow><b>[{0}]</b></color> near the location i marked for you,check your map ...", locName);
					HintStart = String.Format("$op_quest_hunt_start_pr_1 <color=yellow><b>[{0}]</b></color> $op_quest_hunt_start_po_1 ", locName);
					break;
				case QuestType.Treasure:
					//HintStart = String.Format("There a chest burried under a  <color=yellow><b>[{0}]</b></color> near the location i marked for you,check your map ...", locName);
					HintStart = String.Format("$op_quest_treasure_start_pr_1 <color=yellow><b>[{0}]</b></color> $op_quest_treasure_start_po_1 ", locName);
					break;
				case QuestType.Dungeon:
					//HintStart = String.Format("There a chest in the dungeon <color=yellow><b>[{0}]</b></color> near the location i marked for you,check your map ...", locName);
					HintStart = String.Format("$op_quest_dungeon_start_pr_1 <color=yellow><b>[{0}]</b></color> $op_quest_dungeon_start_po_1 ", locName);
					break;
			}

		}
		private void SetHintTarget()
		{
			switch (GetQuestType())
			{
				case QuestType.Hunt:
					//HintTarget = string.Format("Looks like you are close to the <color=yellow><b>[{0}]</b></color> Watchout!", locName);
					HintStart = String.Format("$op_quest_hunt_target_pr_1 <color=yellow><b>[{0}]</b></color> $op_quest_hunt_target_po_1 ", locName);
					break;
				case QuestType.Treasure:
					//HintTarget = string.Format("Looks like you are close to the chest,look around find a <color=yellow><b>[{0}]</b></color>", locName);
					HintStart = String.Format("$op_quest_treasure_target_pr_1 <color=yellow><b>[{0}]</b></color> $op_quest_treasure_target_po_1 ", locName);
					break;
				case QuestType.Dungeon:
					//HintTarget = string.Format("Looks like you are close to the dungeon,look around find a <color=yellow><b>[{0}]</b></color>", locName);
					HintStart = String.Format("$op_quest_dungeon_target_pr_1 <color=yellow><b>[{0}]</b></color> $op_quest_dungeon_target_po_1 ", locName);
					break;
			}
		}
		public void Begin()
		{

			OdinData.Data.QuestCount++;
			m_index = OdinData.Data.QuestCount;
			SetLocName();
			SetQuestName();
			SetHintStart();
			SetRange(30.RollDice(30 + Level * 30));
			SetPosition();
			SetPin();
			ShowMessage("start");
			Tweakers.QuestHintHugin((isMain ? "Main" : " $op_quest_side ") + " $op_quest_quest " + m_index + " : " + QuestName, HintStart);
			//+UpdateQuestList();
		}
		public void SearchBegin()
		{
			MessageHud.instance.ShowBiomeFoundMsg((isMain ? "Main" : " $op_quest_side ") + " $op_quest_quest " + m_index + "\n" + QuestName + "\n $op_quest_start", true);
			Tweakers.QuestHintHugin((isMain ? "Main" : " $op_quest_side ") + "$op_quest_quest " + m_index + " : " + QuestName, HintStart);
		}
		public void Discovered()
		{
			SetHintTarget();
			Tweakers.QuestHintHugin((isMain ? "Main" : "$op_quest_side ") + " $op_quest_quest " + m_index + " : " + QuestName, HintTarget);
		}
		public void Finish()
		{
			RemovePin();
			//+UpdateQuestList();
			Clear();
		}
		public void Clear()
		{
			string result = "$op_quest_stolen";
			if (isMeInsideQuestArea() || ZNet.instance.IsLocalInstance())
			{
				result = "$op_quest_clear";
			}
			MessageHud.instance.ShowBiomeFoundMsg((isMain ? "Main" : " $op_quest_side ") + " $op_quest_quest " + m_index + "\n" + QuestName + "\n" + result, true);
			QuestManager.instance.MyQuests.Remove(ID);
		}
		private bool isMeInsideQuestArea()
		{
			Vector3 ppos = Player.m_localPlayer.transform.position;
			return Tweakers.isInsideArea(ppos, new Vector3(m_realPostion.x, ppos.y, m_realPostion.z), 200);
		}
		public void Giveup()
		{
			DBG.blogWarning("Client Giveup Quest");
			RemovePin();
			//+ZRoutedRpc.instance.InvokeRoutedRPC("RPC_ServerGiveup", new object[] { ID });
			this.Clear();
		}
		public void ShowMessage(string result)
		{
			string n  = "$op_quest_clear"+result;
			MessageHud.instance.ShowBiomeFoundMsg(m_message+n,true);
		}
	}
}
