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
	public class OdinData : MonoBehaviour
	{
		#region Var
		#region serialization
		[Serializable]
		public class DataTable : SerializationBinder
		{
			public int Credits = 100;
			public bool hasWolf = false;
			public bool hasTroll = false;
			public List<string> BlackList = new List<string>();
			public List<OdinData.TaskDataTable> Tasks = null;
			public int TaskCount = 0;
			public Dictionary<string, int> SearchTaskList = new Dictionary<string, int>();
			public List<TaskManager.ClientTaskData> ClientTaskDatas = new List<TaskManager.ClientTaskData>();
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
			public long owner;
			public int Key = 0;
			public int Level = 1;
			public bool m_pause = false;
			public bool m_isInit = false;
			public bool m_finished = false;
			public bool m_isClear = false;
			public string Id = "0_0";
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

		#endregion serialization
		#region interl
		public static int Credits;
		public static Dictionary<string, int> ItemSellValue = new Dictionary<string, int>();
		public static DataTable Data;
		#endregion interl

		#region GameCfgData
		public static Dictionary<string, int> MeadsValue = new Dictionary<string, int>(){
{"ExpMeadS",5},
{"ExpMeadM",10},
{"ExpMeadL",20},
{"WeightMeadS",20},
{"WeightMeadM",30},
{"WeightMeadL",40},
{"InvisibleMeadS",30},
{"InvisibleMeadM",60},
{"InvisibleMeadL",90},
{"PickaxeMeadS",20},
{"PickaxeMeadM",30},
{"PickaxeMeadL",60},
{"BowsMeadS",20},
{"BowsMeadM",30},
{"BowsMeadL",60},
{"SwordsMeadS",20},
{"SwordsMeadM",30},
{"SwordsMeadL",60},
{"SpeedMeadsL",20},
{"AxeMeadS",20},
{"AxeMeadM",30},
{"AxeMeadL",60}
		};
		#endregion GameCfgData

		#endregion Var

		#region Mono
		private void Awake()
		{
			if (DevTool.DisableSaving)
			{
				Credits = 1000;
			}
			Data = new DataTable();
			if (Plugin.CFG_ItemSellValue.Value == "") { return; }
			string[] l1 = Plugin.CFG_ItemSellValue.Value.Split(new char[] { ';' });
			for (int i = 0; i < l1.Length; i++)
			{
				string[] c = l1[i].Split(new char[] { ':' });
				if (c.Length == 0)
				{
					continue;
				}
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
		#endregion Mono

		#region Credits
		public static void AddCredits(int s, Transform m_head)
		{
			Credits += s;
			Player.m_localPlayer.m_skillLevelupEffects.Create(m_head.position, m_head.rotation, m_head, 1f);
		}
		public static bool RemoveCredits(int s)
		{
			if (Credits - s < 0)
			{
				return false;
			}
			Credits -= s;
			return true;
		}
		#endregion Credits
		#region Save And Load
		public static void saveOdinData(string name)
		{
			if (DevTool.DisableSaving)
			{
				OdinPlus.isLoaded = true;
				return;
			}
			#region Save
			if (ZNet.instance.IsServer())
			{
				Data.Tasks = TaskManager.Save();
			}
			TaskManager.ClientSave();
			Data.Credits = Credits;
			#endregion Save

			#region Serialize
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
			#endregion Serialize

			DBG.blogWarning("OdinDataSaved:" + name);
		}
		public static void loadOdinData(string name)
		{
			if (DevTool.DisableSaving)
			{
				OdinPlus.isLoaded = true;
				return;
			}
			#region Serial
			string file = Path.Combine(Application.persistentDataPath, (name + ".odinplus"));
			if (!File.Exists(@file))
			{
				OdinPlus.isLoaded = true;
				Credits = 100;
				DBG.blogWarning("Profile not exists:" + name);
				return;
			}
			FileStream fileStream = new FileStream(@file, FileMode.Open, FileAccess.Read);
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Binder = new DataTable();
			Data = (DataTable)formatter.Deserialize(fileStream);
			fileStream.Close();
			#endregion Serial

			#region Load
			Credits = Data.Credits;
			if (ZNet.instance.IsServer())
			{
				TaskManager.Load(Data.Tasks);
			}
			TaskManager.ClientLoad();
			LocationManager.BlackList = Data.BlackList;
			LocationManager.RemoveBlackList();
			#endregion Load

			OdinPlus.isLoaded = true;
			DBG.blogWarning("OdinDataLoaded:" + name);
		}
		#endregion Save And Load

	}
}
