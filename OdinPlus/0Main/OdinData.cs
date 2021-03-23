using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace OdinPlus
{
	class OdinData
	{
		public static int score;
		public static Dictionary<string, int> ItemSellValue = new Dictionary<string, int>();
		[Serializable]
		public class DataTable
		{
			public int score = 0;
			public bool hasWolf = false;
			public bool hasTroll = false;
			public List<string> BlackList = new List<string>();
			public List<TaskManager.TaskDataTable> Tasks;
		}
		public static DataTable Data = new DataTable();
		public static void init()
		{
			if (Plugin.CFG_ItemSellValue.Value == "") { return; }
			string[] l1 = Plugin.CFG_ItemSellValue.Value.Split(new char[] { ';' });
			for (int i = 0; i < l1.Length; i++)
			{
				string[] c = l1[i].Split(new char[] { ':' });
				try
				{
					ItemSellValue.Add(c[0], int.Parse(c[1]));
				}
				catch (Exception e)
				{
					DBG.blogWarning("CFG Error,Check Your ItemSellValue");
					DBG.blogWarning(e);
				}
			}

		}
		public static void AddScore(int s, Transform m_head)
		{
			score += s;
			Player.m_localPlayer.m_skillLevelupEffects.Create(m_head.position, m_head.rotation, m_head, 1f);
		}
		public static bool RemoveScore(int s)
		{
			if (score - s < 0)
			{
				return false;
			}
			score -= s;
			return true;
		}
		public static void saveOdinData(string name)
		{
			#region Save
			Data.Tasks = TaskManager.Save();
			Data.score = score;
			#endregion Save

			string file = Path.Combine(Application.persistentDataPath, (name + ".odinplus"));
			if (File.Exists(@file))
			{
				//add Backup
			}
			FileStream fileStream = new FileStream(@file, FileMode.Create, FileAccess.Write);

			IFormatter formatter = new BinaryFormatter();
			formatter.Serialize(fileStream, Data);
			fileStream.Close();
            DBG.blogWarning("OdinDataSaved:" + name);
		}
		public static void loadOdinData(string name)
		{
			string file = Path.Combine(Application.persistentDataPath, (name + ".odinplus"));
			if (File.Exists(@file))
			{
				FileStream fileStream = new FileStream(@file, FileMode.Open, FileAccess.Read);

				IFormatter formatter = new BinaryFormatter();
				Data = (DataTable)formatter.Deserialize(fileStream);
				fileStream.Close();

				#region Load
				score = Data.score;
				TaskManager.Load(Data.Tasks);
				#endregion Load
				OdinPlus.isLoaded=true;
				DBG.blogWarning("OdinDataLoaded:" + name);
				return;
			}
			else
			{
				DBG.blogWarning("Profile not exists:"+name);
			}
		}
	}
}
