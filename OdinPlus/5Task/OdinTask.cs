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
		public string locName;
		protected GameObject root;
		
		#endregion Data
		#region internal
		protected Vector3 m_position;
		protected float m_range;
		protected TaskManager.TaskType m_type;
		protected bool m_targetClear = false;
		protected bool m_start = false;
		protected bool m_finished = false;
		protected bool m_pause = false;
		protected bool m_discovered = false;
		protected bool m_isInit = false;
		protected ZoneSystem.LocationInstance location;
		protected Action Init;
		protected bool m_finded;
		#endregion internal
		#region in
		public int Key;
		public int Level;
		public bool isMain;
		#endregion in
		#region out
		public string HintTarget;
		public string HintStart;
		public GameObject Reward;
		public int Id;
		#endregion out
		#endregion Var

		#region Mono
		private void Update()
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
			if (isLoaded()&&!IsFinsih())
			{
				CheckTarget();
			}
			if (IsFinsih())
			{
				Clear();
			}
		}
		#endregion Mono

		#region Feature

		public virtual void Giveup()
		{
			Clear();
		}
		public virtual void Finish()
		{
			MessageHud.instance.ShowBiomeFoundMsg(isMain ? "Main" : "Side" + " Quest Clear", true);
			Minimap.instance.RemovePin(m_position,3);
			m_finished = true;
		}
		public virtual void Pause()
		{
			m_pause = !m_pause;
		}
		#endregion Feature

		#region internal Feature
		protected virtual void Begin()
		{
			Key=TaskManager.GameKey;
			Level=TaskManager.Level;
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
			m_start = true;
			SetLocation();
			SetRange();
			SetPosition();
			SetPin();
			MessageHud.instance.ShowBiomeFoundMsg(isMain ? "Main" : "Side" + " Quest Start", true);
		}
		protected virtual void SetLocation()
		{
			var list = locList[Key];
			int ind = list.Length.RollDice();
			locName = list[ind];
			m_finded = ZoneSystem.instance.FindClosestLocation(locName, Game.instance.GetPlayerProfile().GetCustomSpawnPoint(), out location);
			root = location.m_location.m_prefab.gameObject;
			Id=location.m_location.m_hash;
			locName = Regex.Replace(locName, @"[\d-]", string.Empty);
		}
		protected virtual void InitTire0() { }
		protected virtual void InitTire1() { }
		protected virtual void InitTire2() { }
		protected virtual void InitTire3() { }
		protected virtual void InitTire4() { }
		protected virtual void SetPosition()
		{
			m_position = location.m_position.GetRandomLocation(m_range);
		}
		protected virtual void SetRange()
		{
			m_range = 100.RollDice();
		}
		public virtual void SetPin()
		{
			Minimap.instance.DiscoverLocation(m_position, Minimap.PinType.Icon3, "Odin Quest");
			Chat.instance.SendPing(m_position);
		}
		protected virtual void Discovery()
		{
			Tweakers.SendRavenMessage("Quest Name",HintTarget);
		}
		protected virtual void CheckTarget()
		{

		}
		protected virtual void Clear()
		{
			
			Destroy(gameObject);
		}
		#endregion internal Feature

		#region Tool
		public bool IsFinsih()
		{
			return m_finished;
		}
		public bool IsStarted()
		{
			return m_start;
		}
		public bool IsPause()
		{
			return m_pause;
		}
		public bool IsDiscovered()
		{
			return m_discovered;
		}
		public void ClearTarget()
		{
			m_targetClear = true;
		}
		public bool isLoaded()
		{
			return ZoneSystem.instance.IsZoneLoaded(location.m_position);
		}
		public void SetTaskType(TaskManager.TaskType t)
		{
			m_type = t;
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

		#region Static Tool
		#endregion Static Tool
	}
}