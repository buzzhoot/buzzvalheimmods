using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
namespace OdinPlus
{
	public class TaskManager : MonoBehaviour
	{
		#region  var
		#region Data
		public enum TaskType { Treasure = 1 , Hunt = 2, Dungeon= 3, Search =4  };
		private static string[] RefKeys = { "defeated_eikthyr", "defeated_gdking", "defeated_bonemass", "defeated_moder", "defeated_goblinking" };
		public const int MaxLevel = 3;
		public static List<ClientTaskData> MyTasks = new List<ClientTaskData>();
		public static TaskManager instance;
		#endregion Data
		#region Out
		public static int GameKey;
		public static bool isMain = false;
		#endregion Out
		#region in
		public static GameObject Root;
		public static int Level = 1;
		#endregion interal
		#region Internal
		private static TaskType tempType;
		#endregion Internal
		#endregion  var

		#region Mono

		private void Awake()
		{
			instance = this;
			Root = new GameObject("TaskRoot");
			Root.transform.SetParent(OdinPlus.Root.transform);

			//-? reg rpc
			ReigsterRpc();
		}
		private void ReigsterRpc()
		{
			if (ZNet.instance.IsServer())
			{
				ZRoutedRpc.instance.Register<int, int, bool>("RPC_ServerCreateTask", new Action<long, int, int, bool>(RPC_ServerCreateTask));
				ZRoutedRpc.instance.Register<string>("RPC_ServerGiveup", new Action<long, string>(RPC_ServerGiveup));
			}
			ZRoutedRpc.instance.Register<string, string, Vector3>("RPC_CreateTaskSucced", new Action<long, string, string, Vector3>(RPC_CreateTaskSucced));
			ZRoutedRpc.instance.Register<int, string>("RPC_CreateTaskFailed", new Action<long, int, string>(RPC_CreateTaskFailed));
			ZRoutedRpc.instance.Register<string>("RPC_ClientFinish", new Action<long, string>(RPC_ClientFinish));
		}
		#endregion Mono

		#region Tool
		public static bool HasTask()
		{
			return !(MyTasks.Count == 0);
		}
		public static int Count()
		{
			return MyTasks.Count;
		}
		public static int CheckKey()
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
		public static void PrintTaskList()
		{
			string n = "";
			foreach (var task in MyTasks)
			{
				n += task.PrintData();
			}
			Tweakers.TaskTopicHugin("Quest List", n);
		}

		#endregion Tool

		#region Feature
		public static void CreateRandomTask()
		{
			UnityEngine.Random.InitState((int)Time.time);
			TaskType[] a = new TaskType[] { TaskType.Treasure };
			switch (CheckKey())
			{
				case 0:
					a = new TaskType[] { TaskType.Treasure, TaskType.Treasure };
					break;
				case 1:
					a = new TaskType[] { TaskType.Treasure, TaskType.Dungeon };
					break;
				case 2:
					a = new TaskType[] { TaskType.Treasure, TaskType.Dungeon, TaskType.Hunt, TaskType.Dungeon, TaskType.Dungeon };
					break;
				case 3:
					a = new TaskType[] { TaskType.Treasure, TaskType.Dungeon, TaskType.Hunt };
					break;
				case 4:
					a = new TaskType[] { TaskType.Treasure, TaskType.Dungeon, TaskType.Hunt };
					break;
				case 5:
					a = new TaskType[] { TaskType.Treasure, TaskType.Dungeon, TaskType.Hunt };
					break;
			}
			int l = a.Length;
			//if (1f.RollDice() < 0.1)
			//{
			//CreateTask(TaskType.Search);
			//return;
			//}
			instance.CreateTask(a[l.RollDice()]);
		}
		public void CreateTask(TaskType t)
		{
			tempType = t;
			ZRoutedRpc.instance.InvokeRoutedRPC("RPC_ServerCreateTask", new object[] {(int)t,Level, isMain});
		}
		public void RPC_CreateTaskSucced(long sender, string id, string lname, Vector3 pos)
		{
			var task = new ClientTaskData();
			task.Id = id;
			task.locName = lname;
			task.m_type = tempType;
			task.Begin(pos);
			DBG.blogWarning(string.Format("Client :Create Task {0} {1} at {2}", id, lname, pos));
		}
		public void RPC_CreateTaskFailed(long sender, int t, string lname)
		{
			DBG.InfoCT("Try Agian,report " + lname + "  " + t + "   to Buzz,thx");
			DBG.blogError(string.Format("Cannot Place Task :  {0} {1}", t, lname));
		}
		public static void RPC_ServerCreateTask(long sender, int t, int lvl, bool main)
		{
			TaskType ttp = (TaskType)t;
			var go = new GameObject("Task");
			go.transform.SetParent(Root.transform);
			go.SetActive(false);
			switch (ttp)
			{
				case TaskType.Treasure:
					go.AddComponent<TreasureTask>();
					break;
				case TaskType.Hunt:
					go.AddComponent<HuntTask>();
					break;
				case TaskType.Dungeon:
					go.AddComponent<DungeonTask>();
					break;
				case TaskType.Search:
					//go.AddComponent<SearchTask>();
					break;
			}
			var a = go.GetComponent<OdinTask>();//?
			a.SetOwner(sender);
			a.Level = lvl;
			a.isMain = main;
			go.SetActive(true);
		}
		public static bool GiveUpTask(int ind)
		{
			foreach (var task in MyTasks)
			{
				if (task.m_index == ind)
				{
					task.Giveup();
					DBG.blogInfo("Client give up task" + ind);
					return true;
				}
			}
			return false;
		}
		public void RPC_ServerGiveup(long sender, string ID)
		{
			foreach (var task in Root.GetComponentsInChildren<OdinTask>())
			{
				if (task.Id == ID)
				{
					task.Giveup();
					DBG.blogInfo("Server giveup task ID:" + ID);
					return;
				}
			}
			DBG.blogError("Giveup : Server Cant Find task ID:" + ID);
			return;
		}
		public void RPC_ClientFinish(long sender, string ID)
		{
			foreach (var task in MyTasks)
			{
				if (task.Id == ID)
				{
					task.Finish();
					return;
				}
			}
			DBG.blogError("Client Can't find task" + ID);
		}
		public static void Clear()
		{
			if (ZNet.instance.IsServer())
			{
				ServerClear();
			}
			ClientClear();

		}
		public static void ServerClear()
		{
			foreach (Transform t in Root.GetComponentsInChildren<Transform>())
			{
				if (t.gameObject != Root)
				{
					Destroy(t.gameObject);
				}
			}
			DBG.blogWarning("Server task data cleared");
		}
		public static void ClientClear()
		{
			MyTasks.Clear();
			DBG.blogWarning("Client task data cleared");
		}
		#endregion Feature

		#region internalTool
		#endregion internalTool

		#region save&Load

		public static void ClientSave()
		{
			OdinData.Data.ClientTaskDatas = MyTasks;
			DBG.blogWarning("Client saved tasks");
		}
		public static void ClientLoad()
		{
			MyTasks = OdinData.Data.ClientTaskDatas;

			//Check if task stoled
			DBG.blogWarning("Client loaed tasks");
		}
		public static List<OdinData.TaskDataTable> Save()
		{
			if (Root.transform.childCount == 0)
			{
				DBG.blogWarning("Sever Save:Task is null");
				return null;
			}
			var data = new List<OdinData.TaskDataTable>();
			int i = 0;
			foreach (var item in Root.GetComponentsInChildren<OdinTask>())
			{
				data.Add(item.Save());
				i++;
			}
			DBG.blogWarning("Server Task saved:" + i);
			return data;
		}
		public static void Load(List<OdinData.TaskDataTable> data)
		{
			if (data == null)
			{
				DBG.blogWarning("Server Load:Task is null");
				return;
			}
			Root.SetActive(false);
			int i = 0;
			int e = 0;
			foreach (var item in data)
			{
				var go = new GameObject("Task" + item.Id);
				go.transform.parent = Root.transform;
				OdinTask component = null;
				switch (item.m_type)
				{
					case TaskType.Treasure:
						component = go.AddComponent<TreasureTask>();
						break;
					case TaskType.Hunt:
						component = go.AddComponent<HuntTask>();
						break;
					case TaskType.Dungeon:
						component = go.AddComponent<DungeonTask>();
						break;
					case TaskType.Search:
						component = go.AddComponent<SearchTask>();
						break;
				}
				if (component.Load(item))
				{
					i++;
				}
				else
				{
					e++;
				}
			}
			if (e != 0)
			{
				var s = e + " Quest Load failed";
				DBG.blogWarning(s);
			}
			Root.SetActive(true);
			DBG.blogWarning("Server Loaded Task Count: " + i);
		}
		#endregion save&Load

		#region Client
		[Serializable]
		public class ClientTaskData
		{
			public string locName = "";
			public string Id = "0_0";
			public TaskManager.TaskType m_type;
			public string HintTarget;//-?
			public string HintStart;//-?
			public string taskName;
			public int m_index;
			public float m_positionX = 0f;
			public float m_positionY = 0f;
			public float m_positionZ = 0f;
			public float m_range;

			private void SetPin()
			{
				Minimap.instance.DiscoverLocation(new Vector3(m_positionX, m_positionY, m_positionZ), Minimap.PinType.Icon3, (isMain ? "Main" : "Side") + "Quest " + m_index + " : " + taskName);
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
				Tweakers.TaskHintHugin((isMain ? "Main" : "Side") + "Quest " + m_index + " : " + taskName, HintTarget);
			}
			public TaskManager.TaskType GetTaskType()
			{
				return m_type;
			}
			public string PrintData()
			{
				string n = "\n" + (isMain ? "Main" : "Side");
				n += String.Format(" Quest [<color=yellow><b>{0}</b></color>] : {1}", m_index, taskName);
				return n;
			}
			private void SetHintStart()
			{
				switch (GetTaskType())
				{
					case TaskType.Hunt:
						HintStart = String.Format("There is a <color=yellow><b>[{0}]</b></color> near the location i marked for you,check your map ...", locName);
						break;
					case TaskType.Treasure:
						HintStart = String.Format("There a chest burried under a  <color=yellow><b>[{0}]</b></color> near the location i marked for you,check your map ...", locName);
						break;
					case TaskType.Dungeon:
						HintStart = String.Format("There a chest in the dungeon <color=yellow><b>[{0}]</b></color> near the location i marked for you,check your map ...", locName);
						break;
				}

			}
			private void SetHintTarget()
			{
				switch (GetTaskType())
				{
					case TaskType.Hunt:
						HintTarget = string.Format("Looks like you are close to the <color=yellow><b>[{0}]</b></color> Watchout!", locName);
						break;
					case TaskType.Treasure:
						HintTarget = string.Format("Looks like you are close to the chest,look around find a <color=yellow><b>[{0}]</b></color>", locName);
						break;
					case TaskType.Dungeon:
						HintTarget = string.Format("Looks like you are close to the dungeon,look around find a <color=yellow><b>[{0}]</b></color>", locName);
						break;
				}
			}
			public void Begin(Vector3 pos)
			{
				m_index = MyTasks.Count;
				SetLocName();
				SetTaskName();
				SetHintStart();
				SetRange(30.RollDice(30 + Level * 30));
				SetPosition(pos);
				SetPin();
				MessageHud.instance.ShowBiomeFoundMsg((isMain ? "Main" : "Side") + " Quest " + m_index + "\n" + taskName + "\nStart", true);
				Tweakers.TaskHintHugin((isMain ? "Main" : "Side") + "Quest " + m_index + " : " + taskName, HintStart);

			}
			public void SearchBegin()
			{
				MessageHud.instance.ShowBiomeFoundMsg((isMain ? "Main" : "Side") + " Quest " + m_index + "\n" + taskName + "\nStart", true);
				Tweakers.TaskHintHugin((isMain ? "Main" : "Side") + "Quest " + m_index + " : " + taskName, HintStart);
			}
			public void Discovered()
			{
				SetHintTarget();
				Tweakers.TaskHintHugin((isMain ? "Main" : "Side") + "Quest " + m_index + " : " + taskName, HintTarget);
			}
			public void Finish()
			{
				RemovePin();
				OdinMunin.ResetTimer();
				Clear();
			}
			public void Clear()
			{
				string result = "Stolen";
				if (isMeInsideTaskArea())
				{
					result = "Clear";
				}
				MessageHud.instance.ShowBiomeFoundMsg((isMain ? "Main" : "Side") + "Quest " + m_index + "\n" + taskName + "\n" + result, true);
				MyTasks.Remove(this);
			}
			private bool isMeInsideTaskArea()
			{
				Vector3 ppos = Player.m_localPlayer.transform.position;
				return Tweakers.isInsideArea(ppos, new Vector3(m_positionX, m_positionY, m_positionZ), 30);
			}
			public void Giveup()
			{
				DBG.blogWarning("Client Giveup Task");
				ZRoutedRpc.instance.InvokeRoutedRPC("RPC_ServerGiveup", new object[] { Id });
				this.Clear();
			}
		}
		public void CheckDiscover()
		{

		}
		#endregion Client

	}
}