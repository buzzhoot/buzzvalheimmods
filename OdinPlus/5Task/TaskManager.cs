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
		private static string[] RefKeys = { "defeated_eikthyr", "defeated_gdking", "defeated_bonemass", "defeated_moder", "defeated_goblinking" };
		#endregion Data
		#region Out
		public static OdinTask CurrentTask;
		public static int GameKey;
		public static bool isMain;
		#endregion Out
		#region in
		public static GameObject Root;
		public static int Level;
		#endregion interal
		#region Internal
		private bool isPrefabed = false;
		private Transform PrefabRoot;
		#endregion Internal
		#endregion  var

		#region Mono

		private void Awake()
		{
			Root = new GameObject("TaskRoot");
			Root.transform.SetParent(OdinPlus.Root.transform);
			PrefabRoot = OdinPlus.PrefabParent.transform;
			CreatePrefab();
		}
		#endregion Mono

		#region Tool
		public bool HasTask()
		{
			return !(CurrentTask == null);
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
		private void CreatePrefab()
		{
			if (isPrefabed) { return; }

			for (int i = 1; i < 6; i++)
			{
				var go = Instantiate(ObjectDB.instance.GetItemPrefab("OdinLegacy"), PrefabRoot);
				go.name = "OdinLegacy" + i;
				var lgc = go.GetComponent<ItemDrop>().m_itemData;
				lgc.m_quality = i;
			}
			isPrefabed = true;
		}
		#endregion Tool

		#region Feature
		public static void CreateRandomTask()
		{
			UnityEngine.Random.InitState((int)Time.time);
			int count = Enum.GetNames(typeof(TaskType)).Length;
			switch (CheckKey())
			{
				case 0:
					CreateTask(TaskType.Treasure);
					break;
				case 1:
					break;
				case 2:
					break;
				case 3:
					break;
				case 4:
					break;
				default:
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
					CurrentTask = go.AddComponent<TreasureTask>();
					break;
				case TaskType.Hunt:
					break;
				case TaskType.Dungeon:
					break;
				case TaskType.Search:
					break;
			}
		}
		public static void GiveUpTask()
		{

		}
		public static void Clear()
		{
			foreach (Transform t in Root.GetComponentsInChildren<Transform>())
			{
				Destroy(t);
			}
		}
		#endregion Feature
		#region internalTool
		#endregion internalTool
		#region save&Load
		[Serializable]
		public class TaskDataTable
		{
			public TaskType m_type = TaskType.Treasure;
			public string taskName;
			public int Key;
			public int Level;
			public string Id;
			public bool isMain = false;
			public bool m_pause = false;
			public bool m_isInit = false;
			public bool m_discovered = false;
			public bool m_finished = false;
			public bool m_isClear = false;

		}

		public static List<TaskManager.TaskDataTable> Save()
		{
			var data = new List<TaskManager.TaskDataTable>();
			foreach (var item in Root.GetComponentsInChildren<OdinTask>())
			{
				data.Add(item.Save());
			}
			return data;
		}
		public static void Load(List<TaskManager.TaskDataTable> data)
		{
			Root.SetActive(false);
			int i = 0;
			int e = 0;
			foreach (var item in data)
			{
				var go = new GameObject("Task" + i);
				go.transform.parent = Root.transform;8a

				switch (item.m_type)
				{
					case TaskType.Treasure:
						go.AddComponent<TreasureTask>();
						break;
					case TaskType.Hunt:
						break;
					case TaskType.Dungeon:
						break;
					case TaskType.Search:
						break;
				}
				if (go.GetComponent<OdinTask>().Load(item))
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