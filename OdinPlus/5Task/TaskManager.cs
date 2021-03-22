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
		#endregion Out
		#region in
			public static GameObject Root;
			public static int Level;
		#endregion interal
		#endregion  var

		#region Mono
		
		private void Update()
		{
			Root= new GameObject("TaskRoot");
			Root.transform.SetParent(OdinPlus.Root.transform);
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
			return result;
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
					SetTaskLevel(Level);
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
			switch (t)
			{
				case TaskType.Treasure:
					CurrentTask=Root.AddComponent<TreasureTask>();
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
		public static void FinishTask()
		{
			CurrentTask.Finish();
		}
		#endregion Feature
		#region internalTool
		private static void SetTaskLevel(int level)
		{
			CurrentTask.Level=level;
		}
		#endregion internalTool
		#region save&Load

		#endregion save&Load
	}
}