using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OdinPlus
{
    public enum TaskType { Treasure = 1, Hunt = 2, Dungeon = 3, Search = 4 }; 
	[Serializable]
	public class Quest
	{
		public string locName = "";
		public string Id = "0_0";
		public TaskType m_type;
		public string HintTarget;
		public string HintStart;
		public string taskName;
		public int m_index;
		public bool isMain;
		public float m_positionX = 0f;
		public float m_positionY = 0f;
		public float m_positionZ = 0f;
		public float m_range;
        public int Level;
		private void SetPin()
		{
			Minimap.instance.DiscoverLocation(new Vector3(m_positionX, m_positionY, m_positionZ), Minimap.PinType.Icon3, (isMain ? "Main" : " $op_task_side ") + " $op_task_quest " + m_index + " : " + taskName);
		}
		protected virtual void SetLocName()
		{
			locName = Regex.Replace(locName, @"[\d-]", string.Empty);
			locName = Regex.Replace(locName, @"[_]", "");
		}
		private void SetTaskName()
		{
			taskName = locName + " " + m_type.ToString();
		}
		private void SetPosition(Vector3 pos)
		{
			Vector3 m_position = pos;
			m_position = m_position.GetRandomLocation(m_range);
			m_positionX = m_position.x;
			m_positionY = m_position.y;
			m_positionZ = m_position.z;
		}
		public void SetRange(int range)
		{
			m_range = range.RollDice();
		}
		private void RemovePin()
		{
			Minimap.instance.RemovePin(new Vector3(m_positionX, m_positionY, m_positionZ), 10);
		}
		public void SendPing()
		{
			Chat.instance.SendPing(new Vector3(m_positionX, m_positionY, m_positionZ));
		}
		private void Discovery()
		{
			Tweakers.TaskHintHugin((isMain ? "Main" : "$op_task_side") + "$op_task_quest " + m_index + " : " + taskName, HintTarget);
		}
		public TaskType GetTaskType()
		{
			return m_type;
		}
		public string PrintData()
		{
			string n = "\n" + (isMain ? "$op_task_main" : " $op_task_side ");
			n += String.Format(" $op_task_quest [<color=yellow><b>{0}</b></color>] : {1}", m_index, taskName);
			return n;
		}
		private void SetHintStart()
		{
			switch (GetTaskType())
			{
				case TaskType.Hunt:
					//HintStart = String.Format("There is a <color=yellow><b>[{0}]</b></color> near the location i marked for you,check your map ...", locName);
					HintStart = String.Format("$op_task_hunt_start_pr_1 <color=yellow><b>[{0}]</b></color> $op_task_hunt_start_po_1 ", locName);
					break;
				case TaskType.Treasure:
					//HintStart = String.Format("There a chest burried under a  <color=yellow><b>[{0}]</b></color> near the location i marked for you,check your map ...", locName);
					HintStart = String.Format("$op_task_treasure_start_pr_1 <color=yellow><b>[{0}]</b></color> $op_task_treasure_start_po_1 ", locName);
					break;
				case TaskType.Dungeon:
					//HintStart = String.Format("There a chest in the dungeon <color=yellow><b>[{0}]</b></color> near the location i marked for you,check your map ...", locName);
					HintStart = String.Format("$op_task_dungeon_start_pr_1 <color=yellow><b>[{0}]</b></color> $op_task_dungeon_start_po_1 ", locName);
					break;
			}

		}
		private void SetHintTarget()
		{
			switch (GetTaskType())
			{
				case TaskType.Hunt:
					//HintTarget = string.Format("Looks like you are close to the <color=yellow><b>[{0}]</b></color> Watchout!", locName);
					HintStart = String.Format("$op_task_hunt_target_pr_1 <color=yellow><b>[{0}]</b></color> $op_task_hunt_target_po_1 ", locName);
					break;
				case TaskType.Treasure:
					//HintTarget = string.Format("Looks like you are close to the chest,look around find a <color=yellow><b>[{0}]</b></color>", locName);
					HintStart = String.Format("$op_task_treasure_target_pr_1 <color=yellow><b>[{0}]</b></color> $op_task_treasure_target_po_1 ", locName);
					break;
				case TaskType.Dungeon:
					//HintTarget = string.Format("Looks like you are close to the dungeon,look around find a <color=yellow><b>[{0}]</b></color>", locName);
					HintStart = String.Format("$op_task_dungeon_target_pr_1 <color=yellow><b>[{0}]</b></color> $op_task_dungeon_target_po_1 ", locName);
					break;
			}
		}
		public void Begin(Vector3 pos)
		{

			OdinData.Data.TaskCount++;
			m_index = OdinData.Data.TaskCount;
			SetLocName();
			SetTaskName();
			SetHintStart();
			SetRange(30.RollDice(30 + Level * 30));
			SetPosition(pos);
			SetPin();
			MessageHud.instance.ShowBiomeFoundMsg((isMain ? "Main" : " $op_task_side ") + " $op_task_quest " + m_index + "\n" + taskName + "\n $op_task_start", true);
			Tweakers.TaskHintHugin((isMain ? "Main" : " $op_task_side ") + " $op_task_quest " + m_index + " : " + taskName, HintStart);
			//+UpdateTaskList();
		}
		public void SearchBegin()
		{
			MessageHud.instance.ShowBiomeFoundMsg((isMain ? "Main" : " $op_task_side ") + " $op_task_quest " + m_index + "\n" + taskName + "\n $op_task_start", true);
			Tweakers.TaskHintHugin((isMain ? "Main" : " $op_task_side ") + "$op_task_quest " + m_index + " : " + taskName, HintStart);
		}
		public void Discovered()
		{
			SetHintTarget();
			Tweakers.TaskHintHugin((isMain ? "Main" : "$op_task_side ") + " $op_task_quest " + m_index + " : " + taskName, HintTarget);
		}
		public void Finish()
		{
			RemovePin();
			OdinMunin.ResetTimer();
			//+UpdateTaskList();
			Clear();
		}
		public void Clear()
		{
			string result = "$op_task_stolen";
			if (isMeInsideTaskArea() || ZNet.instance.IsLocalInstance())
			{
				result = "$op_task_clear";
			}
			MessageHud.instance.ShowBiomeFoundMsg((isMain ? "Main" : " $op_task_side ") + " $op_task_quest " + m_index + "\n" + taskName + "\n" + result, true);
			//+MyTasks.Remove(this);
		}
		private bool isMeInsideTaskArea()
		{
			Vector3 ppos = Player.m_localPlayer.transform.position;
			return Tweakers.isInsideArea(ppos, new Vector3(m_positionX, ppos.y, m_positionZ), 200);
		}
		public void Giveup()
		{
			DBG.blogWarning("Client Giveup Task");
			RemovePin();
			ZRoutedRpc.instance.InvokeRoutedRPC("RPC_ServerGiveup", new object[] { Id });
			this.Clear();
		}

	}
}
