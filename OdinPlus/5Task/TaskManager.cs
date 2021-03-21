using System;
using System.Collections.Generic;
using UnityEngine;
namespace OdinPlus
{
	using UnityEngine;

	public class TaskManager : MonoBehaviour
	{
		#region  var
		#region Data
		public enum TaskType { None, Treasure, Hunt, Dungeon, Search };
		private string[] RefKeys = { "defeated_eikthyr", "defeated_gdking", "defeated_bonemass", "defeated_moder", "defeated_goblinking" };
		#endregion Data
		#region Out
		public static OdinTask CurrentTask;
		#endregion Out
		#endregion  var

		#region Mono

		#endregion Mono

		#region Tool
		public bool HasTask()
		{
			return !(CurrentTask == null);
		}
		#endregion Tool

		#region Feature
		public static void BeginTask()
		{

		}
		public static void GiveUpTask()
		{

		}
		public static void FinishTask()
		{

		}
		#endregion Feature

		#region save&Load

		#endregion save&Load
	}
}