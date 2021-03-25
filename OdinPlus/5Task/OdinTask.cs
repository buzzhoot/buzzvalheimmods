using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using UnityEngine;


/* Game.instance.DiscoverClosestLocation("Vendor_BlackForest", Player.m_localPlayer.transform.position, "Merchant", 8);
Minimap.PinData pinData = Enumerable.First<Minimap.PinData>((List<Minimap.PinData>)Traverse.Create(Minimap.instance).Field("m_pins").GetValue(), (Minimap.PinData p) => p.m_type == Minimap.PinType.None && p.m_name == "");
 */


namespace OdinPlus
{

	public class OdinTask : MonoBehaviour
	{
		#region Var
		#region Data
		protected string[] m_tier0 = new string[0];
		protected string[] m_tier1 = new string[0];
		protected string[] m_tier2 = new string[0];
		protected string[] m_tier3 = new string[0];
		protected string[] m_tier4 = new string[0];
		protected List<string[]> locList = new List<string[]>();
		protected GameObject root;
		public string locName;
		public int m_index;

		#endregion Data
		#region internal
		protected Vector3 m_position;
		protected float m_range;
		protected Action Init;
		protected bool loading = false;
		#region Real Data
		public TaskManager.TaskType m_type;
		public string taskName;
		public string Id;
		protected bool m_pause = false;
		protected bool m_isInit = false;
		protected bool m_discovered = false;
		protected bool m_finished = false;
		protected bool m_isClear = false;
		protected ZoneSystem.LocationInstance location;
		#endregion Real Data

		#endregion internal
		#region in
		public int Key;
		public int Level;
		public bool isMain = false;
		#endregion in
		#region out
		public string HintTarget;
		public string HintStart;
		public GameObject Reward;
		#endregion out
		#endregion Var

		#region Mono
		protected virtual void Update()
		{
			if (!m_isInit)
			{
				Init();
				return;
			}
			if (!m_discovered)
			{
				m_discovered = isLoaded();
				Discovery();
				return;
			}
			if (isLoaded() && !IsFinsih())
			{
				CheckTarget();
			}
			if (IsFinsih() && !m_isClear)
			{
				Clear();
			}
		}
		#endregion Mono

		#region Feature

		public virtual void Giveup()
		{
			MessageHud.instance.ShowBiomeFoundMsg((isMain ? "Main" : "Side") + " Quest " + m_index + " : " + taskName + " Giveup", true);
			RemovePin();
			Clear();
		}

		public virtual void Pause()
		{
			m_pause = !m_pause;
		}
		#endregion Feature

		#region internal Feature
		protected virtual void Begin()
		{
			Key = TaskManager.GameKey;
			Level = TaskManager.Level;
			isMain = TaskManager.isMain;
			locList = new List<string[]> { m_tier0, m_tier1, m_tier2, m_tier3, m_tier4 };
			switch (Key)
			{

				case 0:
					Init = new Action(InitTire0);
					break;
				case 1:
					Init = new Action(InitTire1);
					break;
				case 2:
					Init = new Action(InitTire2);
					break;
				case 3:
					Init = new Action(InitTire3);
					break;
				case 4:
					Init = new Action(InitTire4);
					break;
			}
			if (!SetLocation())
			{
				return;
			}
			Tweakers.TaskHintHugin((isMain ? "Main" : "Side") + "Quest " + m_index + " : " + taskName, HintStart);
			SetRange(30.RollDice(30 + Level * 30));
			SetPosition();
			SetPin();
			MessageHud.instance.ShowBiomeFoundMsg((isMain ? "Main" : "Side") + " Quest " + m_index + " : " + taskName + " Start", true);
		}
		protected virtual bool SetLocation()
		{
			var list = locList[Key];
			int ind = list.Length.RollDice();
			locName = list[ind];
			if (LocationManager.FindClosestLocation(locName, Game.instance.GetPlayerProfile().GetCustomSpawnPoint(), out Id))
			{
				LocationManager.GetLocationInstance(Id, out location);
				root = location.m_location.m_prefab.gameObject;
				gameObject.name = "Task" + Id;
				SetLocName();
				SetTaskName();
				LocationManager.Remove(Id);
				OdinData.Data.TaskCount++;
				m_index = OdinData.Data.TaskCount;
				return true;
			}
			DBG.InfoCT("Something Went Wrong,Try again");
			DBG.blogWarning(string.Format("Cannot Place Task :  {0} {1}", GetTaskType(), locName));
			DestroyImmediate(this.gameObject);
			return false;
		}
		protected virtual void InitTire0() { }
		protected virtual void InitTire1() { }
		protected virtual void InitTire2() { }
		protected virtual void InitTire3() { }
		protected virtual void InitTire4() { }
		private void SetPin()
		{
			Minimap.instance.DiscoverLocation(m_position, Minimap.PinType.Icon3, (isMain ? "Main" : "Side") + "Quest " + m_index + " : " + taskName);
		}
		protected virtual void Discovery()
		{
			Tweakers.TaskHintHugin((isMain ? "Main" : "Side") + "Quest " + m_index + " : " + taskName, HintTarget);
		}
		protected virtual void CheckTarget() { }
		public virtual void Finish()
		{
			MessageHud.instance.ShowBiomeFoundMsg((isMain ? "Main" : "Side") + "Quest " + m_index + " : " + taskName + " Clear", true);
			RemovePin();
			OdinMunin.ResetTimer();
			m_finished = true;
		}
		public virtual void Clear()
		{
			m_isClear = true;
			Destroy(gameObject);
		}
		private void SetLocName()
		{
			locName = Regex.Replace(locName, @"[\d-]", string.Empty);
		}
		private void SetTaskName()
		{
			taskName = locName + " " + GetTaskType().ToString();
		}
		private void SetPosition()
		{
			m_position = location.m_position;
			m_position = m_position.GetRandomLocation(m_range);
		}
		private void RemovePin()
		{
			Minimap.instance.RemovePin(m_position, 10);
		}


		#endregion internal Feature

		#region Tool
		public void SetRange(int range)
		{
			m_range = range.RollDice();
		}
		public void SendPing()
		{
			Chat.instance.SendPing(m_position);
		}
		public bool IsFinsih()
		{
			return m_finished;
		}
		public bool IsPause()
		{
			return m_pause;
		}
		public bool IsDiscovered()
		{
			return m_discovered;
		}
		public bool isLoaded()
		{
			return ZoneSystem.instance.IsZoneLoaded(location.m_position);
		}
		public TaskManager.TaskType GetTaskType()
		{
			return m_type;
		}

		public bool isInsideArea(Vector3 position)
		{
			if (position.y > 3000f)
			{
				return false;
			}
			return Utils.DistanceXZ(position, m_position) < m_range;
		}
		public bool IsPlayerInsideArea()
		{
			foreach (ZDO allCharacterZDO in ZNet.instance.GetAllCharacterZDOS())
			{
				if (isInsideArea(allCharacterZDO.GetPosition()))
				{
					return true;
				}
			}
			return false;
		}
		#endregion Tool

		#region save load
		public bool Load(OdinData.TaskDataTable dat)
		{
			loading = true;
			taskName = dat.taskName;

			m_index = dat.m_index;

			Key = dat.Key;

			Level = dat.Level;

			m_type = dat.m_type;

			if (m_type == TaskManager.TaskType.Search)
			{
				Id = dat.Id;
				gameObject.name = "Task" + Id;
				return true;
			}

			isMain = dat.isMain;

			m_isInit = dat.m_isInit;

			m_position = dat.m_position;

			m_pause = dat.m_pause;

			m_discovered = dat.m_discovered;

			m_finished = dat.m_finished;

			m_isClear = dat.m_isClear;

			Id = dat.Id;
			if (LocationManager.GetLocationInstance(Id, out location))
			{
				root = location.m_location.m_prefab.gameObject;
				locName = location.m_location.m_prefabName;
				SetLocName();
				SetTaskName();
				switch (Key)
				{

					case 0:
						Init = new Action(InitTire0);
						break;
					case 1:
						Init = new Action(InitTire1);
						break;
					case 2:
						Init = new Action(InitTire2);
						break;
					case 3:
						Init = new Action(InitTire3);
						break;
					case 4:
						Init = new Action(InitTire4);
						break;
				}
				return true;

			}
			DestroyImmediate(this.gameObject);
			return false;
		}
		public OdinData.TaskDataTable Save()
		{
			var dat = new OdinData.TaskDataTable()
			{
				taskName = this.taskName,

				m_index = this.m_index,

				Key = this.Key,

				Level = this.Level,

				m_type = this.m_type,

				isMain = this.isMain,

				m_isInit = this.m_isInit,

				m_position = this.m_position,

				m_pause = this.m_pause,

				m_discovered = this.m_discovered,

				m_finished = this.m_finished,

				m_isClear = this.m_isClear,

				Id = this.Id
			};
			return dat;
		}

		#endregion save load
		#region  Tool
		public string PrintData()
		{
			string n = "\n" + (isMain ? "Main" : "Side");
			n += String.Format(" Quest [<color=yellow><b>{0}</b></color>] : {1}", m_index, taskName);
			return n;
		}
		#endregion  Tool
	}
}