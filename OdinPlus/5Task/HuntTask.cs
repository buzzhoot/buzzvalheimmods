using System;
using System.Collections.Generic;
using UnityEngine;
namespace OdinPlus
{
	public class HuntTask:OdinTask
	{
		private void Awake()
		{
			m_type = TaskManager.TaskType.Hunt;

		}
	}
}