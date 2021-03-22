using System;
using System.Collections.Generic;
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
		#region internal
		protected Vector3 m_location;
		protected float m_range;
		protected TaskManager.TaskType m_type;
		protected bool m_targetClear = false;
		protected bool m_start = false;
		protected bool m_finished = false;
		protected bool m_pause = false;
		protected bool m_discovered = false;
		#endregion internal
		#region in
		public int Key;
		public int Level;
		#endregion in
		#region out
		public string HintTarget;
		public string HintStart;
		public ItemDrop.ItemData Reward;
		#endregion out
		#endregion Var

		#region Mono
		protected virtual void Update()
		{
			if (!m_discovered)
			{
				m_discovered = ZoneSystem.instance.IsZoneLoaded(m_location);
			}
		}
		#endregion Mono

		#region Feature
		public virtual void Begin()
		{
			m_start = true;
			SetLocation();
			SetRange();
			SetPin();
		}
		public virtual void SetPin()
		{
			ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "DiscoverLocationRespons", new object[]
			{
				"OdinMission",
				Minimap.PinType.Icon0,
				m_location.GetRandomLocation()
			});
		}
		public virtual void Giveup()
		{
			Clear();
		}
		public virtual void Finish()
		{
			Clear();
			m_finished = true;
		}
		public virtual void Pause()
		{
			m_pause = !m_pause;
		}
		#endregion Feature

		#region internal Feature
		protected virtual void SetLocation()
		{

		}
		protected virtual void SetRange()
		{

		}
		protected virtual void Clear()
		{

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
			return Utils.DistanceXZ(position, m_location) < m_range;
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