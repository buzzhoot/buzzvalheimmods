using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace OdinPlus
{
	public class OdinData:MonoBehaviour
	{
		public static int score;
		public static Dictionary<string, int> ItemSellValue = new Dictionary<string, int>();
		[Serializable]
		public class DataTable : SerializationBinder
		{
			public int score = 0;
			public bool hasWolf = false;
			public bool hasTroll = false;
			public List<string> BlackList = new List<string>();
			public List<OdinData.TaskDataTable> Tasks=null;
			public override Type BindToType(string assemblyName, string typeName)
			{
				Type tyType = null;
				string sShortAssemblyName = assemblyName.Split(',')[0];

				Assembly[] ayAssemblies = AppDomain.CurrentDomain.GetAssemblies();

				foreach (Assembly ayAssembly in ayAssemblies)
				{
					if (sShortAssemblyName == ayAssembly.FullName.Split(',')[0])
					{
						tyType = ayAssembly.GetType(typeName);
						break;
					}
				}
				return tyType;

			}
		}
		[Serializable]
		public class TaskDataTable : SerializationBinder
		{
			public TaskManager.TaskType m_type = TaskManager.TaskType.Treasure;
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
			public override Type BindToType(string assemblyName, string typeName)
			{
				Type tyType = null;
				string sShortAssemblyName = assemblyName.Split(',')[0];

				Assembly[] ayAssemblies = AppDomain.CurrentDomain.GetAssemblies();

				foreach (Assembly ayAssembly in ayAssemblies)
				{
					if (sShortAssemblyName == ayAssembly.FullName.Split(',')[0])
					{
						tyType = ayAssembly.GetType(typeName);
						break;
					}
				}
				return tyType;
			}

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

			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Binder = new DataTable();
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

				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Binder = new DataTable();
				Data = (DataTable)formatter.Deserialize(fileStream);
				fileStream.Close();

				#region Load
				score = Data.score;
				TaskManager.Load(Data.Tasks);
				#endregion Load
				OdinPlus.isLoaded = true;
				DBG.blogWarning("OdinDataLoaded:" + name);
				return;
			}
			else
			{
				DBG.blogWarning("Profile not exists:" + name);
			}
		}
	}
}
