using System;
using System.Collections.Generic;
using UnityEngine;

namespace OdinPlus
{

	public class OdinTask
	{
		#region Var
		protected Vector3 m_location;
		protected float m_range;
		protected TaskManager.TaskType m_type;
		protected bool m_start = false;
		protected bool m_finished = false;
		protected bool m_pause=false;
		#endregion Var

		#region Mono

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

		}
		public virtual void Clear()
		{

		}
		public virtual void Giveup()
		{

		}
		public virtual void Finish()
		{
			Clear();
			m_finished = true;
		}
		public virtual void Pause()
		{
			m_pause=!m_pause;
		}
		#endregion Feature

		#region internal Feature
		protected virtual void SetLocation()
		{

		}
		protected virtual void SetRange()
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
	}
}