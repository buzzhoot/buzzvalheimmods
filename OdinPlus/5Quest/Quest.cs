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
		public string m_ownerName="";
		public QuestType m_type;
		public Vector3 m_realPostion;
		public bool hasPIN = false;//XXX
		public Vector3 m_pinPosition = Vector3.zero;
		public float m_range;
		public List<Quest> m_chain = new List<Quest>();

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

		#region Function
		//HELP is using extesion better than this?
		private void SetPin()
		{
			if (CheckPinNeed())
			{
				Minimap.instance.DiscoverLocation(m_pinPosition, Minimap.PinType.Icon3, m_message);
			}
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
			if (CheckPinNeed())
			{
				Minimap.instance.RemovePin(m_pinPosition, 10);
			}
		}
		public void SendPing()
		{
			Chat.instance.SendPing(m_pinPosition);
		}
		public QuestType GetQuestType()
		{
			return m_type;
		}
		public string PrintData()
		{
			if (m_message == "")
			{
				return "";
			}
			//string n = "\n" + (isMain ? "$op_quest_main" : " $op_quest_side ");
			//n += String.Format(" $op_quest_quest [<color=yellow><b>{0}</b></color>] : {1}", m_index, QuestName);
			return m_message;
		}
		public void ShowMessage(string result)
		{
			if (m_message == "")
			{
				return;
			}
			string n = "\n" + " $op_quest_" + result;
			MessageHud.instance.ShowBiomeFoundMsg(m_message + n, true);
		}
		public void ShowMuninMessage(string msg)
		{
			if (m_message == "" || msg == null)
			{
				return;
			}
			m_message.Replace('\n', ' ');
			Tweakers.QuestHintHugin(m_message, msg);
		}
		public bool CheckPinNeed()
		{
			if (m_pinPosition == Vector3.zero)
			{
				return false;
			}
			return true;
		}
		#endregion Function
		public void Begin()
		{
			OdinData.Data.QuestCount++;
			m_index = OdinData.Data.QuestCount;
			SetLocName();
			SetQuestName();
			SetRange(30.RollDice(30 + Level * 30));
			SetPosition();
			SetPin();
			ShowMessage("start");
			ShowMuninMessage(HintStart);
			QuestManager.instance.UpdateQuestList();
		}
		public void Discovered()
		{
			//HACK
			ShowMuninMessage(HintTarget);
		}
		public void Finish()
		{
			RemovePin();
			Clear();
			ShowMessage("clear");
			//HACK
			QuestManager.instance.MyQuests.Remove(ID);
			QuestManager.instance.UpdateQuestList();
		}
		public void Clear()
		{
			string result = "stolen";
			if (isMeInsideQuestArea() || ZNet.instance.IsLocalInstance())
			{
				result = "clear";
			}
			ShowMessage(result);
		}
		private bool isMeInsideQuestArea()
		{
			//OPT move to util                        
			Vector3 ppos = Player.m_localPlayer.transform.position;
			Vector2i val = ZoneSystem.instance.GetZone(ppos);
			return ID.ToV2I() == val;
		}
		public void Giveup()
		{
			RemovePin();
			ShowMessage("giveup");
		}
	}
}
