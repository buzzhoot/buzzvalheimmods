using System;
using System.Collections.Generic;
using UnityEngine;
namespace OdinPlus
{
	public class TaskManager : MonoBehaviour
	{
		#region  var
		#region Data
		public enum TaskType { Treasure, Hunt, Dungeon, Search };
		private static string[] RefKeys = { "defeated_eikthyr", "defeated_gdking", "defeated_bonemass", "defeated_moder" };
		public const int MaxLevel = 3;
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

		#endregion Internal
		#endregion  var

		#region Mono

		private void Awake()
		{
			Root = new GameObject("TaskRoot");
			Root.transform.SetParent(OdinPlus.Root.transform);
		}
		#endregion Mono

		#region Tool
		public static bool HasTask()
		{
			return !(Root.transform.childCount == 0);
		}
		public static int Count()
		{
			return Root.transform.childCount;
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
			foreach (var task in Root.GetComponentsInChildren<OdinTask>())
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
			//?int count = Enum.GetNames(typeof(TaskType)).Length;
			switch (CheckKey())
			{
				case 0:
					CreateTask(TaskType.Search);
					break;
				case 1:
					CreateTask(TaskType.Treasure);
					break;
				case 2:
					CreateTask(TaskType.Treasure);
					break;
				case 3:
					CreateTask(TaskType.Treasure);
					break;
				case 4:
					CreateTask(TaskType.Treasure);
					break;
			}
		}
		private static void CreateTask(TaskType t)
		{
			var go = new GameObject("Task");
			go.transform.SetParent(Root.transform);
			switch (t)
			{
				case TaskType.Treasure:
					go.AddComponent<TreasureTask>();
					break;
				case TaskType.Hunt:
					break;
				case TaskType.Dungeon:
					break;
				case TaskType.Search:
					go.AddComponent<SearchTask>();
					break;
			}
		}
		public static bool GiveUpTask(int ind)
		{
			foreach (var task in Root.GetComponentsInChildren<OdinTask>())
			{
				if (task.m_index == ind)
				{
					task.Giveup();
					return true;
				}
			}
			return false;
		}
		public static void Clear()
		{
			foreach (Transform t in Root.GetComponentsInChildren<Transform>())
			{
				if (t.gameObject != Root)
				{
					Destroy(t.gameObject);
				}

			}
		}
		#endregion Feature

		#region internalTool
		#endregion internalTool

		#region save&Load
		public static List<OdinData.TaskDataTable> Save()
		{
			if (Root.transform.childCount == 0)
			{
				DBG.blogInfo("Load:Task is null");
				return null;
			}
			var data = new List<OdinData.TaskDataTable>();
			int i = 0;
			foreach (var item in Root.GetComponentsInChildren<OdinTask>())
			{
				data.Add(item.Save());
				i++;
			}
			DBG.blogInfo("Task saved:" + i);
			return data;
		}
		public static void Load(List<OdinData.TaskDataTable> data)
		{
			if (data == null)
			{
				DBG.blogInfo("Load:Task is null");
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
						break;
					case TaskType.Dungeon:
						break;
					case TaskType.Search:
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
				DBG.InfoCT(s);
			}
			Root.SetActive(true);
			DBG.blogInfo("Loaded Task Count: " + i);

			#endregion save&Load
		}
	}
}